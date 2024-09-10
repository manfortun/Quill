using Microsoft.AspNetCore.Mvc;
using Quill.Server.DataAccess;
using Quill.Server.DTOs;
using Quill.Server.Models;
using Quill.Server.Services;

namespace Quill.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly NoteRepository _repository;
    private readonly CacheService _cacheService;
    private readonly BackupService _backupService;
    private readonly TempFileService _tempFileService;
    private readonly string _baseUrl;

    public NotesController(
        NoteRepository repository,
        CacheService cacheService,
        BackupService backupService,
        TempFileService tempFileService,
        IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _cacheService = cacheService;
        _backupService = backupService;
        _tempFileService = tempFileService;
        _baseUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}/api";
    }

    [HttpGet]
    public IActionResult GetNotes()
    {
        List<ReadNoteDto> readNotes = new();

        foreach (Note note in _repository.GetAll(includeContent: false))
        {
            readNotes.Add(new ReadNoteDto
            {
                Title = note.Title,
                Content = note.Content,
                LastWrite = note.LastWrite,
                Path = note.Path,
                LastWriteString = note.GetLastWriteDateTimeString(),
                Links = HypermediaHelper.GenerateLinksForNotes(note, _baseUrl)
            });
        }

        return Ok(readNotes);
    }

    [HttpGet("{identifier}")]
    public IActionResult GetNote(string identifier)
    {
        ReadNoteDto? readNoteDto = _cacheService.TryGetCache<ReadNoteDto>(identifier, nameof(ReadNoteDto));

        if (readNoteDto is null)
        {
            Note? note = _repository.Get(identifier, includeContent: true);

            if (note is null)
            {
                return NotFound();
            }
            else
            {
                readNoteDto = new ReadNoteDto
                {
                    Title = note.Title,
                    Content = note.Content,
                    LastWrite = note.LastWrite,
                    Path = note.Path,
                    LastWriteString = note.GetLastWriteDateTimeString(),
                    Links = HypermediaHelper.GenerateLinksForNotes(note, _baseUrl)
                };

                _cacheService.Cache(identifier, readNoteDto, nameof(ReadNoteDto));
            }

        }

        return Ok(readNoteDto);
    }

    [HttpPost]
    public IActionResult CreateNewNote([FromBody] CreateNewNoteDto request)
    {
        try
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest("Title is required to create a new note.");
            }

            Enums.SaveResult result;
            string identifier = string.Empty;
            // when request.OldTitle is null or empty, the file is new
            if (string.IsNullOrEmpty(request.OldTitle))
            {
                result = _repository.Create(request, out identifier);
            }
            else
            {
                result = _repository.Update(request, out identifier);
            }

            if (result == Enums.SaveResult.WithDuplicateTitle)
            {
                return BadRequest("A file with this name already exists. Try using another title.");
            }
            else if (result == Enums.SaveResult.FileNotExists)
            {
                return NotFound("The file specified does not exist.");
            }
            else if (result == Enums.SaveResult.Success)
            {
                _cacheService.Decache(identifier, nameof(ReadNoteDto));
                _cacheService.Decache(identifier, nameof(TOCLayer));

                // delete temp file
                this._tempFileService.DeleteTempFile(identifier);

                return Ok(identifier);
            }
            else
            {
                return BadRequest("Something went wrong. Please contact administrator.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-last-backup-date")]
    public IActionResult GetLastBackupDate()
    {
        try
        {
            DateTime lastBackupDate = this._backupService.GetLastSuccessfulBackupDate();

            if (lastBackupDate == DateTime.MinValue)
            {
                return StatusCode(500);
            }
            else
            {
                TimeSpan duration = DateTime.Now - lastBackupDate;
                return Ok(duration.GetLastWriteDateTimeString(lastBackupDate));
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("backup")]
    public async Task<IActionResult> Backup()
    {
        try
        {
            bool result = await this._backupService.BackupAsync();

            if (result)
            {
                return Ok(DateTime.Now.ToString());
            }
            else
            {
                return StatusCode(500);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{identifier}")]
    public IActionResult Delete(string identifier)
    {
        try
        {
            _cacheService.Decache(identifier, nameof(ReadNoteDto));
            _cacheService.Decache(identifier, nameof(TOCLayer));

            var result = _repository.Delete(identifier);

            if (result == Enums.SaveResult.FileNotExists)
            {
                return NotFound("The file does not exist.");
            }
            else if (result == Enums.SaveResult.Success)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Something went wrong. Please contact administrator.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

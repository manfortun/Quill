﻿using Microsoft.AspNetCore.Mvc;
using Quill.Server.DataAccess;
using Quill.Server.DTOs;
using Quill.Server.Services;

namespace Quill.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EditorController : ControllerBase
{
    private readonly NoteRepository _noteRepository;
    private readonly TempFileService _tempFileService;
    private readonly ILogger<EditorController> _logger;

    public EditorController(NoteRepository noteRepository, TempFileService tempFileService, ILogger<EditorController> logger)
    {
        _noteRepository = noteRepository;
        _tempFileService = tempFileService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTemp([FromBody] CreateNewNoteDto note)
    {
        try
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest("Please set title to activate auto-save.");
            }

            bool result = await this._tempFileService.CreateTempFileAsync(note.Title, note.Content);

            if (result)
            {
                _logger.LogTrace("Autosaved instance.");
            }

            return Ok(result ? "Autosaved" : string.Empty);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{identifier}")]
    public IActionResult CheckAutosaved(string identifier)
    {
        try
        {
            bool result = this._tempFileService.CheckTempFileExists(identifier);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-content/{identifier}")]
    public async Task<IActionResult> GetContent(string identifier)
    {
        try
        {
            bool result = this._tempFileService.CheckTempFileExists(identifier);

            if (!result)
            {
                return BadRequest($"The temporary file was not found for {identifier}.");
            }

            string? content = await this._tempFileService.GetContent(identifier);

            if (!string.IsNullOrEmpty(content))
            {
                _logger.LogTrace("Obtained autosaved content for {0}", identifier);
            }
            return Ok(content);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{identifier}")]
    public async Task<IActionResult> DeleteTemp(string identifier)
    {
        try
        {
            _logger.LogTrace("Deleting temp content for {0}", identifier);
            this._tempFileService.DeleteTempFile(identifier);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

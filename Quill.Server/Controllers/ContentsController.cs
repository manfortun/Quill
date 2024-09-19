using Microsoft.AspNetCore.Mvc;
using Quill.Server.DataAccess;
using Quill.Server.DTOs;
using Quill.Server.Services;

namespace Quill.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentsController : ControllerBase
{
    private readonly NoteRepository _repository;
    private readonly TableOfContentService _tocService;
    private readonly CacheService _cacheService;

    public ContentsController(NoteRepository repository, TableOfContentService tocService, CacheService cacheService)
    {
        _repository = repository;
        _tocService = tocService;
        _cacheService = cacheService;
        error;
    }

    [HttpGet("{identifier}")]
    public IActionResult GetTableOfContents(string identifier)
    {
        TOCLayer? tocLayer = _cacheService.TryGetCache<TOCLayer>(identifier, nameof(TOCLayer));

        if (tocLayer is null)
        {
            var note = _repository.Get(identifier);

            if (note is null)
            {
                return NotFound();
            }

            tocLayer = _tocService.GetTOC(note);
            _cacheService.Cache(identifier, tocLayer, nameof(TOCLayer));
        }

        return Ok(tocLayer);
    }
}

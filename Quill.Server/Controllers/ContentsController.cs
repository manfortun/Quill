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
    private readonly ILogger<ContentsController> _logger;

    public ContentsController(NoteRepository repository, TableOfContentService tocService, CacheService cacheService, ILogger<ContentsController> logger)
    {
        _repository = repository;
        _tocService = tocService;
        _cacheService = cacheService;
        _logger = logger;
    }

    [HttpGet("{identifier}")]
    public IActionResult GetTableOfContents(string identifier)
    {
        _logger.LogTrace("Reading table of contents of ID = {0}", identifier);

        TOCLayer? tocLayer = _cacheService.TryGetCache<TOCLayer>(identifier, nameof(TOCLayer));

        if (tocLayer is not null)
        {
            _logger.LogTrace("Table of contents is obtained from cache.");
        }

        if (tocLayer is null)
        {
            var note = _repository.Get(identifier);

            if (note is null)
            {
                _logger.LogTrace("No content found.");
                return NotFound();
            }

            tocLayer = _tocService.GetTOC(note);
            _cacheService.Cache(identifier, tocLayer, nameof(TOCLayer));
        }

        _logger.LogTrace("Returning table of contents.");
        return Ok(tocLayer);
    }
}

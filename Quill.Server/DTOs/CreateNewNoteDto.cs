using Quill.Server.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Quill.Server.DTOs;

public class CreateNewNoteDto : INoteUpdate
{
    [Required]
    public required string Title { get; set; }
    public string? OldTitle { get; set; } = default!;
    public string? Content { get; set; }
}

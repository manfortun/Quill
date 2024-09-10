using Quill.Server.Models;

namespace Quill.Server.DTOs;

public interface IHypermedia
{
    List<Link> Links { get; set; }
}

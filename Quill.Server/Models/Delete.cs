namespace Quill.Server.Models;

public class Delete : Link
{
    public Delete()
    {
        base.Ref = "delete";
        base.Method = "DELETE";
    }
}

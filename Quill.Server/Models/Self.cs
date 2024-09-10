namespace Quill.Server.Models;

public class Self : Link
{
    public Self()
    {
        base.Ref = "self";
        base.Method = "GET";
    }
}

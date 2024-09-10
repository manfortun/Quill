namespace Quill.Server;

public class Enums
{
    public enum SaveResult
    {
        Success,                // file is successully saved
        Failed,                 // file could not be saved due to some generic error
        WithDuplicateTitle,     // file could not be saved because of a name conflict
        FileNotExists,          // file could not be found
        NoFilePathInNote        // file could not be updated because it does not have a file path
    }
}

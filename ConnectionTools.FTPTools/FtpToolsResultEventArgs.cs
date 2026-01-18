namespace ConnectionTools.FTPTools;

public sealed class FtpToolsResultEventArgs : FtpToolsEventArgs
{
    public FtpToolsResultEventArgs(string message, string objectName, EFtpToolsActions action, bool result) : base(
        message, objectName, action)
    {
        Result = result ? EFtpToolsResult.Success : EFtpToolsResult.Failure;
    }

    public FtpToolsResultEventArgs(string message, string objectName, EFtpToolsActions action, bool result, long size) :
        base(message, objectName, action)
    {
        Result = result ? EFtpToolsResult.Success : EFtpToolsResult.Failure;
        FileSize = size;
    }

    public FtpToolsResultEventArgs(string objectName, EFtpToolsActions action, bool result) : base(objectName, action)
    {
        Result = result ? EFtpToolsResult.Success : EFtpToolsResult.Failure;
    }

    public EFtpToolsResult Result { get; }
    public long FileSize { get; }
}
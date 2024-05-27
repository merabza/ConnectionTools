using System;

namespace FTPTools;

public /*open*/ class FtpToolsEventArgs : EventArgs
{
    public FtpToolsEventArgs(string message, string objectName, EFtpToolsActions action)
    {
        Message = message;
        ObjectName = objectName;
        Action = action;
    }

    public FtpToolsEventArgs(string objectName, EFtpToolsActions action) : this(string.Empty, objectName, action)
    {
    }

    public string Message { get; }
    public string ObjectName { get; }
    public EFtpToolsActions Action { get; }
}
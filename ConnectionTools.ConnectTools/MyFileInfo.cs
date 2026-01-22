namespace ConnectionTools.ConnectTools;

public sealed class MyFileInfo
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public MyFileInfo(string fileName, long fileLength)
    {
        FileName = fileName;
        FileLength = fileLength;
    }

    public string FileName { get; set; }
    public long FileLength { get; set; }
}

namespace ConnectTools;

public sealed class MyFileInfo
{
    public MyFileInfo(string fileName, long fileLength)
    {
        FileName = fileName;
        FileLength = fileLength;
    }

    public string FileName { get; set; }
    public long FileLength { get; set; }
}
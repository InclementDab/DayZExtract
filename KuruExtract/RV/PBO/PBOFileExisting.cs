﻿using KuruExtract.RV.Config;

namespace KuruExtract.RV.PBO;

internal sealed class PBOFileExisting : IPBOFileEntry
{
    private readonly FileEntry _fileEntry;
    private readonly PBO _pbo;

    public PBOFileExisting(FileEntry fileEntry, PBO pbo)
    {
        _fileEntry = fileEntry;
        _pbo = pbo;
    }

    public string FileName => _fileEntry.FileName;

    public int TimeStamp => _fileEntry.TimeStamp;

    public int Size => _fileEntry.IsCompressed ? _fileEntry.UncompressedSize : _fileEntry.DataSize;

    public bool IsCompressed => _fileEntry.IsCompressed;

    public int DiskSize => _fileEntry.DataSize;

    public Stream OpenRead()
    {
        return _pbo.GetFileEntryStream(_fileEntry);
    }

    public void Extract(string target)
    {
        var fileName = FileName.Replace("config.bin", "config.cpp");

        var path = Path.Combine(target, fileName);
        var dir = Path.GetDirectoryName(path);

        if (dir != null)
            Directory.CreateDirectory(dir);

        using var targetFile = File.Create(path);
        using var source = OpenRead();

        if (FileName.Contains("config.bin"))
        {
            using var writer = new StreamWriter(targetFile);
            var param = new ParamFile(source);

            writer.Write(param.ToString());
            return;
        }

        source.CopyTo(targetFile);
    }
}
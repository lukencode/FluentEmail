using System;
using System.IO;

namespace FluentEmail.MailPace;

public static class StreamExtensions
{
    public static string ConvertToBase64(this Stream stream)
    {
        if (stream is MemoryStream memoryStream)
        {
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        var bytes = new Byte[(int)stream.Length];

        stream.Seek(0, SeekOrigin.Begin);
        // ReSharper disable once MustUseReturnValue
        stream.Read(bytes, 0, (int)stream.Length);

        return Convert.ToBase64String(bytes);
    }
}
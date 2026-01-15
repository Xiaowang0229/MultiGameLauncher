using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaZi.Library.Json;

public class Json
{
    public static T ReadJson<T>(string contentOrPath)
    {
        string value = ((!File.Exists(contentOrPath)) ? contentOrPath : File.ReadAllText(contentOrPath));
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new Exception("JSON 内容为空");
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
        catch (JsonException innerException)
        {
            throw new Exception("无效的 JSON 内容", innerException);
        }
    }

    public static void WriteJson<T>(string filePath, T data)
    {
        JsonSerializer jsonSerializer = new JsonSerializer
        {
            Formatting = Formatting.Indented
        };
        using StreamWriter textWriter = new StreamWriter(filePath);
        using JsonTextWriter jsonTextWriter = new JsonTextWriter(textWriter);
        jsonTextWriter.Formatting = Formatting.Indented;
        jsonTextWriter.IndentChar = ' ';
        jsonTextWriter.Indentation = 4;
        jsonSerializer.Serialize(jsonTextWriter, data);
    }
}
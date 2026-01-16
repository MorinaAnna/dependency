using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Serialization;
using UriSerializationHelper;

namespace JsonSerializer.Serialization;

public class JsonSerializerTechnology : IDataSerializer<Uri>
{
    private static readonly JsonSerializerOptions JsonOptions = new ()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly string path;

    public JsonSerializerTechnology(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        this.path = path;
    }

    public void Serialize(IEnumerable<Uri>? source)
    {
        ArgumentNullException.ThrowIfNull(source);

        List<UriAddress> items = source.Select(u => u.ToSerializableObject()).ToList();

        string json = System.Text.Json.JsonSerializer.Serialize(items, JsonOptions);
        File.WriteAllText(this.path, json);
    }
}

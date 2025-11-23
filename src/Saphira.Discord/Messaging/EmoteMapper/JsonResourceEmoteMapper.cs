using System.Text.Json;

namespace Saphira.Discord.Messaging.EmoteMapper;

public class JsonResourceEmoteMapper<T> where T : IEmoteData
{
    private readonly string _resourceFileName;
    private readonly string? _defaultEmote;

    private readonly Lazy<Dictionary<string, string>> _emotes;

    public JsonResourceEmoteMapper(string resourceFileName, string? defaultEmote = null)
    {
        _resourceFileName = resourceFileName;
        _defaultEmote = defaultEmote;
        _emotes = new Lazy<Dictionary<string, string>>(LoadEmotes);
    }

    public string? MapToEmote(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return _defaultEmote;
        }

        return _emotes.Value.TryGetValue(name, out var emote) ? emote : null;
    }

    private Dictionary<string, string> LoadEmotes()
    {
        try
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", _resourceFileName);
            var jsonContent = File.ReadAllText(jsonPath);
            var items = JsonSerializer.Deserialize<List<T>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return items?
                .Where(item => !string.IsNullOrWhiteSpace(item.Emote))
                .ToDictionary(item => item.Name, item => item.Emote, StringComparer.OrdinalIgnoreCase)
                ?? [];
        }
        catch
        {
            return [];
        }
    }
}

public interface IEmoteData
{
    string Name { get; }
    string Emote { get; }
}

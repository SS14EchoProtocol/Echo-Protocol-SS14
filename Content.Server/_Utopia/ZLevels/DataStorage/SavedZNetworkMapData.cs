using System.Text.Json.Serialization;

namespace Content.Server._Utopia.ZLevels;

public sealed class SavedZNetworkMapData
{
    public Dictionary<int, string> LevelPaths { get; set; } = new();
}

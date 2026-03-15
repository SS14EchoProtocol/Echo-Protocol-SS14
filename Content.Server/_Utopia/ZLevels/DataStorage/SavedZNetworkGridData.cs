using System.Numerics;

namespace Content.Server._Utopia.ZLevels;

public sealed class SavedZNetworkGridData
{
    public Dictionary<int, List<SavedGridLevel>> LevelPaths { get; set; } = new();
}

public sealed class SavedGridLevel
{
    public string Path { get; set; } = "";
    public Vector2 Offset { get; set; } = Vector2.Zero;

    public SavedGridLevel(string path, Vector2 offset)
    {
        Path = path;
        Offset = offset;
    }
}

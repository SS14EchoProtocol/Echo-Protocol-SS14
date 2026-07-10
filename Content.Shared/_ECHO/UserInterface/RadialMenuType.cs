using Robust.Shared.Serialization;

namespace Content.Shared._ECHO.UserInterface;

[System.Serializable, NetSerializable]
public enum RadialMenuType : int
{
    FullDecorations = 4,
    OuterLines = 3,
    Simple = 2,
    Legacy = 1
}

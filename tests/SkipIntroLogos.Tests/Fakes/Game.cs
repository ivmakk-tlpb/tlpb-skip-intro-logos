// Fake game types: only the types and members that the mod source reaches.

public class UIManager
{
    public void FadeScreenOverlayIn(float duration, Il2CppSystem.Action callback) { }

    public void FadeScreenOverlayOut(float duration, Il2CppSystem.Action callback) { }
}

public enum WorldState
{
    UNLOADED = 0,
    LOADING = 1,
    LOADED = 2,
    ACTIVE = 3,
}

// A world with the two view distance fields and the game's lerps of SetViewDistance.
public class World : Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase
{
    public float minViewDistance;
    public float maxViewDistance;
    public readonly List<float> ViewDistanceCalls = new List<float>();

    public void SetViewDistance(float viewDistance)
    {
        ViewDistanceCalls.Add(viewDistance);
        minViewDistance = 34f + (50f - 34f) * viewDistance;
        maxViewDistance = 42f + (90f - 42f) * viewDistance;
    }

    public void SetWorldState(WorldState state) { }
}

public class WorldManager
{
    private void setViewDistance(World world) { }

    private void setViewDistance(World world, float viewDistance) { }
}

namespace Il2CppSystem
{
    public class Action : Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase { }
}

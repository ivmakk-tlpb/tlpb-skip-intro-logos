// Fake Unity types: only the members that the mod source reaches.

namespace UnityEngine
{
    public class Object : Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase
    {
        public Object() { }

        public Object(IntPtr pointer) : base(pointer) { }

        // The objects that Destroy received.
        public static readonly List<Object> Destroyed = new List<Object>();

        public static void Destroy(Object obj) { Destroyed.Add(obj); }
    }

    public class Component : Object
    {
        public Component() { }

        public Component(IntPtr pointer) : base(pointer) { }
    }

    public class Behaviour : Component
    {
        public Behaviour() { }

        public Behaviour(IntPtr pointer) : base(pointer) { }
    }

    public class MonoBehaviour : Behaviour
    {
        public MonoBehaviour() { }

        public MonoBehaviour(IntPtr pointer) : base(pointer) { }
    }
}

namespace UnityEngine.Rendering
{
    // Records each stop call. A test sets OnStop to act on a call, for example to throw.
    public static class SplashScreen
    {
        public enum StopBehavior
        {
            StopImmediate = 0,
            FadeOut = 1,
        }

        public static readonly List<StopBehavior> Stops = new List<StopBehavior>();
        public static Action OnStop;

        public static void Stop(StopBehavior stopBehavior)
        {
            Stops.Add(stopBehavior);
            OnStop?.Invoke();
        }
    }
}

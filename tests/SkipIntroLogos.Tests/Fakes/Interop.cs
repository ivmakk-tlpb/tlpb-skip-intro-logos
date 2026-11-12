// Fake IL2CPP interop types: only the members that the mod source reaches.

namespace Il2CppInterop.Runtime.InteropTypes
{
    public class Il2CppObjectBase
    {
        public Il2CppObjectBase() { }

        public Il2CppObjectBase(IntPtr pointer) { }
    }
}

namespace Il2CppInterop.Runtime.Injection
{
    // Records each registered type. A test sets Throw to make the registration fail.
    public static class ClassInjector
    {
        public static readonly List<Type> Registered = new List<Type>();
        public static bool Throw;

        public static void RegisterTypeInIl2Cpp<T>() where T : class
        {
            if (Throw) throw new InvalidOperationException("Cannot register");
            Registered.Add(typeof(T));
        }
    }
}

namespace Il2CppInterop.Runtime
{
    // Records the attach and the detach of a thread to the IL2CPP domain.
    public static class IL2CPP
    {
        public static readonly IntPtr Domain = new IntPtr(0x1000);
        public static readonly IntPtr Thread = new IntPtr(0x2000);
        public static readonly List<string> Calls = new List<string>();

        public static IntPtr il2cpp_domain_get() => Domain;

        public static IntPtr il2cpp_thread_attach(IntPtr domain)
        {
            Calls.Add("attach " + domain.ToInt64());
            return Thread;
        }

        public static void il2cpp_thread_detach(IntPtr thread)
        {
            Calls.Add("detach " + thread.ToInt64());
        }
    }
}

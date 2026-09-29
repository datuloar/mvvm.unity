namespace MvvmUnity.Editor
{
    internal readonly struct ObserverBinding
    {
        public ObserverBinding(string method, string source)
        {
            Method = method;
            Source = source;
        }

        public string Method { get; }

        public string Source { get; }
    }
}

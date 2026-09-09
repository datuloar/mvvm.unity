using System;

namespace MvvmUnity.Core
{
    public static class ObservableSubscriptions
    {
        public static IDisposable Subscribe<T>(
            this IReadOnlyObservableValue<T> source,
            Action<T> changed,
            bool emitCurrent = true)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (changed == null)
                throw new ArgumentNullException(nameof(changed));
            source.Changed += changed;
            if (emitCurrent)
                changed(source.Value);
            return new ActionDisposable(() => source.Changed -= changed);
        }
    }
}

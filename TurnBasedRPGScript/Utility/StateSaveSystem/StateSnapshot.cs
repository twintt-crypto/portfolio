using System.Collections.Generic;

namespace S7
{
    public class StateSnapshot
    {
        private readonly Dictionary<int, object> _states = new Dictionary<int, object>();

        public int Count => _states.Count;

        public void Set<T>(int instanceId, T state)
        {
            _states[instanceId] = state;
        }

        public bool TryGet<T>(int instanceId, out T state)
        {
            if (_states.TryGetValue(instanceId, out object obj) && obj is T typedState)
            {
                state = typedState;
                return true;
            }
            state = default;
            return false;
        }

        public void Clear() => _states.Clear();
    }
}

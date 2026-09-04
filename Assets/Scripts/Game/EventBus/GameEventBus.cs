using System;

namespace S7
{
    public static class GameEventBus
    {
        public static event Action<GameEvent> OnEvent;

        public static void Raise(GameEvent e)
        {
            OnEvent?.Invoke(e);
        }
    }
}

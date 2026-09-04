using System;

namespace WaddleRush.Core
{
    public enum AppState { Boot, LoadProfile, MainMenu, LoadingMatch, Countdown, Playing, Results, Rematch }
    public enum PlayerState { Spawning, Active, CroppedRecovery, Critical, Eliminated }
    public enum BotState { Spawning, Farming, Searching, Attacking, Escaping, Recovering, Eliminated }

    public sealed class StateMachine<T> where T : struct
    {
        public T State { get; private set; }
        public event Action<T, T> Changed;
        public StateMachine(T initial) { State = initial; }
        public bool Transition(T next)
        {
            if (State.Equals(next)) return false;
            var old = State; State = next; Changed?.Invoke(old, next); return true;
        }
    }

    public static class GameConstants
    {
        public const string GameName = "WaddleRush";
        public const string Version = "0.1.0";
        public const string PackageId = "com.example.waddlerush";
    }
}

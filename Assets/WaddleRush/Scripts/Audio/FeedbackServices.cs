using UnityEngine;

namespace WaddleRush.Audio
{
    public enum AudioEvent { FishPickup, GoldenFish, Fusion, MegaFusion, SmallCrop, BigCrop, Boost, Countdown, Victory, Defeat, Button }
    public enum HapticEvent { Tiny, Medium, Heavy }
    public interface IAudioService { void Play(AudioEvent audioEvent, Vector3 position); void SetVolumes(float music, float effects); }
    public interface IHapticService { bool Enabled { get; set; } void Play(HapticEvent hapticEvent); }
    public sealed class SilentAudioService : IAudioService { public void Play(AudioEvent e,Vector3 p){} public void SetVolumes(float m,float e){} }
    public sealed class NullHapticService : IHapticService { public bool Enabled { get;set; } public void Play(HapticEvent e){} }
}

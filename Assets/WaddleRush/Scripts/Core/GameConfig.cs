using UnityEngine;

namespace WaddleRush.Core
{
    [CreateAssetMenu(menuName = "WaddleRush/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        [Header("Match")] public int botCount = 17;
        public float matchSeconds = 180f, arenaHalfSize = 58f;
        public int randomSeed;
        [Header("Movement")] public float speed = 8f, boostSpeed = 12.5f, turnDegreesPerSecond = 170f, boostTurnMultiplier = .72f;
        [Header("Rules")] public float followerSpacing = 1.25f, cropImmunity = 1.25f;
        public int smallFish = 1, mediumFish = 3, goldenFish = 8;
        [Header("Performance")] public int trailCapacity = 2048, fishCount = 95;
        public static GameConfig Defaults() => CreateInstance<GameConfig>();
    }
}

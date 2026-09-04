using System;
using WaddleRush.Core;
using WaddleRush.Simulation;

namespace WaddleRush.AI
{
    public enum BotArchetype { Farmer, Hunter, Ambusher, Bully, Coward, Pro, Chaotic, Opportunist }
    public struct BotPersonality
    {
        public BotArchetype Archetype;
        public float Aggression, Greed, RiskTolerance, ReactionSpeed, PredictionSkill, BoostUsage, Persistence;
        public static BotPersonality Create(BotArchetype type, Random random)
        {
            float R(float center) => Math.Max(0, Math.Min(1, center + (float)(random.NextDouble() - .5) * .3f));
            var aggressive = type == BotArchetype.Hunter || type == BotArchetype.Bully || type == BotArchetype.Pro;
            return new BotPersonality { Archetype = type, Aggression = R(aggressive ? .8f : .35f), Greed = R(type == BotArchetype.Farmer ? .9f : .55f), RiskTolerance = R(aggressive ? .72f : .35f), ReactionSpeed = R(type == BotArchetype.Pro ? .9f : .55f), PredictionSkill = R(type == BotArchetype.Pro ? .9f : .45f), BoostUsage = R(.55f), Persistence = R(.5f) };
        }
    }

    public struct BotContext { public Vec2 Position, Forward, NearestFood, ThreatPosition, TargetPosition; public float BorderDistance, ThreatDistance, TargetDistance; public bool HasFood, HasThreat, HasTarget; }
    public struct BotDecision { public Vec2 DesiredDirection; public bool Boost; public BotState State; }

    public static class BotUtilityEvaluator
    {
        public static BotDecision Decide(BotPersonality p, BotContext c)
        {
            if (c.BorderDistance < 4f)
                return new BotDecision { State = BotState.Escaping, DesiredDirection = (c.Position * -1f).Normalized, Boost = p.BoostUsage > .25f };
            if (c.HasThreat && (c.ThreatDistance < 6f + (1f - p.RiskTolerance) * 8f || c.BorderDistance < 4f))
                return new BotDecision { State = BotState.Escaping, DesiredDirection = (c.Position - c.ThreatPosition).Normalized, Boost = p.BoostUsage > .25f };
            if (c.HasTarget && p.Aggression > .58f && c.TargetDistance < 28f)
                return new BotDecision { State = BotState.Attacking, DesiredDirection = (c.TargetPosition - c.Position).Normalized, Boost = p.BoostUsage > .6f };
            if (c.HasFood) return new BotDecision { State = BotState.Farming, DesiredDirection = (c.NearestFood - c.Position).Normalized };
            return new BotDecision { State = BotState.Searching, DesiredDirection = c.Forward };
        }
    }
}

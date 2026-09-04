using System;
using System.Collections.Generic;
using UnityEngine;
using WaddleRush.AI;
using WaddleRush.Core;
using WaddleRush.Simulation;

namespace WaddleRush.Presentation
{
    public sealed class ColonyActor
    {
        public readonly int Id; public readonly bool IsPlayer; public readonly string Name;
        public readonly Colony Colony; public readonly TrailBuffer Trail;
        public readonly GameObject Head;
        public int Score, Fish, Cropped, Fusions, Emperors;
        public Vector3 Position => Head.transform.position;
        public Vector3 Forward => Head.transform.forward;
        public readonly List<GameObject> Followers = new List<GameObject>();
        readonly Color accent; readonly GameConfig config; readonly System.Random random;
        BotPersonality personality; float heading, decisionAt, boostEnergy = 1f; Vec2 desired;

        public ColonyActor(int id, string name, bool player, Vector3 position, Color color, GameConfig config, System.Random random)
        {
            Id = id; Name = name; IsPlayer = player; accent = color; this.config = config; this.random = random;
            Colony = new Colony(3); Trail = new TrailBuffer(config.trailCapacity);
            Head = PrototypeFactory.Penguin(name, color, 1.12f); Head.transform.position = position;
            heading = (float)random.NextDouble() * 360f; Head.transform.rotation = Quaternion.Euler(0, heading, 0);
            personality = BotPersonality.Create((BotArchetype)(id % 8), random); desired = new Vec2(Forward.x, Forward.z);
            RebuildFollowers();
            for (var i = 0; i < 50; i++) Trail.Add(new Vec2(position.x - Forward.x * i * .12f, position.z - Forward.z * i * .12f), new Vec2(Forward.x, Forward.z), 0);
        }

        public void Tick(float dt, Vector2 playerInput, bool boost, IList<Transform> fish, IList<ColonyActor> actors, float now)
        {
            if (IsPlayer && playerInput.sqrMagnitude > .03f) desired = new Vec2(playerInput.x, playerInput.y).Normalized;
            else if (!IsPlayer && now >= decisionAt) { decisionAt = now + Mathf.Lerp(.12f, .55f, 1f - personality.ReactionSpeed); Think(fish, actors); boost = personality.BoostUsage > .7f && random.NextDouble() < .18; }
            var targetAngle = Mathf.Atan2(desired.X, desired.Y) * Mathf.Rad2Deg;
            var boosting = boost && boostEnergy > .02f;
            heading = Mathf.MoveTowardsAngle(heading, targetAngle, config.turnDegreesPerSecond * (boosting ? config.boostTurnMultiplier : 1f) * dt);
            Head.transform.rotation = Quaternion.Euler(0, heading, 0);
            Head.transform.position += Head.transform.forward * (boosting ? config.boostSpeed : config.speed) * dt;
            boostEnergy = Mathf.Clamp01(boostEnergy + (boosting ? -.23f : .12f) * dt);
            var p = Head.transform.position; p.x = Mathf.Clamp(p.x, -config.arenaHalfSize, config.arenaHalfSize); p.z = Mathf.Clamp(p.z, -config.arenaHalfSize, config.arenaHalfSize); Head.transform.position = p;
            Trail.Add(new Vec2(p.x, p.z), new Vec2(Forward.x, Forward.z));
            UpdateFollowers();
        }

        void Think(IList<Transform> fish, IList<ColonyActor> actors)
        {
            var pos = new Vec2(Position.x, Position.z); var context = new BotContext { Position = pos, Forward = new Vec2(Forward.x, Forward.z), BorderDistance = config.arenaHalfSize - Mathf.Max(Mathf.Abs(Position.x), Mathf.Abs(Position.z)) };
            var best = float.MaxValue;
            for (var i = 0; i < fish.Count; i++) if (fish[i] && (fish[i].position - Position).sqrMagnitude < best) { best = (fish[i].position - Position).sqrMagnitude; context.NearestFood = new Vec2(fish[i].position.x, fish[i].position.z); context.HasFood = true; }
            for (var i = 0; i < actors.Count; i++) if (actors[i] != this) { var d = Vector3.Distance(Position, actors[i].Position); if (!context.HasTarget || d < context.TargetDistance) { context.HasTarget = true; context.TargetDistance = d; context.TargetPosition = new Vec2(actors[i].Position.x, actors[i].Position.z); } }
            var decision = BotUtilityEvaluator.Decide(personality, context); desired = decision.DesiredDirection;
            if (context.BorderDistance < 5f) desired = (pos * -1f).Normalized;
            if (personality.Archetype == BotArchetype.Chaotic) desired = new Vec2(desired.X + Mathf.Sin(Time.time * 2f + Id), desired.Y + Mathf.Cos(Time.time * 1.7f + Id)).Normalized;
        }

        public FusionResult AddMass(int mass)
        {
            Fish += mass; for (var i = 0; i < mass; i++) Colony.Units.Add(new PenguinUnit(PenguinTier.Chick));
            var result = FusionSystem.Resolve(Colony); Fusions += result.Fusions; Emperors += result.EmperorsCreated; RebuildFollowers(); return result;
        }

        public CropResult Crop(int hitIndex, float now)
        {
            var result = CropCalculator.CropAfter(Colony, hitIndex, now, config.cropImmunity); if (result.Applied) RebuildFollowers(); return result;
        }

        void RebuildFollowers()
        {
            while (Followers.Count > Colony.Units.Count) { UnityEngine.Object.Destroy(Followers[Followers.Count - 1]); Followers.RemoveAt(Followers.Count - 1); }
            while (Followers.Count < Colony.Units.Count) Followers.Add(PrototypeFactory.Penguin(Name + " follower", accent, .76f));
            for (var i = 0; i < Followers.Count; i++) Followers[i].transform.localScale = Vector3.one * (Colony.Units[i].Tier == PenguinTier.Chick ? .72f : Colony.Units[i].Tier == PenguinTier.Scout ? .95f : 1.2f);
        }

        void UpdateFollowers()
        {
            var behind = 1.5f;
            for (var i = 0; i < Followers.Count; i++) { var sample = Trail.SampleBehind(behind); Followers[i].transform.position = new Vector3(sample.Position.X, .03f, sample.Position.Y); Followers[i].transform.rotation = Quaternion.LookRotation(new Vector3(sample.Direction.X, 0, sample.Direction.Y)); behind += Colony.Units[i].Footprint; }
        }

        public void Dispose() { UnityEngine.Object.Destroy(Head); for (var i = 0; i < Followers.Count; i++) UnityEngine.Object.Destroy(Followers[i]); }
    }
}

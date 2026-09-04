using System;
using System.Collections.Generic;

namespace WaddleRush.Simulation
{
    public enum PenguinTier { Chick = 1, Scout = 3, Emperor = 9 }

    [Serializable]
    public struct PenguinUnit
    {
        public PenguinTier Tier;
        public int Mass => (int)Tier;
        public float Footprint => Tier == PenguinTier.Chick ? 1.15f : Tier == PenguinTier.Scout ? 1.85f : 2.7f;
        public PenguinUnit(PenguinTier tier) { Tier = tier; }
    }

    public sealed class Colony
    {
        public readonly List<PenguinUnit> Units = new List<PenguinUnit>();
        public float CropImmuneUntil;
        public int TotalMass { get { var n = 0; for (var i = 0; i < Units.Count; i++) n += Units[i].Mass; return n; } }
        public float TrailLength { get { var n = 0f; for (var i = 0; i < Units.Count; i++) n += Units[i].Footprint; return n; } }
        public Colony(int chicks = 3) { for (var i = 0; i < chicks; i++) Units.Add(new PenguinUnit(PenguinTier.Chick)); }
    }

    public struct FusionResult { public int Fusions, EmperorsCreated; }

    public static class FusionSystem
    {
        public static FusionResult Resolve(Colony colony)
        {
            var result = new FusionResult();
            bool changed;
            do
            {
                changed = FuseThree(colony, PenguinTier.Chick, PenguinTier.Scout);
                if (changed) result.Fusions++;
                if (FuseThree(colony, PenguinTier.Scout, PenguinTier.Emperor))
                { result.Fusions++; result.EmperorsCreated++; changed = true; }
            } while (changed);
            return result;
        }

        static bool FuseThree(Colony colony, PenguinTier from, PenguinTier to)
        {
            var found = 0;
            for (var i = 0; i < colony.Units.Count; i++)
                if (colony.Units[i].Tier == from && ++found == 3)
                {
                    for (var j = colony.Units.Count - 1; j >= 0 && found > 0; j--)
                        if (colony.Units[j].Tier == from) { colony.Units.RemoveAt(j); found--; }
                    colony.Units.Add(new PenguinUnit(to));
                    return true;
                }
            return false;
        }
    }

    public struct CropResult { public int LostMass, RemovedUnits, RemainingMass; public bool Applied; }

    public static class CropCalculator
    {
        public static CropResult CropAfter(Colony victim, int hitIndex, float now, float immunitySeconds = 1.25f, int protectedUnits = 1)
        {
            if (now < victim.CropImmuneUntil || victim.Units.Count <= protectedUnits) return new CropResult { RemainingMass = victim.TotalMass };
            var firstRemove = Math.Max(protectedUnits, Math.Min(victim.Units.Count, hitIndex + 1));
            var result = new CropResult { RemainingMass = victim.TotalMass };
            for (var i = victim.Units.Count - 1; i >= firstRemove; i--)
            { result.LostMass += victim.Units[i].Mass; result.RemovedUnits++; victim.Units.RemoveAt(i); }
            result.Applied = result.RemovedUnits > 0;
            result.RemainingMass = victim.TotalMass;
            if (result.Applied) victim.CropImmuneUntil = now + immunitySeconds;
            return result;
        }

        public static CropResult MajorCrop(Colony victim, float fraction, float now)
        {
            var remove = Math.Max(1, (int)Math.Ceiling(victim.Units.Count * Math.Max(.35f, Math.Min(.5f, fraction))));
            return CropAfter(victim, Math.Max(0, victim.Units.Count - remove - 1), now);
        }
    }

    public static class DroppedFishCalculator
    {
        public static List<int> Convert(int mass, int maxPickups = 12)
        {
            var values = new List<int>();
            while (mass > 0 && values.Count < maxPickups)
            {
                var slots = maxPickups - values.Count;
                var value = mass >= 8 && mass - 8 >= slots - 1 ? 8 : mass >= 3 && mass - 3 >= slots - 1 ? 3 : 1;
                values.Add(value); mass -= value;
            }
            if (mass > 0) values[values.Count - 1] += mass;
            return values;
        }
    }

    public static class ScoreCalculator
    { public static int Calculate(int mass, int fish, int croppedMass, int rank) => mass * 10 + fish * 2 + croppedMass * 15 + Math.Max(0, 6 - rank) * 50; }
}

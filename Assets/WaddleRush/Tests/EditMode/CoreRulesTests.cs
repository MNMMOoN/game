using System.IO;
using NUnit.Framework;
using UnityEngine;
using WaddleRush.AI;
using WaddleRush.Core;
using WaddleRush.Persistence;
using WaddleRush.Simulation;

namespace WaddleRush.Tests
{
    public sealed class CoreRulesTests
    {
        [Test] public void TrailInsertionAndSampling()
        {
            var t = new TrailBuffer(8); for (var i=0;i<5;i++) t.Add(new Vec2(i,0), new Vec2(1,0), 0);
            Assert.AreEqual(5,t.Count); Assert.AreEqual(2f,t.SampleBehind(2).Position.X,.001f);
        }

        [Test] public void TrailWraparoundKeepsNewest()
        {
            var t=new TrailBuffer(3); for(var i=0;i<6;i++) t.Add(new Vec2(i,0),new Vec2(1,0),0);
            Assert.AreEqual(3,t.Count); Assert.AreEqual(3f,t.Get(0).Position.X);
        }

        [Test] public void CropPreservesFrontAndImmunity()
        {
            var c=new Colony(8); var r=CropCalculator.CropAfter(c,3,10); Assert.AreEqual(4,r.LostMass); Assert.AreEqual(4,c.TotalMass);
            Assert.IsFalse(CropCalculator.CropAfter(c,1,10.5f).Applied); Assert.IsTrue(CropCalculator.CropAfter(c,1,12f).Applied);
        }

        [Test] public void CropProtectsMinimumSurvivor()
        { var c=new Colony(2); CropCalculator.CropAfter(c,-1,1); Assert.AreEqual(1,c.Units.Count); }

        [Test] public void FusionConservesMassAndCompressesTrail()
        {
            var c=new Colony(9); var before=c.TrailLength; var result=FusionSystem.Resolve(c);
            Assert.AreEqual(9,c.TotalMass); Assert.AreEqual(1,c.Units.Count); Assert.AreEqual(PenguinTier.Emperor,c.Units[0].Tier); Assert.Less(c.TrailLength,before); Assert.AreEqual(1,result.EmperorsCreated);
        }

        [Test] public void ThreeChicksBecomeScout()
        { var c=new Colony(3); FusionSystem.Resolve(c); Assert.AreEqual(PenguinTier.Scout,c.Units[0].Tier); }

        [Test] public void DroppedFishPreservesValueAndBoundsObjects()
        { var d=DroppedFishCalculator.Convert(47,10); Assert.LessOrEqual(d.Count,10); Assert.AreEqual(47,d.ConvertAll(x=>x).ToArray()[0] + SumRest(d)); }
        static int SumRest(System.Collections.Generic.List<int> d) { var n=0; for(var i=1;i<d.Count;i++)n+=d[i]; return n; }

        [Test] public void SpatialHashReturnsNearbySegment()
        { var g=new SpatialHashGrid(4); g.Insert(new TrailSegment(2,3,new Vec2(0,0),new Vec2(3,0),.5f)); var r=new System.Collections.Generic.List<TrailSegment>(); g.Query(new Vec2(1,0),1,r); Assert.Greater(r.Count,0); }

        [Test] public void StateMachineTransitionsOnce()
        { var s=new StateMachine<PlayerState>(PlayerState.Spawning); Assert.IsTrue(s.Transition(PlayerState.Active)); Assert.IsFalse(s.Transition(PlayerState.Active)); }

        [Test] public void BotAvoidsBorderWithoutCheating()
        { var p=BotPersonality.Create(BotArchetype.Farmer,new System.Random(1)); var d=BotUtilityEvaluator.Decide(p,new BotContext{Position=new Vec2(57,0),Forward=new Vec2(1,0),BorderDistance=1}); Assert.AreEqual(BotState.Escaping,d.State); Assert.Less(d.DesiredDirection.X,0); }

        [Test] public void SaveLoadAndCorruptionRecovery()
        {
            var dir=Path.Combine(Application.temporaryCachePath,"waddlerush-test-"+System.Guid.NewGuid()); Directory.CreateDirectory(dir);
            try { var s=new LocalSaveService(dir); var p=new ProfileData{coins=42}; s.Save(p); p.coins=13; s.Save(p); File.WriteAllText(Path.Combine(dir,"profile.json"),"broken"); Assert.AreEqual(42,s.Load().coins); }
            finally { Directory.Delete(dir,true); }
        }
    }
}

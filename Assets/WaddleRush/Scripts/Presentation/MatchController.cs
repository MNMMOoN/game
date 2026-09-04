using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WaddleRush.Core;
using WaddleRush.AI;
using WaddleRush.Persistence;
using WaddleRush.Simulation;

namespace WaddleRush.Presentation
{
    public struct MatchStats { public int Rank, Score, MaxMass, BiggestCrop, Fish, Fusions, Emperors; }

    public sealed class MatchController : MonoBehaviour
    {
        readonly List<ColonyActor> actors = new List<ColonyActor>(); readonly List<Transform> fish = new List<Transform>();
        readonly SpatialHashGrid collisionGrid = new SpatialHashGrid(4f); readonly List<TrailSegment> collisionCandidates = new List<TrailSegment>(64);
        GameConfig config; ColonyActor player; System.Random random; float left, leaderboardAt; Text hud, leaderboard; GameBootstrap bootstrap; bool boost; GameObjectPool fishPool;
        int maxMass, biggestCrop;

        public void Initialize(GameConfig cfg, ProfileData profile, ILocalSaveService saves, GameBootstrap owner)
        {
            config = cfg; bootstrap = owner; if(System.Array.Exists(Environment.GetCommandLineArgs(),x=>x=="--waddlerush-stress")){cfg.botCount=24;cfg.fishCount=180;} var seed = cfg.randomSeed != 0 ? cfg.randomSeed : Environment.TickCount; random = new System.Random(seed); left = cfg.matchSeconds;
            BuildArena(); SpawnActors(); SpawnFish(cfg.fishCount); BuildHud(seed);
        }

        void BuildArena()
        {
            var ice = GameObject.CreatePrimitive(PrimitiveType.Cube); ice.name = "Aurora Ice Bay"; ice.transform.SetParent(transform); ice.transform.position = new Vector3(0, -.65f, 0); ice.transform.localScale = new Vector3(120, 1, 120); ice.GetComponent<Renderer>().sharedMaterial = PrototypeFactory.Material(new Color(.22f, .78f, .84f)); Destroy(ice.GetComponent<Collider>());
            var light = new GameObject("Aurora Light").AddComponent<Light>(); light.transform.SetParent(transform); light.type = LightType.Directional; light.intensity = 1.15f; light.transform.rotation = Quaternion.Euler(50, -35, 0);
            RenderSettings.ambientLight = new Color(.42f, .55f, .63f);
        }

        void SpawnActors()
        {
            player = new ColonyActor(0, "PLAYER", true, Vector3.zero, new Color(.15f, .95f, 1f), config, random); actors.Add(player);
            for (var i = 1; i <= config.botCount; i++) { var p = new Vector3(Range(-48, 48), 0, Range(-48, 48)); var color = Color.HSVToRGB((float)random.NextDouble(), .72f, 1f); var bot = new ColonyActor(i, BotNames.Get(i * 7 + random.Next(BotNames.Count)), false, p, color, config, random); actors.Add(bot); if (i % 4 == 0) bot.AddMass(random.Next(3, 11)); }
            var cam = Camera.main; cam.transform.position = new Vector3(0, 17, -13); cam.transform.rotation = Quaternion.Euler(55, 0, 0); var follow = cam.gameObject.GetComponent<CameraFollower>() ?? cam.gameObject.AddComponent<CameraFollower>(); follow.enabled=true; follow.target = player.Head.transform;
        }

        void SpawnFish(int count) { fishPool=new GameObjectPool(CreateFishObject,transform,count); for (var i = 0; i < count; i++) SpawnFishAt(new Vector3(Range(-55,55), .05f, Range(-55,55)), random.NextDouble() < .08 ? 8 : random.NextDouble() < .25 ? 3 : 1); }
        GameObject CreateFishObject(){var go=GameObject.CreatePrimitive(PrimitiveType.Sphere);Destroy(go.GetComponent<Collider>());return go;}
        void SpawnFishAt(Vector3 at, int value)
        {
            var go = fishPool.Get(); go.name = "Fish:" + value; go.transform.position = at; go.transform.localScale = Vector3.one * (value == 8 ? .65f : value == 3 ? .48f : .34f); go.GetComponent<Renderer>().sharedMaterial = PrototypeFactory.Material(value == 8 ? new Color(1f,.75f,.05f) : new Color(.95f,.42f,.18f)); fish.Add(go.transform);
        }

        void BuildHud(int seed)
        {
            var canvasGo = new GameObject("Match HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)); canvasGo.transform.SetParent(transform); var canvas = canvasGo.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; var scale = canvasGo.GetComponent<CanvasScaler>(); scale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scale.referenceResolution = new Vector2(1080,1920);
            hud = GameBootstrap.Title(canvas.transform, "", new Vector2(0, 420), 25); leaderboard = GameBootstrap.Title(canvas.transform, "", new Vector2(330, 260), 19);
            var b = GameBootstrap.Button(canvas.transform, "BOOST", new Vector2(370,-720), new Vector2(240,150), delegate { });
            var trigger = b.gameObject.AddComponent<HoldButton>(); trigger.Changed += value => boost = value;
            GameBootstrap.Title(canvas.transform, "DRAG ANYWHERE TO STEER   •   Seed " + seed, new Vector2(0,-500), 18);
        }

        void Update()
        {
            if (config == null) return; var input = ReadInput(); left -= Time.deltaTime;
            for (var i = 0; i < actors.Count; i++) actors[i].Tick(Time.deltaTime, input, boost, fish, actors, Time.time);
            CollectFish(); CheckCrops(); maxMass = Mathf.Max(maxMass, player.Colony.TotalMass);
            hud.text = "#" + Rank(player) + "    SCORE " + Score(player) + "\nMASS " + player.Colony.TotalMass + "     " + Mathf.CeilToInt(left) + "s";
            if (Time.time >= leaderboardAt) { leaderboardAt = Time.time + .35f; UpdateLeaderboard(); }
            if (left <= 0) Finish();
        }

        Vector2 ReadInput()
        {
            if (Input.touchCount > 0) { var t = Input.GetTouch(0); if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled) return (t.deltaPosition.sqrMagnitude > 1 ? t.deltaPosition : t.position - new Vector2(Screen.width, Screen.height) * .5f).normalized; }
            if (Input.GetMouseButton(0)) { var p = (Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) * .5f; return p.normalized; }
            return Vector2.zero;
        }

        void CollectFish()
        {
            for (var a = 0; a < actors.Count; a++) for (var i = fish.Count - 1; i >= 0; i--) if (fish[i] && (fish[i].position - actors[a].Position).sqrMagnitude < 1.3f)
            { var value = int.Parse(fish[i].name.Substring(5)); fishPool.Release(fish[i].gameObject); fish.RemoveAt(i); actors[a].AddMass(value); SpawnFishAt(new Vector3(Range(-55,55),.05f,Range(-55,55)), random.NextDouble() < .08 ? 8 : 1); }
        }

        void CheckCrops()
        {
            collisionGrid.Clear();
            for(var a=0;a<actors.Count;a++) for(var i=1;i<actors[a].Followers.Count;i++)
            {var p0=actors[a].Followers[i-1].transform.position;var p1=actors[a].Followers[i].transform.position;collisionGrid.Insert(new TrailSegment(actors[a].Id,i,new Vec2(p0.x,p0.z),new Vec2(p1.x,p1.z),.45f));}
            for(var a=0;a<actors.Count;a++)
            {
                var point=new Vec2(actors[a].Position.x,actors[a].Position.z);collisionGrid.Query(point,.65f,collisionCandidates);
                for(var c=0;c<collisionCandidates.Count;c++)
                {
                    var segment=collisionCandidates[c];if(segment.OwnerId==actors[a].Id||Geometry2D.DistancePointSegment(point,segment.A,segment.B)>.7f)continue;
                    ColonyActor victim=null;for(var v=0;v<actors.Count;v++)if(actors[v].Id==segment.OwnerId){victim=actors[v];break;}if(victim==null)continue;
                    var result=victim.Crop(segment.UnitIndex,Time.time);if(!result.Applied)continue;actors[a].Cropped+=result.LostMass;if(actors[a]==player)biggestCrop=Mathf.Max(biggestCrop,result.LostMass);
                    foreach(var value in DroppedFishCalculator.Convert(result.LostMass))SpawnFishAt(victim.Position+new Vector3(Range(-2,2),.1f,Range(-2,2)),value);break;
                }
            }
        }

        int Score(ColonyActor actor) => ScoreCalculator.Calculate(actor.Colony.TotalMass, actor.Fish, actor.Cropped, Rank(actor));
        int Rank(ColonyActor actor) { var rank = 1; for (var i = 0; i < actors.Count; i++) if (actors[i].Colony.TotalMass > actor.Colony.TotalMass) rank++; return rank; }
        void UpdateLeaderboard() { actors.Sort((a,b) => b.Colony.TotalMass.CompareTo(a.Colony.TotalMass)); var s = "TOP WADDLES\n"; for (var i=0;i<Mathf.Min(5,actors.Count);i++) s += (i+1)+". "+actors[i].Name+"  "+actors[i].Colony.TotalMass+"\n"; if (Rank(player)>5) s += "…\n"+Rank(player)+". PLAYER  "+player.Colony.TotalMass; leaderboard.text=s; }
        void Finish() { enabled = false; var stats = new MatchStats { Rank=Rank(player), Score=Score(player), MaxMass=maxMass, BiggestCrop=biggestCrop, Fish=player.Fish, Fusions=player.Fusions, Emperors=player.Emperors }; bootstrap.ShowResults(stats); }
        float Range(float min, float max) => min + (float)random.NextDouble() * (max-min);
        void OnDestroy() { for (var i=0;i<actors.Count;i++) actors[i].Dispose(); fishPool?.Dispose(); }
    }

    public sealed class CameraFollower : MonoBehaviour
    {
        public Transform target;
        void LateUpdate() { if (!target) return; var desired = target.position - target.forward * 10f + Vector3.up * 17f; transform.position = Vector3.Lerp(transform.position, desired, 5f * Time.deltaTime); transform.LookAt(target.position + target.forward * 3f); }
    }

    public sealed class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    { public event Action<bool> Changed; public void OnPointerDown(PointerEventData e) => Changed?.Invoke(true); public void OnPointerUp(PointerEventData e) => Changed?.Invoke(false); }
}

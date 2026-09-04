using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WaddleRush.Core;
using WaddleRush.Persistence;

namespace WaddleRush.Presentation
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        ILocalSaveService saves; ProfileData profile; GameObject current;
        static Font Font => Resources.GetBuiltinResource<Font>("Arial.ttf");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void StartGame()
        {
            if (FindObjectOfType<GameBootstrap>()) return;
            new GameObject("WaddleRush Bootstrap").AddComponent<GameBootstrap>();
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject); Application.targetFrameRate = 60; Screen.orientation = ScreenOrientation.Portrait;
            saves = new LocalSaveService(Application.persistentDataPath); profile = saves.Load(); ShowMenu();
        }

        void Clear() { if (current) Destroy(current); current = new GameObject("Screen"); }

        public void ShowMenu()
        {
            Clear(); CameraSetup(new Color(.025f, .11f, .19f));
            var penguin = PrototypeFactory.Penguin("Hero", new Color(.25f, .9f, 1f), 1.5f); penguin.transform.SetParent(current.transform); penguin.transform.position = new Vector3(0, 0, 2);
            var canvas = Canvas(); Title(canvas.transform, "WADDLE\nRUSH", new Vector2(0, 310), 58);
            Title(canvas.transform, "Level " + profile.level + "     ◆ " + profile.coins, new Vector2(0, 205), 24);
            Button(canvas.transform, "PLAY", new Vector2(0, -245), new Vector2(330, 105), StartMatch);
            Title(canvas.transform, "CUSTOMIZE     MISSIONS     STATS     SETTINGS", new Vector2(0, -330), 18);
        }

        public void StartMatch()
        {
            Clear(); current.AddComponent<MatchController>().Initialize(GameConfig.Defaults(), profile, saves, this);
        }

        public void ShowResults(MatchStats stats)
        {
            Clear(); CameraSetup(new Color(.02f, .08f, .14f));
            profile.matches++; profile.fishCollected += stats.Fish; profile.croppedMass += stats.Cropped; profile.fusions += stats.Fusions;
            var earned = stats.Score / 20; profile.coins += Mathf.Max(5, earned / 4); profile.xp += earned; profile.level = 1 + profile.xp / 500; profile.highScore = Mathf.Max(profile.highScore, stats.Score); saves.Save(profile);
            var canvas = Canvas(); Title(canvas.transform, "RESULTS", new Vector2(0, 350), 48);
            Title(canvas.transform, "RANK  #" + stats.Rank + "\n\nSCORE  " + stats.Score + "\nMAX MASS  " + stats.MaxMass + "\nBIGGEST CROP  " + stats.BiggestCrop + "\nFISH  " + stats.Fish + "\nFUSIONS  " + stats.Fusions + "\nEMPERORS  " + stats.Emperors, new Vector2(0, 80), 25);
            Button(canvas.transform, "PLAY AGAIN", new Vector2(0, -260), new Vector2(350, 95), StartMatch);
            Button(canvas.transform, "HOME", new Vector2(0, -365), new Vector2(220, 70), ShowMenu);
        }

        void CameraSetup(Color color)
        {
            var cam = Camera.main; if (!cam) { cam = new GameObject("Main Camera").AddComponent<Camera>(); cam.tag = "MainCamera"; }
            cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = color; cam.transform.position = new Vector3(0, 5, -9); cam.transform.rotation = Quaternion.Euler(15, 0, 0);
            var follower = cam.GetComponent<CameraFollower>(); if (follower) follower.enabled = false;
        }

        Canvas Canvas()
        {
            if (!FindObjectOfType<EventSystem>()) { var e = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)); e.transform.SetParent(current.transform); }
            var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)); go.transform.SetParent(current.transform);
            var canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1080, 1920); scaler.matchWidthOrHeight = .5f; return canvas;
        }

        public static Text Title(Transform parent, string value, Vector2 position, int size)
        {
            var go = new GameObject(value, typeof(RectTransform), typeof(Text)); go.transform.SetParent(parent, false); var text = go.GetComponent<Text>(); text.text = value; text.font = Font; text.fontSize = size; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white;
            var rect = (RectTransform)go.transform; rect.sizeDelta = new Vector2(900, 480); rect.anchoredPosition = position; return text;
        }

        public static Button Button(Transform parent, string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action)
        {
            var go = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button)); go.transform.SetParent(parent, false); var rect = (RectTransform)go.transform; rect.sizeDelta = size; rect.anchoredPosition = pos; go.GetComponent<Image>().color = new Color(.08f, .72f, .86f);
            var text = Title(go.transform, label, Vector2.zero, 31); ((RectTransform)text.transform).sizeDelta = size; var button = go.GetComponent<Button>(); button.onClick.AddListener(action); return button;
        }
    }
}

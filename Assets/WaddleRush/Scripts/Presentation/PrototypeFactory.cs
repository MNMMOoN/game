using UnityEngine;
using System.Collections.Generic;

namespace WaddleRush.Presentation
{
    public static class PrototypeFactory
    {
        static readonly Dictionary<Color32, Material> Materials = new Dictionary<Color32, Material>();
        public static Material Material(Color color)
        {
            var key=(Color32)color;if(Materials.TryGetValue(key,out var cached))return cached;
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { color = color };
            Materials[key]=material;
            return material;
        }

        public static GameObject Penguin(string name, Color accent, float scale = 1f)
        {
            var root = new GameObject(name);
            var body = Primitive(PrimitiveType.Sphere, root.transform, new Vector3(0, .55f, 0), new Vector3(.72f, 1.05f, .62f) * scale, Color.black);
            Primitive(PrimitiveType.Sphere, body.transform, new Vector3(0, .05f, -.48f), new Vector3(.56f, .7f, .12f), new Color(.92f, .96f, 1f));
            Primitive(PrimitiveType.Sphere, body.transform, new Vector3(-.2f, .25f, -.53f), Vector3.one * .12f, Color.white);
            Primitive(PrimitiveType.Sphere, body.transform, new Vector3(.2f, .25f, -.53f), Vector3.one * .12f, Color.white);
            Primitive(PrimitiveType.Sphere, body.transform, new Vector3(-.2f, .25f, -.6f), Vector3.one * .055f, Color.black);
            Primitive(PrimitiveType.Sphere, body.transform, new Vector3(.2f, .25f, -.6f), Vector3.one * .055f, Color.black);
            Primitive(PrimitiveType.Cube, body.transform, new Vector3(0, .08f, -.62f), new Vector3(.18f, .1f, .23f), new Color(1f, .55f, .08f));
            Primitive(PrimitiveType.Cube, body.transform, new Vector3(0, -.08f, -.52f), new Vector3(.78f, .11f, .14f), accent);
            return root;
        }

        public static GameObject Primitive(PrimitiveType type, Transform parent, Vector3 localPosition, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(type); go.transform.SetParent(parent, false); go.transform.localPosition = localPosition; go.transform.localScale = scale;
            Object.Destroy(go.GetComponent<Collider>()); go.GetComponent<Renderer>().sharedMaterial = Material(color); return go;
        }
    }
}

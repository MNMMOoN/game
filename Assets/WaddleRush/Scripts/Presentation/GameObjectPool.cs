using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaddleRush.Presentation
{
    public sealed class GameObjectPool : IDisposable
    {
        readonly Stack<GameObject> inactive = new Stack<GameObject>(); readonly Func<GameObject> factory; readonly Transform parent;
        public int ActiveCount { get; private set; } public int InactiveCount => inactive.Count;
        public GameObjectPool(Func<GameObject> factory, Transform parent, int prewarm)
        { this.factory=factory; this.parent=parent; for(var i=0;i<prewarm;i++){var go=factory();go.transform.SetParent(parent);go.SetActive(false);inactive.Push(go);} }
        public GameObject Get() { var go=inactive.Count>0?inactive.Pop():factory(); go.transform.SetParent(parent);go.SetActive(true);ActiveCount++;return go; }
        public void Release(GameObject go) { if(!go||!go.activeSelf)return;go.SetActive(false);inactive.Push(go);ActiveCount--; }
        public void Dispose(){foreach(var go in inactive)if(go)UnityEngine.Object.Destroy(go);inactive.Clear();}
    }
}

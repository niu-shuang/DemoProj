using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoProj
{
    public abstract class ViewBase : UIPanel
    {
        public abstract UniTask LoadTask();
        public abstract void OnFinishLoad();

        public abstract void OnEndView();
    }
}

using Cysharp.Threading.Tasks;
using J;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoProj
{
    public abstract class ViewBase : UIPanel
    {
        public abstract UniTask LoadTask(DividableProgress progress);
        public abstract void OnFinishLoad();

        public abstract void OnEndView();
    }
}

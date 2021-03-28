using Cysharp.Threading.Tasks;
using J;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoProj
{
    public partial class HomeView : ViewBase
    {
        public override UniTask LoadTask(DividableProgress progress)
        {
            return UniTask.CompletedTask;
        }

        public override void OnEndView()
        {
            
        }

        public override void OnFinishLoad()
        {
            
        }
    }
}

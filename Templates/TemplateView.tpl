using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using J;

namespace DemoProj
{
    public partial class TemplateView : ViewBase
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

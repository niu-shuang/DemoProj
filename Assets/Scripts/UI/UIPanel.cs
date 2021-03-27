using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using J;
using UniRx;
using UnityEngine;

namespace DemoProj
{
    public abstract class UIPanel : MonoBehaviour,IDisposable
    {
        protected CompositeDisposable disposable;
        public UITweenSequence uiAnimation;

        public virtual void Init()
        {
            disposable = new CompositeDisposable();
        }

        public virtual async UniTask Show()
        {
            await uiAnimation.PlayAsync();
        }

        public virtual async UniTask Hide()
        {
            await uiAnimation.PlayReverseAsync();
        }

        public virtual void Dispose()
        {
            disposable?.Dispose();
        }
    }
}

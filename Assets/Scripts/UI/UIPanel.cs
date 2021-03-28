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
        public UIPanelView view;

        public virtual void Init(UIPanelView view)
        {
            this.view = view;
            disposable = new CompositeDisposable();
        }

        public virtual async UniTask Show()
        {
            await view.uiTweenSequence.PlayAsync();
        }

        public virtual async UniTask Hide()
        {
            await view.uiTweenSequence.PlayReverseAsync();
        }

        public virtual void Dispose()
        {
            disposable?.Dispose();
        }

    }
}

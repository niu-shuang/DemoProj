using J;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace DemoProj
{
    public class LoadingPlate : MonoBehaviour
    {
        public Image bar;
        public Text progressText;
        public UITweenSequence uiAnimation;
        public void SetProgress(DividableProgress dividableProgress)
        {
            progressText.text = $"0%";
            bar.fillAmount = 0;

            dividableProgress.Subscribe(progress =>
            {
                bar.fillAmount = progress;
                progressText.text = $"{(int)(progress * 100)}%";
            }).AddTo(this);
        }
    }

}

using J;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace DemoProj
{
    public class Gauge : Component
    {
        [SerializeField]
        private Image bar;

        public void SetProgress(float percentage)
        {
            bar.fillAmount = percentage;
        }

        public void SetDividableProgress(DividableProgress progress)
        {
            progress.Subscribe(percentage =>
            {
                bar.fillAmount = percentage;
            }).AddTo(this);
        }
    }
}

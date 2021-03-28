using J;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DemoProj
{
    public class Gauge : UIBehaviour
    {
        [SerializeField]
        private Image bar;

        public void SetProgress(float percentage)
        {
            bar.fillAmount = percentage;
        }

        public void SetDividableProgress(DividableProgress progress)
        {
            bar.fillAmount = 0;
            progress.Subscribe(percentage =>
            {
                bar.fillAmount = percentage;
            }).AddTo(this);
        }
    }
}

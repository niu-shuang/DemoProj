using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace DemoProj
{
    public abstract class UITweenerBase : MonoBehaviour
	{
		public Ease easeType = Ease.OutCirc;
		public abstract void Init();
		public abstract Tween Play(float duration);
		public bool keepAlive = false;

        [OnValueChanged("OnValueChanged")]
        public bool isReverse;

		protected abstract void OnValueChanged();
	}
}

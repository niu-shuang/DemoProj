using DG.Tweening;
using System;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DemoProj
{
	public class UIFadeTweener : UITweenerBase
	{
		public float from = 0;
		public float to = 1;
		public MaskableGraphic target;
		public UIBehaviour mask;

		public override void Init()
		{
			if (!target) return;
			var color = target.color;
			color.a = from;
			target.color = color;
			if(mask == null)
				mask = GetComponent<Mask>();
		}

		public override Tween Play(float duration)
		{
			if (mask != null)
				mask.enabled = true;
			Observable.Timer(TimeSpan.FromSeconds(duration))
				.Subscribe(_ =>
				{
					if (mask != null)
						mask.enabled = false;
				});
			//var temp = target.DOFade(0, 0.1f).SetDelay(0.1f);
			//temp.OnComplete(() => { });
			return target.DOFade(to, duration).SetEase(easeType);
		}

		protected override void OnValueChanged()
		{
			if(!isReverse)
			{
				from = 0;
				to = 1;
			}
			else
			{
				from = 1;
				to = 0;
			}
		}

		private void Reset()
		{
			target = GetComponent<MaskableGraphic>();
			mask = GetComponent<Mask>();
		}
	}
}

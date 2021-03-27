using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DemoProj
{
    [RequireComponent(typeof(Graphic))]
	public class UIBlinkTweener : UITweenerBase
	{
		public Image target;
		public int blinkCount = 3;
		public UIBehaviour mask;

		public override void Init()
		{
			if (!target) return;
			var color = target.color;
			color.a = 0;
			target.color = color;
			if(mask == null)
				mask = GetComponent<Mask>();
		}

		public override Tween Play(float duration)
		{
			var sequence = DOTween.Sequence();
			if (mask != null)
				mask.enabled = true;
			Observable.Timer(TimeSpan.FromSeconds(duration))
				.Subscribe(_ =>
				{
					if (mask != null)
						mask.enabled = false;
				});
			for (int i = 0; i < blinkCount; i++)
			{
				if (i == 0)
					sequence.Append(target.DOFade(1, 0));
				else
					sequence.Append(target.DOFade(i % 2 == 0 ? 1 : 0, 0).SetDelay(duration / (blinkCount - 1)));
			}
			return sequence;
		}

		protected override void OnValueChanged()
		{

		}

		private void Reset()
		{
			target = GetComponent<Image>();
			mask = GetComponent<Mask>();
		}
	}
}

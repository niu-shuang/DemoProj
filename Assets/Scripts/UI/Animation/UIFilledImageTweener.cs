using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DemoProj
{
	[RequireComponent(typeof(Image))]
	public class UIFilledImageTweener : UITweenerBase
	{
		public Image target;

		public float from = 0;
		public float to = 1;
		private UIBehaviour mask;

		public override Tween Play(float duration)
		{
			if (mask != null)
				mask.enabled = true;
			target.fillAmount = from;
			Observable.Timer(TimeSpan.FromSeconds(duration))
				.Subscribe(_ =>
				{
					if (mask != null)
						mask.enabled = false;
				});
			return target.DOFillAmount(to, duration).SetEase(easeType);
		}

		public override void Init()
		{
			if (gameObject == null) return;
			if(target)
				target.fillAmount = from;
			if(mask == null)
				mask = GetComponent<Mask>();
		}

		protected override void OnValueChanged()
		{
			if(isReverse)
			{
				from = 1;
				to = 0;
			}
			else
			{
				from = 0;
				to = 1;
			}
		}

		private void Reset()
		{
			target = GetComponent<Image>();
			mask = GetComponent<Mask>();
		}
	}
}


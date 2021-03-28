using DG.Tweening;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DemoProj
{
	public class UIGroupFadeTweener : UITweenerBase
	{
		public float from = 0;
		public float to = 1;
		public CanvasGroup target;

		public override void Init()
		{
			if (!target) return;

			target.alpha = from;
		}

		public override Tween Play(float duration)
		{
			return target.DOFade(to, duration).SetEase(easeType);
		}

		protected override void OnValueChanged()
		{
			if (!isReverse)
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
			target = GetComponent<CanvasGroup>();
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace DemoProj
{
	public class UIMoveTweener : UITweenerBase
	{
		public RectTransform target;

		public Vector2 from;
		public Vector2 to;

		public override void Init()
		{
			if(target)
				target.anchoredPosition = from;
		}

		public override Tween Play(float duration)
		{
			return target.DOAnchorPos(to, duration).SetEase(easeType);
		}

		protected override void OnValueChanged()
		{
            from = Vector2.zero;
            to = target.anchoredPosition;
		}

		private void Reset()
		{
			target = GetComponent<RectTransform>();
			from = target.anchoredPosition;
		}
	}
}

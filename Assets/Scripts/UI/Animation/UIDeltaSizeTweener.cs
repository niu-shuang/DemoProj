using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using System;

namespace DemoProj
{
    public class UIDeltaSizeTweener : UITweenerBase
	{
		public RectTransform target;
		private UIBehaviour mask;

		public Vector2 originSize;
		public Vector2 targetSize;

		public enum eDirection
		{
			Horizontal,
			Vertical,
			Both
		}

		public eDirection direction =  eDirection.Horizontal;

		public Tween currentTween;

		public override void Init()
		{
			if(target)
				target.sizeDelta = originSize;
			mask = GetComponent<RectMask2D>();
			if (mask == null)
			{
				mask = GetComponent<Mask>();
			}
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
			currentTween = target.DOSizeDelta(targetSize, duration).SetEase(easeType);
			return currentTween;
		}

		protected override void OnValueChanged()
		{
			switch (direction)
			{
				case eDirection.Vertical:
					{
						if(!isReverse)
						{
							originSize = new Vector2(target.sizeDelta.x, 0);
							targetSize = target.sizeDelta;
						}
						else
						{
							originSize = target.sizeDelta;
							targetSize = new Vector2(target.sizeDelta.x, 0);
						}
					}
					break;
				case eDirection.Horizontal:
					{
						if (!isReverse)
						{
							originSize = new Vector2(0, target.sizeDelta.y);
							targetSize = target.sizeDelta;
						}
						else
						{
							originSize = target.sizeDelta;
							targetSize = new Vector2(0, target.sizeDelta.y);
						}
					}
					break;
				case eDirection.Both:
					{
						if (!isReverse)
						{
							originSize = Vector2.zero;
							targetSize = target.sizeDelta;
						}
						else
						{
							originSize = target.sizeDelta;
							targetSize = Vector2.zero;
						}
					}
					break;
				default:
					break;
			}
		}

		private void Reset()
		{
			target = GetComponent<RectTransform>();
			OnValueChanged();
		}
	}
}

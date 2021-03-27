using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DemoProj
{
	public class UIScaleTweener : UITweenerBase
	{
        public enum eDirection
		{
			Horizontal,
			Vertical,
			Both
		}
        [OnValueChanged("OnValueChanged")]
        public eDirection direction = eDirection.Horizontal;

		public Vector3 from;
		public Vector3 to;

		public Transform target;

		public override void Init()
		{
			if(target)
				target.localScale = from;
		}

		public override Tween Play(float duration)
		{
			return target.DOScale(to, duration).SetEase(easeType);
		}

		protected override void OnValueChanged()
		{
			switch (direction)
			{
				case eDirection.Horizontal:
					if (!isReverse)
					{
						from = new Vector3(0, 1, 1);
						to = Vector3.one;
					}
					else
					{
						from = Vector3.one;
						to = new Vector3(0, 1, 1);
					}
					break;
				case eDirection.Vertical:
					if (!isReverse)
					{
						from = new Vector3(1, 0, 1);
						to = Vector3.one;
					}
					else
					{
						from = Vector3.one;
						to = new Vector3(1, 0, 1);
					}
					break;
				case eDirection.Both:
					if (!isReverse)
					{
						from = Vector3.zero;
						to = Vector3.one;
					}
					else
					{
						from = Vector3.one;
						to = Vector3.zero;
					}
					break;
				default:
					break;
			}
		}

		private void Reset()
		{
			target = transform;
			OnValueChanged();
		}
	}
}

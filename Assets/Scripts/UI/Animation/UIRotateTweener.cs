using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace DemoProj
{
	[RequireComponent(typeof(RectTransform))]
	public class UIRotateTweener : UITweenerBase
	{
		public RectTransform target;
		public float originAngle;
		public float targetAngle;

		public override void Init()
		{
			if (!target) return;
			target.localRotation =  Quaternion.Euler(0, 0, originAngle);
		}

		public override Tween Play(float duration)
		{
			return target.DOLocalRotate(new Vector3(0, 0, -targetAngle), duration).SetEase(easeType);
		}

		protected override void OnValueChanged()
		{
			
		}

		private void Reset()
		{
			target = GetComponent<RectTransform>();
		}
	}
}


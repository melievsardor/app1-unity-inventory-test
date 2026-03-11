using System;
using UnityEngine;
using UnityEngine.UI;

namespace Com.ForbiddenByte.OSA.Demos.Common.CommandPanels
{
	public class LabelWithSliderPanel : MonoBehaviour
	{
		public Text labelText, minLabelText, maxLabelText;
		public Slider slider;

		public bool Interactable { set { slider.interactable = value; } }


		public void Init(string label, string minLabel, string maxLabel)
		{
			labelText.text = label;
			minLabelText.text = minLabel;
			maxLabelText.text = maxLabel;
		}

        //public void Set(int v1, int mAX_CELLS_PER_GROUP_FACTOR_WHEN_INFERRING, int v2)
        //{
        //    throw new NotImplementedException();
        //}

        internal void Set(float min, float max, float val)
		{
			slider.minValue = min;
			slider.maxValue = max;
			slider.onValueChanged.Invoke(slider.value = val);
		}
	}
}

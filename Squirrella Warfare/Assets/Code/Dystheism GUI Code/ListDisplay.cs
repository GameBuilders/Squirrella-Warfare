using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class ListDisplay <T> {
	public ICollection<T> contents;
	Action<T> chooseCallback = t => {};
	List<T> presented;
	int linesVisible;
	float sliderValue;
	int scrollOffset;
	Rect scrollerBox;
	public ListDisplay (ICollection<T> referenceContentsTo) {
		contents = referenceContentsTo;
	}
	public void Draw (Rect box) {
		linesVisible = ((int) box.height ) / 20;
		scrollerBox = new Rect(box.x + box.width - 20, box.y, 20, 20 * linesVisible);
		presented = contents.ToList();
		if (Input.GetAxis("Scroll Wheel") != 0)
			sliderValue -= Input.GetAxis("Scroll Wheel");
		sliderValue = GUI.VerticalScrollbar(scrollerBox, sliderValue, Mathf.Max(Mathf.Min(linesVisible, presented.Count), 1), 0f, Mathf.Max(1, presented.Count));
		var unconstrainedScrollOffset = Mathf.RoundToInt(sliderValue);
		scrollOffset = Mathf.Max(Mathf.Min(unconstrainedScrollOffset, presented.Count - linesVisible), 0);
		if (scrollOffset != unconstrainedScrollOffset)
			sliderValue = scrollOffset;
		presented = presented.GetRange(scrollOffset, Mathf.Min(linesVisible, presented.Count - scrollOffset));
		for (var i = 0; i < presented.Count; ++i) 
			DrawEntry(box, i, presented[i]);
	}
	void DrawEntry (Rect outsideBox, int index, T toShow) {
		var oldColor = GUI.color;
		var displayString = toShow == null ? "None" : toShow.ToString();
        GUI.color = Color.white;
		var buttonRect = new Rect(outsideBox.x, outsideBox.y + index * 20, outsideBox.width - 30, 20);
		GUI.color = oldColor;
        if (GUI.Button(buttonRect, displayString)) {
            chooseCallback(toShow);
        }			
	}
	public ListDisplay<T> OnChoose (Action<T> callback) {
		chooseCallback = callback;
		return this;
	}
}

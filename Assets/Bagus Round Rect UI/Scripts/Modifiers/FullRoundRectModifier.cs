using UnityEngine;
using System.Collections;
using UnityEngine.UI.ProceduralImage;

[ModifierID("Full Round Rect")]
public class FullRoundRectModifier : RoundRectImageModifier {
	#region implemented abstract members of ProceduralImageModifier
	public override Vector4 CalculateRadius (Rect imageRect){
		float radius = Mathf.Min (imageRect.width,imageRect.height)*0.5f;
		return new Vector4 (radius, radius, radius, radius);
	}
	#endregion
}

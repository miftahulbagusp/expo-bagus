using UnityEngine;
using UnityEngine.UI.ProceduralImage;

[ModifierID("Uniform Round Rect")]
public class UniformRoundRectModifier : RoundRectImageModifier {
	[SerializeField]
    private float radius;

	public float Radius {
		get {
			return radius;
		}
		set {
			radius = value;
		}
	}

	#region implemented abstract members of ProceduralImageModifier

	public override Vector4 CalculateRadius (Rect imageRect){
		float r = this.radius;
		return new Vector4(r,r,r,r);
	}

	#endregion
	
}

using UnityEngine;
using UnityEngine.UI.ProceduralImage;

[ModifierID("Free Round Rect")]
public class FreeRoundRectModifier : RoundRectImageModifier {
    private Vector4 radius;
    [SerializeField]
    private float topLeft;
    [SerializeField]
    private float topRight;
    [SerializeField]
    private float bottomRight;
    [SerializeField]
    private float bottomLeft;

	public Vector4 Radius {
		get {
			return radius;
		}
		set {
            radius.x = topLeft;
            radius.y = topRight;
            radius.z = bottomRight;
            radius.w = bottomLeft;
			//radius = value;
		}
	}

	#region implemented abstract members of ProceduralImageModifier

	public override Vector4 CalculateRadius (Rect imageRect){
        radius.x = topLeft;
        radius.y = topRight;
        radius.z = bottomRight;
        radius.w = bottomLeft;
        return radius;
	}

	#endregion
}

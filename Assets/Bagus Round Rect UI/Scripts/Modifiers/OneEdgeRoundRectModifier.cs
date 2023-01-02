using UnityEngine;
using System.Collections;
using UnityEngine.UI.ProceduralImage;

[ModifierID("One Edge Round Rect")]
public class OneEdgeRoundRectModifier : RoundRectImageModifier {
    [SerializeField]
    private RoundRectImageEdge side;
    [SerializeField]
    private float radius;

	public enum RoundRectImageEdge{
		Top,
		Bottom,
		Left,
		Right
	}

	public float Radius {
		get {
			return radius;
		}
		set {
			radius = value;
		}
	}

	public RoundRectImageEdge Side {
		get {
			return side;
		}
		set {
			side = value;
		}
	}

	#region implemented abstract members of ProceduralImageModifier

	public override Vector4 CalculateRadius (Rect imageRect){
		switch (side) {
		case RoundRectImageEdge.Top:
				return new Vector4(radius,radius,0,0);
		case RoundRectImageEdge.Right:
				return new Vector4(0,radius,radius,0);
		case RoundRectImageEdge.Bottom:
				return new Vector4(0,0,radius,radius);
		case RoundRectImageEdge.Left:
				return new Vector4(radius,0,0,radius);
		default:
				return new Vector4(0,0,0,0);
		}
	}

	#endregion
}



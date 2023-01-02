using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace UnityEditor.UI {

    /// <summary>
    /// This class adds a Menu Item "GameObject/UI/Atlas Round Rect Image"
    /// Bahviour of this command is the same as with regular Images
    /// </summary>
    public class RoundRectImageEditorUtility {
		[MenuItem("GameObject/UI/Bagus Round Rect UI")]
		public static void AddRoundRectImage(){
			GameObject gObject = new GameObject ();
			gObject.AddComponent<RoundRectImage> ();
			gObject.name = "Round Rect Image";
			if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponentInParent<Canvas> () != null) {
				gObject.transform.SetParent (Selection.activeGameObject.transform, false);
				Selection.activeGameObject = gObject;
			}
			/*else if (Selection.activeGameObject != null) {
				//selected GameObject is not child of canvas:
			}*/
			else {
				if(GameObject.FindObjectOfType<Canvas>()==null)	{
					EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
				}
				Canvas c = GameObject.FindObjectOfType<Canvas>();
				gObject.transform.SetParent (c.transform, false);
				Selection.activeGameObject = gObject;
			}
		}
		/// <summary>
		/// Replaces an Image Component with a Round Rect Image Component.
		/// </summary>
		[MenuItem("CONTEXT/Image/Replace with Round Rect Image")]
		public static void ReplaceWithRoundRectImage(MenuCommand command){

			Image image = (Image)command.context;
			GameObject obj = image.gameObject;
			GameObject.DestroyImmediate (image);
			obj.AddComponent<RoundRectImage> ();
		}
    }
}

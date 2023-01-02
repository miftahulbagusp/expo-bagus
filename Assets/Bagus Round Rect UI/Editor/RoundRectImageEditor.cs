using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using System;

namespace UnityEditor.UI
{
	[CustomEditor(typeof(RoundRectImage),true)]
	[CanEditMultipleObjects]
	public class RoundRectImageEditor : ImageEditor {
		static List<ModifierID> attrList;
 
		SerializedProperty m_borderWidth;
		SerializedProperty m_falloffDist;
		int selectedId;

		protected override void OnEnable () {
			base.OnEnable ();
			attrList = ModifierUtility.GetAttributeList ();

			m_borderWidth = serializedObject.FindProperty("borderWidth");
			m_falloffDist = serializedObject.FindProperty("fallOffDistance");

			if((target as RoundRectImage).GetComponent<RoundRectImageModifier> ()!=null){
				selectedId = attrList.IndexOf(((ModifierID[])(target as RoundRectImage).GetComponent<RoundRectImageModifier> ().GetType ().GetCustomAttributes(typeof(ModifierID),false))[0]);
			}
			selectedId = Mathf.Max (selectedId,0);
			EditorApplication.update -= UpdateRoundRectImage;
			EditorApplication.update += UpdateRoundRectImage;
		}

		/// <summary>
		/// Updates the procedural image in Edit mode. This will prevent issues when working with layout components.
		/// </summary>
		public void UpdateRoundRectImage(){
			if (target != null) {
				(target as RoundRectImage).Update ();
			} else {
				EditorApplication.update -= UpdateRoundRectImage;
			}
		}

		public override void OnInspectorGUI(){
            serializedObject.Update();

			EditorGUILayout.PropertyField (m_Color);
			RaycastControlsGUI();
			TypeGUI();
			EditorGUILayout.Space ();
			ModifierGUI ();
			EditorGUILayout.PropertyField (m_borderWidth);
			EditorGUILayout.PropertyField (m_falloffDist);
			serializedObject.ApplyModifiedProperties ();
		}

		protected void ModifierGUI(){
			GUIContent[] con = new GUIContent[attrList.Count];
			for (int i = 0; i < con.Length; i++) {
				con[i] = new GUIContent(attrList[i].Name);
			}
			int index = EditorGUILayout.Popup (new GUIContent("Modifier Type"),selectedId,con);
			if(selectedId != index){
				selectedId = index;
				foreach (var item in targets) {
					DestroyImmediate ((item as RoundRectImage).GetComponent<RoundRectImageModifier>(),true);
					(item as RoundRectImage).ModifierType = ModifierUtility.GetTypeWithId(attrList[selectedId].Name);
					
					MoveComponentBehind((item as RoundRectImage),(item as RoundRectImage).GetComponent<RoundRectImageModifier>());

				}
				//Exit GUI prevents Unity from trying to draw destroyed components editor;
				EditorGUIUtility.ExitGUI();
			}
		}

		public override void OnPreviewGUI(Rect rect, GUIStyle background) {
			RoundRectImage image = target as RoundRectImage;
			if (image == null|| image.sprite == null) return;

			float min = Mathf.Min (rect.width,rect.height-50);
			rect = new Rect (rect.x+rect.width/2-min/2,rect.y+rect.height/2-min/2-20,min,min);

			Rect r = image.rectTransform.rect;

			//Fixing proportion of rect
			Vector2 center = rect.center;
			if (r.width > r.height) {
				rect.height = rect.height*(r.height/r.width);

			} else {
				rect.width = rect.width*(r.width/r.height);
			}
			//recenter rect
			rect.center = center;

			EditorGUI.DrawPreviewTexture(rect, image.sprite.texture, image.material);
		}

		public override string GetInfoString() {
			RoundRectImage image = target as RoundRectImage;
			return string.Format("Modifier: {0}, Line-Weight: {1}",attrList[selectedId].Name, image.BorderWidth);
		}
		/// <summary>
		/// Moves a component behind a reference component.
		/// </summary>
		/// <param name="reference">Reference component.</param>
		/// <param name="componentToMove">Component to move.</param>
		static void MoveComponentBehind (Component reference, Component componentToMove){
			if(reference == null || componentToMove == null || reference.gameObject != componentToMove.gameObject){
				return;
			}
			Component[] comps = reference.GetComponents<Component>();
			List<Component> list = new List<Component>();
			list.AddRange(comps);
			int i = list.IndexOf(componentToMove)-list.IndexOf(reference);
			while (i!=1){
				if(i < 1){
					UnityEditorInternal.ComponentUtility.MoveComponentDown(componentToMove);
					i++;
				}
				else if(i > 1){
					UnityEditorInternal.ComponentUtility.MoveComponentUp(componentToMove);
					i--;
				}
			}
		}
	}
}
using System;
using System.Reflection;
using System.Collections.Generic;

namespace UnityEngine.UI.ProceduralImage{
	/// <summary>
	/// Helps with getting ModifierID Attributes etc.
	/// </summary>
	public static class ModifierUtility {
		/// <summary>
		/// Gets the instance with identifier specified in a ModifierID Attribute.
		/// </summary>
		/// <returns>The instance with identifier.</returns>
		/// <param name="id">Identifier.</param>
		public static RoundRectImageModifier GetInstanceWithId(string id){
			return (RoundRectImageModifier)Activator.CreateInstance(GetTypeWithId(id));
		}
		/// <summary>
		/// Gets the type with specified in a ModifierID Attribute.
		/// </summary>
		/// <returns>The type with identifier.</returns>
		/// <param name="id">Identifier.</param>
		public static Type GetTypeWithId(string id){
			foreach(Type type in Assembly.GetAssembly(typeof(RoundRectImageModifier)).GetTypes()) {
				if (type.IsSubclassOf(typeof(RoundRectImageModifier))){
					if(((ModifierID[])type.GetCustomAttributes(typeof(ModifierID),false))[0].Name == id){
						return type;
					}
				}
			}
			return null;
		}
		/// <summary>
		/// Gets a list of Attributes of type ModifierID.
		/// </summary>
		/// <returns>The attribute list.</returns>
		public static List<ModifierID> GetAttributeList(){
			List<ModifierID> l = new List<ModifierID> ();
			foreach(Type type in Assembly.GetAssembly(typeof(RoundRectImageModifier)).GetTypes()) {
				if (type.IsSubclassOf(typeof(RoundRectImageModifier))){
					l.Add (((ModifierID[])type.GetCustomAttributes(typeof(ModifierID),false))[0]);
				}
			}
			return l;
		}
	}
}
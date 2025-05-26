#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Drakkar
{
	[CustomPropertyDrawer(typeof(ToggleBool))]
	class ToggleBoolDrawer : PropertyDrawer 
	{
		public override float GetPropertyHeight(SerializedProperty property,GUIContent label) => ConditionalShowingAttribute.VisibilityHeight(EditorGUI.GetPropertyHeight(property,label),property,this);
		public Rect GetXSizedRect(ref Rect pos,float sizex)
		{
			Rect n=pos;
			pos.x+=sizex;
			return new Rect(n.x,n.y,sizex,17);
		}

		public override void OnGUI(Rect position,SerializedProperty property,GUIContent label)
		{
			if (!((ToggleBool)attribute).full)
				position=EditorGUI.PrefixLabel(position,new GUIContent(" "));
			if (ConditionalShowingAttribute.IsVisibleProperty(property,this))
				property.boolValue=DrakkarEditor.GUIToggle(property.boolValue,label.text,new Rect(position.x,position.y,position.width,17));
		}
	}
}
#endif
using UnityEditor;
using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	[CustomPropertyDrawer(typeof(WrappedComponentAttribute))]
	public sealed class WrappedComponentDrawer : PropertyDrawer {
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			property.isExpanded = true;
			return EditorGUI.GetPropertyHeight(property)
			       - EditorGUIUtility.standardVerticalSpacing
			       - EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginChangeCheck();
			var endProperty = property.GetEndProperty();
			var childProperty = property.Copy();
			childProperty.NextVisible(true);
			while (!SerializedProperty.EqualContents(childProperty, endProperty)) {
				position.height = EditorGUI.GetPropertyHeight(childProperty);
				EditorGUI.PropertyField(position, childProperty, true);
				position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
				childProperty.NextVisible(false);
			}
		}
	}
}
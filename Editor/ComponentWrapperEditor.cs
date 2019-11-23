using System;
using System.Reflection;
using UnityEditor;

namespace Leopotam.Ecs.Hybrid {
	[CustomEditor(typeof(BaseComponentWrapper), true)]
	public class ComponentWrapperEditor : Editor {
		private const string componentValueName = "value";

		protected Type componentType { get; private set; }

		private BaseComponentWrapper wrapper;
		private SerializedProperty serializedData;
		private bool notMarkedSerializable;
		private bool multipleComponents;

		protected virtual void OnEnable() {
			wrapper = (BaseComponentWrapper)target;
			serializedData = serializedObject.FindProperty(componentValueName);
			Undo.undoRedoPerformed += UndoRedoPerformed;

			var type = target.GetType();
			multipleComponents = wrapper.GetComponents(type).Length > 1;

			FieldInfo field = null;
			while (type.BaseType != typeof(BaseComponentWrapper)) {
				type = type.BaseType;
				if (type == null) break;

				field = type.GetField(componentValueName, BindingFlags.Instance | BindingFlags.NonPublic);
			}

			if (field == null) return;
			componentType = field.FieldType;

			if (serializedData != null) return;
			notMarkedSerializable = !Attribute.IsDefined(componentType, typeof(SerializableAttribute)) &&
			                        componentType.GetFields(BindingFlags.Public | BindingFlags.Instance).Length > 0;
		}

		public override void OnInspectorGUI() {
			DisplayStatusMessages();
			serializedObject.Update();
			if (serializedData == null) return;

			EditorGUI.BeginChangeCheck();
			DisplayComponent(serializedData);
			if (!EditorGUI.EndChangeCheck()) return;

			wrapper.MarkAsUpdated();
			serializedObject.ApplyModifiedProperties();
		}

		private void UndoRedoPerformed() {
			wrapper.MarkAsUpdated();
		}

		protected virtual void DisplayComponent(SerializedProperty property) {
			EditorGUILayout.PropertyField(property);
		}

		protected virtual void DisplayStatusMessages() {
			if (notMarkedSerializable) {
				EditorGUILayout.HelpBox($"Component type {componentType.Name} should be marked with {nameof(SerializableAttribute)}", MessageType.Warning);
			}

			if (multipleComponents) {
				EditorGUILayout.HelpBox("Entity must have only one component per type!", MessageType.Error);
			}
		}
	}
}
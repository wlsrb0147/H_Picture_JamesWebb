namespace Xarbrough.SelectionUtility
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;
using Debug = DebugEx;

	internal sealed class StringListSetting : Setting<List<string>>
	{
		private readonly ReorderableList reorderableList;
		private bool foldout;

		public StringListSetting(string key, List<string> defaultValue = null) : base(key, defaultValue)
		{
			Value ??= new List<string>();

			reorderableList = new ReorderableList(Value, typeof(string),
				draggable: false, displayHeader: true, displayAddButton: true, displayRemoveButton: true)
			{
				drawElementCallback = DrawListElement,
				drawHeaderCallback = _ => { /* disable the header */ },
				onAddCallback = AddCallback,
				headerHeight = 3f,
			};
			reorderableList.elementHeight -= 1;

			foldout = EditorPrefs.GetBool(key + "Foldout", false);
		}

		private void AddCallback(ReorderableList list)
		{
			base.Value.Add(string.Empty);
		}

		private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			rect.height = EditorGUIUtility.singleLineHeight;
			rect.y += 1;
			rect.xMin += 12;
			Value[index] = EditorGUI.TextField(rect, Value[index]);
		}

		private Rect foldoutRect;

		protected override List<string> DrawProperty(GUIContent label, List<string> value)
		{
			EditorGUI.BeginChangeCheck();
			foldout = EditorGUILayout.Foldout(foldout, label, toggleOnLabelClick: true);
			if (EditorGUI.EndChangeCheck())
				EditorPrefs.SetBool(Key + "Foldout", foldout);

			foldoutRect = GUILayoutUtility.GetLastRect();

			if (foldout)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(16);
				EditorGUILayout.BeginVertical();

				EditorGUI.BeginChangeCheck();
				reorderableList.DoLayoutList();
				if (EditorGUI.EndChangeCheck())
					SanitizeInput();

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}

			return value;
		}

		protected override void HandleContextClick(Rect propertyRect)
		{
			// Only use the foldout instead of the whole property rect, because context-click
			// on the reorderable list would interfere with clicking entries (e.g. copy/paste).
			base.HandleContextClick(foldoutRect);
		}

		private void SanitizeInput()
		{
			if (Value == null)
				return;

			for (int i = 0; i < Value.Count; i++)
			{
				// Remove spaces because they usually don't carry meaning.
				// Casing is ignored during comparison later, because
				// the default and saved values should not be forced all lowercase.
				Value[i] = Value[i].Replace(" ", string.Empty);
			}
		}

		protected override List<string> LoadValue()
		{
			if (EditorPrefs.HasKey(Key) == false)
				return DefaultValue.ToList();

			string serializedValue = EditorPrefs.GetString(Key, string.Empty);
			var newValues = serializedValue.Split(',').ToList();

			if (reorderableList != null)
				reorderableList.list = newValues;

			return newValues;
		}

		protected override void SaveValue(List<string> value)
		{
			EditorPrefs.SetString(Key, string.Join(",", value.ToArray()));
		}

		public override void Reset()
		{
			Value = new List<string>(DefaultValue);

			if (reorderableList != null)
				reorderableList.list = Value;
		}

		public bool Contains(string value)
		{
			if (Value == null)
				return false;

			value = value.ToLower();

			return Value.Any(t => t.ToLower() == value);
		}

		public override string ToString()
		{
			if (Value == null)
				return "Empty";

			return string.Join(",", Value.ToArray());
		}
	}
}

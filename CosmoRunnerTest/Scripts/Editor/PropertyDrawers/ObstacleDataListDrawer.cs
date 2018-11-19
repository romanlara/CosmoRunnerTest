/* -------------------------------------------
 * Author: Sergio Roman Lara Espinosa de los Monteros (lara.ems.roman@gmail.com)
 * Created: 2018-Nov-09
 * -------------------------------------------
 */

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(Segment.ObstacleDataList), true)]
public class ObstacleDataListDrawer : PropertyDrawer 
{
	ReorderableList m_ReorderableList;
	string m_PropertyName = string.Empty;

	private void Init (SerializedProperty property)
	{
		if (m_ReorderableList != null)
			return;

		SerializedProperty array = property.FindPropertyRelative("m_List");

		m_ReorderableList = new ReorderableList(property.serializedObject, array);
		m_ReorderableList.drawElementCallback = DrawOptionData;
		m_ReorderableList.drawHeaderCallback = DrawHeader;
		m_ReorderableList.elementHeight += 3;
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		Init(property);

		m_PropertyName = label.text;
		m_ReorderableList.DoList(position);
	}

	private void DrawHeader (Rect rect)
	{
		GUI.Label(rect, m_PropertyName);
	}

	private void DrawOptionData (Rect rect, int index, bool isActive, bool isFocused)
	{
		SerializedProperty itemData = m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index);
		SerializedProperty itemCanUse = itemData.FindPropertyRelative("m_CanUse");
		SerializedProperty itemPrefab = itemData.FindPropertyRelative("m_Prefab");

		RectOffset offset = new RectOffset(0, 0, -1, -3);
		rect = offset.Add(rect);
		rect.height = EditorGUIUtility.singleLineHeight;

		float width = rect.width;

		rect.width = 16f;
		EditorGUI.PropertyField(rect, itemCanUse, GUIContent.none);
		rect.x += 16f;
		rect.width = width - 16f;
		EditorGUI.PropertyField(rect, itemPrefab, GUIContent.none);
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		Init(property);

		return m_ReorderableList.GetHeight();
	}
}

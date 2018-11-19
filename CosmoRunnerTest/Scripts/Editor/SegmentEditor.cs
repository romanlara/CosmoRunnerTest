using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Segment))]
class SegmentEditor : Editor
{
	protected Segment m_Segment;

	public void OnEnable()
	{
		m_Segment = target as Segment;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUILayout.Space ();

		if (GUILayout.Button("Add obstacles"))
		{
			ArrayUtility.Add(ref m_Segment.obstaclePositions, 0.0f);
		}

		if (m_Segment.obstaclePositions != null)
		{
			int toremove = -1;
			for (int i = 0; i < m_Segment.obstaclePositions.Length; ++i)
			{
				GUILayout.BeginHorizontal();
				m_Segment.obstaclePositions[i] = EditorGUILayout.Slider(m_Segment.obstaclePositions[i], 0.0f, 1.0f);
				if (GUILayout.Button("-", GUILayout.MaxWidth(32)))
					toremove = i;
				GUILayout.EndHorizontal();
			}

			if (toremove != -1)
				ArrayUtility.RemoveAt(ref m_Segment.obstaclePositions, toremove);
		}
	}
}

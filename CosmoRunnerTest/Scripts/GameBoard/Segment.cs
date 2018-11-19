using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour 
{
	[System.Serializable]
	public class ObstacleData 
	{
		[SerializeField]
		private bool m_CanUse;
		public bool canUse { get { return m_CanUse; } set { m_CanUse = value; } }

		[SerializeField]
		private AObstacle m_Prefab;
		public AObstacle prefab { get { return m_Prefab; } set { m_Prefab = value; } }

		public ObstacleData () {}
		public ObstacleData (bool canUse, AObstacle prefab) : this()
		{
			this.canUse = canUse;
			this.prefab = prefab;
		}
	}

	[System.Serializable]
	public class ObstacleDataList
	{
		[SerializeField]
		private List<ObstacleData> m_List;
		public List<ObstacleData> list { get { return m_List; } set { m_List = value; } }

		public ObstacleDataList () { list = new List<ObstacleData>(); }
	}

	// -------------------------------------------------------------------------

	public Transform pathParent;
	public GameBoard manager;

	public Transform objectRoot;
	public Transform collectibleTransform;

	[Space]

	[SerializeField]
	private ObstacleDataList m_PossibleObstacles;
	public List<ObstacleData> possibleObstacles { get { return m_PossibleObstacles.list; } set { m_PossibleObstacles.list = value; } }

	[HideInInspector]
	public float[] obstaclePositions;

	protected float m_WorldLength;
	public float worldLength { get { return m_WorldLength; } }

	void OnEnable () 
	{
		UpdateWorldLength();

		GameObject obj = new GameObject("ObjectRoot");
		obj.transform.SetParent(transform);
		objectRoot = obj.transform;

		obj = new GameObject("Collectibles");
		obj.transform.SetParent(objectRoot);
		collectibleTransform = obj.transform;
	}
	
	public void GetPointAtInWorldUnit(float wt, out Vector3 pos, out Quaternion rot)
	{
		float t = wt / m_WorldLength;
		GetPointAt(t, out pos, out rot);
	}

	public void GetPointAt(float t, out Vector3 pos, out Quaternion rot)
	{
		float clampedT = Mathf.Clamp01(t);
		float scaledT = (pathParent.childCount - 1) * clampedT;
		int index = Mathf.FloorToInt(scaledT);
		float segmentT = scaledT - index;

		Transform orig = pathParent.GetChild(index);
		if (index == pathParent.childCount - 1)
		{
			pos = orig.position;
			rot = orig.rotation;
			return;
		}

		Transform target = pathParent.GetChild(index + 1);

		pos = Vector3.Lerp(orig.position, target.position, segmentT);
		rot = Quaternion.Lerp(orig.rotation, target.rotation, segmentT);
	}

	protected void UpdateWorldLength()
	{
		m_WorldLength = 0;

		for (int i = 1; i < pathParent.childCount; ++i)
		{
			Transform orig = pathParent.GetChild(i - 1);
			Transform end = pathParent.GetChild(i);

			Vector3 vec = end.position - orig.position;
			m_WorldLength += vec.magnitude;
		}
	}

	public void Cleanup()
	{
		while(collectibleTransform.childCount > 0)
		{
			Transform t = collectibleTransform.GetChild(0);
			t.SetParent(null);
			PoolSystem.instance.PoolIn(t.gameObject);
		}

		Destroy(gameObject);
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (pathParent == null)
			return;

		Color c = Gizmos.color;
		Gizmos.color = Color.red;
		for (int i = 1; i < pathParent.childCount; ++i)
		{
			Transform orig = pathParent.GetChild(i - 1);
			Transform end = pathParent.GetChild(i);

			Gizmos.DrawLine(orig.position, end.position);
		}

		Gizmos.color = Color.blue;
		for (int i = 0; i < obstaclePositions.Length; ++i)
		{
			Vector3 pos;
			Quaternion rot;
			GetPointAt(obstaclePositions[i], out pos, out rot);
			Gizmos.DrawSphere(pos, 0.5f);
		}

		Gizmos.color = c;
	}
	#endif
}

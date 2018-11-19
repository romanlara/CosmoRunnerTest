using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour 
{
	protected static GameBoard s_Instance;
	public static GameBoard instance { get { return s_Instance; } }

	// -------------------------------------------------------------------------

	[Header("Movements")]
	public CharInputController characterController;
	public float minSpeed = 5.0f;
	public float maxSpeed = 10.0f;
	public int speedStep = 4;
	public float laneOffset = 1.0f;

	public bool invincible = false;

	[System.Serializable]
	public struct ZoneSegment
	{
		public int length;
		public Segment[] prefabList;
	}
	[Header("Zones")]
	public ZoneSegment zone;

	[Header("Shop")]
	public GameObject shop;

	[Header("Play Time")]
	[Range(10, 120)]
	public int playTime = 60;

	// -------------------------------------------------------------------------

	protected float m_TimeLeft;

	protected int m_TrackSeed = -1;
	public int trackSeed {  get { return m_TrackSeed; } set { m_TrackSeed = value; } }

	protected float m_TimeToStart = -1.0f;
	public float timeToStart { get { return m_TimeToStart; } }

	protected int m_Multiplier;
	public int multiplier {  get { return m_Multiplier; } }

	protected float m_TotalWorldDistance;
	public float worldDistance {  get { return m_TotalWorldDistance; } }

	protected float m_Speed;
	public float speed {  get { return m_Speed; } }
	public float speedRatio {  get { return (m_Speed - minSpeed) / (maxSpeed - minSpeed); } }

	// -------------------------------------------------------------------------

	protected List<Segment> m_Segments = new List<Segment>();
	public List<Segment> segments { get { return m_Segments; } }
	public Segment currentSegment { get { return m_Segments[0]; } }
	protected float m_CurrentSegmentDistance;

	protected List<Segment> m_PastSegments = new List<Segment>();
	protected int m_SafeSegementLeft;

	protected int m_CurrentZone;
	protected float m_CurrentZoneDistance;
	protected int m_PreviousSegment = -1;

	// -------------------------------------------------------------------------

	protected bool m_IsMoving;
	public bool isMoving {  get { return m_IsMoving; } }

	protected bool m_Rerun;
	public bool isRerun { get { return m_Rerun; } set { m_Rerun = value; } }

	protected int m_Score;
	protected float m_ScoreAccum;

	// -------------------------------------------------------------------------

	const float k_FloatingOriginThreshold = 10000f;

	protected const float k_CountdownToStartLength = 5f;
	protected const float k_CountdownSpeed = 1.5f;
	protected const float k_StartingSegmentDistance = 2f;
	protected const int k_StartingSafeSegments = 2;
	protected const int k_StartingCoinPoolSize = 256;
	protected const int k_DesiredSegmentCount = 10;
	protected const float k_SegmentRemovalDistance = -30f;
	protected const float k_Acceleration = 0.2f;

	protected void Awake()
	{
		m_ScoreAccum = 0.0f;
		s_Instance = this;
	}

	void Start () 
	{
		Begin ();
		StartMove ();
	}

	public void StartMove(bool isRestart = true)
	{
		m_IsMoving = true;
		if(isRestart)
			m_Speed = minSpeed;
	}

	public void StopMove()
	{
		m_IsMoving = false;
	}

	IEnumerator WaitToStart()
	{
		float length = k_CountdownToStartLength;
		m_TimeToStart = length;

		while (m_TimeToStart >= 0)
		{
			yield return null;
			m_TimeToStart -= Time.deltaTime * k_CountdownSpeed;
		}

		m_TimeToStart = -1;

		StartMove();
	}

	IEnumerator PlayTime()
	{
		while (m_TimeLeft < playTime) 
		{
			yield return null;
			m_TimeLeft += Time.deltaTime;
		}

		StopMove ();
		shop.SetActive (true);
	}

	public void Begin()
	{
		if (!m_Rerun)
		{
			if (m_TrackSeed != -1)
				Random.InitState(m_TrackSeed);
			else
				Random.InitState((int)System.DateTime.Now.Ticks);

			m_CurrentSegmentDistance = k_StartingSegmentDistance;
			m_TotalWorldDistance = 0.0f;

			characterController.gameObject.SetActive(true);

			Camera.main.transform.SetParent(characterController.transform, true);

			m_CurrentZone = 0;
			m_CurrentZoneDistance = 0;

			gameObject.SetActive(true);
			characterController.gameObject.SetActive(true);

			m_Score = 0;
			m_ScoreAccum = 0;

			m_SafeSegementLeft = k_StartingSafeSegments;
		}

		shop.SetActive (false);
		characterController.Init ();
		StartCoroutine (WaitToStart ());
		StartCoroutine (PlayTime ());
	}

	public void End()
	{
		foreach (Segment seg in m_Segments)
		{
			Destroy(seg.gameObject);
		}

		for (int i = 0; i < m_PastSegments.Count; ++i)
		{
			Destroy(m_PastSegments[i].gameObject);
		}

		m_Segments.Clear();
		m_PastSegments.Clear();

		gameObject.SetActive(false);

		Camera.main.transform.SetParent(null);

		characterController.gameObject.SetActive(false);
	}

	void Update ()
	{
		while (m_Segments.Count < k_DesiredSegmentCount)
		{
			SpawnNewSegment();
		}

		if (!m_IsMoving)
			return;

		float scaledSpeed = m_Speed * Time.deltaTime;
		m_ScoreAccum += scaledSpeed;
		m_CurrentZoneDistance += scaledSpeed;

		int intScore = Mathf.FloorToInt(m_ScoreAccum);
		if (intScore != 0) AddScore(intScore);
		m_ScoreAccum -= intScore;

		m_TotalWorldDistance += scaledSpeed;
		m_CurrentSegmentDistance += scaledSpeed;

		if(m_CurrentSegmentDistance > m_Segments[0].worldLength)
		{
			m_CurrentSegmentDistance -= m_Segments[0].worldLength;

			m_PastSegments.Add(m_Segments[0]);
			m_Segments.RemoveAt(0);

		}

		Vector3 currentPos;
		Quaternion currentRot;
		Transform characterTransform = characterController.transform;

		m_Segments[0].GetPointAtInWorldUnit(m_CurrentSegmentDistance, out currentPos, out currentRot);

		bool needRecenter = currentPos.sqrMagnitude > k_FloatingOriginThreshold;

		if (needRecenter)
		{
			int count = m_Segments.Count;
			for(int i = 0; i < count; i++)
			{
				m_Segments[i].transform.position -= currentPos;
			}

			count = m_PastSegments.Count;
			for(int i = 0; i < count; i++)
			{
				m_PastSegments[i].transform.position -= currentPos;
			}

			m_Segments[0].GetPointAtInWorldUnit(m_CurrentSegmentDistance, out currentPos, out currentRot);
		}

		characterTransform.rotation = currentRot;
		characterTransform.position = currentPos;

		for(int i = 0; i < m_PastSegments.Count; ++i)
		{
			if ((m_PastSegments[i].transform.position - currentPos).z < k_SegmentRemovalDistance)
			{
				m_PastSegments[i].Cleanup();
				m_PastSegments.RemoveAt(i);
				i--;
			}
		}
	}

	public void AddScore(int amount)
	{
		int finalAmount = amount;
		m_Score += finalAmount * m_Multiplier;
	}

	public void SpawnNewSegment()
	{
		int segmentUse = Random.Range(0, zone.prefabList.Length);
		if (segmentUse == m_PreviousSegment) segmentUse = (segmentUse + 1) % zone.prefabList.Length;

		Segment segmentToUse = zone.prefabList[segmentUse];
		Segment newSegment = Instantiate(segmentToUse, Vector3.zero, Quaternion.identity);

		Vector3 currentExitPoint;
		Quaternion currentExitRotation;
		if (m_Segments.Count > 0)
		{
			m_Segments[m_Segments.Count - 1].GetPointAt(1.0f, out currentExitPoint, out currentExitRotation);
		}
		else
		{
			currentExitPoint = transform.position;
			currentExitRotation = transform.rotation;
		}

		newSegment.transform.rotation = currentExitRotation;

		Vector3 entryPoint;
		Quaternion entryRotation;
		newSegment.GetPointAt(0.0f, out entryPoint, out entryRotation);


		Vector3 pos = currentExitPoint + (newSegment.transform.position - entryPoint);
		newSegment.transform.position = pos;
		newSegment.manager = this;

		newSegment.transform.localScale = new Vector3((Random.value > 0.5f ? -1 : 1), 1, 1);
		newSegment.objectRoot.localScale = new Vector3(1.0f/newSegment.transform.localScale.x, 1, 1);

		if (m_SafeSegementLeft <= 0)
			SpawnObstacle(newSegment);
		else
			m_SafeSegementLeft -= 1;

		m_Segments.Add(newSegment);
	}

	public void SpawnObstacle(Segment segment)
	{
		if (segment.possibleObstacles.Count != 0)
		{
			for (int i = 0; i < segment.obstaclePositions.Length; ++i)
			{
				segment.possibleObstacles[Random.Range(0, segment.possibleObstacles.Count)].prefab.Spawn(segment, segment.obstaclePositions[i]);
			}
		}

		SpawnCoin(segment);
	}

	public void SpawnCoin(Segment segment)
	{
		const float increment = 1.5f;
		float currentWorldPos = 0.0f;
		int currentLane = Random.Range(0,3);

		while (currentWorldPos < segment.worldLength)
		{
			Vector3 pos;
			Quaternion rot;
			segment.GetPointAtInWorldUnit(currentWorldPos, out pos, out rot);


			bool laneValid = true;
			int testedLane = currentLane;
			while(Physics.CheckSphere(pos + ((testedLane - 1) * laneOffset * (rot*Vector3.right)), 0.4f, 1<<9))
			{
				testedLane = (testedLane + 1) % 3;
				if (currentLane == testedLane)
				{
					laneValid = false;
					break;
				}
			}

			currentLane = testedLane;

			if(laneValid)
			{
				pos = pos + ((currentLane - 1) * laneOffset * (rot * Vector3.right));

				GameObject toUse;
				if (PoolSystem.instance)
				{
					toUse = PoolSystem.instance.PoolOut (Catalog.Coin);
					toUse.SetActive (true);
					toUse.transform.position = pos;
					toUse.transform.rotation = rot;
					toUse.transform.SetParent(segment.collectibleTransform, true);
				}
			}

			currentWorldPos += increment;
		}
	}
}

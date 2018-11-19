using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharInputController : MonoBehaviour 
{
	public GameBoard gameBoard;
	public CharCollider characterCollider;
	public float laneChangeSpeed = 1.0f;

	protected float m_JumpStart;
	protected bool m_Jumping;
	public bool isJumping { get { return m_Jumping; } }

	[Header("Controls")]
	public float jumpLength = 2.0f;
	public float jumpHeight = 1.2f;

	public float slideLength = 2.0f;

	// -------------------------------------------------------------------------

	protected int m_ObstacleLayer;

	protected int m_CurrentLane = k_StartingLane;
	protected Vector3 m_TargetPosition = Vector3.zero;

	protected readonly Vector3 k_StartingPosition = Vector3.forward * 2f;

	protected const int k_StartingLane = 1;
	protected const float k_GroundingSpeed = 80f;
	protected const float k_ShadowRaycastDistance = 100f;

	public void Init()
	{
		transform.position = k_StartingPosition;
		m_TargetPosition = Vector3.zero;

		m_CurrentLane = k_StartingLane;
		characterCollider.transform.localPosition = Vector3.zero;

		m_ObstacleLayer = 1 << LayerMask.NameToLayer("Obstacle");
	}

	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			ChangeLane(-1);
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			ChangeLane(1);
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			Jump();
		}

		Vector3 verticalTargetPosition = m_TargetPosition;

		if(m_Jumping)
		{
			if (gameBoard.isMoving)
			{
				float correctJumpLength = jumpLength * (1.0f + gameBoard.speedRatio);
				float ratio = (gameBoard.worldDistance - m_JumpStart) / correctJumpLength;
				if (ratio >= 1.0f)
				{
					m_Jumping = false;
				}
				else
				{
					verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
				}
			}
			else
			{
				verticalTargetPosition.y = Mathf.MoveTowards (verticalTargetPosition.y, 0, k_GroundingSpeed * Time.deltaTime);
				if (Mathf.Approximately(verticalTargetPosition.y, 0f))
				{
					m_Jumping = false;
				}
			}
		}

		characterCollider.transform.localPosition = Vector3.MoveTowards(characterCollider.transform.localPosition, verticalTargetPosition, laneChangeSpeed * Time.deltaTime);
	}

	public void Jump()
	{
		if (!m_Jumping)
		{
			m_JumpStart = gameBoard.worldDistance;
			m_Jumping = true;
		}
	}

	public void ChangeLane(int direction)
	{
		if (!gameBoard.isMoving)
			return;

		int targetLane = m_CurrentLane + direction;

		if (targetLane < 0 || targetLane > 2)
			return;

		m_CurrentLane = targetLane;
		m_TargetPosition = new Vector3((m_CurrentLane - 1) * gameBoard.laneOffset, 0, 0);
	}
}

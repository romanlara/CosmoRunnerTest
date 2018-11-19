using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCollider : MonoBehaviour 
{
	public CharInputController controller;

	protected BoxCollider m_Collider;
	public new BoxCollider collider { get { return m_Collider; } }

	protected float m_StartingColliderHeight;

	protected readonly Vector3 k_SlidingColliderScale = new Vector3 (1.0f, 0.5f, 1.0f);
	protected readonly Vector3 k_NotSlidingColliderScale = new Vector3(1.0f, 2.0f, 1.0f);

	protected const float k_MagnetSpeed = 10f;
	protected const int k_CoinsLayerIndex = 8;
	protected const int k_ObstacleLayerIndex = 9;

	void Start () 
	{
		m_Collider = GetComponent<BoxCollider>();
		m_StartingColliderHeight = m_Collider.bounds.size.y;
	}

	public void Slide(bool sliding)
	{
		if (sliding)
		{
			m_Collider.size = Vector3.Scale(m_Collider.size, k_SlidingColliderScale);
			m_Collider.center = m_Collider.center - new Vector3(0.0f, m_Collider.size.y * 0.5f, 0.0f);
		}
		else
		{
			m_Collider.center = m_Collider.center + new Vector3(0.0f, m_Collider.size.y * 0.5f, 0.0f);
			m_Collider.size = Vector3.Scale(m_Collider.size, k_NotSlidingColliderScale);
		}
	}

	protected void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.layer == k_CoinsLayerIndex)
		{
			PoolSystem.instance.PoolIn (c.gameObject);
			CoinManager.instance.coinsAccum = 1;
		}
		else if(c.gameObject.layer == k_ObstacleLayerIndex)
		{
			c.enabled = false;

			AObstacle ob = c.gameObject.GetComponent<AObstacle>();

			if (ob != null)
			{
				ob.Impacted();
			}
			else
			{
				Destroy(c.gameObject);
			}
		}
	}
}

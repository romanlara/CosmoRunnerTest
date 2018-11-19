using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour 
{
	public enum pivot { X, Y, Z }

	[Header("Rotation")]
	[SerializeField]
	private float m_Speed = 2.5f;
	public float speed { get { return m_Speed; } set { m_Speed = value; } }

	[SerializeField]
	private pivot m_Axis;
	public Vector3 axis 
	{ 
		get 
		{ 
			switch (m_Axis) 
			{
			default:
			case pivot.Y: return Vector3.up;
			case pivot.X: return Vector3.right;
			case pivot.Z: return Vector3.forward;
			} 
		} 
	}

	public Transform mesh;

	void Update () 
	{
		mesh.transform.Rotate (axis * speed);
	}
}

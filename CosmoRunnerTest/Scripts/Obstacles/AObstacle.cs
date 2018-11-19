using UnityEngine;

public abstract class AObstacle : MonoBehaviour 
{
	public Animator anim { get { return GetComponent<Animator> (); } }
	
	public abstract void Spawn(Segment segment, float t);

	public virtual void Impacted () 
	{
		if (anim != null)
		{
			anim.SetTrigger("play");
		}
	}
}

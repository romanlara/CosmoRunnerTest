using UnityEngine;

public class BigBarricade : AObstacle 
{
	public override void Spawn(Segment segment, float t)
	{
		Vector3 position;
		Quaternion rotation;
		segment.GetPointAt(t, out position, out rotation);
		GameObject obj = Instantiate(gameObject, position, rotation);
		obj.transform.SetParent(segment.objectRoot, true);
	}
}

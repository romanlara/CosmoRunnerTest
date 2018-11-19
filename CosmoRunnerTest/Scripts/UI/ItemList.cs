using System.Collections;
using UnityEngine;

public class ItemList : MonoBehaviour 
{
	public GameObject template;

	public string[] items;

	public Transform container { get { return transform.GetChild (0); } }

	public void Populate ()
	{
		foreach (Transform t in container)
		{
			Destroy(t.gameObject);
		}

		for (int i = 0; i < items.Length; i++) 
		{
			GameObject newEntry = Instantiate (template) as GameObject;
			newEntry.transform.SetParent (container, false);
			newEntry.SetActive (true);

			Item itm = newEntry.GetComponent<Item> ();
			itm.name = items[i] + i;
			itm.cost = (i * 2).ToString ();
		}
	}
}

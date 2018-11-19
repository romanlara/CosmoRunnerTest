using UnityEngine;

public class Shop : MonoBehaviour 
{
	[SerializeField]
	private ItemList m_ItemList;
	public ItemList itemList { get { return m_ItemList; } set { m_ItemList = value; } }

	void OnEnable () 
	{
		if (itemList)
			itemList.Populate ();
	}
}

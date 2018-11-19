using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour 
{
	[SerializeField]
	private Text m_NameUI;
	public Text nameUI { get { return m_NameUI; } set { m_NameUI = value; } }
	public string name { set { nameUI.text = value; } }

	[SerializeField]
	private	Text m_CostUI;
	public Text costUI { get { return m_CostUI; } set { m_CostUI = value; } }
	public string cost { set { costUI.text = value; } }
}

using UnityEngine;
using UnityEngine.UI;

public class CoinManager: MonoBehaviour 
{
	public static CoinManager instance; 

	[SerializeField]
	private Text m_TextCoins;
	public Text textCoins { get { return m_TextCoins; } set { m_TextCoins = value; } }

	private int m_CoinsAccum;
	public int coinsAccum { get { return m_CoinsAccum; } set { m_CoinsAccum += Mathf.Clamp (value, 0, 99999); } }

	void Awake () 
	{
		if (instance == null)
			instance = this;
	}

	void Update ()
	{
		if (textCoins) 
		{
			textCoins.text = coinsAccum.ToString();
		}
	}
}

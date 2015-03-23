using UnityEngine;
using System.Collections;

public class ButtonServerJoinGameScript : MonoBehaviour {

	public delegate void ClickAction();
	public static event ClickAction OnClicked;
	
	public void Join_Button()
	{
		if(OnClicked != null)
		{
			// we have event listeners
			OnClicked();
		}
		else
		{
			Debug.LogError(this.ToString() + "no OnClicked() listeners");
		}
	}
}

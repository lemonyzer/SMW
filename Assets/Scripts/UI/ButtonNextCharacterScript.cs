using UnityEngine;
using System.Collections;

public class ButtonNextCharacterScript : MonoBehaviour {

	public delegate void ClickAction();
	public static event ClickAction OnClicked;

	public void Next_Button()
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

using UnityEngine;
using System.Collections;

public class ButtonStartLoadingGameScene : MonoBehaviour {

	public delegate void ClickAction();
	public static event ClickAction OnClicked;

	public void Start_Button()
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

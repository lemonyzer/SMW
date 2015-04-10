using UnityEngine;
using System.Collections;

public class ButtonSwitchTeamScript : MonoBehaviour {

	public delegate void ClickAction();
	public static event ClickAction OnClicked;

	public void SwitchTeams_Button()
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

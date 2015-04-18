using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectorSlotScript : MonoBehaviour {

	public Text characterName;
	public Image characterImage;
	public Button playerIdAndColor;
	public Button btnNextCharacter;
	public Button btnSwitchTeams;

	public void UpdateSlot(Player player)
	{
		characterName.text = player.characterScriptableObject.charName;
		//The ColorBlock for this selectable object.
		// TODO   Modifications will not be visible if transition is not ColorTint.
		if(player.team != null)
		{
			ColorBlock cb = playerIdAndColor.colors;
			cb.normalColor = player.team.mColors[0];
			playerIdAndColor.colors = cb;
		}
		else
		{
			ColorBlock cb = playerIdAndColor.colors;
			cb.normalColor = Color.white;
			playerIdAndColor.colors = cb;
			playerIdAndColor.transform.FindChild("TextSlotNumber").GetComponent<Text>().color = Color.black;
            Debug.LogError("player has no team!");
		}

		playerIdAndColor.transform.FindChild("TextSlotNumber").GetComponent<Text>().text = player.getNetworkPlayer().ToString();
		characterImage.sprite = player.characterScriptableObject.charIdleSprites[0];
	}
}

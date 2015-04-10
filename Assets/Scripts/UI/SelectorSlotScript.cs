using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectorSlotScript : MonoBehaviour {

	public Text characterName;
	public Image characterImage;
	public Button playerIdAndColor;
	public Button next;
	public Button switchTeams;

	public void UpdateSlot(Player player)
	{
		characterName.text = player.characterScriptableObject.charName;
		playerIdAndColor.transform.FindChild("TextSlotNumber").GetComponent<Text>().text = player.getNetworkPlayer().ToString();
		characterImage.sprite = player.characterScriptableObject.charIdleSprites[0];
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectorSlotScript : MonoBehaviour {

	public Text characterName;
	public Image characterImage;
	public Button playerIdAndColor;
	public Button next;

	public void UpdateSlot(Player player)
	{
		characterName.text = player.characterAvatarScript.name;
		playerIdAndColor.transform.FindChild("TextSlotNumber").GetComponent<Text>().text = player.getNetworkPlayer().ToString();
		characterImage.sprite = player.characterAvatarScript.gameObject.GetComponent<SpriteRenderer>().sprite;
	}
}

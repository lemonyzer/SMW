using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiSlotScript : MonoBehaviour {

	public Text characterName;
	public Image characterImage;
	public Button playerIdAndColor;
	public Button next;

	GameObject gameController;

	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag(Tags.gameController);
	}

	public void btn_Next()
	{
		gameController.GetComponent<TeamAndCharacterSelection>().NextCharacter_Button();
	}

	public void btn_ServerJoins()
	{
		gameController.GetComponent<TeamAndCharacterSelection>().ServerJoins_Button();
	}

	public void UpdateSlot(Player player)
	{
		characterName.text = player.characterAvatarScript.name;
		playerIdAndColor.transform.FindChild("TextSlotNumber").GetComponent<Text>().text = player.getNetworkPlayer().ToString();
		characterImage.sprite = player.characterAvatarScript.gameObject.GetComponent<SpriteRenderer>().sprite;
	}
}

using UnityEngine;
using System.Collections;

public class ChangeColorBtnScript : MonoBehaviour {

	GameObject[] sprites;

	void OnGUI()
	{
		if (GUILayout.Button ("Change Color"))
		{
			sprites = GameObject.FindGameObjectsWithTag("Player");

			foreach(GameObject go in sprites)
			{
				ColorScript script = go.GetComponent<ColorScript>();
				script.EditSpriteRendererSprite(go.GetComponent<SpriteRenderer>(), script.colorId);
				script.colorId++;
				script.colorId = script.colorId % 4;
			}
		}
	}
}

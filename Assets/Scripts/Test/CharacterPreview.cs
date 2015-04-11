using UnityEngine;
using System.Collections;

public class CharacterPreview : MonoBehaviour {

	public Sprite[] run;
	public NetworkPlayer netPlayerOwner;
	int currentSpriteIndex = 0;
	SpriteRenderer spriteRenderer;

	float switchSpriteTime = 0.1f;
	float currentSpriteTime = 0f;

	// Use this for initialization
	void Start ()
	{
		spriteRenderer = this.GetComponent<SpriteRenderer>();

		if(run == null)
		{
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
		if(currentSpriteTime > switchSpriteTime)
		{
			currentSpriteTime = 0f;

			spriteRenderer.sprite = run[currentSpriteIndex++];
		}

		currentSpriteTime += Time.deltaTime;
		currentSpriteIndex = currentSpriteIndex % 2;
	}
}

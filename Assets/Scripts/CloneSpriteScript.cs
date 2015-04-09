using UnityEngine;
using System.Collections;

public class CloneSpriteScript : MonoBehaviour {

	SpriteRenderer cloneSpriteRenderer;
	SpriteRenderer sourceSpriteRenderer;

	void Awake() 
	{
		cloneSpriteRenderer = this.GetComponent<SpriteRenderer>();
		sourceSpriteRenderer = this.transform.parent.GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void LateUpdate () {

		cloneSpriteRenderer.sortingLayerID = sourceSpriteRenderer.sortingLayerID;
		cloneSpriteRenderer.color = sourceSpriteRenderer.color;
		cloneSpriteRenderer.sprite = sourceSpriteRenderer.sprite;

	}
}

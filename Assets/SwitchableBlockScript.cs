using UnityEngine;
using System.Collections;

public class SwitchTargetBlockScript : MonoBehaviour {

	public MapBlock mapBlock;
	public Sprite defaultStateSprite;
	public Sprite switchStateSprite;
	public SpriteRenderer blockSpriteRenderer;
	public BoxCollider2D myCollider;
	public bool on;

	// Use this for initialization
	void Init () {
		blockSpriteRenderer = this.gameObject.AddComponent<SpriteRenderer> ();
		myCollider = this.gameObject.AddComponent<BoxCollider2D> ();
		myCollider.offset += new Vector2 (+0.5f,+0.5f);
	}

	void Start () 
	{

	}

	public void CreateBlock (bool state, Sprite defaultSprite, Sprite otherSprite, string spriteLayer)
	{
		Init ();
		on = state;

		myCollider.enabled = on;

//		if (on)
//		{
//			myCollider.enabled = true;
//		}
//		else if (on)
//		{
//			myCollider.enabled = true;
//		}
		defaultStateSprite = defaultSprite;
		switchStateSprite = otherSprite;
		blockSpriteRenderer.sprite = defaultSprite;
		blockSpriteRenderer.sortingLayerName = spriteLayer;
	}

	public void Switch ()
	{
		if (on)
		{
			// was on, now off
			blockSpriteRenderer.sprite = switchStateSprite;
		}
		else
		{
			// was off, now on
			blockSpriteRenderer.sprite = defaultStateSprite;
		}
		on = !on;
		myCollider.enabled = on;
	}
}

using UnityEngine;
using System.Collections;

public class SwitchableBlockScript : MonoBehaviour {

	public MapBlock mapBlock;
	public Sprite currentStateSprite;
	public Sprite defaultStateSprite;
	public Sprite switchStateSprite;
	public SpriteRenderer blockSpriteRenderer;
	public BoxCollider2D myCollider;
	public bool on;

	// Use this for initialization
	void Init () {
		blockSpriteRenderer = this.GetComponent<SpriteRenderer> ();
		myCollider = this.GetComponent<BoxCollider2D> ();
	}

	void Start () 
	{

	}

	public void CreateBlock (bool state, Sprite defaultSprite, Sprite otherSprite)
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
		currentStateSprite = defaultSprite;
		switchStateSprite = otherSprite;
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
	}
}

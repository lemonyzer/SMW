using UnityEngine;
using System.Collections;

public class SwitchTargetBlockScript : MonoBehaviour {

	public MapBlock mapBlock;
	public Sprite onStateSprite;
	public Sprite offStateSprite;
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

	public void CreateBlock (MapBlock mapBlock, bool state, Sprite onSprite, Sprite offSprite, string spriteLayer)
	{
		Init ();

		this.mapBlock = mapBlock;
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
		onStateSprite = onSprite;
		offStateSprite = offSprite;
		if (on)
			blockSpriteRenderer.sprite = onSprite;
		else
			blockSpriteRenderer.sprite = offSprite;
			
		blockSpriteRenderer.sortingLayerName = spriteLayer;
		this.gameObject.layer = LayerMask.NameToLayer (Layer.blockLayerName);
		
	}

	public void Switch ()
	{
//		Debug.Log (this.ToString () + " switching from " + on + " to " + !on );
		on = !on;
		if (on)
		{
			blockSpriteRenderer.sprite = onStateSprite;
		}
		else
		{
			blockSpriteRenderer.sprite = offStateSprite;
		}
		myCollider.enabled = on;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChildData
{
	//child components
	public GameObject gameObject;		// enthält alle components des childs (ersetzt List<ComponentData> components)
	//child
	public string name;
	public string tag;
	public string layerName;
	public Vector3 position;
	
	public ChildData(string name, string tag, string layerName, Vector3 position)
	{
		this.name = name;
		this.tag = tag;
		this.layerName = layerName;
		this.position = position;

		gameObject = new GameObject(name);
		if(tag != "")
		{
			gameObject.tag = tag;
		}
		gameObject.layer = LayerMask.NameToLayer(layerName);
		gameObject.transform.position = position;
	}
	
	/**
	 * 	generic?
	 **/
	public void Add(BoxCollider2D box, bool enabled, Vector2 size, Vector2[] smartOffset, bool isTrigger, int smartCloneCount)
	{
		for(int i=0; i < smartCloneCount; i++)
		{
			BoxCollider2D boxSmart = box;
//			if(i == 0)
//			{
//				boxSmart = box;
//			}
			if(i > 0)
			{
				boxSmart = this.gameObject.AddComponent<BoxCollider2D>();
			}

			boxSmart.isTrigger = isTrigger;
			boxSmart.size = size;
			boxSmart.offset = smartOffset[i];												// <--- smart
			//all
			boxSmart.enabled = enabled;
			//current smart Add finish
		}
	}
	
	public void Add(SpriteRenderer spriteRenderer, bool enabled, Sprite sprite, Color color, int sortingLayer)
	{
		for(int i=0; i < 1; i++)
		{
			if(true)
			{
				spriteRenderer.sprite = sprite;
				spriteRenderer.color = color;
				spriteRenderer.sortingLayerID = sortingLayer;
				
				//all
				spriteRenderer.enabled = enabled;
			}
			//current smart Add finish
		}
	}

	public void Add(Rigidbody2D rb2d, float gravityScale, bool fixedAngle)
	{
		for(int i=0; i < 1; i++)
		{
			if(true)
			{
				rb2d.gravityScale = gravityScale;
				rb2d.fixedAngle = fixedAngle;
				
				//all
//				rb2d.enabled = enabled;
			}
			//current smart Add finish
		}
	}
	
	public void Add(Animator anim, bool enabled, RuntimeAnimatorController runtimeAnimatorController)
	{
		for(int i=0; i < 1; i++)
		{
			if(true)
			{
				anim.runtimeAnimatorController = runtimeAnimatorController;
				
				//all
				anim.enabled = enabled;
			}
			//current smart Add finish
		}
	}

	public void Add(NetworkView networkView, bool enabled, NetworkedPlayer netPlayerScript)
	{
		for(int i=0; i < 1; i++)
		{
			if(true)
			{
				networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
				networkView.observed = netPlayerScript;
				
				//all
				networkView.enabled = enabled;
			}
			//current smart Add finish
		}
	}
	
	public void Add(Behaviour script, bool enabled)
	{
		for(int i=0; i < 1; i++)
		{
			if(true)
			{

				//all
				script.enabled = enabled;
			}
			//current smart Add finish
		}
	}
}

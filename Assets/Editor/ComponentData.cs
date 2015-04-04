using UnityEngine;
using System.Collections;

public class ComponentData
{
	public System.Type componentType;
//	public UnityEngine.Object genericComponent;
	
	//smart
	public int smartCloneCount = 1;		// sond würden nur mehrfach BoxCollider geadded werden
	
	//all
	public bool enabled;
	
	//Boxcollider2D
	public Vector2 size;
	public Vector2[] smartOffset;
	public bool isTrigger = false;
	
	//SpriteRenderer
	public Sprite sprite;
	public Color color; 
	public int sortingLayer; 
	
	//Animator
	public RuntimeAnimatorController runtimeAnimatorController;
	public AnimatorController animatorController;

	public ComponentData(System.Type componentType, bool enabled, Vector2 size, Vector2[] smartOffset, bool isTrigger, int smartCloneCount)
	{
		if (componentType == typeof(BoxCollider2D))
		{
			this.componentType = componentType;		//wichtig
			this.enabled = enabled;					//all
			this.size = size;
			this.smartOffset = smartOffset;
			this.isTrigger = isTrigger;
			
			this.smartCloneCount = smartCloneCount;
		}
	}
	
	public ComponentData(System.Type componentType, bool enabled, Sprite sprite, Color color)
	{
		if (componentType == typeof(SpriteRenderer))
		{
			this.componentType = componentType;		//wichtig
			this.enabled = enabled;					//all
			this.sprite = sprite;
			this.color = color;
		}
	}

//	public ComponentData(System.Type componentType, bool enabled, AnimatorController animatorController)
//	{
//		if(componentType == typeof(Animator))
//		{
//			this.componentType = componentType;		//wichtig
//			this.enabled = enabled;					//all
//			this.animatorController = animatorController;
//		}
//	}

	public ComponentData(System.Type componentType, bool enabled, RuntimeAnimatorController runtimeAnimatorController)
	{
		if(componentType == typeof(Animator))
		{
			this.componentType = componentType;		//wichtig
			this.enabled = enabled;					//all
			this.runtimeAnimatorController = runtimeAnimatorController;
		}
	}
	
	public ComponentData(System.Type componentType, bool enabled)
	{
		this.componentType = componentType;		//wichtig
		this.enabled = enabled;					//all
	}
}

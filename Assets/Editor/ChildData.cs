using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChildData
{
	//			public GameObject gameObject;		//TODO weitere möglichkeit! gameObject muss kein parent gesetzt werden, nur vorbereiteter boxcollider/spriterenderer wird benötigt
	//child
	public string name;
	public string tag;
	public string layerName;
	public Vector3 position;
	
	//child components
	public List<ComponentData> components;
	
	public ChildData(string name, string tag, string layerName, Vector3 position)
	{
		//				gameObject = new GameObject(name);
		this.name = name;
		this.tag = tag;
		this.layerName = layerName;
		this.position = position;
		
		components = new List<ComponentData>();
	}
	
	/**
	 * 	generic?
	 **/
	public void Add(System.Type componentType, bool enabled, Vector2 size, Vector2[] smartOffset, bool isTrigger, int smartCloneCount)
	{
		components.Add (new ComponentData(componentType,enabled,size,smartOffset,isTrigger, smartCloneCount));
	}
	
	public void Add(System.Type componentType, bool enabled, Sprite sprite, Color color)
	{
		components.Add (new ComponentData(componentType,enabled,sprite,color));
	}
	
//	public void Add(System.Type componentType, bool enabled, AnimatorController animatorController)
//	{
//		components.Add (new ComponentData(componentType, enabled, animatorController));
//	}

	public void Add(System.Type componentType, bool enabled, RuntimeAnimatorController runtimeAnimatorController)
	{
		components.Add (new ComponentData(componentType, enabled, runtimeAnimatorController));
	}
	
	public void Add(System.Type componentType, bool enabled)
	{
		components.Add (new ComponentData(componentType,enabled));
	}
}

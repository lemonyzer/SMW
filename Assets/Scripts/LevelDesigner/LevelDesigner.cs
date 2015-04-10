using UnityEngine;
using System.Collections;

public class LevelDesigner : MonoBehaviour {

	public GameObject prefab;
	public Sprite sprite;
	public Vector2 gizmoPosition;
	public float depth = 0;
	public Color gizmoColor = Color.grey;
	public Vector3 rotation;

	public bool useOffset = false;
	public float offsetX = -0.5f;
	public float offsetY = -0.5f;

	public float sizeX = 1;
	public float sizeY = 1;

	void OnDrawGizmos()
	{
		Gizmos.color = gizmoColor;
		if(useOffset)
			Gizmos.DrawWireCube(new Vector3(gizmoPosition.x+offsetX, gizmoPosition.y+offsetY, depth), new Vector3(sizeX,sizeY,1));
		else
			Gizmos.DrawWireCube(new Vector3(gizmoPosition.x, gizmoPosition.y, depth), new Vector3(sizeX,sizeY,1));
//		Debug.Log(sprite.bounds);
	}
}

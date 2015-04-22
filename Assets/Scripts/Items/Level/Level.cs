using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

	GameObject background;
	SpriteRenderer bgSpriteRenderer;

	bool backgroundFound = false;

	public float left;
	public float z;
	public float bottom;
	public float width;
	public float height;


	void Awake()
	{
		background = GameObject.FindGameObjectWithTag(Tags.tag_background);
		if(background != null)
		{
			bgSpriteRenderer = background.GetComponent<SpriteRenderer>();
			backgroundFound = true;


			left = bgSpriteRenderer.bounds.center.x - bgSpriteRenderer.bounds.extents.x;
			bottom = bgSpriteRenderer.bounds.center.y - bgSpriteRenderer.bounds.extents.y;
			
			width = bgSpriteRenderer.bounds.extents.x*2;
			height = bgSpriteRenderer.bounds.extents.y*2;
		}
		else
		{
			Debug.LogError("kein Background gesetzt, kein RandomSpawnPoint berechenbar!!!");

			/**
			 *	Mit Camera rechnen!!! 
			 **/
		}
	}

	public Vector3 getRandomSpawnPosition()
	{
		if(backgroundFound)
		{
//			float x,y,z=0;
//			x = Camera.main.transform.position.x;
//			y = Camera.main.transform.position.y;
			z = 0;


			// hat weniger mit camera zu tun!!!
			// die position des background gameobjects ist wichtig!

			// nehmen wir an transform bei -10,-7.5,0
			// spriteRenderer.sprite bounds
			// center = (0,0,0)
			
			//Sprite Pivot leftbottom! testweiße wurde background position verschoben
			//			GameController (Level): Cam Position: (0.0, 0.0, -10.0)
			//			GameController (Level): BackgroundGO Position: (10.0, 7.5, 0.0)
			//			GameController (Level): Renderer Bounds: Center: (20.0, 15.0, 0.0), Extents: (10.0, 7.5, 0.1)
			//			GameController (Level): Sprite Bounds: Center: (10.0, 7.5, 0.0), Extents: (10.0, 7.5, 0.1)
			
			//Sprite Pivot leftbottom! background correct positioniert 
			//			GameController (Level): Cam Position: (0.0, 0.0, -10.0)
			//			GameController (Level): BackgroundGO Position: (-10.0, -7.5, 0.0)
			//			GameController (Level): Renderer Bounds: Center: (0.0, 0.0, 0.0), Extents: (10.0, 7.5, 0.1)
			//			GameController (Level): Sprite Bounds: Center: (10.0, 7.5, 0.0), Extents: (10.0, 7.5, 0.1)
			
			//Sprite Pivot leftbottom! background (0,0,0) positioniert
			//			GameController (Level): Cam Position: (0.0, 0.0, -10.0)
			//			GameController (Level): BackgroundGO Position: (0.0, 0.0, 0.0)
			//			GameController (Level): Renderer Bounds: Center: (10.0, 7.5, 0.0), Extents: (10.0, 7.5, 0.1)
			//			GameController (Level): Sprite Bounds: Center: (10.0, 7.5, 0.0), Extents: (10.0, 7.5, 0.1)

			left = bgSpriteRenderer.bounds.center.x - bgSpriteRenderer.bounds.extents.x;
			bottom = bgSpriteRenderer.bounds.center.y - bgSpriteRenderer.bounds.extents.y;

			width = bgSpriteRenderer.bounds.extents.x*2;
			height = bgSpriteRenderer.bounds.extents.y*2;

			//float width = bgSpriteRenderer.sprite.bounds.extents.x*2 - left;
			//float width = bgSpriteRenderer.sprite.bounds.extents.x*2;
			//float height = bgSpriteRenderer.sprite.bounds.extents.y*2 - bottom;
			//float height = bgSpriteRenderer.sprite.bounds.extents.y*2;


			// Beam Zone abziehen (immer ++ bei left)
			left++;
			width--;

			// Floor abziehen (immer ++ bei bottom)
			bottom++;
			height--;

			
//			if(left <= 0)
//			{
//				return new Vector3(Random.Range(left,width+left),Random.Range(bottom,height),z);
//			}
//			else
//			{
//				return new Vector3(Random.Range(left,width-left),Random.Range(bottom,height),z);
//			}
			return new Vector3(Random.Range(left,width-Mathf.Abs(left)),Random.Range(bottom,height-Mathf.Abs(bottom)),z);

		}
		else
		{
			Debug.LogError("no GameObject with Tag" + Tags.tag_background + " found in current Scene. Cant calculate SpawnArea!");
			return Vector3.zero;
		}
	}

	// Use this for initialization
	void Start () {
		Debug.Log(this.ToString() +": Cam Position: " + Camera.main.transform.position);
		if(background != null)
		{
			Debug.Log(this.ToString() +": BackgroundGO Position: " + background.transform.position);
			Debug.Log(this.ToString() +": Renderer Bounds: " + bgSpriteRenderer.bounds);
			Debug.Log(this.ToString() +": Sprite Bounds: " + bgSpriteRenderer.sprite.bounds);
		}
	}
}

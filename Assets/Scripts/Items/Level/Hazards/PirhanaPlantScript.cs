using UnityEngine;
using System.Collections;

[System.Serializable]
public enum PirhanaDirection {
	upwards = 0,
	downwards = 1,
	toright = 2,
	toleft = 3 
}

public class PirhanaPlantScript : MonoBehaviour {

	public RuntimeAnimatorController greenEatingAnimatorController;
	public RuntimeAnimatorController redShootingPrihanaAnimatorController;
	public RuntimeAnimatorController greenShootingPrihanaAnimatorController;
	public RuntimeAnimatorController redEatingAnimatorController;
	public SpriteRenderer hazardRenderer;
	public BoxCollider2D hazardCollider;
	public Hazard hazard;
	public MapHazard mapHazard;

	public Sprite[] sprites;

	public void CreateHazard (Hazard hazard, MapHazard mapHazard)
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Death");	// TODO

		hazardCollider = this.gameObject.AddComponent <BoxCollider2D> ();
		hazardRenderer = this.gameObject.AddComponent <SpriteRenderer> ();
		hazardRenderer.sprite = hazard.previewSprite;
		hazardRenderer.sortingLayerName = "Hazards";
		this.hazard = hazard;
		this.mapHazard = mapHazard;

		Animator anim = this.gameObject.AddComponent<Animator> ();

		// dparam[0] == velocity
		// dparam[1] == angle
		// dparam[2] == radius
		
		// dAngle == Angle
		
		// iparam[0] == freq
		// iparam[1] == direction

		Vector2 colliderSize = Vector2.one;
		Vector2 colliderOffset = Vector2.one;


		if (mapHazard.iparam[1] == (short) PirhanaDirection.upwards)
		{
//			this.gameObject.transform.position += Vector3.down;	//TODO
		}
		else if (mapHazard.iparam[1] == (short) PirhanaDirection.downwards)
		{
//			this.gameObject.transform.position += Vector3.right;
			this.gameObject.transform.localScale = new Vector3 (1f,-1f,1f);
//			Debug.Log (this.ToString () + " HazardType.pirhana_plants_1_target", this);
		}
		else if (mapHazard.iparam[1] == (short) PirhanaDirection.toleft)
		{
			
		}
		else if (mapHazard.iparam[1] == (short) PirhanaDirection.toright)
		{
			
		}

		if (hazard.type == HazardType.pirhana_plants_0_random)
		{
			anim.runtimeAnimatorController = greenShootingPrihanaAnimatorController;
			colliderOffset.x = 0.5f;
			colliderOffset.y = 0.75f;

			colliderSize.x = 1f;
			colliderSize.y = 1.5f;
		}
		else if (hazard.type == HazardType.pirhana_plants_1_target)
		{
			anim.runtimeAnimatorController = redShootingPrihanaAnimatorController;
			colliderOffset.x = 0.5f;
			colliderOffset.y = 0.75f;
			
			colliderSize.x = 1f;
			colliderSize.y = 1.5f;
		}
		else if (hazard.type == HazardType.pirhana_plants_2_animated)
		{
			anim.runtimeAnimatorController = redEatingAnimatorController;
			colliderOffset.x = 0.5f;
			colliderOffset.y = 1f;	//
			
			colliderSize.x = 1f;
			colliderSize.y = 2f;	//
		}
		else if (hazard.type == HazardType.pirhana_plants_3_animated)
		{
			anim.runtimeAnimatorController = greenEatingAnimatorController;
			colliderOffset.x = 0.5f;
			colliderOffset.y = 0.75f;
			
			colliderSize.x = 1f;
			colliderSize.y = 1.5f;
		}

		anim.applyRootMotion = true;

		hazardCollider.offset = colliderOffset;
		hazardCollider.size = colliderSize;
	}

	[System.Serializable]
	public enum PirhanaState{
		hidden,
		release,
		released,
		close,
		count
	}

	public PirhanaState myState = PirhanaState.released;

	float frequenz = 5f;
	float currTime;
	float nextStateTimeStamp;

	void Start ()
	{
		currTime = Time.time;
		nextStateTimeStamp = GetRandomTimeStampInFrequenz ();
	}

	float GetRandomTimeStampInFrequenz ()
	{
		return Time.time + Random.Range (0f,frequenz);
	}

	void FixedUpdate ()
	{
		currTime += Time.fixedDeltaTime;

		if (currTime >= nextStateTimeStamp)
		{
			nextStateTimeStamp = GetRandomTimeStampInFrequenz ();
			NextState ();
		}


	}

	void NextState ()
	{
		if (myState == PirhanaState.hidden)
		{
			
		}
		else if (myState == PirhanaState.release)
		{
			
		}
		else if (myState == PirhanaState.released)
		{
			
		}
		else if (myState == PirhanaState.close)
		{
			
		}

//		myState++;
		myState = (PirhanaState) (((int) (myState+1)) % ((int) PirhanaState.count));

	}
}

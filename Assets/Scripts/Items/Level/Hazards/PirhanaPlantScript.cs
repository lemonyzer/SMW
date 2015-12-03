using UnityEngine;
using System.Collections;

public class PirhanaPlantScript : MonoBehaviour {

	[System.Serializable]
	public enum PirhanaDirection {
		upwards = 0,
		downwards = 1,
		toright = 2,
		toleft = 3 
	}

	public SpriteRenderer hazardRenderer;
	public BoxCollider2D hazardCollider;
	public Hazard hazard;
	public MapHazard mapHazard;

	public Sprite[] sprites;

	public void CreateHazard (Hazard hazard, MapHazard mapHazard)
	{
		hazardCollider = this.gameObject.AddComponent <BoxCollider2D> ();
		hazardRenderer = this.gameObject.AddComponent <SpriteRenderer> ();
		hazardRenderer.sprite = hazard.previewSprite;
		hazardRenderer.sortingLayerName = "Hazards";
		this.hazard = hazard;
		this.mapHazard = mapHazard;

		// dparam[0] == velocity
		// dparam[1] == angle
		// dparam[2] == radius
		
		// dAngle == Angle
		
		// iparam[0] == freq
		// iparam[1] == direction



		if (mapHazard.iparam[1] == (short) PirhanaDirection.upwards)
		{
			this.gameObject.transform.position += Vector3.down;
		}
		else if (mapHazard.iparam[1] == (short) PirhanaDirection.downwards)
		{
			this.gameObject.transform.localScale = new Vector3 (1f,-1f,1f);
		}
		else if (mapHazard.iparam[1] == (short) PirhanaDirection.toleft)
		{
			
		}
		else if (mapHazard.iparam[1] == (short) PirhanaDirection.toright)
		{
			
		}

		if (hazard.type == HazardType.pirhana_plants_0_random)
		{
			
		}
		else if (hazard.type == HazardType.pirhana_plants_1_target)
		{
			
		}
		else if (hazard.type == HazardType.pirhana_plants_2_animated)
		{
			
		}
		else if (hazard.type == HazardType.pirhana_plants_3_animated)
		{
			
		}

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

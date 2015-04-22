using UnityEngine;
using System.Collections;

public class Bot : MonoBehaviour {

	public GameObject target;
	public GameObject closest;
	public bool targetHigher;
	public bool targetAtSameHeight;
	public float heightSaveOffset = 0.5f;
	public float targetDirection;
	public float targetDistance;
	public float jumpRange = 4.0f;

	private PlatformUserControl inputScript;
	private float inputVelocity;
	private bool inputJump;

	private PlatformCharacter myPlatformCharacter;

	private float changeDirectionInterval=0.5f; // in Sekunden
	private bool ableToChangeDirection = false;
	private float deltaLastDirectionChange;

	// Use this for initialization
	void Start () {
		inputScript = this.GetComponent<PlatformUserControl>();
		myPlatformCharacter = this.GetComponent<PlatformCharacter>();
		deltaLastDirectionChange = 0.7f;
		changeDirectionInterval = RandomAIReactionTime();				// neue Reaktionszeit setzen
	}

	float RandomAIReactionTime()
	{
		return Random.Range(0.1f,0.7f);
	}
	
	// Update is called once per frame
	void Update () {
		CheckTimeBetweenLastDirectionChange();
		//FindClosestGameObjectWithPriority();
		target = FindClosestPlayerWithGameControllerNew2015();
		AIMove();
		inputScript.inputJump = inputJump;
		inputScript.SetInputHorizontal(inputVelocity);
//		MoveKeyboard(inputVelocity, inputJump);
	}

	void CheckTimeBetweenLastDirectionChange() {
		deltaLastDirectionChange += Time.deltaTime;
		if(deltaLastDirectionChange > changeDirectionInterval)				// Bot individueller  machen
		{																	// alle halbe Sekunde darf er Richtung wechseln
			ableToChangeDirection=true;
			deltaLastDirectionChange=0.0F;
			changeDirectionInterval = RandomAIReactionTime();				// neue Reaktionszeit setzen
		}
		else
			ableToChangeDirection=false;
	}

//	GameObject FindClosestPlayerWithGameController() {
//		
//		targetDistance = Mathf.Infinity;
//		Vector3 myPosition = transform.position;
//		
//		foreach (GameObject go in stats.playerList)
//		{
//			if(go != null)
//			{
//				if(go.layer != gameObject.layer)
//				{
//					// Nur andere Spieler können target werden!
//					Vector3 diff = go.transform.position - myPosition;		// targetPosition - eigenePosition
//					// diff = negatativ => eigene Position höher
//					// diff = 0 		=> gleiche Höhe
//					// diff = positiv 	=> eigene Position niedriger
//					float curDistance = diff.sqrMagnitude;
//					if (curDistance < targetDistance) {
//						closest = go;
//						targetDistance = curDistance;
//						
//						if( (diff.y <= heightSaveOffset) && (diff.y >= -heightSaveOffset) )			// save offset!!! physic has no 0.0F precision!
//						{
//							targetHigher = false;
//							targetAtSameHeight = true;
//						}
//						else if(diff.y > heightSaveOffset)
//						{
//							targetHigher = true;
//							targetAtSameHeight = false;
//						}
//						else
//						{
//							targetHigher = false;
//							targetAtSameHeight = false;
//						}
//						
//						if( -10.0f < diff.x && diff.x < 0.0f )			//between -10 and 0
//						{
//							targetDirection = -1;
//						}
//						else if( 10.0f < diff.x && diff.x < 20.0f )		//between 10 and 20
//						{
//							targetDirection = -1;
//						}
//						else 											//else (between 0 and 10)
//						{
//							targetDirection = +1;
//						}
//						
//					}
//				}
//			}
//			//			else
//			//				Debug.Log("GameObject layer: " + go.layer + " == Player(Ki) layer: " + gameObject.layer);
//		}
//		
//		//		Debug.Log(character.name + " my position: " + myPosition);
//		//		Debug.Log(character.name + " hunts " + closest.name + " with position: " + closest.transform.position);
//		return closest;
//	}
//
//	GameObject FindClosestPlayerWithGameControllerNew() {
//		
//		targetDistance = Mathf.Infinity;
//		Vector3 myPosition = transform.position;
//		
//		//		for(IEnumerator e = spawnScript.playerDictonary.GetEnumerator(); e.MoveNext();)
//		//		{
//		//
//		//		}
//		if(gameController == null)
//			return null;
//		
//		if(spawnScript == null)
//			return null;
//		
//		foreach(GameObject go in spawnScript.playerDictonary.Keys)
//		{
//			if(go != null)
//			{
//				// GameObject existiert
//				
//				if(go.layer != gameObject.layer)
//				{
//					// Nur Spieler aus anderem Team (Teamlayer) können target werden!
//					
//					if(!spawnScript.playerDictonary[go].getCharacter().getPlatformCharacter().isDead)
//					{
//						// Nur wenn Spieler lebt
//						
//						Vector3 diff = go.transform.position - myPosition;		// targetPosition - eigenePosition
//						// diff = negatativ => eigene Position höher
//						// diff = 0 		=> gleiche Höhe
//						// diff = positiv 	=> eigene Position niedriger
//						float curDistance = diff.sqrMagnitude;
//						if (curDistance < targetDistance) {
//							closest = go;
//							targetDistance = curDistance;
//							
//							if( (diff.y <= heightSaveOffset) && (diff.y >= -heightSaveOffset) )			// save offset!!! physic has no 0.0F precision!
//							{
//								targetHigher = false;
//								targetAtSameHeight = true;
//							}
//							else if(diff.y > heightSaveOffset)
//							{
//								targetHigher = true;
//								targetAtSameHeight = false;
//							}
//							else
//							{
//								targetHigher = false;
//								targetAtSameHeight = false;
//							}
//							
//							if( -10.0f < diff.x && diff.x < 0.0f )			//between -10 and 0
//							{
//								targetDirection = -1;
//							}
//							else if( 10.0f < diff.x && diff.x < 20.0f )		//between 10 and 20
//							{
//								targetDirection = -1;
//							}
//							else 											//else (between 0 and 10)
//							{
//								targetDirection = +1;
//							}
//						}
//					}
//				}
//			}
//			//			else
//			//				Debug.Log("GameObject layer: " + go.layer + " == Player(Ki) layer: " + gameObject.layer);
//		}
//		
//		//		Debug.Log(character.name + " my position: " + myPosition);
//		//		Debug.Log(character.name + " hunts " + closest.name + " with position: " + closest.transform.position);
//		return closest;
//	}

	GameObject FindClosestPlayerWithGameControllerNew2015() {
		
		targetDistance = Mathf.Infinity;
		Vector3 myPosition = transform.position;
		
		//		for(IEnumerator e = spawnScript.playerDictonary.GetEnumerator(); e.MoveNext();)
		//		{
		//
		//		}
//		if(gameController == null)
//			return null;
//		
//		if(spawnScript == null)
//			return null;

		foreach(Player player in PlayerDictionaryManager._instance.Values())
		{

			if(player.platformCharacterScript.gameObject != null)
			{
				GameObject go = player.platformCharacterScript.gameObject;
				if(go != this.gameObject)
				{
					if(player.platformCharacterScript.CanTakeDamage())
					{
						// Nur wenn Spieler lebt
						
						Vector3 diff = go.transform.position - myPosition;		// targetPosition - eigenePosition
						// diff = negatativ => eigene Position höher
						// diff = 0 		=> gleiche Höhe
						// diff = positiv 	=> eigene Position niedriger
						float curDistance = diff.sqrMagnitude;
						if (curDistance < targetDistance) {
							closest = go;
							targetDistance = curDistance;
							
							if( (diff.y <= heightSaveOffset) && (diff.y >= -heightSaveOffset) )			// save offset!!! physic has no 0.0F precision!
							{
								targetHigher = false;
								targetAtSameHeight = true;
							}
							else if(diff.y > heightSaveOffset)
							{
								targetHigher = true;
								targetAtSameHeight = false;
							}
							else
							{
								targetHigher = false;
								targetAtSameHeight = false;
							}
							
							if( -10.0f < diff.x && diff.x < 0.0f )			//between -10 and 0
							{
								targetDirection = -1;
							}
							else if( 10.0f < diff.x && diff.x < 20.0f )		//between 10 and 20
							{
								targetDirection = -1;
							}
							else 											//else (between 0 and 10)
							{
								targetDirection = +1;
							}
						}
					}
				}
			}
		}
			//			else
			//				Debug.Log("GameObject layer: " + go.layer + " == Player(Ki) layer: " + gameObject.layer);
		
		//		Debug.Log(character.name + " my position: " + myPosition);
		//		Debug.Log(character.name + " hunts " + closest.name + " with position: " + closest.transform.position);
		return closest;
	}

	// Update is called once per frame
	void AIMove () {
		/* Spotting überflüssig, da alle Mitspielderpositionen bekannt sind -> bewege zum nächsten Spieler */
		/*
		spotted = Physics2D.OverlapCircle (spotCheck.position, spottingRadius, whatIsSpotted);
		*/
		
		if(target != null)
		{
			if(target.GetComponent<PlatformCharacter>().isInRageModus)
			{
				// Rage ganz nah, wegrennen!
				inputVelocity = -targetDirection;
				if(targetDistance < jumpRange) 
				{
					inputJump = true;				// Target in Jumprange, Springen!
				}
			}
			else
			{
				if(!targetHigher)
				{
					//Bot ist höher oder auf gleicher Höhe
					inputJump = false;
					
					if(ableToChangeDirection)					// Bot leichter machen
						inputVelocity = targetDirection;
					
					if(targetAtSameHeight)
					{
						if(!myPlatformCharacter.isInRageModus)
						{
							if(targetDistance < jumpRange) 
							{
								inputJump = true;				// Target in Jumprange, Springen!
							}
						}
					}
				}
				else if(targetHigher)
				{
					inputJump = true;
				}
			}
		}
		else
		{
			// kein Gegner mehr da
			inputJump = false;
			inputVelocity = 0f;
		}
	}
}

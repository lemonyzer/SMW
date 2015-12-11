using UnityEngine;
using System.Collections;

public class FlameCannonScript : MonoBehaviour {

	Transform myTransform;
	public MapHazard hazard;
	float shootIntervalMin = 2f;
	float shootIntervalMax = 4f;
	public float currenFlameDuration = 0f;
	public float timeStampLastShoot;
	public float timeStampNextShoot;
	public GameObject flames;

	public void Create (MapHazard hazard, Hazard listHazard)
	{
		this.hazard = hazard;
		flames = new GameObject ("Flammes");
		flames.transform.SetParent (this.gameObject.transform, true);

		Vector3 scale = Vector3.one;
		Vector3 offset = Vector3.zero;
		if (hazard.iparam[1] == 0)
		{
			// -> flammen nach rechts
			scale.x = 1;
		}
		else if (hazard.iparam[1] == 1)
		{
			// <- flammen nach links
			scale.x = -1;
			offset.x = +1;
		}

		Vector3 pos = this.transform.position;
		// Pivot Center
		Sprite sprite = listHazard.projectile[0];
//		pos.x += scale.x * sprite.bounds.extents.x - sprite.bounds.center.x + offset.x;
//		pos.x += offset.x + sprite.bounds.extents.x - sprite.bounds.center.x + offset.x * (sprite.bounds.center.x - sprite.bounds.extents.x);

		// Pivot: ANY!
		//  -> offset = 0
		//  <- offset = 1 == 32 (tilewidth)
		pos.x += offset.x + scale.x * (sprite.bounds.extents.x - sprite.bounds.center.x);
		pos.y += sprite.bounds.extents.y - sprite.bounds.center.y;

		BoxCollider2D hitArea = flames.AddComponent <BoxCollider2D> ();
		Vector2 hitAreaOffset = Vector2.zero;
		hitAreaOffset.x = sprite.bounds.center.x; 
		hitAreaOffset.y = sprite.bounds.center.y;
		hitArea.offset = hitAreaOffset;

		Vector2 hitAreaSize = Vector2.zero;
		hitAreaSize.x = sprite.bounds.size.x;
		hitAreaSize.y = sprite.bounds.size.y;
		hitArea.size = hitAreaSize;
		
//		// Pivot BottomLeft
//		pos.x += listHazard.projectile[0].bounds.extents.x - listHazard.projectile[0].bounds.center.x;
//		pos.y += listHazard.projectile[0].bounds.extents.y - listHazard.projectile[0].bounds.center.y;

//		Debug.Log (this.transform.position);
//		Debug.Log (listHazard.projectile[0].bounds);
//		Debug.Log (listHazard.projectile[0].bounds.size);



		flames.transform.position = pos; 

		AnimatedTile animTileScript = flames.AddComponent<AnimatedTile> ();
		animTileScript.Init (listHazard.projectile.ToArray(), 0);

		SpriteRenderer flameRenderer = flames.GetComponent<SpriteRenderer> ();
		flameRenderer.sortingLayerName = "Hazards";

		//animTileScript.SetAnimation ();

		flames.transform.localScale = scale;
	}
	

	// Use this for initialization
	void Awake () {
		myTransform = this.transform;
	}

	public float GetRandomTimeStamp ()
	{
		return Random.Range (shootIntervalMin, shootIntervalMax);
	}


	// Update is called once per frame
	void Update () {
	
		if (Time.time >= timeStampNextShoot)
		{
			Shoot ();
		}
	}

	void Shoot ()
	{
		currenFlameDuration = GetRandomTimeStamp ();
		timeStampNextShoot = Time.time + currenFlameDuration + GetRandomTimeStamp ();

		flames.SetActive (true);

		Invoke ("DisableFlames", currenFlameDuration);

//		Color color = Color.red;
//		float duration = 2f;
//		bool depthTest = false;
//		Vector3 refPos = this.transform.position;
//		Vector3 endPos = refPos + new Vector3 (scale.x,0f,0f);
//		Vector3 arrowUpPart = new Vector3 (-scale.x, 1f, 0f) *0.5f;
//		Vector3 arrowDownPart = new Vector3 (-scale.x, -1f, 0f) *0.5f;
//		Debug.DrawLine (refPos, endPos, color, duration, depthTest);
//		Debug.DrawLine (endPos, endPos + arrowUpPart, color, duration, depthTest);
//		Debug.DrawLine (endPos, endPos + arrowDownPart, color, duration, depthTest);
		
	}

	void DisableFlames ()
	{
		flames.SetActive (false);
	}
}

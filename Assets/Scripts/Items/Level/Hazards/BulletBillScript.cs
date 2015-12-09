using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletBillScript : MonoBehaviour {

	public MapHazard hazard;
	public int poolSize = 2;
	static List<GameObject> bulletPool;
	static GameObject bulletBillPool;
	public GameObject bulletPrefab;
	float shootIntervalMin = 2f;
	float shootIntervalMax = 4f;
	public float timeStampLastShoot;
	public float timeStampNextShoot;


	public float GetRandomTimeStamp ()
	{
		return Random.Range (shootIntervalMin, shootIntervalMax);
	}

	void Awake ()
	{
		InitPool ();
	}

	void InitPool ()
	{
		if (bulletPool == null)
		{
			bulletBillPool = new GameObject ("BulletBillPool");
			bulletPool = new List<GameObject> ();
			for (int i=0; i< poolSize; i++)
			{
				AddObjectToPool (bulletBillPool);
			}
		}
	}

	GameObject AddObjectToPool (GameObject poolGO)
	{
		GameObject currentGO = GameObject.Instantiate (bulletPrefab);
		currentGO.SetActive (false);
		currentGO.transform.SetParent (poolGO.transform, true);
		bulletPool.Add (currentGO);
		return currentGO;
	}

	GameObject GetPooledObject ()
	{
		for (int i=0; i< bulletPool.Count; i++)
		{
			if (bulletPool[i] == null)
			{
				Debug.LogError (this.ToString () + " pool element not valid!");
				continue;
			}
			if (!bulletPool[i].activeSelf)
			{
				return bulletPool[i];
			}
		}
		// no unused object in pool found, add new one
		return AddObjectToPool (bulletBillPool);
	}

	// Use this for initialization
	void Start () {
	
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
		timeStampNextShoot = Time.time + GetRandomTimeStamp ();
		GameObject currBullet = GetPooledObject ();
		currBullet.SetActive (true);
		Vector3 scale = Vector3.one;
		Vector3 direction = Vector3.zero;
		float velocity = hazard.dparam[0];
		Vector3 veloctiy3 = new Vector3 (velocity, 0f, 0f); 
		direction.x = velocity / velocity;
		scale.x = velocity / Mathf.Abs(velocity);
		currBullet.transform.localScale = scale;
//		currBullet.GetComponent<Rigidbody2D>().AddForce (veloctiy3);
		BulletBillProjectileScript projectile = currBullet.GetComponent<BulletBillProjectileScript> ();
		Vector3 releasePos = this.transform.position + new Vector3 (-scale.x,0f,0f);
		projectile.Release (releasePos, direction, velocity);
		// projectile.Release (velocity);

		Color color = Color.red;
		float duration = 2f;
		bool depthTest = false;
		Vector3 refPos = this.transform.position;
		Vector3 endPos = refPos + new Vector3 (scale.x,0f,0f);
		Vector3 arrowUpPart = new Vector3 (-scale.x, 1f, 0f) *0.5f;
		Vector3 arrowDownPart = new Vector3 (-scale.x, -1f, 0f) *0.5f;
		Debug.DrawLine (refPos, endPos, color, duration, depthTest);
		Debug.DrawLine (endPos, endPos + arrowUpPart, color, duration, depthTest);
		Debug.DrawLine (endPos, endPos + arrowDownPart, color, duration, depthTest);
		
	}
	
}

using UnityEngine;
using System.Collections;

public abstract class Projectile {

	//public AudioClip spawnSound;
	public AudioClip throwSound;
	public AudioClip pickUpSound;

	public virtual void Release(GameObject ownerCharacter)
	{

	}

//	public abstract override void Collecting(GameObject itemGO, PlatformCharacter collector);
//	public abstract override void Collected(PlatformCharacter collector, NetworkMessageInfo info);

	protected string resourcesFolder = "";				//wird vererbt
	protected string projectilePrefabFilename = "";		//wird vererbt

	protected Vector3 projectileSpawnPositionOffset;	//wird vererbt

	public Projectile(string resFolder, string prefabFilename, Vector3 spawnOffset)
	{
		this.resourcesFolder = resFolder;
		this.projectilePrefabFilename = prefabFilename;
		this.projectileSpawnPositionOffset = spawnOffset;
	}

	public Vector3 ProjectileSpawnPositionOffset		//ist von überall erreichbar
	{
		get
		{
			return projectileSpawnPositionOffset;
		}
		set
		{
			projectileSpawnPositionOffset = value;
		}
	}

//	public abstract GameObject SpawnSingle(GameObject ownerCharacter, Vector3 playerPosition);

	public virtual GameObject Instantiate(GameObject ownerCharacter)
	{
		//Resource finden und laden
		
		GameObject projectilePrefab = (GameObject) Resources.Load(resourcesFolder+"/"+projectilePrefabFilename, typeof(GameObject));
		if(projectilePrefab == null)
		{
			Debug.LogError("bulletPrefab coudn't be loaded!!!! check path / and name");
			return null;
			//			return;
		}
		
		//GameObject instantieren
		//		Vector3 playerPosition = ownerCharacter.transform.position;
		
		GameObject projectileGameObject = null;
		if(Network.isServer)
		{
			projectileGameObject = (GameObject) Network.Instantiate( projectilePrefab, new Vector3(ownerCharacter.transform.localScale.x * projectileSpawnPositionOffset.x,1* projectileSpawnPositionOffset.y,1* projectileSpawnPositionOffset.z) + ownerCharacter.transform.position, Quaternion.identity, 0);
		}
		else if(Network.peerType == NetworkPeerType.Disconnected)
		{
			projectileGameObject = (GameObject) GameObject.Instantiate(projectilePrefab, new Vector3(ownerCharacter.transform.localScale.x * projectileSpawnPositionOffset.x,1* projectileSpawnPositionOffset.y,1* projectileSpawnPositionOffset.z) + ownerCharacter.transform.position, Quaternion.identity);
		}
		
		//GameObject Script anpassen
		
		AuthoritativeProjectile projectileScript = projectileGameObject.GetComponent<AuthoritativeProjectile>();
		
		projectileScript.ownerCharacter = ownerCharacter.GetComponent<PlatformCharacter>();// important!!!
		
		return projectileGameObject;
	}
	
}

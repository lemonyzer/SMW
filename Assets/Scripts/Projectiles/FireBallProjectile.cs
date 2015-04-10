using UnityEngine;
using System.Collections;

public class FireBallProjectile : Projectile {
	
//	public abstract override void Collecting(GameObject itemGO, PlatformCharacter collector);
//	public abstract override void Collected(PlatformCharacter collector, NetworkMessageInfo info);

//	public string resourcesFolder = "PowerUps";									<--- durch vererbung enthalten
//	public string projectilePrefabFilename = "";								<--- durch vererbung enthalten
//
//	public  Vector3 projectileSpawnPositionOffset = new Vector3(0.75f,0,0);		<--- durch vererbung enthalten

	public FireBallProjectile(string resFolder, string prefabFilename, Vector3 spawnOffset) : base(resFolder, prefabFilename, spawnOffset)
	{
	}

	public override GameObject Instantiate(GameObject ownerCharacter)
	{
		GameObject projectileGO = base.Instantiate(ownerCharacter);

		// setze anfangsbewegung
		AuthoritativeProjectile projectileScript = projectileGO.GetComponent<AuthoritativeProjectile>();
//		projectileScript.moveDirection = new Vector3(ownerCharacter.transform.localScale.x,0,0);
		
		projectileGO.GetComponent<Rigidbody2D>().velocity = new Vector2(ownerCharacter.transform.localScale.x * projectileScript.moveSpeed.x, 0f );

		// setze bewegungsrichtung 
		projectileGO.transform.FindChild("GroundStopper").GetComponent<BulletBounce>().moveDirection.x = ownerCharacter.transform.localScale.x;

		return projectileGO;
	}
	
}

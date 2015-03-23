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

	public override GameObject SpawnSingle(GameObject ownerCharacter)
	{
		GameObject projectileGO = base.SpawnSingle(ownerCharacter);

		projectileGO.transform.FindChild("GroundStopper").GetComponent<BulletBounce>().moveDirection.x = ownerCharacter.transform.localScale.x;

		return projectileGO;
	}
	
}

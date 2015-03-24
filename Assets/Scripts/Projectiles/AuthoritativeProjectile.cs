using UnityEngine;
using System.Collections;

public abstract class AuthoritativeProjectile : MonoBehaviour {

	//	public delegate void OnProjectileHit(GameObject killer, GameObject victim);
	//	public static event OnProjectileHit onProjectileHit;

//	protected NetworkPlayer _owner;
//	abstract public NetworkPlayer owner { get; set;}

    public NetworkPlayer netOwner;
	public GameObject ownerCharacter;

	public Vector3 moveSpeed = new Vector3(5,5,0);
//	public Vector3 moveDirection = new Vector3(1,0,0);
	
}

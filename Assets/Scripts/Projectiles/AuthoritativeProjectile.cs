using UnityEngine;
using System.Collections;

public abstract class AuthoritativeProjectile : MonoBehaviour {

	//	public delegate void OnProjectileHit(GameObject killer, GameObject victim);
	//	public static event OnProjectileHit onProjectileHit;

//	protected NetworkPlayer _owner;
//	abstract public NetworkPlayer owner { get; set;}

	public AudioClip projectileSpawnAudioClip;
	public GameObject projectileHitEffectPrefab;
	public float hitEffectDurationTime = 2f;
    public NetworkPlayer netOwner;
	public PlatformCharacter ownerCharacter;

	public Vector3 moveSpeed = new Vector3(5,5,0);
//	public Vector3 moveDirection = new Vector3(1,0,0);


	public virtual void Awake()
	{
		if(projectileSpawnAudioClip != null)
		{
			this.GetComponent<AudioSource>().PlayOneShot(projectileSpawnAudioClip);
		}
	}

	public virtual void OnDestroy()
	{
		if(projectileHitEffectPrefab != null)
		{
			Destroy(Instantiate(projectileHitEffectPrefab, this.transform.position, Quaternion.identity),hitEffectDurationTime);
		}
	}

}

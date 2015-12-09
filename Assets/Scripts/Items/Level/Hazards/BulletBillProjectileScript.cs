using UnityEngine;
using System.Collections;

public class BulletBillProjectileScript : MonoBehaviour {

	Vector3 moveDirection;
	float velocity;
	Transform myTransform;
	float delayToDisable = 4f;

	// Use this for initialization
	void Awake () {
		myTransform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		myTransform.Translate (moveDirection * velocity * Time.deltaTime);
	}

	public void Release (Vector3 position, Vector3 direction, float velocity)
	{
		myTransform.position = position;
		moveDirection = direction;
		this.velocity = velocity;

//		Destroy (this.gameObject, 2f);
//		StartCoroutine ("DelayDisable");
		Invoke ("DelayDisable", delayToDisable);
	}

	void DelayDisable ()
	{
//		yield return new WaitForSeconds(delayToDisable);
		this.gameObject.SetActive (false);
	}
}

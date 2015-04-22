using UnityEngine;
using System.Collections;

public class inpuutt : MonoBehaviour {

	public Vector3 moveDirection;

	public bool invincible = false;

	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		moveDirection.x = Input.GetAxis("Horizontal");
		invincible = Input.GetKeyUp(KeyCode.Space);

		anim.SetFloat("hSpeed", moveDirection.x);
		anim.SetBool("Grounded", true);
		if(invincible)
		{
			invincible = false;
			anim.SetTrigger("Invincible");
		}


	
	}
}

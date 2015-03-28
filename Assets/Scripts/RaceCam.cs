using UnityEngine;
using System.Collections;

public class RaceCam : MonoBehaviour {

	public bool holdStartPos = true; 
	public GameObject target;

//	void Awake()
//	{
////		currentMode = camMode.
//	}
//
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}

	// auto
	private float hSpeed = 4;
	private Vector3 moveDirection = Vector3.right;

	// follow
	private Vector3 camPosition;

//	public struct camMode {
//		autoRight,
//		autoLeft,
//		autoRightLeft,
//		followLocalPlayer,
//		followSelectedPlayer,
//	}
//
//	public camMode currentMode;

	void FixedUpdate()
	{
		if(!holdStartPos)
		{
			if(target == null)
			{
				if(transform.position.x <= 0)
				{
					moveDirection = Vector3.right;
				}
				else if(transform.position.x >= 60)
				{
					moveDirection = Vector3.left;
				}
				transform.Translate( moveDirection * hSpeed * Time.fixedDeltaTime);
			}
			else
			{
				camPosition = new Vector3(target.transform.position.x, 0,-10);
				//Debug.Log(camPosition);
				transform.position = camPosition;
			}
		}
	}

	public void removeTarget()
	{
		target = null;
	}

	public void setTarget(GameObject newTarget)
	{
		target = newTarget;
	}
}

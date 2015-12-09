using UnityEngine;
using System.Collections;

public class MovingPlatformScript : MonoBehaviour {

	[SerializeField]
	public MovingPlatform movingPlatform;

	[SerializeField]
	public Vector3 startPos;

	[SerializeField]
	public Vector3 endPos;

	[SerializeField]
	public Vector3 moveDirection;

	[SerializeField]
	public bool hinweg = true;

	[SerializeField]
	Transform myTransform;

	Vector3 diff;
	
	float velocityTranslation = 0.24f;

	[SerializeField]
	Vector3 center;

	[SerializeField]
	Vector3 radius;

	[SerializeField]
	Vector3 cRadius;

	[SerializeField]
	Vector3 axis;

	// Use this for initialization
	void Start () {
		myTransform = this.transform;
		startPos = myTransform.position;
		if (movingPlatform.path.iPathType == (short) MovingPathType.StraightPath)
		{
			endPos.x = movingPlatform.path.endX / 32f - 10f;
			endPos.y = 15f - movingPlatform.path.endY / 32f - 7.5f;
			endPos.z = myTransform.position.z;

			moveDirection = endPos - startPos;
		}
		else if (movingPlatform.path.iPathType == (short) MovingPathType.StraightPathContinuous)
		{
			moveDirection.x = Mathf.Cos (movingPlatform.path.angle);
			if (Mathf.Abs (moveDirection.x) < 0.001)
				moveDirection.x = 0;

			moveDirection.y = -1* Mathf.Sin (movingPlatform.path.angle);
			if (Mathf.Abs (moveDirection.y) < 0.001)
				moveDirection.y = 0;
		}
		else if (movingPlatform.path.iPathType == (short) MovingPathType.EllipsePath)
		{
			center.x = movingPlatform.path.dCenterX / 32f - 10f;
			center.y = 15f - movingPlatform.path.dCenterY / 32f - 7.5f;
			center.z = myTransform.position.z;

			radius.x = movingPlatform.path.dRadiusX / 32.0f;
			radius.y = movingPlatform.path.dRadiusY / 32.0f;
			radius.z = myTransform.position.z;
		}
		else if (movingPlatform.path.iPathType == (short) MovingPathType.HazardString)
		{
			center.x = movingPlatform.path.dCenterX / 32f - 10f;
			center.y = 15f - movingPlatform.path.dCenterY / 32f - 7.5f;
			center.z = myTransform.position.z;
			
			radius.x = movingPlatform.path.dRadiusX / 32.0f;
			radius.y = movingPlatform.path.dRadiusY / 32.0f;
			radius.z = myTransform.position.z;

			movingPlatform.path.velocity *= 1f/Time.fixedDeltaTime *2*35;

//			movingPlatform.path.dAngle += movingPlatform.path.velocity;
			// TODO check, first rotation!
			myTransform.RotateAround (myTransform.position, Vector3.forward, movingPlatform.path.dAngle);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {

		if (movingPlatform.path.iPathType == (short) MovingPathType.StraightPath)
		{
			/*
			 * velocity *= -1;
			 */
			if (hinweg)
			{
				diff = endPos - myTransform.position;
				if (diff.sqrMagnitude < 0.1f)
					hinweg = !hinweg;
				else
					myTransform.Translate (moveDirection * Time.deltaTime * movingPlatform.path.velocity); 
			}
			else
			{
				diff = startPos - myTransform.position;
				if (diff.sqrMagnitude < 0.1f)
					hinweg = !hinweg;
				else
					myTransform.Translate (-1 * moveDirection * Time.deltaTime * movingPlatform.path.velocity); 
			}


			Debug.DrawLine (startPos + Vector3.left, startPos + Vector3.right);
			Debug.DrawLine (startPos + Vector3.up, startPos + Vector3.down);

			Debug.DrawLine (endPos + Vector3.left, endPos + Vector3.right);
			Debug.DrawLine (endPos + Vector3.up, endPos + Vector3.down);

		}
		else if (movingPlatform.path.iPathType == (short) MovingPathType.StraightPathContinuous)
		{
			myTransform.Translate (moveDirection * Time.deltaTime * movingPlatform.path.velocity);

			// 20 + 20/2 = 30
			// 15 + 15/2 = 22,5

			if (myTransform.position.x <= -20f)
			{
				myTransform.position += new Vector3(20f,0f,0f);
			}
			else if (myTransform.position.x >= 20f)
			{
				myTransform.position += new Vector3(-20f,0f,0f);
			}

			if (myTransform.position.y <= -15f)
			{
				myTransform.position += new Vector3(0f,15f,0f);
			}
			else if (myTransform.position.y >= 15f)
			{
				myTransform.position += new Vector3(0f,-15f,0f);
			}
		}
		else if (movingPlatform.path.iPathType == (short) MovingPathType.EllipsePath)
		{
			// Circle
//			myTransform.RotateAround (center, Vector3.forward, Time.deltaTime * movingPlatform.path.velocity);
//			myTransform.rotation = Quaternion.identity;

			// Ellipse
			movingPlatform.path.dAngle += movingPlatform.path.velocity;

			cRadius.x = Mathf.Cos (movingPlatform.path.dAngle) * radius.x;
			cRadius.y = Mathf.Sin (movingPlatform.path.dAngle) * radius.y;
			cRadius.z = 0f;

			myTransform.position = center + cRadius;

			if (moveDirection.x >= 0.0f)
			{
				// obere Ellipsenhälfte

			}

			else
			{
				// untere Ellipsenhälfte
			}

//			Debug.DrawLine (startPos, cRadius);
//			Debug.DrawLine (center, myTransform.position, Color.red);
//			Debug.DrawLine (center, center+cRadius, Color.yellow);

		}
		else if (movingPlatform.path.iPathType == (short) MovingPathType.HazardString)
		{
//			movingPlatform.path.dAngle += movingPlatform.path.velocity;
//			myTransform.RotateAround (myTransform.position, Vector3.forward, movingPlatform.path.dAngle);
			myTransform.RotateAround (myTransform.position, Vector3.forward, Time.deltaTime * movingPlatform.path.velocity);
		}
		
	}
}

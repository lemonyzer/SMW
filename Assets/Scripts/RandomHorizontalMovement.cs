using UnityEngine;
using System.Collections;

public class RandomHorizontalMovement : MonoBehaviour {

	bool teleportEnabled = true;
	bool teleported = false;

	float maxSpeed = 3.5f;
	float minSpeed = 0.5f;


	int direction;

	Vector2 velocity;
	Vector3 position;

	float maxHeight = 7f;
	float minHeight = 5f;

	float minStartPositionX = -10f;
	float maxStartPositionX = +10f;


	void Start () {
		InitCloud();
	}

	int RandomSign()
	{
		return Random.value < .5? 1 : -1;
	}

	float RandomValue(float minValue, float maxValue)
	{
		return Random.Range(minValue, maxValue);
	}

	void RandomDirection()
	{
		velocity.x *= RandomSign();
	}

	void RandomSpeed()
	{
		velocity.x = RandomValue(minSpeed, maxSpeed);
	}

	void RandomPosition()
	{
		RandomPosX();
		RandomHeight();
	}

	void RandomPosX()
	{
		position.x = RandomValue(minStartPositionX, maxStartPositionX);
		this.transform.position = position;
	}

	void RandomHeight()
	{
		position.y = RandomValue(minHeight, maxHeight);
		this.transform.position = position;
	}

	void ChangeSpeed()
	{
		if(velocity.x > 0)
		{
			RandomSpeed();
		}
		else if(velocity.x < 0)
		{
			RandomSpeed();
			velocity.x *= -1;
		}
	}

	void InitCloud()
	{
		RandomSpeed();
		RandomDirection();
		RandomPosition();
	}
	
	// Update is called once per frame
	void Update () {
	
		this.transform.Translate(velocity * Time.deltaTime);
		CheckBeam();
	}

	void CheckBeam()
	{
		if(!teleportEnabled)
			return;
		
		//playerPos spriterenderer boundaries
		Vector2 objPos = new Vector2(transform.position.x, transform.position.y);

		teleported = false;

		// Beam
		// 0.5 = half player size (pivot.x)
		// if players pos < leftborder+0.5
		// beam to rightborder-0.5
		if(transform.position.x < -11.5f)
		{
			teleported = true;
			objPos.x += 23f;
		}
		else if(transform.position.x > 11.5f)
		{
			teleported = true;
			objPos.x -= 23f;
		}

		if(teleported)
		{
			ChangeSpeed();
			direction = RandomSign();
			velocity.x *= direction;
			objPos.x *= direction;
			objPos.y = RandomValue(minHeight, maxHeight);
			transform.position = objPos;
		}
	}
}

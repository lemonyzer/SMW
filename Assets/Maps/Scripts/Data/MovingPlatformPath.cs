using UnityEngine;
using System;

[Serializable]
public class MovingPlatformPath {

	[SerializeField]
	public int iPathType;

	[SerializeField]
	public float velocity;
	[SerializeField]
	public float startX;
	[SerializeField]
	public float startY;
	[SerializeField]
	public float endX;
	[SerializeField]
	public float endY;
	[SerializeField]
	public bool preview;

	[SerializeField]
	public float angle;

	[SerializeField]
	public float dAngle;
	[SerializeField]
	public float dRadiusX;
	[SerializeField]
	public float dRadiusY;
	[SerializeField]
	public float dCenterX;
	[SerializeField]
	public float dCenterY;
	
	public MovingPlatformPath(float vel, float startX, float startY, float endX, float endY, bool preview)
	{
		this.velocity = vel;
		this.startX = startX;
		this.startY = startY;
		this.endX = endX;
		this.endY = endY;
		this.preview = preview;
	}

	public MovingPlatformPath(float vel, float startX, float startY, float angle, bool preview)
	{
		this.velocity = vel;
		this.startX = startX;
		this.startY = startY;
		this.angle = angle;
		this.preview = preview;
	}

	public MovingPlatformPath (float vel, float dAngle, float dRadiusX, float dRadiusY, float dCenterX, float dCenterY, bool preview)
	{
		this.velocity = vel;
		this.dAngle = dAngle;
		this.dRadiusX = dRadiusX;
		this.dRadiusY = dRadiusY;
		this.dCenterX = dCenterX;
		this.dCenterY = dCenterY;
		this.preview = preview;
	}

	public MovingPlatformPath (float startX, float startY)
	{
		this.startX = startX;
		this.startY = startY;
	}

	public virtual float Velocity (float vel)
	{
		return velocity;
	}
}
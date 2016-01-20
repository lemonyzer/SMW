using System;

[Serializable]
public class StraightPath : MovingPlatformPath {
	
//	[SerializeField]
//	public float velocity;
//	[SerializeField]
//	public float startX;
//	[SerializeField]
//	public float startY;
//	[SerializeField]
//	public float endX;
//	[SerializeField]
//	public float endY;
//	[SerializeField]
//	public bool preview;
	
	public StraightPath(float vel, float startX, float startY, float endX, float endY, bool preview) : 
		base (vel, startX, startY, endX, endY, preview)
	{
		this.velocity = Velocity (vel);
		this.startX = startX;
		this.startY = startY;
		this.endX = endX;
		this.endY = endY;
		this.preview = preview;
	}

	public StraightPath(float vel, float startX, float startY, float angle, bool preview) : 
		base (vel, startX, startY, angle, preview)
	{
		this.velocity = Velocity (vel);
		this.startX = startX;
		this.startY = startY;
		this.angle = angle;
		this.preview = preview;
	}

//	public override float Velocity (float vel)
//	{
//		return velocity * 4.0f;
//	}
}

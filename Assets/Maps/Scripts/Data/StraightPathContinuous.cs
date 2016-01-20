using System;

[Serializable]
public class StraightPathContinuous : StraightPath {
	
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
	
	public StraightPathContinuous(float vel, float startX, float startY, float angle, bool preview) : 
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
using System;

[Serializable]
public class EllipsePath : MovingPlatformPath {
	
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

//	[SerializeField]
//	float dAngle;
//	[SerializeField]
//	float dRadiusX;
//	[SerializeField]
//	float dRadiusY;
//	[SerializeField]
//	float dCenterX;
//	[SerializeField]
//	float dCenterY;
	
	public EllipsePath (float vel, float dAngle, float dRadiusX, float dRadiusY, float dCenterX, float dCenterY, bool preview) : 
		base (vel, dAngle, dRadiusX, dRadiusY, dCenterX, dCenterY, preview)
	{
		this.velocity = Velocity (vel);
		this.dAngle = dAngle;
		this.dRadiusX = dRadiusX;
		this.dRadiusY = dRadiusY;
		this.dCenterX = dCenterX;
		this.dCenterY = dCenterY;
		this.preview = preview;
	}

//	public override float Velocity (float vel)
//	{
//		return velocity / 0.003f;
//	}
}
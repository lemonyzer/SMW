using System;

[Serializable]
public class MapHazard
{
	public HazardType iType;
	public short ix;
	public short iy;
	
	public short[] iparam = new short[Globals.NUMMAPHAZARDPARAMS];
	public float[] dparam = new float[Globals.NUMMAPHAZARDPARAMS];
	
	// dparam[0] == velocity
	// dparam[1] == angle
	// dparam[2] == radius

	// dAngle == Angle

	//* Pirhana
	// iparam[0] == freq
	// iparam[1] == direction
	
	//* Bullet Bill
	// iparam[1] == direction * velocity
};
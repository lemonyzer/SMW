using System;

[Serializable]
public class WarpExit
{
	public short direction;
	public short connection;
	public short id;
	
	public short x; //Player location where player warps out of
	public short y; 
	
	public short lockx;  //Location to display lock icon
	public short locky;  
	
	public short warpx;  //map grid location for first block in warp
	public short warpy;
	public short numblocks;  //number of warp blocks for this warp
	
	public short locktimer;  //If > 0, then warp is locked and has this many frames left until unlock
};
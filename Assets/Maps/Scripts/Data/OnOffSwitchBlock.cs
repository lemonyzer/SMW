using UnityEngine;
using System;

[Serializable]
public class OnOffSwitchBlock
{
	[SerializeField]
	int x;
	[SerializeField]
	int y;

	public OnOffSwitchBlock (int x, int y, bool state)
	{
		this.x = x;
		this.y = y;
		this.state = state;
	}

	public int X {
		get {return x;}
		private set {x = value;}
	}
	
	public int Y {
		get {return y;}
		private set {y = value;}
	}
	
	[SerializeField]
	bool state;

	public bool State {
		get {return state;}
		set {state = value;}
	}
}
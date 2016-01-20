using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SwitchConnection
{
	[SerializeField]
	List<SwitchTargetBlock> targetBlocks;
	[SerializeField]
	List<SwitchTargetBlockScript> targetBlocksScripts;

	[SerializeField]
	List<OnOffSwitchBlock> switches;
	[SerializeField]
	List<OnOffSwitchBlockScript> switchScripts;

//	[SerializeField]
//	int x;
//	[SerializeField]
//	int y;

//	[SerializeField]
//	bool state;
	
	public SwitchConnection()
	{
		targetBlocks = new List<SwitchTargetBlock> ();
		targetBlocksScripts = new List<SwitchTargetBlockScript> ();
		switches = new List<OnOffSwitchBlock> ();
		switchScripts = new List<OnOffSwitchBlockScript> ();
//		iSettings = new short[Globals.NUM_BLOCK_SETTINGS];
//		iSettings = new List<short> ();
	}

	public void AddSwitch (int x, int y, bool state)
	{
		switches.Add (new OnOffSwitchBlock (x, y, state));
	}
	public void AddSwitchScript (OnOffSwitchBlockScript switchScript)
	{
		switchScripts.Add (switchScript);
	}
	
//	public bool State {
//		get {return state;}
//		set {state = value;}
//	}

	public void AddBlock (SwitchTargetBlock block)
	{
		targetBlocks.Add (block);
	}
	public void AddBlockScript (SwitchTargetBlockScript blockScript)
	{
		targetBlocksScripts.Add (blockScript);
	}

	public void Switch ()
	{
		for (int i=0; i< targetBlocksScripts.Count; i++)
		{
			if (targetBlocksScripts[i] != null)
			{
				targetBlocksScripts[i].Switch ();
			}
			else
				Debug.LogError ("targetBlocksScripts[" + i + "] == NULL");
		}
		for (int i=0; i< switchScripts.Count; i++)
		{
			if (switchScripts[i] != null)
			{
				switchScripts[i].Switch ();
			}
			else
				Debug.LogError ("switchScripts[" + i + "] == NULL");
		}
	}
};
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class MapBlock
{
	[SerializeField]
	public short iType;
	//	public short iSettings[NUM_BLOCK_SETTINGS];
	[SerializeField]
	private List<short> iSettings;
	[SerializeField]
	public bool fHidden;

	public MapBlock()
	{
//		iSettings = new short[Globals.NUM_BLOCK_SETTINGS];
		iSettings = new List<short> ();
	}

	public void AddSetting (short settingsItem)
	{
		iSettings.Add (settingsItem);
	}

	public short GetSetting (int index)
	{
		return iSettings[index];
	}

	public int SettingsCount ()
	{
		return iSettings.Count;
	}
};
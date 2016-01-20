using UnityEngine;
using System;

[Serializable]
public class TilesetTranslation 
{
	[SerializeField]
	public short iTilesetID;
//	public char[] szName;	// TODO 128 -> TILESET_TRANSLATION_CSTRING_SIZE
	// szName ersetzt durch string!!!
	[SerializeField]
	public string Name;

	public TilesetTranslation()
	{
		iTilesetID = 0;
		Name = null;
	}
};
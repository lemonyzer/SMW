using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SmwCharacterList : ScriptableObject {

	public List<SmwCharacter> characterList;

	public void Save()
	{
		UnityEditor.EditorUtility.SetDirty(this);
	}

}

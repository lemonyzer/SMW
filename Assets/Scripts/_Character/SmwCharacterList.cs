using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SmwCharacterList : ScriptableObject {

	public List<SmwCharacter> characterList;

	public void Save()
	{
#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
#endif
	}

	public void SetCharacterIDs()
	{
		for(int i=0; i < characterList.Count; i++)
		{
			if(characterList[i] != null)
			{
				characterList[i].SetID(i);
			}
		}
	}

	public void SetAllNotInUse()
	{
		for(int i=0; i < characterList.Count; i++)
		{
			if(characterList[i] != null)
			{
				characterList[i].charInUse = false;
//				characterList[i].netPlayer = null;		// geht nicht NetworkPlayer = valuetype
				characterList[i].player = null;
			}
		}
	}

	public SmwCharacter GetFirstUnselected()
	{
		for (int i=0; i < characterList.Count; i++)
		{
			if (characterList[i] != null)
			{
				if (characterList[i].charInUse == false)
				{
					Debug.Log (this.ToString() + " Character " + i + " frei");
					return characterList[i];
				}
			}
		}
		Debug.LogError (this.ToString() + " alle Charactere in Benutzung");
		return null;
	}


	public SmwCharacter GetNextUnselected(int currentId)
	{
		SmwCharacter temp;
		
		if(currentId < 0)
			currentId = 0;
		
		for(int i=currentId; i < characterList.Count; i++)
		{
			temp = characterList[i];
			if(temp == null)
			{
				Debug.LogError("CharactersArray Element "+ i +" has no SmcCharacter, check " + this.ToString());
				break;
			}
			if(!temp.charInUse)
			{
				Debug.Log (this.ToString() + " Character " + i + " frei");
				return temp;
			}
		}
		
		if(currentId > 0 && currentId < characterList.Count)
		{
			for(int i=0; i < currentId; i++)
			{
				temp = characterList[i];
				if(temp == null)
				{
					Debug.LogError("CharactersArray Element "+ i +" has no SmcCharacter check " + this.ToString());
					break;
				}
				if(!temp.charInUse)
				{
					Debug.Log (this.ToString() + " Character " + i + " frei");
					return temp;
				}
			}
		}
		
		return null;
	}


	public SmwCharacter Get (int i) {
		if(i < characterList.Count && i >= 0)
		{
			return characterList[i];
		}
		else
			return null;
	}


}

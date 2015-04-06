using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SmwCharacterList : ScriptableObject {

	public List<Character> charactersList;
	[SerializeField]
	private List<SmwCharacter> characterSOList;

	public int Count 
	{ 
		get { return characterSOList.Count; }
	}

//	public List<Character> characters

	// Awake() wird bei ScriptableObject.Create asugeführt!!!!


	private void Check()
	{
		if(charactersList == null)
		{
			Debug.LogWarning(this.ToString() + " characterSOList war nicht vorhanden");
			characterSOList = new List<SmwCharacter>();
		}
		else
			Debug.LogWarning(this.ToString() + " characterSOList war vorhanden");
	}


	public SmwCharacterList()
	{
		Debug.LogWarning(this.ToString() + " Konstruktor () - > ScriptableObject erzeugt");		// wird auch at Runtime ausgeführt
		Check();
	}

	public void Awake()
	{
		Debug.LogWarning(this.ToString() + " Awake ()");		// Awake() wird bei ScriptableObject.Create asugeführt!!!!
		Check();
	}

	public void Start()
	{
		Debug.LogWarning(this.ToString() + " Start ()");
		Check();
	}




	public void Save()
	{
#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
#endif
	}

	public void SetCharacterIDs()
	{
		for(int i=0; i < characterSOList.Count; i++)
		{
			if(characterSOList[i] != null)
			{
				characterSOList[i].SetID(i);
			}
		}
	}

	public void SetAllNotInUse()
	{
		for(int i=0; i < characterSOList.Count; i++)
		{
			if(characterSOList[i] != null)
			{
				characterSOList[i].charInUse = false;
//				characterList[i].netPlayer = null;		// geht nicht NetworkPlayer = valuetype
				characterSOList[i].player = null;
			}
		}
	}

	public SmwCharacter GetFirstUnselected()
	{
		for (int i=0; i < characterSOList.Count; i++)
		{
			if (characterSOList[i] != null)
			{
				if (characterSOList[i].charInUse == false)
				{
					Debug.Log (this.ToString() + " Character " + i + " frei");
					return characterSOList[i];
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
		
		for(int i=currentId; i < characterSOList.Count; i++)
		{
			temp = characterSOList[i];
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
		
		if(currentId > 0 && currentId < characterSOList.Count)
		{
			for(int i=0; i < currentId; i++)
			{
				temp = characterSOList[i];
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
		if(i < characterSOList.Count && i >= 0)
		{
			return characterSOList[i];
		}
		else
			return null;
	}

	public void Add(SmwCharacter charSO)
	{
		if(charSO != null)
			characterSOList.Add(charSO);
	}

	public void RemoveAt (int index)
	{
		characterSOList.RemoveAt (index);
	}

	public void Clear ()
	{
		characterSOList.Clear ();
	}

}

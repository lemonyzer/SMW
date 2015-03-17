using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterLibrary : MonoBehaviour {

	public GameObject[] characters;

	// Initialisierung
	void Awake()
	{
		for(int i=0; i < characters.Length; i++)
		{
			characters[i].GetComponent<CharacterAvatar>().id = i;
			characters[i].GetComponent<CharacterAvatar>().inUse = false;
		}

	}

	public CharacterAvatar GetFirstUnselected()
	{
		CharacterAvatar temp;
		for(int i=0; i < characters.Length; i++)
		{
			temp = characters[i].GetComponent<CharacterAvatar>();
			if(temp == null)
			{
				Debug.Log("CharactersArray Element "+ i +" has no CharacterAvatar Component!");
				break;
			}
			if(!temp.inUse)
			{
				return temp;
			}
		}
		return null;
	}

	// 
	public CharacterAvatar GetNextUnselected(int currentAvatarId)
	{
		CharacterAvatar temp;

		if(currentAvatarId < 0)
			currentAvatarId = 0;

		for(int i=currentAvatarId; i < characters.Length; i++)
		{
			temp = characters[i].GetComponent<CharacterAvatar>();
			if(temp == null)
			{
				Debug.Log("CharactersArray Element "+ i +" has no CharacterAvatar Component!");
				break;
			}
			if(!temp.inUse)
			{
				return temp;
			}
		}

		if(currentAvatarId > 0 && currentAvatarId < characters.Length)
		{
			for(int i=0; i < currentAvatarId; i++)
			{
				temp = characters[i].GetComponent<CharacterAvatar>();
				if(temp == null)
				{
					Debug.Log("CharactersArray Element "+ i +" has no CharacterAvatar Component!");
					break;
				}
				if(!temp.inUse)
				{
					return temp;
				}
			}
		}

		return null;
	}

	 
	public CharacterAvatar Get (int i) {
		if(i < characters.Length && i >= 0)
		{
			return characters[i].GetComponent<CharacterAvatar>();
		}
		else
			return null;
	}





























//	public GameObject getRandom () {
//		return characters[Random.Range(0,characters.Length)];
//	}
//	
//	public GameObject get (int i) {
//		if(i < characters.Length && i >= 0)
//		{
//			return characters[i];
//		}
//		else
//			return null;
//	}
//
//	public GameObject getLast(int i)
//	{
//		if((i-1) < 0)
//		{
//			i = characters.Length;
//		}
//		return get (i-1);
//	}
//
//	public GameObject getNext(int i)
//	{
//		if((i+1) >= characters.Length)
//		{
//			i = -1;
//		}
//		return get (i+1);
//	}
//
//	public void showCharacter()
//	{
//		Instantiate(getRandom(), Vector3.zero, Quaternion.identity);
//	}
}

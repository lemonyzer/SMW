using UnityEngine;
using System.Collections;

public class CharacterAvatar : MonoBehaviour {

	public int id;
	public string name;
	public bool inUse;
	public NetworkPlayer selector;

	void Awake()
	{
		name = this.gameObject.name;
	}

}

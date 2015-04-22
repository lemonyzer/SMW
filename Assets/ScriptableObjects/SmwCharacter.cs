using UnityEngine;
using System.Collections;

[System.Serializable]
public class SmwCharacter : ScriptableObject {


	// Awake() wird bei ScriptableObject.Create asugeführt!!!!

//	public SmwCharacter ()
//	{
//		Debug.Log(this.ToString() + " konstruktor ()");			// wird auch at Runtime ausgeführt
//	}
//	public void Awake ()
//	{
//		Debug.Log(this.ToString() + " Awake ()");				// Awake() wird bei ScriptableObject.Create asugeführt!!!!
//	}
//	public void Start ()
//	{
//		Debug.Log(this.ToString() + " Start ()");
//	}
	public void OnEnable()
	{
		Debug.Log("<color=green>" + this.ToString() + " OnEnable () </color>", this);		// OnEnable() wird bei ScriptableObject.Create asugeführt!!!!
		Check();
	}

	void Check()
	{

	}

	public string charName;
	public int charId;
	public GameObject charPrefab;
	public GameObject unityNetworkPrefab;
	public bool charInUse;
	public NetworkPlayer netPlayer;
	public Player player;
	public Sprite[] charSpritesheet;
	public RuntimeAnimatorController runtimeAnimatorController;

	public void SetID(int i)
	{
		charId = i;
		Save ();
	}
	
	public void SetUnityNetworkPrefab(GameObject prefab)
	{
		this.unityNetworkPrefab = prefab;
	}

	public void SetPrefab(GameObject prefab)
	{
		this.charPrefab = prefab;
	}
	
	public void Save()
	{
#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty (this);					// vielleicht
#endif
	}
	public Sprite[] charIdleSprites;
	public Sprite[] charRunSprites;
	public Sprite[] charJumpSprites;
	public Sprite[] charSkidSprites;
	public Sprite[] charDieSprites;
//	public Sprite[] charGameOverSprites;
	public Sprite[] charHeadJumpedSprites;

	public void SetCharSpritesheet(Sprite[] sprites)
	{
		charSpritesheet = sprites;

		if(sprites.Length < 6)
		{
			Debug.LogError("Sprite needs do be prepared (sliced to 6 sprites), no automating slicing");
			return;
		}

		//Idle
		charIdleSprites = new Sprite[1];
		charIdleSprites[0] = charSpritesheet[0];
		//charIdleSprites[0] = Sprite.Create(charSpriteSheet[0].texture, charSpriteSheet[0].rect, charSpriteSheet[0].pivot);

		//Run
		charRunSprites = new Sprite[2];
		charRunSprites[0] = charSpritesheet[0];
		charRunSprites[1] = charSpritesheet[1];

		//Jump
		charJumpSprites = new Sprite[1];
		charJumpSprites[0] = charSpritesheet[2];

		//Skid - ChangeRunDirection
		charSkidSprites = new Sprite[1];
		charSkidSprites[0] = charSpritesheet[3];

		//Die
		charDieSprites = new Sprite[1];
		charDieSprites[0] = charSpritesheet[4];

		//HeadJumped
		charHeadJumpedSprites = new Sprite[1];
		charHeadJumpedSprites[0] = charSpritesheet[5];

		//TODO important
		Save ();				// speichere Asset (Änderung wird übernommen)
		//TODO important
	}

//	void SetupAnimationStateSprites(Sprite[] stateSprites, uint spriteCount)
//	{
//		stateSprites = new Sprite[spriteCount];
//	}


}

using UnityEngine;
using System.Collections;

[System.Serializable]
public class SmwCharacter : ScriptableObject {

	public string charName;
	public int charId;
	public Sprite[] charSprites;

	public Sprite[] charIdleSprites;
	public Sprite[] charRunSprites;
	public Sprite[] charJumpSprites;
	public Sprite[] charSkidSprites;
	public Sprite[] charDieSprites;
//	public Sprite[] charGameOverSprites;
	public Sprite[] charHeadJumpedSprites;

	public void SetCharSprites(Sprite[] sprites)
	{
		charSprites = sprites;

		if(sprites.Length < 6)
		{
			Debug.LogError("Sprite needs do be prepared (sliced to 6 sprites), no automating slicing");
			return;
		}

		//Idle
		charIdleSprites = new Sprite[1];
		charIdleSprites[0] = charSprites[0];

		//Run
		charRunSprites = new Sprite[2];
		charRunSprites[0] = charSprites[0];
		charRunSprites[1] = charSprites[1];

		//Jump
		charJumpSprites = new Sprite[1];
		charJumpSprites[0] = charSprites[2];

		//Skid - ChangeRunDirection
		charSkidSprites = new Sprite[1];
		charSkidSprites[0] = charSprites[3];

		//Die
		charDieSprites = new Sprite[1];
		charDieSprites[0] = charSprites[4];

		//HeadJumped
		charHeadJumpedSprites = new Sprite[1];
		charHeadJumpedSprites[0] = charSprites[5];
	}

//	void SetupAnimationStateSprites(Sprite[] stateSprites, uint spriteCount)
//	{
//		stateSprites = new Sprite[spriteCount];
//	}


}

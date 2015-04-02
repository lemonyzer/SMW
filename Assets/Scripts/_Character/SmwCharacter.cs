using UnityEngine;
using System.Collections;

[System.Serializable]
public class SmwCharacter : ScriptableObject {

	public string charName;
	public int charId;
	public Sprite[] charSpriteSheet;

	public Sprite[] charIdleSprites;
	public Sprite[] charRunSprites;
	public Sprite[] charJumpSprites;
	public Sprite[] charSkidSprites;
	public Sprite[] charDieSprites;
//	public Sprite[] charGameOverSprites;
	public Sprite[] charHeadJumpedSprites;

	public void SetCharSprites(Sprite[] sprites)
	{
		charSpriteSheet = sprites;

		if(sprites.Length < 6)
		{
			Debug.LogError("Sprite needs do be prepared (sliced to 6 sprites), no automating slicing");
			return;
		}

		//Idle
		charIdleSprites = new Sprite[1];
		charIdleSprites[0] = charSpriteSheet[0];
		//charIdleSprites[0] = Sprite.Create(charSpriteSheet[0].texture, charSpriteSheet[0].rect, charSpriteSheet[0].pivot);

		//Run
		charRunSprites = new Sprite[2];
		charRunSprites[0] = charSpriteSheet[0];
		charRunSprites[1] = charSpriteSheet[1];

		//Jump
		charJumpSprites = new Sprite[1];
		charJumpSprites[0] = charSpriteSheet[2];

		//Skid - ChangeRunDirection
		charSkidSprites = new Sprite[1];
		charSkidSprites[0] = charSpriteSheet[3];

		//Die
		charDieSprites = new Sprite[1];
		charDieSprites[0] = charSpriteSheet[4];

		//HeadJumped
		charHeadJumpedSprites = new Sprite[1];
		charHeadJumpedSprites[0] = charSpriteSheet[5];
	}

//	void SetupAnimationStateSprites(Sprite[] stateSprites, uint spriteCount)
//	{
//		stateSprites = new Sprite[spriteCount];
//	}


}

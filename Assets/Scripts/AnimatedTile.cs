using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AnimatedTile : MonoBehaviour {
	public bool reverseAnim = false;
	public int animationStep = 1;
	SpriteRenderer spriteRenderer;
	float lastChangeTimeStamp = 0f;
	float nextChangeTimeStamp = 0f;
	float timeStep = 0.25f;
	bool pauseBetweemAnimation = false;
	[SerializeField]
	Sprite[] animationSprites;			// TODO needs to be public to initialize in Editor Mode!!! or [SerializeField] TODO ????  BAD WAY ???? TODO
	int currentState = 0;

	/// <summary>
	/// Init the specified sprites and animationStartState.
	/// 
	/// init animationArray
	/// 
	/// init spriteRenderer
	/// add SpriteRendererComponent if not added
	/// 
	/// add first animationSprite to SpriteRenderer
	/// 
	/// </summary>
	/// <param name="sprites">Sprites.</param>
	/// <param name="animationStartState">Animation start state.</param>
	public void Init(Sprite[] sprites, int animationStartState)
	{
		if (spriteRenderer == null)
		{
			spriteRenderer = this.GetComponent<SpriteRenderer>();
			if (spriteRenderer == null)
			{
				spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
			}
		}

		if(sprites == null)
		{
			Debug.LogError("sprites == null");
			return;
		}
		animationSprites = sprites;

		if (animationStartState > 0 && animationStartState < animationSprites.Length)
		{
			// setze currentState auf anfangswert
			currentState = animationStartState;
			// setzte erste Animationstile 
			spriteRenderer.sprite = animationSprites[animationStartState];
		}
		else
		{
			// setze currentState auf anfangswert
			currentState = 0;
			// setzte erste Animationstile 
			spriteRenderer.sprite = animationSprites[0];
		}
	}

	public void SetAnimation(Sprite[] sprites)
	{
		if(sprites == null)
		{
			Debug.LogError("sprites == null");
			return;
		}
		animationSprites = sprites;
		spriteRenderer = this.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = animationSprites[0];
	}

	bool Check()
	{
		if (animationSprites == null)
		{
			this.enabled = false;
			return false;
		}
		if (animationSprites.Length <= 1)			// nur ein Sprite -> keine Animation
		{
			return false;
		}
		return true;
	}

	public void Next(int step)
	{
		if (!Check())
			return;

		currentState += step;

		if (currentState >= animationSprites.Length)
		{
			currentState = 0;
		}

		if (currentState < 0)
		{
			currentState = animationSprites.Length-1;	// check kontrolliert das arrray Länge >1 ! wenn 1... nur ein bild... muss nicht wechseln, wenn länge 0  dann würde currenState = -1  werden und arrays können nicht mit negativem index angesprochen werden
		}

		if (animationSprites[currentState] != null)
		{
			spriteRenderer.sprite = animationSprites[currentState];
		}

	}

	public void Awake()
	{
		spriteRenderer = this.GetComponent<SpriteRenderer>();
	}

	public void Start()
	{
		if(!Check())
			return;
	}

	public void Update()
	{
		if( Time.time > lastChangeTimeStamp+timeStep)
		{
			lastChangeTimeStamp = Time.time;
			if(reverseAnim)
				Next (-animationStep);
			else
				Next (animationStep);
		}
	}

	public void LateUpdate()
	{

	}
}

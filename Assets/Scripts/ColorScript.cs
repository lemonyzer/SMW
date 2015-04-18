using UnityEngine;
using System.Collections;

public class ColorScript : MonoBehaviour {

	SpriteRenderer spriteRenderer;

	int colorId = 0;
	int mColorCount = 4;
	int mColorIntensityCount = 3;

	// Use this for initialization
	void Start () {
		spriteRenderer = this.GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if (GUILayout.Button ("Change Color"))
		{
			EditSpriteRendererSprite(spriteRenderer, colorId);
			colorId++;
			colorId = colorId % mColorCount;
//			Debug.Log(colorId);
		}
	}

	void EditSpriteRendererSprite (SpriteRenderer fSpriteRenderer, int fColorId)
	{
		ModifyTexture2D(fSpriteRenderer.sprite.texture, fColorId);
	}
	
	public void ModifyTexture2D(Texture2D texture, int fColorId)
	{
		int fPixelChanged = 0;
		texture.filterMode = FilterMode.Bilinear;
		texture.wrapMode = TextureWrapMode.Clamp;
		
		for (int y = 0; y < texture.height; y++)
		{
			#if UNITY_EDITOR
			//Debug.Log("y: " + y);
			#endif
			for (int x = 0; x < texture.width; x++)
			{
				Color32 currentColor = texture.GetPixel (x,y);

//#if UNITY_EDITOR
//				if( y == 0 && x == 0)
//				{
//					Debug.Log("GetPixel("+y+", "+x+") = " + currentColor);
//				}
//				else if ( y == 18 && x == 18)
//				{
//					Debug.Log("GetPixel("+y+", "+x+") = " + currentColor);
//				}
//#endif

				Color32 newColor = new Color32();
				bool pixelHasReferenceColor = false;
				// schleife:
				// schaue ob aktueller Pixel einer der folgenden referenz Farben besitzt:
				for (int iColor = 0; iColor < mColorCount; iColor++)
				{
					Color32 refColor;
					for (int iColorIntensity = 0; iColorIntensity < mColorIntensityCount; iColorIntensity++)
					{
						refColor = TeamColor.referenceColors[iColor,iColorIntensity];

//						#if UNITY_EDITOR
//						if( y == 0 && x == 0)
//						{
//							Debug.Log("refColor = " + refColor);
//							Debug.Log("newColor = " + Team.referenceColors[fColorId,iColorIntensity]);
//						}
//						else if ( y == 18 && x == 18)
//						{
//							Debug.Log("refColor = " + refColor);
//							Debug.Log("newColor = " + Team.referenceColors[fColorId,iColorIntensity]);
//						}
//						#endif

						if(currentColor.Equals(refColor))
						{
							newColor = TeamColor.referenceColors[fColorId,iColorIntensity];
							pixelHasReferenceColor = true;
							break;
						}
					}
					if(pixelHasReferenceColor)
						break;
				}
				
				if(pixelHasReferenceColor)
				{
					texture.SetPixel (x, y, newColor);
					fPixelChanged++;
				}
				
			}
		}
		Debug.Log("Anzahl an geänderten Pixel = " + fPixelChanged);
		texture.Apply();
	}

	void CreateNewTeamSprite(int fTeamColorId)
	{
		Texture2D modifiedCopy = CopyAndModifyTexture2D(spriteRenderer.sprite.texture, fTeamColorId);
		
		if(modifiedCopy == null)
		{
			Debug.LogError("Problem bei der Modifizierung");
		}
		
		Sprite newSprite = Sprite.Create (modifiedCopy, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f), 32f);
		
		this.spriteRenderer.sprite = newSprite;
	}
	
	public Texture2D createTextureCopy (Texture2D texture)
	{
		return null;
	}


	public Texture2D CopyAndModifyTexture2D(Texture2D texture, int fColorId)
	{
		Texture2D copyTexture = new Texture2D(texture.width, texture.height);
		
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		
		for (int y = 0; y < copyTexture.height; y++)
		{
			for (int x = 0; x < copyTexture.width; x++)
			{
				Color32 currentColor = texture.GetPixel (x,y);
				Color32 newColor = new Color32();
				bool pixelHasReferenceColor = false;
				// schleife:
				// schaue ob aktueller Pixel einer der folgenden referenz Farben besitzt:
				for (int iColor = 0; iColor < mColorCount; iColor++)
				{
					Color32 refColor;
					for (int iColorIntensity = 0; iColorIntensity < mColorIntensityCount; iColorIntensity++)
					{
						refColor = TeamColor.referenceColors[iColor,iColorIntensity];
						
						if(currentColor.Equals(refColor))
						{
							newColor = TeamColor.referenceColors[fColorId,iColorIntensity];
							pixelHasReferenceColor = true;
							break;
						}
					}
				}
				
				if(!pixelHasReferenceColor)
					newColor = currentColor;
				
				copyTexture.SetPixel (x, y, newColor);

			}
		}
		copyTexture.Apply();
		return copyTexture;
	}


//
//	public void UpdateCharacterTexture()
//	{
//		Sprite[] loadSprite = Resources.LoadAll<Sprite> (spritePath);
//		Texture2D characterTexture2D = CopyTexture2D(loadSprite[0].texture);
//		
//		int i = 0;
//		while(i != characterSprites.Length)
//		{
//			//SpriteRenderer sr = GetComponent<SpriteRenderer>();
//			//string tempName = sr.sprite.name;
//			//sr.sprite = Sprite.Create (characterTexture2D, sr.sprite.rect, new Vector2(0,1));
//			//sr.sprite.name = tempName;
//			
//			//sr.material.mainTexture = characterTexture2D;
//			//sr.material.shader = Shader.Find ("Sprites/Transparent Unlit");
//			string tempName = characterSprites[i].name;
//			characterSprites[i] = Sprite.Create (characterTexture2D, characterSprites[i].rect, new Vector2(0,1));
//			characterSprites[i].name = tempName;
//			names[i] = tempName;
//			++i;
//		}
//		
//		SpriteRenderer sr = GetComponent<SpriteRenderer>();
//		sr.material.mainTexture = characterTexture2D;
//		sr.material.shader = Shader.Find ("Sprites/Transparent Unlit");
//		
//	}

}

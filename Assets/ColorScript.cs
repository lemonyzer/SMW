using UnityEngine;
using System.Collections;

public class Team
{
	public static Color32[] referenceColors = new Color32[] {
		new Color32 ((byte)128, (byte)0, (byte)0, (byte)255),
		new Color32 ((byte)192, (byte)0, (byte)0, (byte)255),
		new Color32 ((byte)255, (byte)0, (byte)0, (byte)255) };
	
	public static Color32[] teamGreenColors = new Color32[] {
		new Color32 ((byte)90, (byte)146, (byte)0, (byte)255),
		new Color32 ((byte)123, (byte)178, (byte)0, (byte)255),
		new Color32 ((byte)165, (byte)219, (byte)0, (byte)255) };
}

public class ColorScript : MonoBehaviour {

	SpriteRenderer spriteRenderer;

	bool reverse = false;

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
			EditSpriteRendererSprite(spriteRenderer, reverse);
			reverse = !reverse;
		}
	}

	void EditSpriteRendererSprite (SpriteRenderer fSpriteRenderer, bool freverse)
	{
		ModifyTexture2D(fSpriteRenderer.sprite.texture, freverse);
	}
	
	public void ModifyTexture2D(Texture2D texture, bool freverse)
	{
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		
		for (int y = 0; y < texture.height; y++)
		{
			for (int x = 0; x < texture.width; x++)
			{

				Color32 currentColor = texture.GetPixel (x,y);

				Color32 newColor = new Color32();
				bool pixelHasReferenceColor = false;
				// schleife:
				// schaue ob aktueller Pixel einer der folgenden referenz Farben besitzt:
				for (int i = 0; i < Team.referenceColors.Length; i++)
				{
					if (Team.referenceColors.Length != Team.teamGreenColors.Length)
						return;

					Color32 refColor;

					if(!freverse)
					{
						refColor = Team.referenceColors[i];
						newColor = Team.teamGreenColors[i];
					}
					else
					{
						refColor = Team.teamGreenColors[i];
						newColor = Team.referenceColors[i];
					}
						
					if(currentColor.Equals(refColor))
					{
						pixelHasReferenceColor = true;
						break;
					}
				}
				
				if(pixelHasReferenceColor)
				{
					texture.SetPixel (x, y, newColor);
				}
				
			}
		}
		texture.Apply();
	}

	void CreateNewTeamSprite()
	{
		Texture2D modifiedCopy = CopyAndModifyTexture2D(spriteRenderer.sprite.texture);
		
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


	public Texture2D CopyAndModifyTexture2D(Texture2D texture)
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
				bool pixelColorSet = false;
				// schleife:
				// schaue ob aktueller Pixel einer der folgenden referenz Farben besitzt:
				for (int i = 0; i < Team.referenceColors.Length; i++)
				{
					if (Team.referenceColors.Length != Team.teamGreenColors.Length)
						return null;
					
					Color32 refColor = Team.referenceColors[i];
					//					if(currentColor == refColor)
					if(currentColor.Equals(refColor))
					{
						newColor = Team.teamGreenColors[i];
						pixelColorSet = true;
						break;
					}
				}
				
				if(!pixelColorSet)
					newColor = currentColor;
				
				//				Color32 currentColor = texture.GetPixel (x,y);
				//				Color32 newColor = new Color32(currentColor.g, currentColor.r, currentColor.b, currentColor.a);
				
				copyTexture.SetPixel (x, y, newColor);
				//				texture.SetPixel (x, y, newColor);
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

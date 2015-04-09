using UnityEngine;
using System.Collections;

using UnityEditor;


public class CreateAlpha8Helper : EditorWindow {

	[MenuItem ("Window/Alpha8 Editor %#a")]
	static void Init () {
		GetWindow (typeof (CreateAlpha8Helper));
	}

	Sprite originalSprite;
	int colorPaletteSize = 30;
	int colorCount = 0;
	bool overrideSourceAlpha = true;
	int overrideAlphaValue = 255;		// 255 = full visible, 0 = transparent

	TextureFormat mColorMapFormat = TextureFormat.RGB24;			// wird in rotem Kanel gespeichert
	TextureFormat mColorPaletteFormat = TextureFormat.ARGB32;

	/// <summary>
	/// Raises the enable event. We use it to set some references and do some initialization. I don`t figured out how to make a variable persistent in Unity Editor yet so most of the codes here can useless
	/// </summary>
	void OnEnable()
	{
	}

	// Use this for initialization
	void OnGUI () {
		originalSprite = EditorGUILayout.ObjectField("Original Sprite", originalSprite, typeof(Sprite), false) as Sprite;

		if(GUILayout.Button("make Alpha8"))
		{
			CreateAlpha8Asset(originalSprite);
		}
	}
	
	void OnFocus()
	{
	}

	bool isTexturFormatAccessable(Texture2D texture)
	{
		if (texture.format == TextureFormat.Alpha8 ||
		    texture.format == TextureFormat.ARGB32 ||
		    texture.format == TextureFormat.RGBA32 ||
		    texture.format == TextureFormat.RGB24 ||
		    texture.format == TextureFormat.RFloat ||
		    texture.format == TextureFormat.RGBAFloat ||
		    texture.format == TextureFormat.RGFloat)
		{
			Debug.Log (texture.format.ToString());
			return true;
		}
		else
		{
			Debug.LogError("wrong TextureFormat (" + texture.format + ")");
			return false;
		}
	}

	void CreateAlpha8Asset(Sprite sprite)
	{

		if(sprite.texture.alphaIsTransparency)
			Debug.LogError("sprite.texture.alphaIsTransparency!");

		// erstelle Color Palette von original Sprite
		Color32[] spriteColorPalette = CreateColorPalette(sprite.texture);

		// erstelle colorMap von original Sprite und ColoPalette
		Texture2D colorMap = CreateColorMap(sprite.texture, spriteColorPalette);
		if(colorMap == null)
		{
			Debug.LogError ("colorMap == null");
			return;
		}

		// original Sprite Dateipfad
		string spritePath = AssetDatabase.GetAssetPath (sprite);
		if(string.IsNullOrEmpty(spritePath))
		{
			Debug.LogError("spritePath == empty or NULL!");
			return;
		}

		// erstelle Texture von Color Palette zum Speichern als Bild
		Texture2D colorPaletteTexture = new Texture2D( colorPaletteSize, 1, mColorPaletteFormat, false);	//TODO ARGB32

		if (isTexturFormatAccessable(colorPaletteTexture))
		{
			colorPaletteTexture.SetPixels32 (spriteColorPalette);
			colorPaletteTexture.Apply();
		}



		// check ob alle benötigten Daten erstellt wurden
		if( spriteColorPalette == null || colorMap == null || colorPaletteTexture == null)
		{
			Debug.Log("fehler");
			return;
		}

		// save with AssetDatabase
//		string spriteColorPalettePath = spritePath + "_ColorPalette.asset";
//		
//		string spriteMapPath = spritePath + "_alpha8.asset";
//		Rect spriteMapRect = new Rect(0, 0, colorMap.width, colorMap.height);
//		Sprite spriteMap = Sprite.Create(colorMap, spriteMapRect, new Vector2(0.5f,0.5f));
//		AssetDatabase.CreateAsset(spriteMap, spriteMapPath);
//		AssetDatabase.SaveAssets();

//		Debug.LogWarning(spritePath);

		string relPathColorPalete = spritePath + "_colorPalette_" + colorPaletteTexture.format.ToString() + ".png";
//		Debug.LogWarning(relPathColorPalete);
		GenerateAssetFromTexture(colorPaletteTexture, relPathColorPalete);

		string relPathColorMap = spritePath + "_colorMap_" + colorMap.format.ToString() + ".png";
//		Debug.LogWarning(relPathColorMap);
		GenerateAssetFromTexture(colorMap, relPathColorMap);
		

		// Application.dataPath ohne /Assets
//		string absPathColorMap = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + spritePath + "_colorPalette_" + colorPaletteTexture.format.ToString() + ".png";
//		Debug.LogWarning(absPathColorMap);
//		System.IO.File.WriteAllBytes( absPathColorMap, colorPaletteTexture.EncodeToPNG());
//
//
		// Application.dataPath ohne /Assets
//		string absPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + spritePath + "_colorMap_" + colorMap.format.ToString() + ".png";
//		Debug.LogWarning(absPath);
//		System.IO.File.WriteAllBytes( absPath, colorMap.EncodeToPNG());

	}


	void GenerateAssetFromTexture(Texture2D texture, string relPath)
	{
		string AssetPath = AssetDatabase.GenerateUniqueAssetPath(relPath);

		// Write texture to file
		System.IO.File.WriteAllBytes (AssetPath, texture.EncodeToPNG());

		// Delete generated texture
		UnityEngine.Object.DestroyImmediate(texture);

		// Import Asset
		AssetDatabase.ImportAsset(AssetPath);

		AssetDatabase.SaveAssets();
		// Get Imported Texture
		//Texture2D tex = AssetDatabase.LoadAllAssetsAtPath(AssetPath, typeof(Texture2D)) as Texture2D;
	}

	//	Unsupported texture format - needs to be ARGB32, RGBA32, RGB24, Alpha8 or one of float formats
	//	UnityEngine.Texture2D:SetPixel(Int32, Int32, Color)

	public Texture2D CreateColorMap(Texture2D texture, Color32[] colorPalette)
	{
		Texture2D mapTexture = new Texture2D(texture.width, texture.height, mColorMapFormat, false);		//TODO Alpha8 8bit
		if (!isTexturFormatAccessable(mapTexture))
		{
			Debug.Log("mapTexture wrong TexturFormat!");
			return null;
		}


		if(colorPalette == null)
		{
			Debug.LogError("colorPalette == null");
			return null;
		}

		float spreizFaktor = 255.0f/ Mathf.Min(colorCount, colorPalette.Length);
//		Debug.Log(spreizFaktor);
//		Debug.Log(Mathf.FloorToInt(spreizFaktor));

		for (int y = 0; y < mapTexture.height; y++)
		{
			for (int x = 0; x < mapTexture.width; x++)
			{
				Color32 currentColor = texture.GetPixel (x,y);				// GetPixel(0,0) <--- unten links ist Bildanfang

//				if (y==0)
//				{
//					Debug.Log("GetPixel ("+x+","+y+" = " + currentColor);
//				}
//				if (y==0 && ( x>=10 && x <=15 ))
//				{
//					Debug.Log("GetPixel ("+x+","+y+" = " + currentColor);
//				}

				if(overrideSourceAlpha)
					currentColor.a = (byte)overrideAlphaValue;
				Color32 newColor;

				int colorId = GetColorIdFromPalette(colorPalette, currentColor);

				//Spektrumspreizung
				colorId = colorId * Mathf.FloorToInt(spreizFaktor);

				// wenn Farbe nicht in der Palette gefunden wurde!
				if( colorId == -1 )
					colorId = 0;

				// speichere Farbwert aus Farbpalette im Red Channel
				newColor = new Color32((byte) colorId,0,0,0);

				// setze Pixel
				mapTexture.SetPixel (x, y, newColor);

			}
//			if(y == 1)
//			{
//				return null;
//			}
		}

		mapTexture.Apply();
		return mapTexture;
	}


	public int GetColorIdFromPalette(Color32[] colorPalette, Color32 color)
	{
		if(colorPalette == null)
		{
			Debug.LogError("colorPalette == null!");
			return -1;
		}

		for(int colorId=0; colorId<colorPalette.Length; colorId++)
		{
			//check ob inhalt existiert
//			if(colorPalette[colorId] != null)
//			{
				//check ob color schon in farbpalette!
				//check ob aktuelles arrayElement der color entspricht
				if(colorPalette[colorId].Equals(color))
				{
					//color schon enthalten, return colorId
					return colorId;
				}
//			}
//			else
//			{
//				Debug.LogError("colorPalette["+colorId+"] == null!");
//				return -1;
//			}
		}
		// code wird nur erreich wenn color noch nicht in colorPalette
		Debug.LogWarning("Schleife zu Ende, keine Farbe in Palette gefunden!");
		return -1;
	}


	public bool IsColorInPalette(Color32[] colorPalette, Color32 color)
	{
		for(int colorId=0; colorId<colorPalette.Length; colorId++)
		{
			//check ob inhalt existiert
//			if(colorPalette[colorId] != null)
//			{
				//check ob color schon in farbpalette!
				//check ob aktuelles arrayElement der color entspricht
				if(colorPalette[colorId].Equals(color))
				{
					//color schon enthalten, return colorId
					return true;
				}
//			}
//			else
//			{
//				Debug.LogError("colorPalette["+colorId+"] == null!");
//				return false;
//			}
		}
		// code wird nur erreich wenn color noch nicht in colorPalette
//		Debug.LogWarning("Schleife zu Ende, keine entsprechende Farbe in Palette gefunden!");
		return false;
	}

	/// <summary>
	/// Adds the color to palette. DOES NOT CHECK IF ALREADY IN!!!
	/// </summary>
	/// <returns>The color to palette.</returns>
	/// <param name="colorPalette">Color palette.</param>
	/// <param name="color">Color.</param>
	public bool AddColorToPalette(Color32[] colorPalette, Color32 color, int index)
	{
//		Debug.Log("index = " + index + ", maxLänge = " + colorPalette.Length);
//		return 0;
		if(index < 0 || index >= colorPalette.Length)
		{
			Debug.LogError("index ausßerhalb colorPalette size!!");
			return false;
		}
		else
		{
			colorPalette[index] = color;
			return true;
		}
		
	}


//	public int AddColorToPaletteReturnIdIfAlreadyIn(Color[] colorPalette, Color color)
//	{
//		Debug.Log(colorPalette.Length);
//		return 0;
//		for(int colorId=0; colorId<colorPalette.Length; colorId++)
//		{
//			//check ob inhalt existiert
//			if(colorPalette[colorId] != null)
//			{
//				//check ob color schon in farbpalette!
//				//check ob aktuelles arrayElement der color entspricht
//				if(colorPalette[colorId].Equals(color))
//				{
//					//color schon enthalten, return colorId
//					return colorId;
//				}
//			}
//		}
//		// code wird nur erreich wenn color noch nicht in colorPalette
//
//	}


	public Color32[] CreateColorPalette(Texture2D texture)
	{
//		Texture2D copyTexture = new Texture2D(texture.width, texture.height, TextureFormat.Alpha8, false);
		Color32[] copyColorArray = texture.GetPixels32();
		
//		texture.filterMode = FilterMode.Point;
//		texture.wrapMode = TextureWrapMode.Clamp;

		Color32[] colorPalette = new Color32[colorPaletteSize];		// 8 bit -> 256 Werte 	// TODO shader kann nur 30 bits
		colorCount = 0;
		
		for (int y = 0; y < texture.height; y++)
		{
			for (int x = 0; x < texture.width; x++)
			{
//				Color currentColor = copyColorArray[x*y];		//TODO
				Color32 currentColor = texture.GetPixel(x,y);
				if(overrideSourceAlpha)
					currentColor.a = (byte)overrideAlphaValue;			// Source Texture has ALPHA  FIX!!!!!

				if(!IsColorInPalette(colorPalette, currentColor))
				{
					AddColorToPalette(colorPalette, currentColor, colorCount);
					colorCount++;
				}
			}
		}

		Debug.Log("Anzahl gefundener Farben: " + colorCount + ". Palette max. länge = " + colorPaletteSize);

		return colorPalette;

	}


	/**
	 * This function works only on ARGB32, RGB24 and Alpha8 texture formats.
	 * For other formats SetPixel is ignored.
	 * The texture also has to have Read/Write Enabled flag set in the import settings.
	 * See Also: SetPixels, GetPixel, Apply.
	 **/

//	public Texture2D CopyAndModifyTexture2D(Texture2D texture)
//	{
//		Texture2D copyTexture = new Texture2D(texture.width, texture.height, TextureFormat.Alpha8, false);
//		Color[] color = texture.GetPixels(0, 0, texture.width, texture.height);
//
//		texture.filterMode = FilterMode.Point;
//		texture.wrapMode = TextureWrapMode.Clamp;
//		
//		for (int y = 0; y < copyTexture.height; y++)
//		{
//			for (int x = 0; x < copyTexture.width; x++)
//			{
//				Color32 currentColor = texture.GetPixel (x,y);
//				Color32 newColor = new Color32();
//				bool pixelHasReferenceColor = false;
//				// schleife:
//				// schaue ob aktueller Pixel einer der folgenden referenz Farben besitzt:
//				for (int iColor = 0; iColor < mColorCount; iColor++)
//				{
//					Color32 refColor;
//					for (int iColorIntensity = 0; iColorIntensity < mColorIntensityCount; iColorIntensity++)
//					{
//						refColor = Team.referenceColors[iColor,iColorIntensity];
//						
//						if(currentColor.Equals(refColor))
//						{
//							newColor = Team.referenceColors[fColorId,iColorIntensity];
//							pixelHasReferenceColor = true;
//							break;
//						}
//					}
//				}
//				
//				if(!pixelHasReferenceColor)
//					newColor = currentColor;
//				
//				copyTexture.SetPixel (x, y, newColor);
//				
//			}
//		}
//		copyTexture.Apply();
//		return copyTexture;
//	}
}

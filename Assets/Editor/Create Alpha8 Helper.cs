using UnityEngine;
using System.Collections;

using UnityEditor;


public class CreateAlpha8Helper : EditorWindow {

	[MenuItem ("Window/Alpha8 Editor %#a")]
	static void Init () {
		GetWindow (typeof (CreateAlpha8Helper));
	}

	bool useSpreizFaktor = false;
	Texture2D originalTexture = EditorGUIUtility.whiteTexture;
	int colorPaletteSize = 30;
	int minColorPaletteSize = 1;
	int maxColorPaletteSize = 255;
	int colorCount = 0;
	bool ignoreSourceAlphaChannel = false;
	bool overrideSourceAlpha = false;	// macht keinen sinn
	int overrideAlphaValue = 255;		// 255 = full visible, 0 = transparent

	// Color Palette
//	TextureImporterFormat mColorPaletteImportFormat = TextureImporterFormat.RGBA32;
	TextureImporterFormat mColorPaletteImportFormat = TextureImporterFormat.RGBA32;
	TextureFormat mColorPaletteFormat = TextureFormat.RGBA32;
	TextureImporterSettings mColorPaletteImportSettings;
	bool bColorPaletteUseAdvancedImportSettings = true;
	


	// Color Map
	TextureImporterFormat mColorMapImportFormat = TextureImporterFormat.Alpha8;
	TextureFormat mColorMapFormat = TextureFormat.Alpha8;			
	// Color Map Channels
	// Color.this[int]
	// Access the r, g, b, a components using [0], [1], [2], [3] respectively.
	//	int mColorMapChannel = 0;	// wird im Alpha Kanal gespeichert
	bool writeChAlpha = true, writeChRed = true, writeChGreen = true, writeChBlue = true;
	// Color Map Import Settings
	TextureImporterSettings mColorMapImportSettings;
	bool bColorMapUseAdvancedImportSettings = true;
//	bool bCreateMipmaps = true;
////	TextureImporterFormat textureFormat = TextureImporterFormat.DXT1;
//	int  maxTextureSize = 2048;
//	TextureImporterNPOTScale  npotScale = TextureImporterNPOTScale.ToNearest;
////	bool bCompressTexturesOnImport = true;
//	bool bAlphaIsTransparency = false;

	int[] allowedMaxTextureSize;
	string[] allowedMaxTextureSizesString;

	void OnEnable_InitMaxTextureSizes()
	{
		allowedMaxTextureSize = new int[] { 64, 128, 256, 512, 1024, 2048, 4096 };
		allowedMaxTextureSizesString = new string[allowedMaxTextureSize.Length];
		for (int i = 0; i < allowedMaxTextureSize.Length; ++i)
			allowedMaxTextureSizesString[i] = allowedMaxTextureSize[i].ToString();
	}

	string CmColorPaletteImportSettingsTextureFormat = "CmColorPaletteImportSettingsTextureFormat";
	string CmColorMapImportSettingsTextureFormat = "CmColorMapImportSettingsTextureFormat";

	void SetLastUsedFormat (string key, int value)
	{
		EditorPrefs.SetInt(key, value);
	}

	int GetLastUsedFormat (string key)
	{
		if (EditorPrefs.HasKey(key))
		{
			return EditorPrefs.GetInt(key);
		}
		return -1;
	}

	/// <summary>
	/// Raises the enable event. We use it to set some references and do some initialization.
	/// I don`t figured out how to make a variable persistent in Unity Editor yet so most of the codes here can useless
	/// </summary>
	void OnEnable()
	{
		OnEnable_InitMaxTextureSizes();

		// mColorPaletteImportSettings
		mColorPaletteImportSettings = new TextureImporterSettings();
		int lastValue = -10101;
		lastValue = GetLastUsedFormat(CmColorPaletteImportSettingsTextureFormat);
		Debug.Log("lastValue = " + lastValue);
		if( lastValue != -10101)
			mColorPaletteImportSettings.textureFormat = (TextureImporterFormat) lastValue;
		else
			mColorPaletteImportSettings.textureFormat = mColorPaletteImportFormat;
		mColorPaletteImportSettings.maxTextureSize = 256;

		// mColorMapImportSettings
		mColorMapImportSettings = new TextureImporterSettings();
		lastValue = -10101;
		lastValue = GetLastUsedFormat(CmColorMapImportSettingsTextureFormat);
		Debug.Log("lastValue = " + lastValue);
		if( lastValue != -10101)
			mColorMapImportSettings.textureFormat = (TextureImporterFormat) lastValue;			// kann auch negativ werden
		else
			mColorMapImportSettings.textureFormat = mColorMapImportFormat;
		mColorMapImportSettings.maxTextureSize = 256;


	}

	bool showInverted = false;
	Texture2D invertedTexture = null;

	void InvertColors() { 
		for (int m = 0; m < invertedTexture.mipmapCount; m++)
		{
			Color[] c = invertedTexture.GetPixels(m);							// keine Punktoperation!
			
			for (int i=0 ;i < c.Length; i++) {					
				c[i].r = 1 - c[i].r;										// keine Punktoperation!
				c[i].g = 1 - c[i].g;										// keine Punktoperation!
				c[i].b = 1 - c[i].b;										// keine Punktoperation!
			}
			invertedTexture.SetPixels(c, m); 
		}
		invertedTexture.Apply();
	}

	void GreyScale(bool useRed, bool useGreen, bool useBlue, bool useAlpha, bool redConst, int redValue, bool alphaConst, int alphaValue) { 
		for (int y = 0; y < testTexture.height; y++)
		{
			for (int x = 0 ; x < testTexture.width; x++)
			{																			// fange unten links im Bild an
				int spreizFaktor = Mathf.FloorToInt(256/testTexture.height);
				byte greyValue = (byte) (y*spreizFaktor);

				Color32 newColor = new Color32(0,0,0,0);

				if(useRed)
					newColor.r = greyValue;
				// const redValue
				if(redConst)
					newColor.r = (byte) redValue;

				if(useGreen)
					newColor.g = greyValue;

				if(useBlue)
					newColor.b = greyValue;

				if(useAlpha)
					newColor.a = greyValue;
				// const alphaValue
				if(alphaConst)
					newColor.a = (byte) alphaValue;

				testTexture.SetPixel(x,y, newColor);
			}
		}
		testTexture.Apply();
	}

	int testTextureWidth = 256;
	int testTextureHeight = 256;


	SpriteRenderer spriteRenderer;
	bool showTestTexture = false;
	Texture2D testTexture;

	TextureFormat mTextureFormat = TextureFormat.Alpha8;
	bool mUseRed = false;
	bool mRedConst = false;
	int mRedConstValue = 255;
	bool mUseGreen = false;
	bool mUseBlue = false;
	bool mUseAlpha = false;
	bool mAlphaConst = false;
	int mAlphaConstValue = 255;
	bool mAlphaIsTransparency = false;

	bool mOverrideMipMap = false;
	bool mOverrideMipMapValue = false;
	

	bool isTexturFormatAccessable(TextureFormat textureFormat)
	{
		if (textureFormat == TextureFormat.Alpha8 ||
		    textureFormat == TextureFormat.RGBA4444 ||
		    textureFormat == TextureFormat.ARGB4444 ||
		    textureFormat == TextureFormat.ARGB32 ||
		    textureFormat == TextureFormat.RGBA32 ||
		    textureFormat == TextureFormat.RGB24 ||
		    textureFormat == TextureFormat.RFloat ||
		    textureFormat == TextureFormat.RGBAFloat ||
		    textureFormat == TextureFormat.RGFloat)
		{
			Debug.Log (textureFormat.ToString());
			return true;
		}
		else
		{
			Debug.LogError("wrong TextureFormat (" + textureFormat + ")");
			return false;
		}
	}

	void OnGUI_OriginalTexurePreview()
	{
		if(originalTexture) {
			EditorGUI.PrefixLabel(new Rect(25,45,192,32),3,new GUIContent("Preview:"));
			EditorGUI.DrawPreviewTexture(new Rect(25,60,192,32),originalTexture);
			EditorGUI.PrefixLabel(new Rect(192 + 25 + 25,45,192,32),2,new GUIContent("Alpha:"));
			EditorGUI.DrawTextureAlpha(new Rect(192 + 25 + 25,60,192,32),originalTexture);
			EditorGUI.PrefixLabel(new Rect(192 + 25 + 25 + 192 + 25 + 25,45,192,32),0,new GUIContent("Inverted:"));
			if(showInverted)
				EditorGUI.DrawPreviewTexture(new Rect(192 + 25 + 25 + 192 + 25 + 25,60,192,32),invertedTexture);
			//			if(GUI.Button(Rect(3,position.height - 25, position.width-6,20),"Clear texture")) {
			//				texture = EditorGUIUtility.whiteTexture;
			//				showInverted = false;
		}
		GUILayout.Space(15);
		
		if(GUILayout.Button("Process Inverted"))
		{
			if(invertedTexture)
				DestroyImmediate(invertedTexture);
			//Copy the new texture
			invertedTexture = new Texture2D(originalTexture.width, 
			                                originalTexture.height, 
			                                originalTexture.format, 
			                                (originalTexture.mipmapCount != 0));
			for (int m = 0; m < originalTexture.mipmapCount; m++) 
				invertedTexture.SetPixels(originalTexture.GetPixels(m), m);
			InvertColors();
			showInverted = true;
		}
	}


	void OnGUI_TestTexture()
	{
		if(testTexture) {
			EditorGUI.PrefixLabel(			new Rect(256,128,100,15), 0, new GUIContent("Test Texture:"));			// 0 gibt an SortingLayer an
			EditorGUI.DrawPreviewTexture(	new Rect(256,128,256,256), testTexture);
			EditorGUI.PrefixLabel(			new Rect(256+25,128,100,15), 0, new GUIContent("Alpha:"));			// 0 gibt an SortingLayer an
			EditorGUI.DrawTextureAlpha(		new Rect(256+256+25,128,256,256), testTexture);
		}

		spriteRenderer = EditorGUILayout.ObjectField("Sprite Renderer", spriteRenderer, typeof(SpriteRenderer), true) as SpriteRenderer;
		mUseRed = GUILayout.Toggle(mUseRed, "use Red Channel");
		mRedConst = GUILayout.Toggle(mRedConst, "Red Konstant?");
		if(mRedConst)
			mRedConstValue = EditorGUILayout.IntSlider("Red Value", mRedConstValue, 0, 255);
		mUseGreen = GUILayout.Toggle(mUseGreen, "use Green Channel");
		mUseBlue = GUILayout.Toggle(mUseBlue, "use Blue Channel");
		mUseAlpha = GUILayout.Toggle(mUseAlpha, "use Alpha Channel");
		mAlphaConst = GUILayout.Toggle(mAlphaConst, "Alpha Konstant?");
		if(mAlphaConst)
			mAlphaConstValue = EditorGUILayout.IntSlider("Alpha Value", mAlphaConstValue, 0, 255);
		
		mAlphaIsTransparency = GUILayout.Toggle(mAlphaIsTransparency, "Alpha Is Transparency");
		if(testTexture)
		{
			if(mAlphaIsTransparency)
			{
				testTexture.alphaIsTransparency = true;
				if(spriteRenderer)
					spriteRenderer.sprite.texture.alphaIsTransparency = true;
			}
			else
			{
				testTexture.alphaIsTransparency = true;
				if(spriteRenderer)
					spriteRenderer.sprite.texture.alphaIsTransparency = true;
			}
			//Repaint();	// repaint GUI zeichnet immer alpha kanal, preview texture nutzt alphakanal nicht
		}
		
		mOverrideMipMap = GUILayout.Toggle(mOverrideMipMap, "Override MipMap");
		mOverrideMipMapValue = GUILayout.Toggle(mOverrideMipMapValue, "Override MipMap Value");
		
		EditorGUI.BeginChangeCheck();
		TextureFormat newTextureFormat = (TextureFormat) EditorGUILayout.EnumPopup("Texture Format", mTextureFormat);
		if (EditorGUI.EndChangeCheck())
		{
			if (isTexturFormatAccessable(newTextureFormat))
			{
				mTextureFormat = newTextureFormat;
			}
		}
		
		
		if(GUILayout.Button("Process GreyScale Alpha8"))
		{
			if(testTexture)
				DestroyImmediate(testTexture);
			else
				testTexture = EditorGUIUtility.whiteTexture;
			//Copy the new texture
			//			bool mipmap = (originalTexture.mipmapCount != 0);
			bool mipmap = false;
			if(mOverrideMipMap)
				mipmap = mOverrideMipMapValue;
			testTexture = new Texture2D(testTextureWidth, 
			                            testTextureHeight, 
			                            mTextureFormat,
			                            mipmap);
			GreyScale(mUseRed, mUseGreen, mUseBlue, mUseAlpha, mRedConst, mRedConstValue, mAlphaConst ,mAlphaConstValue);
			showTestTexture = true;

			if(spriteRenderer != null)
			{
				Sprite greyScaleSprite = Sprite.Create(testTexture, new Rect(0,0,testTexture.width,testTexture.height), new Vector2(0.5f,0.5f), testTexture.width);
				spriteRenderer.sprite = greyScaleSprite;

			}
		}
	}


	// Use this for initialization
	void OnGUI () {


//		EditorGUILayout.PropertyField(this, "Window Script");
//		EditorGUILayout.PropertyField("Window Script", this, typeof(CreateAlpha8Helper), false) as CreateAlpha8Helper;
//		test = EditorGUILayout.ObjectField("Window Script", test, typeof(CreateAlpha8Helper), false) as CreateAlpha8Helper;

		GUILayout.Label("Source", EditorStyles.boldLabel);
		originalTexture = EditorGUILayout.ObjectField("Original Texture", originalTexture, typeof(Texture2D), false) as Texture2D;

		OnGUI_OriginalTexurePreview();

		OnGUI_TestTexture();


		GUILayout.Space(32);


		overrideSourceAlpha = EditorGUILayout.Toggle("Override Alpha Channel", overrideSourceAlpha);
		overrideAlphaValue = EditorGUILayout.IntField("Override Value", overrideAlphaValue);
			
		GUILayout.Label("Color Palette", EditorStyles.boldLabel);
		colorPaletteSize = EditorGUILayout.IntSlider("Size", colorPaletteSize, minColorPaletteSize, maxColorPaletteSize);
		mColorPaletteFormat = (TextureFormat) EditorGUILayout.EnumPopup("Color Palette Format", mColorPaletteFormat);
//		mColorPaletteImportFormat = (TextureImporterFormat) EditorGUILayout.EnumPopup("Color Palette Import Format", mColorPaletteImportFormat);

		// save texture format changes to Editor Prefs
		EditorGUI.BeginChangeCheck();
		bColorPaletteUseAdvancedImportSettings = OnGUI_ShowAdvancedImportSettings (mColorPaletteImportSettings, bColorPaletteUseAdvancedImportSettings);
		if (EditorGUI.EndChangeCheck())
		{
			SetLastUsedFormat(CmColorPaletteImportSettingsTextureFormat, (int) mColorPaletteImportSettings.textureFormat);
//			Debug.LogWarning(mColorPaletteImportSettings.textureFormat.ToString() + " " + (int) mColorPaletteImportSettings.textureFormat );
		}


		GUILayout.Label("Color Map", EditorStyles.boldLabel);
		GUILayout.Label("Write Channel");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Red");
		writeChRed 		= EditorGUILayout.Toggle(writeChRed);//, GUILayout.ExpandWidth(false));
		GUILayout.Label("Green");
		writeChGreen 	= EditorGUILayout.Toggle(writeChGreen);//, GUILayout.ExpandWidth(false));
		GUILayout.Label("Blue");
		writeChBlue 	= EditorGUILayout.Toggle(writeChBlue);//, GUILayout.ExpandWidth(false));
		GUILayout.Label("Alpha");
		writeChAlpha 	= EditorGUILayout.Toggle(writeChAlpha);//, GUILayout.ExpandWidth(false));
		GUILayout.EndHorizontal();
		useSpreizFaktor = EditorGUILayout.Toggle("Use Spreizfaktor", useSpreizFaktor);
		mColorMapFormat = (TextureFormat) EditorGUILayout.EnumPopup("Color Map Format", mColorMapFormat);
//		mColorMapImportFormat = (TextureImporterFormat) EditorGUILayout.EnumPopup("Color Map Import Format", mColorMapImportFormat);


		// save texture format changes to Editor Prefs
		EditorGUI.BeginChangeCheck();
		bColorMapUseAdvancedImportSettings = OnGUI_ShowAdvancedImportSettings (mColorMapImportSettings, bColorMapUseAdvancedImportSettings);
		if (EditorGUI.EndChangeCheck())
		{
			SetLastUsedFormat(CmColorMapImportSettingsTextureFormat, (int) mColorMapImportSettings.textureFormat);
//			Debug.LogWarning(mColorMapImportSettings.textureFormat.ToString() + " " + (int) mColorMapImportSettings.textureFormat );
		}


		if(GUILayout.Button("Calculate Color Palette and Color Map"))
		{
			CreateAlpha8Asset(originalTexture);
		}


		if(GUILayout.Button("Test"))
		{

		}
	}

	bool OnGUI_ShowAdvancedImportSettings (TextureImporterSettings tiSettings, bool useSettings)
	{
		useSettings = 
			EditorGUILayout.BeginToggleGroup("Override Import Settings", useSettings);

		tiSettings.textureFormat = (TextureImporterFormat) EditorGUILayout.EnumPopup("Color Map Import Format", tiSettings.textureFormat);
		tiSettings.mipmapEnabled = EditorGUILayout.Toggle("Create mip maps", tiSettings.mipmapEnabled );
		tiSettings.alphaIsTransparency = EditorGUILayout.Toggle("Alpha Is Transparency", tiSettings.alphaIsTransparency);
//		textureFormat = (TextureImporterFormat) EditorGUILayout.EnumPopup("Texture format", (System.Enum)textureFormat);              
		tiSettings.maxTextureSize = EditorGUILayout.IntPopup("Max Texture Size", tiSettings.maxTextureSize, allowedMaxTextureSizesString, allowedMaxTextureSize);
		tiSettings.npotScale = (TextureImporterNPOTScale)EditorGUILayout.EnumPopup("Non-power-of-2 scale", tiSettings.npotScale);
		EditorGUILayout.EndToggleGroup();

		return useSettings;
	}

	void OnFocus()
	{
	}

	void CreateAlpha8Asset(Texture2D texture)
	{
		if(!texture)
		{
			Debug.LogError("texture == null");
			return;
		}

		if(texture.alphaIsTransparency)
			Debug.LogError("sprite.texture.alphaIsTransparency!");

		// erstelle Color Palette von original Sprite
		Color32[] spriteColorPalette = CreateColorPalette(texture);

		// erstelle colorMap von original Sprite und ColoPalette
		Texture2D colorMap = CreateColorMap(texture, spriteColorPalette);
		if(colorMap == null)
		{
			Debug.LogError ("colorMap == null");
			return;
		}
		Debug.Log("colorMap Format = " + colorMap.format.ToString());
		Debug.Log("mColorMapFormat = " + mColorMapFormat.ToString());
//		Debug.Log("TextureFormat.Alpha8 " + TextureFormat.Alpha8.ToString());
//		Debug.Log("TextureFormat.ARGB32 " + TextureFormat.ARGB32.ToString());
//		Debug.Log("TextureFormat.RGB24 " + TextureFormat.RGB24.ToString());
//		Debug.Log("TextureFormat.RGBA32 " + TextureFormat.RGBA32.ToString());

		          

		// original Sprite Dateipfad
		string spritePath = AssetDatabase.GetAssetPath (texture);
		if(string.IsNullOrEmpty(spritePath))
		{
			Debug.LogError("spritePath == empty or NULL!");
			return;
		}

		// erstelle Texture von Color Palette zum Speichern als Bild
		Texture2D colorPaletteTexture = new Texture2D( colorPaletteSize, 1, mColorPaletteFormat, false);	//TODO ARGB32

		if (isTexturAccessable(colorPaletteTexture))
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
		GenerateAssetFromTexture(colorPaletteTexture, relPathColorPalete, mColorPaletteImportSettings);

		string relPathColorMap = spritePath + "_colorMap_" + colorMap.format.ToString() + ".png";
//		Debug.LogWarning(relPathColorMap);
		GenerateAssetFromTexture(colorMap, relPathColorMap, mColorMapImportSettings);
		

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




	//	Unsupported texture format - needs to be ARGB32, RGBA32, RGB24, Alpha8 or one of float formats
	//	UnityEngine.Texture2D:SetPixel(Int32, Int32, Color)

	public Texture2D CreateColorMap(Texture2D texture, Color32[] colorPalette)
	{
		Texture2D mapTexture = new Texture2D(texture.width, texture.height, mColorMapFormat, false);		//TODO Alpha8 8bit
		if (!isTexturAccessable(mapTexture))
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
		if(!useSpreizFaktor)
			spreizFaktor = 1.0f;
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
				Color newColor;

				int colorId = GetColorIdFromPalette(colorPalette, currentColor);

				//Spektrumspreizung
				colorId = colorId * Mathf.FloorToInt(spreizFaktor);

//				if(colorId == Mathf.FloorToInt(colorCount/2))
//				{
//					Debug.Log( "colorId = " + colorId );
//					Debug.Log( "(byte) colorId = " + (byte) colorId );
//				}

				// wenn Farbe nicht in der Palette gefunden wurde!
				if( colorId == -1 )
				{
					colorId = 0;
					Debug.LogError ("Farbe " + texture.GetPixel (x,y) + " nicht in der Palette gefunden! Pixel x= " + x + ", y= " + y);
				}

				// speichere Farbwert aus Farbpalette im Red Channel
				newColor = new Color32(0,0,0,0);
//				newColor = new Color32((byte) colorId,0,0,0);
				if(writeChRed)
					newColor[0] = (byte) colorId;
				if(writeChGreen)
					newColor[1] = (byte) colorId;
				if(writeChBlue)
					newColor[2] = (byte) colorId;
				if(writeChAlpha)
					newColor[3] = (byte) colorId;

//				newColor[mColorMapChannel] = (byte) colorId;

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

	bool isTexturAccessable(Texture2D texture)
	{
		if (isTexturFormatAccessable(texture.format))
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




	void GenerateAssetFromTexture(Texture2D texture, string relPath, TextureImporterSettings textureImporterSettings)
	{
		string AssetPath = AssetDatabase.GenerateUniqueAssetPath(relPath);
		
		// Write texture to file
		System.IO.File.WriteAllBytes (AssetPath, texture.EncodeToPNG());
		
		// Delete generated texture
		UnityEngine.Object.DestroyImmediate(texture);
		
		// Import Asset
		AssetDatabase.ImportAsset(AssetPath);
		
		AssetDatabase.SaveAssets();

		ConfigureForAdvancedTexture(AssetPath, textureImporterSettings);
		// Get Imported Texture
		//Texture2D tex = AssetDatabase.LoadAllAssetsAtPath(AssetPath, typeof(Texture2D)) as Texture2D;
	}

	void ConfigureForAdvancedTexture(string assetPath, TextureImporterSettings textureImporterSettings)
	{
		TextureImporter TexImport = AssetImporter.GetAtPath(assetPath) as TextureImporter;
		TextureImporterSettings tiSettings = new TextureImporterSettings();
		TexImport.textureType = TextureImporterType.Advanced;
		TexImport.ReadTextureSettings(tiSettings);

		tiSettings.alphaIsTransparency = textureImporterSettings.alphaIsTransparency;
		tiSettings.mipmapEnabled = textureImporterSettings.mipmapEnabled;
		tiSettings.readable = textureImporterSettings.readable;
		tiSettings.maxTextureSize = textureImporterSettings.maxTextureSize;
		tiSettings.textureFormat = textureImporterSettings.textureFormat;
		tiSettings.filterMode = FilterMode.Point;
		tiSettings.wrapMode = TextureWrapMode.Clamp;
		tiSettings.npotScale = TextureImporterNPOTScale.None;
		TexImport.SetTextureSettings(tiSettings);
		//Save changes
		AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
		AssetDatabase.Refresh();
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

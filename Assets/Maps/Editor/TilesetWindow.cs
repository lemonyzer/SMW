#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;
using UnityEditor;

public class TilesetWindow : EditorWindow {
	
	#region Variables
	static TilesetWindow currWindow;
	Tileset currentTileset;
	string w_TilesetTileTypeFilePath;
	List<TileType> w_TilesetTileTypesList;
	#endregion
	
	#region Main Methods

	public static Tileset CreateEmpty()
	{
		Tileset newTilesetAsset = ScriptableObject.CreateInstance<Tileset>();
		
		AssetDatabase.CreateAsset(newTilesetAsset, "Assets/tileset_NewEmptySO.asset");
		AssetDatabase.SaveAssets();
		
		//		EditorUtility.FocusProjectWindow();			// <-- Satck Overflow
		Selection.activeObject = newTilesetAsset;
		return newTilesetAsset;
	}
	
	public Tileset Create(string tilesetName, Sprite tilesetSprite, List<TileType> tileTypesList, Vector2 tilePivot)
	{
		Tileset newTilesetAsset = ScriptableObject.CreateInstance<Tileset>();
		newTilesetAsset.tilesetName = tilesetName;
		newTilesetAsset.TilesetSprite = tilesetSprite;
		if (tileTypesList != null)
			newTilesetAsset.TileTypes = tileTypesList.ToArray();
		newTilesetAsset.tilePivot = tilePivot;
		AssetDatabase.CreateAsset(newTilesetAsset, "Assets/Maps/tileset_" + tilesetName + ".asset"); //TODO dateiname nur gültige zeichen
		AssetDatabase.SaveAssets();
		
		//		EditorUtility.FocusProjectWindow();		// <-- Satck Overflow
		Selection.activeObject = newTilesetAsset;
		return newTilesetAsset;
	}
	
	[MenuItem("SMW/Tileset/Tileset Window",false,1)]
	public static void Init()
	{
		if(currWindow == null)
		{
			currWindow = (TilesetWindow) EditorWindow.GetWindow(typeof(TilesetWindow));
			currWindow.title = "Tileset";
			//			currWindow.minSize = new Vector2(256,512);
		}
		else
		{
			currWindow.Show();
		}
	}
	
	int w_TilesetPixelPerUnit = 32;
	int w_SubTilePosX = 0;
	int w_SubTilePosY = 0;
	
	//	Sprite w_subTile = null;
	
	SpriteAlignment w_TileSpriteAlignment = SpriteAlignment.BottomLeft;
	Sprite w_TilesetSprite = null;
	Vector2 w_customTilePivotOffset = Vector2.zero;
	int w_TilePixelWidth = 32;
	int w_TilePixelHeight = 32;
	
	Color32 w_TargetColor = new Color32(255,0,255,255);
	Color32 w_ReplacementColor = new Color32(255,0,255,0);

	string w_TilesetName = "";
	
	void OnGUI()
	{
		//EditorGUILayout.Space(10);
		GUILayout.BeginHorizontal();
		{
			GUILayout.Space(10);
			GUILayout.BeginVertical();
			{
				GUILayout.Space(10);
				GUILayout.Label("SMW Tileset Creation", EditorStyles.boldLabel);
				GUILayout.BeginHorizontal();
				{
					GUILayout.BeginVertical();
					{
						GUILayout.Label("Tileset from Sprite", EditorStyles.boldLabel);
						//						EditorGUILayout.LabelField("Sprite");
						EditorGUI.BeginChangeCheck();
						w_TilesetSprite = EditorGUILayout.ObjectField("Sprite", w_TilesetSprite, typeof(Sprite), false,  GUILayout.ExpandWidth(true)) as Sprite;
						if(EditorGUI.EndChangeCheck())
						{
							if(w_TilesetSprite != null)
								w_TilesetName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(w_TilesetSprite));
							else
								w_TilesetName = "";

							if(w_TilesetTileTypesList != null)
							{
								w_TilesetTileTypesList.Clear();
								w_TilesetTileTypesList = null;
							}

						}
						EditorGUILayout.LabelField("TilesetName: "+ w_TilesetName, EditorStyles.boldLabel);
						w_TargetColor = EditorGUILayout.ColorField("Target Color:", w_TargetColor);
						w_ReplacementColor = EditorGUILayout.ColorField("Replacement Color:", w_ReplacementColor);
						if(w_TilesetSprite == null)
							GUI.enabled = false;
						if(GUILayout.Button("Prepare Sprite Texture"))
						{
							w_TilesetSprite = PrepareSpriteTexture(w_TilesetSprite, 32, w_TargetColor, w_ReplacementColor);
						}
						if(GUILayout.Button("Select Tileset's TileTypeFile"))
						{
							if(OnGUI_OpenFile(out m_LastFilePath, "tls"))
							{
								// file slected and openeed
								w_TilesetTileTypeFilePath = m_LastFilePath;
								m_FileOpened = true;
								w_TilesetTileTypesList = ReadTileTypes(w_TilesetTileTypeFilePath);
								if(w_TilesetTileTypesList == null)
								{
									Debug.LogError("w_TilesetTileTypesList == null");
								}
								else
								{
									for(int i=0; i< w_TilesetTileTypesList.Count; i++)
									{
										Debug.Log(Path.GetDirectoryName(m_LastFilePath) + "/" + Path.GetFileName(m_LastFilePath) + " " + w_TilesetTileTypesList[i] + " - " + w_TilesetTileTypesList[i].ToString());// + " " + w_TilesetTileTypes[i]);
									}
								}
							}
							else
							{
								// file open dialouge cancled
								w_TilesetTileTypesList.Clear();
								w_TilesetTileTypesList = null;
								w_TilesetTileTypeFilePath = "";
								m_FileOpened = false;
							}
						}
						EditorGUILayout.LabelField("Tileset's Tile Type File Path = " + w_TilesetTileTypeFilePath, GUILayout.ExpandWidth(true));
						
						bool current = GUI.enabled;
						w_TileSpriteAlignment = (SpriteAlignment) EditorGUILayout.EnumPopup("Pivot", w_TileSpriteAlignment);
						
						if (w_TileSpriteAlignment != SpriteAlignment.Custom) {
							// deaktiviere custom Offset
							GUI.enabled = false;
						}
						// GUI: Custom Pivot
						w_customTilePivotOffset = EditorGUILayout.Vector2Field("Custom Offset", w_customTilePivotOffset);
						GUI.enabled = current;
						
						w_TilePixelWidth = EditorGUILayout.IntField("Tile Width (px)", w_TilePixelWidth);
						w_TilePixelHeight = EditorGUILayout.IntField("Tile Height (px)", w_TilePixelHeight);
						if(w_TilesetSprite != null)
						{
							GUI.enabled = true;
							GUILayout.Label("Spritename = " + w_TilesetSprite.name);
							if (GUILayout.Button("Slice & Prepare Tileset Sprite", GUILayout.ExpandWidth(false)))
							{
								TextureImporter textureImporter = (TextureImporter) UnityEditor.TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(w_TilesetSprite));
								PerformMetaSlice(w_TilesetSprite.texture, textureImporter, w_TileSpriteAlignment, w_customTilePivotOffset, w_TilePixelWidth, w_TilePixelHeight, w_TilesetPixelPerUnit);
							}
							Color temp = GUI.color;
							if(w_TilesetSprite.pixelsPerUnit == 32)
							{
								GUI.color = Color.green;    
								//								EditorGUILayout.LabelField("<color=green>PixelPerUnit = " + w_TilesetSprite.pixelsPerUnit + "</color>");
							}
							else
							{
								GUI.color = Color.red;    
								//								EditorGUILayout.LabelField("<color=red>PixelPerUnit = " + w_TilesetSprite.pixelsPerUnit + "</color>");
							}
							EditorGUILayout.LabelField("PixelPerUnit = " + w_TilesetSprite.pixelsPerUnit);
							GUI.color = temp;
							
						}
						else
						{
							GUILayout.Label("no Sprite selected", EditorStyles.boldLabel);
							GUI.enabled = false;
						}
						if(!m_FileOpened)
						{
							if(GUI.enabled)
							{
								GUILayout.Label("no Tileset TileTypeFile selected", EditorStyles.boldLabel);
								GUI.enabled = false;
							}
						}
						if(w_TilesetTileTypesList == null)
						{
							if(GUI.enabled)
							{
								GUILayout.Label("no Tileset TileTypeFile in List", EditorStyles.boldLabel);
								GUI.enabled = false;
							}
						}
						GUILayout.BeginHorizontal ();
						if (GUILayout.Button("Create Tileset from Sprite & TLS File", GUILayout.ExpandWidth(false)))
						{
							currentTileset = Create(w_TilesetName, w_TilesetSprite, w_TilesetTileTypesList, GetPivotValue(w_TileSpriteAlignment, w_customTilePivotOffset));
							//							EditorUtility.FocusProjectWindow();							<-- Satck Overflow
							//							Selection.activeObject = currentTileset;
						}
						bool lastState = GUI.enabled;
						if (true)
						{
							//TODO
							GUI.enabled = true;
						}
						if (GUILayout.Button("Create Tileset from Sprite without TLS File", GUILayout.ExpandWidth(false)))
						{
							currentTileset = Create(w_TilesetName, w_TilesetSprite, null, GetPivotValue(w_TileSpriteAlignment, w_customTilePivotOffset));
							//							EditorUtility.FocusProjectWindow();							<-- Satck Overflow
							//							Selection.activeObject = currentTileset;
						}
						GUI.enabled = lastState;
						GUILayout.EndHorizontal ();
					}
					GUILayout.EndVertical();
					GUI.enabled = true;
					if (GUILayout.Button("Create empty Tileset SO", GUILayout.ExpandWidth(false)))
					{
						currentTileset = CreateEmpty();
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10);
				GUILayout.Label("Current Tileset", EditorStyles.boldLabel);
				currentTileset = EditorGUILayout.ObjectField("Tileset ", currentTileset, typeof(Tileset) ,false) as Tileset;
				GUILayout.Space(10);
				GUILayout.Label("Tileset Tile Preview", EditorStyles.boldLabel);
				
				w_TilesetPixelPerUnit = EditorGUILayout.IntField("Tileset Pixel per Unit", w_TilesetPixelPerUnit);
				
				w_SubTilePosX = EditorGUILayout.IntField("X (iCol)", w_SubTilePosX);
				w_SubTilePosY = EditorGUILayout.IntField("Y (iRow)", w_SubTilePosY);
				
				//				if (GUILayout.Button("Show Tile (SubSprite)", GUILayout.ExpandWidth(false)))
				//				{
				//					CreateGameObjectWithSubSprite(w_SubTilePosX, w_SubTilePosY);
				//				}
				if (GUILayout.Button("Show Tile (NewSprite)", GUILayout.ExpandWidth(false)))
				{
					CreateGameObjectWithNewSprite(w_SubTilePosX, w_SubTilePosY);
				}
				if (GUILayout.Button("Show Tile (AssetSlicedSprite)", GUILayout.ExpandWidth(false)))
				{
					CreateGameObjectWithAssetSprite(w_SubTilePosX, w_SubTilePosY);
				}
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
		//		Repaint();
	}
	
	void CreateGameObjectWithAssetSprite(int x, int y)
	{
		GameObject spriteGO = new GameObject("Tile SubSprite x=" + x + " y= " + y);
		SpriteRenderer spriteRenderer = spriteGO.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = currentTileset.GetTileSprite(x, y);
		Selection.activeGameObject = spriteGO;
	}
	
	void CreateGameObjectWithNewSprite(int x, int y)
	{
		GameObject spriteGO = new GameObject("Tile NewSprite x=" + x + " y= " + y);
		SpriteRenderer spriteRenderer = spriteGO.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = currentTileset.GetNewCreatetTileSprite(x, y);
		Selection.activeGameObject = spriteGO;
	}
	
	Sprite PrepareSpriteTexture(Sprite originalSprite, float pixelPerUnit, Color32 targetColor, Color32 replacementColor)
	{
		string spriteAssetPath = AssetDatabase.GetAssetPath(originalSprite);

		//Load modified Asset
		originalSprite = SetupSpriteTextureImporter(originalSprite, pixelPerUnit, true, false);

		// Generate Texture with Alpha Channel
		Texture2D tex = new Texture2D(originalSprite.texture.width, originalSprite.texture.height, TextureFormat.ARGB32, false);
		// kopiere Sprite Texture
//		tex.SetPixels(sprite.texture.GetPixels());	// //TODO wenn ColorArray kopiert wird, verliert tex alpha channel
//		tex.Apply();

		ChangeTargetColorToAlpha(originalSprite.texture, tex, targetColor);	// alphachannel schreiben

		// Generate Unique Asset Path for new Texture
//		Debug.Log("AssetDatabase.GetSubFolders(spriteAssetPath) = " + AssetDatabase.GetSubFolders(spriteAssetPath));
//		//AssetDatabase.GetSubFolders(spriteAssetPath) = System.String[]
//		Debug.Log("Path.GetDirectoryName(spriteAssetPath) = " + Path.GetDirectoryName(spriteAssetPath));
//		//Path.GetDirectoryName(spriteAssetPath) = Assets/Textures/Tileset
		string newAssetPath = Path.GetDirectoryName(spriteAssetPath) + "/" + Path.GetFileNameWithoutExtension(spriteAssetPath) + "_ARGB32_" + Path.GetExtension(spriteAssetPath);		
		Debug.Log("newAssetPath = " + newAssetPath);
		//newAssetPath = Assets/Textures/Tilesetsmb1_ARGB32_.png
		string newUniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);
		Debug.Log("newUniqueAssetPath = " + newUniqueAssetPath);
		
		//Wite Texture to File
		byte[] bytes = tex.EncodeToPNG();
		System.IO.File.WriteAllBytes(newUniqueAssetPath, bytes);		// Editor Folder!
		bytes = null;
		// Delete generated Texture
		UnityEngine.Object.DestroyImmediate(tex);
		// Import Texture with Alpha-Channel Asset
//		Texture2D assetTex = AssetDatabase.LoadAssetAtPath(newUniqueAssetPath, typeof(Texture2D)) as Texture2D;
		// Importiere eben geschriebene Datei als Asset
		AssetDatabase.ImportAsset(newUniqueAssetPath);
		// Get imported Asset
		Sprite assetSpriteCopyWithAlpha = AssetDatabase.LoadAssetAtPath(newUniqueAssetPath, typeof(Sprite)) as Sprite;
		if(assetSpriteCopyWithAlpha != null)
		{
			assetSpriteCopyWithAlpha = SetupSpriteTextureImporter(assetSpriteCopyWithAlpha, pixelPerUnit, false, true);
			return assetSpriteCopyWithAlpha;
//			ChangeTargetColor(originalSprite.texture, assetSpriteCopy.texture, targetColor, replacementColor);		// TODO geht nicht!!! Texture kann im Sprite nicht verändert werden, Sprite nach änderung neu laden bringt auch nichts
//
//			//Save changes																							// TODO bringt nichts
//			AssetDatabase.ImportAsset(newUniqueAssetPath, ImportAssetOptions.ForceUpdate);
//			AssetDatabase.Refresh();
//			
//			//Load modified Sprite 
//			assetSpriteCopy = AssetDatabase.LoadAssetAtPath(newUniqueAssetPath, typeof(Sprite)) as Sprite;			// TODO bringt nichts
		}
		else
		{
			Debug.LogError("assetSpriteCopy == " + "NULL");
			return null;
		}
	}

	public static Sprite SetupSpriteTextureImporter(Sprite sprite, float pixelPerUnit, bool freadable, bool alphaIsTransparency)
	{
		string spriteAssetPath = AssetDatabase.GetAssetPath(sprite);
		TextureImporter texImporter = (TextureImporter) TextureImporter.GetAtPath(spriteAssetPath);
		texImporter.textureType = TextureImporterType.Advanced;
		// Properties are ReadOnly
		//		texImporter.spritePixelsPerUnit = pixelPerUnit;
		//		texImporter.wrapMode = TextureWrapMode.Clamp;
		//		texImporter.filterMode = FilterMode.Point;
		//		texImporter.mipmapEnabled = false;
		//		texImporter.maxTextureSize = 1024;
		//		texImporter.textureFormat = TextureImporterFormat.ARGB32;
		//		texImporter.isReadable = true;
		
		TextureImporterSettings texImportSettings = new TextureImporterSettings();
		texImportSettings.spriteMode = (int) SpriteImportMode.Single;
		texImportSettings.spritePixelsPerUnit = pixelPerUnit;
		texImportSettings.wrapMode = TextureWrapMode.Clamp;
		texImportSettings.filterMode = FilterMode.Point;
		texImportSettings.mipmapEnabled = false;
		texImportSettings.maxTextureSize = 1024;
		texImportSettings.textureFormat = TextureImporterFormat.ARGB32;
		texImportSettings.readable = freadable;
		texImportSettings.alphaIsTransparency = alphaIsTransparency;
		
		texImporter.SetTextureSettings(texImportSettings);
		//Save changes
		AssetDatabase.ImportAsset(spriteAssetPath, ImportAssetOptions.ForceUpdate);
		AssetDatabase.Refresh();

		//Load modified Sprite 
		sprite = AssetDatabase.LoadAssetAtPath(spriteAssetPath, typeof(Sprite)) as Sprite;;
		return sprite;
	}
	
	void ChangeTargetColorToAlpha(Texture2D originalTexture, Texture2D modifiedTexture, Color32 targetColor)
	{
		int targetColorFoundCount = 0;
		for(int y=0; y<originalTexture.height; y++)
		{
			for(int x=0; x<originalTexture.width; x++)
			{
				Color32 currentOriginalPixelColor = originalTexture.GetPixel(x,y);
				byte alphaValue = 255;
				if(currentOriginalPixelColor.Equals(targetColor))
				{
					targetColorFoundCount++;
					alphaValue = 0;
				}

				Color32 modifiedTexturePixelColor = new Color32(currentOriginalPixelColor.r,
				                                                currentOriginalPixelColor.g,
				                                                currentOriginalPixelColor.b,
				                                                alphaValue);

				modifiedTexture.SetPixel(x,y, modifiedTexturePixelColor);
			}
		}
		if(targetColorFoundCount > 0)
		{
			modifiedTexture.Apply();
		}
		Debug.Log("TargetColor: " + targetColor.ToString() + " found " + targetColorFoundCount + " times" );
	}

	void ReplaceTargetColor(Texture2D originalTexture, Texture2D modifiedTexture, Color32 targetColor, Color32 replacementColor)
	{
		int targetColorFoundCount = 0;
		for(int y=0; y<originalTexture.height; y++)
		{
			for(int x=0; x<originalTexture.width; x++)
			{
				Color32 currentOriginalPixelColor = originalTexture.GetPixel(x,y);
				if(currentOriginalPixelColor.Equals(targetColor))
				{
					targetColorFoundCount++;
				}
				
				Color32 modifiedTexturePixelColor = replacementColor;
				
				modifiedTexture.SetPixel(x,y, modifiedTexturePixelColor);
			}
		}
		if(targetColorFoundCount > 0)
		{
			modifiedTexture.Apply();
		}
		Debug.Log("TargetColor: " + targetColor.ToString() + " found " + targetColorFoundCount + " times" );
	}
	#endregion
	
	List<TileType> ReadTileTypes(string filePath)
	{
		if(File.Exists(filePath))
		{
			FileStream fs = new FileStream(filePath, FileMode.Open);
			BinaryReader binReader = new BinaryReader(fs);
			
			try {
				
				// lese Anzahl der TileTypes aus
				int iTileTypeSize = FileIO.ReadInt(binReader);
				Debug.Log("iTileTypeSize = " + iTileTypeSize);
				
				if(iTileTypeSize <= 0 || iTileTypeSize > 1024)
				{
					Debug.LogError("lese Anzahl der TileTypes aus: iTileTypeSize ("+iTileTypeSize+") < 0 ||  iTileTypeSize ("+iTileTypeSize+") > 1024");
					binReader.Close();
					fs.Close();
					return null;
				}
				
				//				tiletypes = new TileType[iTileTypeSize];
				List<TileType> tilesetTileTypesList = new List<TileType>();
				
				for(short i = 0; i < iTileTypeSize; i++)
				{
					//					tiletypes[i] = (TileType)ReadInt(tsf);
					TileType value = (TileType) FileIO.ReadInt(binReader);
					tilesetTileTypesList.Add(value);
				}
				
				return tilesetTileTypesList;
			}
			catch (Exception exception)
			{
				Debug.LogError("Tileset's TileType reading Error: \n" + exception);
			}
			finally
			{
				binReader.Close();
				fs.Close();
			}
		}
		else
		{
			Debug.LogError("File Path: " +filePath + " doesn't exists!");
			return null;
		}
		
		return null;
	}
	
	
	string EP_LastWorkingTileSetTileTypeFilePath = "EP_LastWorkingTileSetTileTypeFilePath";
	string m_LastWorkingTileSetTileTypeFilePath = "";
	string m_LastFilePath = "";
	bool m_FileOpened = false;
	
	bool OnGUI_OpenFile(out string absPath, string fileExtension)
	{
		// open folder dialog
		absPath = EditorUtility.OpenFilePanel ("Select Tilesets TileTypeFile", m_LastWorkingTileSetTileTypeFilePath, fileExtension);
		if(!string.IsNullOrEmpty(absPath))
		{
			m_LastWorkingTileSetTileTypeFilePath = absPath;
			//absolutenPath in EditorPrefs speichern 
			EditorPrefs.SetString(EP_LastWorkingTileSetTileTypeFilePath, m_LastWorkingTileSetTileTypeFilePath);
			
			return true;
		}
		else
		{
			return false;
			
		}
	}
	
	
	
	
	
	private void PerformMetaSlice(Texture2D texture, TextureImporter spriteImporter, SpriteAlignment tileAlignment, Vector2 customTilePivot, int tilePixelWidth, int tilePixelHeight, int pixelPerUnity)
	{
		if(spriteImporter != null)
		{
			Debug.Log("PerformMetaSlice: " + spriteImporter.assetPath);
			//			TextureImporter myImporter = null;
			//			myImporter = TextureImporter.GetAtPath (AssetDatabase.GetAssetPath(sprite)) as TextureImporter ;
			
			bool failed = false;
			List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();
			
			// falls multiple 
			int numTilesX = Mathf.FloorToInt(texture.width / tilePixelWidth);
			int numTilesY = Mathf.FloorToInt(texture.height / tilePixelHeight);
			
			Debug.Log("Tileset width " + texture.width + " sliced in " + numTilesX + " Tiles");
			Debug.Log("Tileset height " + texture.height + " sliced in " + numTilesY + " Tiles");
			
			int i=0;	// subsprite name
			for (int y=0; y<numTilesY; y++)
			{
				for (int x=0; x<numTilesX; x++)
				{
					try {
						
						SpriteMetaData spriteMetaData = new SpriteMetaData
						{
							alignment = (int)tileAlignment,
							border = new Vector4(),
							name = System.IO.Path.GetFileNameWithoutExtension(spriteImporter.assetPath) + "_" + i++,
							pivot = GetPivotValue(tileAlignment, customTilePivot),
							rect = new Rect(x*tilePixelWidth, texture.height-(y+1)*tilePixelHeight, tilePixelWidth, tilePixelHeight)
						};
						
						// erhalte sliced Texture
						//					slicedSprite[i] = Sprite.Create(unslicedSprite.texture, spriteMetaData.rect, spriteMetaData.pivot, pixelPerUnit);
						
						metaDataList.Add(spriteMetaData);
						
					}
					catch (Exception exception) {
						failed = true;
						Debug.LogException(exception);
					}
				}
				//				if(y == 1)
				//					break;
			}
			
			if (!failed) {
				spriteImporter.spritePixelsPerUnit = pixelPerUnity;					// setze PixelPerUnit
				spriteImporter.spriteImportMode = SpriteImportMode.Multiple; 		// setze MultipleMode
				spriteImporter.spritesheet = metaDataList.ToArray();				// weiße metaDaten zu
				
				EditorUtility.SetDirty (spriteImporter);
				
				try
				{
					AssetDatabase.StartAssetEditing();
					AssetDatabase.ImportAsset(spriteImporter.assetPath);
				}
				catch (Exception e)
				{
					Debug.LogError("wtf " + e.ToString());
				}
				finally
				{
					AssetDatabase.StopAssetEditing();
					//					myImporter.SaveAndReimport();
					//Close();
				}
			}
			else
			{
				Debug.LogError( spriteImporter.assetPath + " failed");
				//				SpriteAssetInfo(spriteImporter);
			}
		}
		else
		{
			Debug.LogError( " sprite == null");
		}
	}
	
	public static Vector2 GetPivotValue(SpriteAlignment alignment, Vector2 customOffset)
	{
		switch (alignment)
		{
		case SpriteAlignment.Center:
			return new Vector2(0.5f, 0.5f);
		case SpriteAlignment.TopLeft:
			return new Vector2(0.0f, 1f);
		case SpriteAlignment.TopCenter:
			return new Vector2(0.5f, 1f);
		case SpriteAlignment.TopRight:
			return new Vector2(1f, 1f);
		case SpriteAlignment.LeftCenter:
			return new Vector2(0.0f, 0.5f);
		case SpriteAlignment.RightCenter:
			return new Vector2(1f, 0.5f);
		case SpriteAlignment.BottomLeft:
			return new Vector2(0.0f, 0.0f);
		case SpriteAlignment.BottomCenter:
			return new Vector2(0.5f, 0.0f);
		case SpriteAlignment.BottomRight:
			return new Vector2(1f, 0.0f);
		case SpriteAlignment.Custom:
			return customOffset;
		default:
			return Vector2.zero;
		}
	}
	
	private void PerformMetaSliceFromDownToTop(Texture2D texture, TextureImporter spriteImporter, SpriteAlignment tileAlignment, Vector2 tilePivot, int tilePixelWidth, int tilePixelHeight, int pixelPerUnity)
	{
		if(spriteImporter != null)
		{
			Debug.Log("PerformMetaSlice: " + spriteImporter.assetPath);
			//			TextureImporter myImporter = null;
			//			myImporter = TextureImporter.GetAtPath (AssetDatabase.GetAssetPath(sprite)) as TextureImporter ;
			
			bool failed = false;
			List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();
			
			// falls multiple 
			int numTilesX = Mathf.FloorToInt(texture.width / tilePixelWidth);
			int numTilesY = Mathf.FloorToInt(texture.height / tilePixelHeight);
			
			Debug.Log("Tileset width " + texture.width + " sliced in " + numTilesX + " Tiles");
			Debug.Log("Tileset height " + texture.height + " sliced in " + numTilesY + " Tiles");
			
			int i=0;	// subsprite name
			for (int y=0; y<numTilesY; y++)
			{
				for (int x=0; x<numTilesX; x++)
				{
					try {
						
						SpriteMetaData spriteMetaData = new SpriteMetaData
						{
							alignment = (int)tileAlignment,
							border = new Vector4(),
							name = System.IO.Path.GetFileNameWithoutExtension(spriteImporter.assetPath) + "_" + i++,
							pivot = tilePivot,
							rect = new Rect(x*tilePixelWidth, y*tilePixelHeight, tilePixelWidth, tilePixelHeight)
						};
						
						// erhalte sliced Texture
						//					slicedSprite[i] = Sprite.Create(unslicedSprite.texture, spriteMetaData.rect, spriteMetaData.pivot, pixelPerUnit);
						
						metaDataList.Add(spriteMetaData);
						
					}
					catch (Exception exception) {
						failed = true;
						Debug.LogException(exception);
					}
				}
				if(y == 1)
					break;
			}
			
			if (!failed) {
				spriteImporter.spritePixelsPerUnit = pixelPerUnity;					// setze PixelPerUnit
				spriteImporter.spriteImportMode = SpriteImportMode.Multiple; 		// setze MultipleMode
				spriteImporter.spritesheet = metaDataList.ToArray();				// weiße metaDaten zu
				
				EditorUtility.SetDirty (spriteImporter);
				
				try
				{
					AssetDatabase.StartAssetEditing();
					AssetDatabase.ImportAsset(spriteImporter.assetPath);
				}
				catch (Exception e)
				{
					Debug.LogError("wtf " + e.ToString());
				}
				finally
				{
					AssetDatabase.StopAssetEditing();
					//					myImporter.SaveAndReimport();
					//Close();
				}
			}
			else
			{
				Debug.LogError( spriteImporter.assetPath + " failed");
				//				SpriteAssetInfo(spriteImporter);
			}
		}
		else
		{
			Debug.LogError( " sprite == null");
		}
	}
	
}
#endif

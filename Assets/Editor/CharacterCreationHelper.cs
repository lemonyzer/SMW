using UnityEngine;
using System.Collections;

using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
//using UnityEditor.TextureImporter;

using System;
using System.Collections.Generic;
//using System.Xml;

public class CharacterCreationHelper : EditorWindow {

	// properties for all characters
	public AnimationClip spawnAnimClip;
	public AnimationClip protectionAnimClip;
	public AnimationClip rageAnimClip;

	public Sprite kingSprite;
	public Sprite iceWandSprite;
	//public AnimatorController iceWandAnimatorController;
	public RuntimeAnimatorController iceWandAnimatorController;

	public Color color_rootRenderer 						= new Color(1f,1f,1f,1f);		// ALL (ROOT SpriteRenderer)
	public Color color_rootCloneRenderer 					= new Color(1f,1f,1f,1f);		// ALL
	public Color color_kingRenderer		 					= new Color(1f,1f,1f,1f);		// ALL
	public Color color_iceWallRenderer	 					= new Color(1f,1f,1f,1f);		// ALL
	public Color color_currentEstimatedPosOnServer 			= new Color(1f,1f,1f,1f);	// localplayer Character's	only
	public Color color_LastRecvedPos 						= new Color(1f,1f,1f,0.25f);	// all other Character's	vergangene Position
	public Color color_PredictedPosSimulatedWithLastInput 	= new Color(1f,1f,1f,0.25f);	// all other Character's	vergangene Position
	public Color color_PredictedPosCalculatedWithLastInput 	= new Color(1f,1f,1f,0.25f);	// all other Character's	vergangene Position
	
	public int rootRendererSortingLayer;
	public int rootCloneRendererSortingLayer;
	public int kingRendererSortingLayer;
	public int iceWalledRendererSortingLayer;
	public int currentEstimatedPosOnServerSortingLayer;
	public int lastRecvdPosRendererSortingLayer;
	public int preSimPosRendererSortingLayer;
	public int preCalclastRecvdPosRendererSortingLayer;
	// Get the sorting layer names
	//int popupMenuIndex;//The selected GUI popup Index
	public string[] GetSortingLayerNames()
	{
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}
	string[] sortingLayerNames;//we load here our Layer names to be displayed at the popup GUI
	/// <summary>
	/// Raises the enable event. We use it to set some references and do some initialization. I don`t figured out how to make a variable persistent in Unity Editor yet so most of the codes here can useless
	/// </summary>
	void OnEnable()
	{
		sortingLayerNames = GetSortingLayerNames(); //First we load the name of our layers

		rootRendererSortingLayer = GetSortingLayerNumber(SortingLayer.name_CharacterBackground);
		rootCloneRendererSortingLayer = GetSortingLayerNumber(SortingLayer.name_CharacterBackground);

		kingRendererSortingLayer = GetSortingLayerNumber(SortingLayer.name_CharacterKing);
		iceWalledRendererSortingLayer = GetSortingLayerNumber(SortingLayer.name_CharacterForeground);

		currentEstimatedPosOnServerSortingLayer = GetSortingLayerNumber(SortingLayer.name_CharacterForeground);
		lastRecvdPosRendererSortingLayer = GetSortingLayerNumber(SortingLayer.name_CharacterForeground);
		preSimPosRendererSortingLayer = GetSortingLayerNumber(SortingLayer.name_CharacterForeground);
		preCalclastRecvdPosRendererSortingLayer = GetSortingLayerNumber(SortingLayer.name_CharacterForeground);

	}

	public int GetSortingLayerNumber(string sortingLayerName)
	{
		for (int i = 0; i<sortingLayerNames.Length;i++) //here we initialize our popupMenuIndex with the current Sort Layer Name
		{
			if (sortingLayerNames[i] == sortingLayerName)
				return i;
		}
		Debug.LogError("Sorting Layer " + sortingLayerName + " nicht gefunden");
		return 0;
	}

	public SmwCharacter smwCharacter;

	public SpriteAlignment spriteAlignment = SpriteAlignment.Center;
	public Vector2 customOffset = new Vector2(0.5f, 0.5f);

	public Sprite spritesheet;
	public Sprite[] slicedSprite;
//	public Texture2D texture2d;
	public int subSpritesCount = 6;
	public int pixelPerUnit = 32;
	public int pixelSizeWidth = 32;
	public int pixelSizeHeight = 32;

	[MenuItem ("Window/Character Editor %#e")]
	static void Init () {
		GetWindow (typeof (CharacterCreationHelper));
	}

//	void OnEnable ()
//	{
//		if(EditorPrefs.HasKey("ObjectPath"))
//		{
//			string objectPath = EditorPrefs.GetString("ObjectPath");
//			smwCharacter = AssetDatabase.LoadAssetAtPath (objectPath, typeof(SmwCharacter)) as SmwCharacter;
//		}
//	}

	TextureImporter myImporter = null;
	bool canSetupScriptableSMWCharacter = false;

	bool SpriteIsPrepared(TextureImporter myImporter)
	{
		if(myImporter.spritePixelsPerUnit == pixelPerUnit &&
		   myImporter.spritePivot == GetPivotValue(spriteAlignment, customOffset) &&
		   myImporter.spriteImportMode == SpriteImportMode.Multiple)
		{
			return true;
		}
		return false;
	}


//	GUIStyle leftAlignment = new GUIStyle();
//
//
	void OnGUI ()
	{

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Character Library", EditorStyles.boldLabel);
		if (GUILayout.Button("Show Character List"))
		{
			// ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
//			EditorUtility.FocusProjectWindow();
//			Selection.activeObject = smwCharacter;
		}
		if (GUILayout.Button("Open Character List"))
		{
			// ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
			//			EditorUtility.FocusProjectWindow();
			//			Selection.activeObject = smwCharacter;
		}
		if (GUILayout.Button("New Character List"))
		{
			// ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
			//			EditorUtility.FocusProjectWindow();
			//			Selection.activeObject = smwCharacter;
		}
		GUILayout.EndHorizontal ();
		


		GUILayout.BeginVertical ();

		GUILayout.Label ("Generic Animations");
		spawnAnimClip = EditorGUILayout.ObjectField("Spawn Animation", spawnAnimClip, typeof(AnimationClip), false) as AnimationClip;
		protectionAnimClip = EditorGUILayout.ObjectField("Protection Animation", protectionAnimClip, typeof(AnimationClip), false) as AnimationClip;
		rageAnimClip = EditorGUILayout.ObjectField("Rage Animation", rageAnimClip, typeof(AnimationClip), false) as AnimationClip;

		GUILayout.Label ("Special Sprites with Animator Controller");
		kingSprite = EditorGUILayout.ObjectField("King Sprite", kingSprite, typeof(Sprite), false) as Sprite;
		iceWandSprite = EditorGUILayout.ObjectField("Ice Wand Sprite", iceWandSprite, typeof(Sprite), false) as Sprite;
		iceWandAnimatorController = EditorGUILayout.ObjectField("Ice Wand AnimatorController", iceWandAnimatorController, typeof(RuntimeAnimatorController), false) as RuntimeAnimatorController;
		//iceWandAnimator = EditorGUILayout.ObjectField("Ice Wand AnimatorController", iceWandAnimator, typeof(Runti), false) as AnimatorController;
		

		GUILayout.Label ("SpriteRenderer");

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("root", EditorStyles.foldout);
		GUILayout.EndHorizontal ();
		rootRendererSortingLayer  = EditorGUILayout.Popup("Sorting Layer", rootRendererSortingLayer, sortingLayerNames);//The popup menu is displayed simple as that
		color_rootRenderer = EditorGUILayout.ColorField("Color", color_rootRenderer);


		GUILayout.BeginHorizontal ();
		GUILayout.Label ("root clones", EditorStyles.foldout);
		GUILayout.EndHorizontal ();
		rootCloneRendererSortingLayer = EditorGUILayout.Popup("Sorting Layer", rootCloneRendererSortingLayer, sortingLayerNames);//The popup menu is displayed simple as that


		GUILayout.BeginHorizontal ();
		GUILayout.Label ("king", EditorStyles.foldout);
		GUILayout.EndHorizontal ();
		kingRendererSortingLayer = EditorGUILayout.Popup("Sorting Layer", kingRendererSortingLayer, sortingLayerNames);//The popup menu is displayed simple as that


		GUILayout.BeginHorizontal ();
		GUILayout.Label ("icewall", EditorStyles.foldout, GUILayout.ExpandWidth(false));
		GUILayout.EndHorizontal ();
		iceWalledRendererSortingLayer = EditorGUILayout.Popup("Sorting Layer", iceWalledRendererSortingLayer, sortingLayerNames, GUILayout.ExpandWidth(true));//The popup menu is displayed simple as that


		GUILayout.BeginHorizontal ();
		GUILayout.Label ("current estim server Po", EditorStyles.foldout, GUILayout.ExpandWidth(false));
		GUILayout.EndHorizontal ();
		currentEstimatedPosOnServerSortingLayer = EditorGUILayout.Popup("Sorting Layer", currentEstimatedPosOnServerSortingLayer, sortingLayerNames, GUILayout.ExpandWidth(true));//The popup menu is displayed simple as that
		color_currentEstimatedPosOnServer = EditorGUILayout.ColorField("Color", color_currentEstimatedPosOnServer, GUILayout.ExpandWidth(true));

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("last recvd Pos", EditorStyles.foldout, GUILayout.ExpandWidth(false));
		GUILayout.EndHorizontal ();
		lastRecvdPosRendererSortingLayer = EditorGUILayout.Popup("Sorting Layer", lastRecvdPosRendererSortingLayer, sortingLayerNames, GUILayout.ExpandWidth(true));//The popup menu is displayed simple as that
		color_LastRecvedPos = EditorGUILayout.ColorField("Color", color_LastRecvedPos, GUILayout.ExpandWidth(true));


		GUILayout.BeginHorizontal ();
		GUILayout.Label ("predicted Pos sim", EditorStyles.foldout);
		GUILayout.EndHorizontal ();
		preSimPosRendererSortingLayer = EditorGUILayout.Popup("Sorting Layer", preSimPosRendererSortingLayer, sortingLayerNames);//The popup menu is displayed simple as that
		color_PredictedPosSimulatedWithLastInput = EditorGUILayout.ColorField("Color", color_PredictedPosSimulatedWithLastInput);


		GUILayout.BeginHorizontal ();
		GUILayout.Label ("predicted Pos calc", EditorStyles.foldout);
		GUILayout.EndHorizontal ();
		preCalclastRecvdPosRendererSortingLayer = EditorGUILayout.Popup("Sorting Layer", preCalclastRecvdPosRendererSortingLayer, sortingLayerNames);//The popup menu is displayed simple as that
		color_PredictedPosCalculatedWithLastInput = EditorGUILayout.ColorField("Color", color_PredictedPosCalculatedWithLastInput);

		GUILayout.EndVertical ();



		GUILayout.BeginHorizontal ();
		GUILayout.Label ("SMW Character Editor", EditorStyles.boldLabel);

		if (smwCharacter != null)
		{
			// wenn ein SMW Character gesetzt ist
			if (GUILayout.Button("Show SMW Character SO"))
			{
				// ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = smwCharacter;
			}
			if(true)
			{

				GUI.enabled = canSetupScriptableSMWCharacter;

				//TODO
				//TODO aktuell wird nicht direkt das Sprite [multiple] als Asset übergeben!!!
				//TODO
				if (GUILayout.Button("SetCharSpritesheet"))
				{
					Debug.Log("Loading Sprites @ " + myImporter.assetPath);
//					slicedSprite = AssetDatabase.LoadAllAssetRepresentationsAtPath (myImporter.assetPath) as Sprite[];
					//slicedSprite = ((Sprite)AssetDatabase.LoadAllAssetsAtPath(myImporter.assetPath)) //.Of //OfType<Sprite>().ToArray();

					UnityEngine.Object[] test = AssetDatabase.LoadAllAssetsAtPath(myImporter.assetPath);

					if(test != null)
					{
						if(test.Length > 1)
						{
							Debug.Log("SubAssets Anzahl = " + test.Length);
							slicedSprite = new Sprite[test.Length -1 ];
							for(int i=1; i< test.Length; i++)
							{
								slicedSprite[i-1] = test[i] as Sprite;
							}
						}
						else
						{
							Debug.LogError("SubAssets Anzahl = " + test.Length);
						}
					}

					//slicedSprite = Resources.LoadAll(myImporter.assetPath) as Sprite[];

//					Sprite[] temp = AssetDatabase.LoadAllAssetsAtPath(myImporter.assetPath) as Sprite[]; 

					if(slicedSprite != null)
					{
						Debug.Log("slicedSprite SubAssets Anzahl = " + slicedSprite.Length);
						smwCharacter.SetCharSpritesheet(slicedSprite);
					}
					else
					{
						Debug.LogError("slicedSprite == null!!!");
					}
//					EditorUtility.SetDirty(smwCharacter);
//					EditorUtility.FocusProjectWindow();
//					Selection.activeObject = smwCharacter;
				}

				GUI.enabled = true;
			}

		}
		if (GUILayout.Button("New Character"))
		{
			// neuen Character erzeugen
			smwCharacter = CreateSmwCharacter.CreateAssetAndSetup();
		}
		GUILayout.EndHorizontal ();

		smwCharacter = EditorGUILayout.ObjectField("SMW Character SO", smwCharacter, typeof(SmwCharacter), false) as SmwCharacter;


		GUILayout.BeginHorizontal ();
		spritesheet = EditorGUILayout.ObjectField("Unsliced Sprite", spritesheet, typeof(Sprite), false) as Sprite;


		bool enabled_SpriteSet = GUI.enabled;
		myImporter = UnityEditor.TextureImporter.GetAtPath ( AssetDatabase.GetAssetPath(spritesheet) ) as TextureImporter ;
		if(myImporter != null)
		{
			GUI.enabled = true;
			if(SpriteIsPrepared(myImporter))
			{
				canSetupScriptableSMWCharacter = true;
			}
			else
			{
				canSetupScriptableSMWCharacter = false;
				enabled_SpriteSet = true;
				GUI.enabled = false;
			}
		}
		else
		{
			canSetupScriptableSMWCharacter = false;
			GUI.enabled = false;
		}

		GUILayout.EndHorizontal ();

		// GUI SubSpriteCount
		subSpritesCount = EditorGUILayout.IntSlider("Sub Sprites #", subSpritesCount, 1, 6);

		// GUI pixelPerUnit
		pixelPerUnit = EditorGUILayout.IntField("Pixel per Unit", pixelPerUnit);

		GUILayout.BeginHorizontal ();
		// GUI: Pivot
		spriteAlignment = (SpriteAlignment) EditorGUILayout.EnumPopup("Pivot", spriteAlignment);
//		bool customPivotEnabled = enabled_SpriteSet;
		if (spriteAlignment != SpriteAlignment.Custom) {
			// deaktiviere custom Offset
			GUI.enabled = false;
		}
		// GUI: Custom Pivot
		EditorGUILayout.Vector2Field("Custom Offset", customOffset);
		GUILayout.EndHorizontal ();

		GUI.enabled = enabled_SpriteSet;

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button("Sprite Info"))
		{
			// Spriteinfo ausgeben
			SpriteAssetInfo(myImporter);
		}
		
		if (GUILayout.Button("meta. Slice"))
		{
			//Grid Slice
			PerformMetaSlice(spritesheet);
		}

		GUI.enabled = false;
		GUILayout.EndHorizontal ();
		GUI.enabled = true;

		networked = EditorGUILayout.Toggle("for Network", networked);

		if (GUILayout.Button("create Prefab"))
		{
			// create Prefab
			CreateCharacterPrefab();
        }
    }

//	[SerializeField] private TextAsset xmlAsset;
//	public TextureImporter importer;

	// thx to http://www.toxicfork.com/154/importing-xml-spritesheet-into-unity3d

	private void PerformMetaSlice(Sprite sprite)
	{
		if(sprite != null)
		{
			UnityEditor.TextureImporter myImporter = null;
			myImporter = UnityEditor.TextureImporter.GetAtPath ( AssetDatabase.GetAssetPath(sprite) ) as TextureImporter ;

			bool failed = false;
			List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();

//			slicedSprite = new Sprite[subSpritesCount];
			// Calculate SpriteMetaData (sliced SpriteSheet)
			for(int i=0; i<subSpritesCount; i++)
			{
				try {

					SpriteMetaData spriteMetaData = new SpriteMetaData
					{
						alignment = (int)spriteAlignment,
						border = new Vector4(),
						name = System.IO.Path.GetFileNameWithoutExtension(myImporter.assetPath) + "_" + i,
						pivot = GetPivotValue(spriteAlignment, customOffset),
						rect = new Rect(i*pixelSizeWidth, 	0, pixelSizeWidth, 	pixelSizeHeight)
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

//XML not needed
//		XmlDocument document = new XmlDocument();
//		document.LoadXml(xmlAsset.text);
		
//		XmlElement root = document.DocumentElement;
//		if (root.Name == "TextureAtlas")
//		{
//			bool failed = false;
//			
//			Texture2D texture = AssetDatabase.LoadMainAssetAtPath(importer.assetPath) as Texture2D;
//			int textureHeight = texture.height;
//			
//			List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();
//			
//			foreach (XmlNode childNode in root.ChildNodes)
//			{
//				if (childNode.Name == "SubTexture") {
//					try {
//						int width = Convert.ToInt32(childNode.Attributes["width"].Value);
//						int height = Convert.ToInt32(childNode.Attributes["height"].Value);
//						int x = Convert.ToInt32(childNode.Attributes["x"].Value);
//						int y = textureHeight - (height + Convert.ToInt32(childNode.Attributes["y"].Value));
//						
//						SpriteMetaData spriteMetaData = new SpriteMetaData
//						{
//							alignment = (int)spriteAlignment,
//							border = new Vector4(),
//							name = childNode.Attributes["name"].Value,
//							pivot = GetPivotValue(spriteAlignment, customOffset),
//							rect = new Rect(x, y, width, height)
//						};
//						
//						metaDataList.Add(spriteMetaData);
//					}
//					catch (Exception exception) {
//						failed = true;
//						Debug.LogException(exception);
//					}
//				}
//				else
//				{
//					Debug.LogError("Child nodes should be named 'SubTexture' !");
//					failed = true;
//				}
//			}
			
			if (!failed) {
				myImporter.spritePixelsPerUnit = pixelPerUnit;
				myImporter.spriteImportMode = SpriteImportMode.Multiple; 
				myImporter.spritesheet = metaDataList.ToArray();
				
				EditorUtility.SetDirty (myImporter);
				
				try
				{
					AssetDatabase.StartAssetEditing();
					AssetDatabase.ImportAsset(myImporter.assetPath);
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
		}
		else
		{
			Debug.LogError("sprite == null");
		}
	}



	//SpriteEditorUtility
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

	bool networked = false;

	void CreateCharacterPrefab()
	{
		string charName = smwCharacter.charName;
		if(smwCharacter.charName == "")
		{
			charName = "unnamedChar";
			Debug.LogError("smwCharacter.charName == \"\"");
		}

		string pathRelativeToAssetsPath = "";

		if(networked)
			pathRelativeToAssetsPath = "Resources/AutoGen Characters/UnityNetwork";
		else
			pathRelativeToAssetsPath = "Prefabs/AutoGen Characters";

		if (!AnimationHelper.CreateFolder (pathRelativeToAssetsPath))
		{
			Debug.LogError("Ordner " + pathRelativeToAssetsPath + " konnte nicht erstellt werden");
			return;
		}

		string pathRelativeToProject = "Assets/" + pathRelativeToAssetsPath;
		string prefabPathRelativeToProject = "Assets/" + pathRelativeToAssetsPath + "/" + charName + ".prefab";

		UnityEngine.Object emptyObj = PrefabUtility.CreateEmptyPrefab (prefabPathRelativeToProject);
        
		//GameObject tempObj = GameObject.CreatePrimitive(prim);
		//GameObject tempObj = new GameObject(BodyPartComponents.components.ToArray());

		// create empty
		GameObject tempObj = new GameObject(charName);

		// build character
		SmartCreate(tempObj);

		// save GO in prefab
		PrefabUtility.ReplacePrefab(tempObj, emptyObj, ReplacePrefabOptions.ConnectToPrefab);

	}

	List<ChildData> childs;

	public void SmartCreate(GameObject characterGO)
	{
		// erzeuge Child Liste
		childs = new List<ChildData> ();

		// fülle Child Liste
		fillChildData();

		// lese Child Liste aus und erzeuge childGO's
		foreach(ChildData child in childs)
		{
			// erzeuge child GO			AND set name!
			GameObject childGO = new GameObject(child.name);

			//connect childGO with characterGO
			childGO.transform.SetParent(characterGO.transform);

			//set childs offset Position
			childGO.transform.position = child.position;

			//set childs layer
			childGO.layer = LayerMask.NameToLayer(child.layerName);

			//lese childs componenten liste aus
			foreach(ComponentData cc in child.components)
			{
				for(int i=0; i < cc.smartCloneCount; i++)
				{
					if(cc.componentType == typeof(Rigidbody2D))
					{
						Rigidbody2D rb2d = childGO.AddComponent<Rigidbody2D>();
						rb2d.gravityScale = 0.0f;
						rb2d.fixedAngle = true;
						//all
						//rb2d.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(BoxCollider2D))
					{
						BoxCollider2D box = childGO.AddComponent<BoxCollider2D>();
						box.isTrigger = cc.isTrigger;
						box.size = cc.size;
						box.offset = cc.smartOffset[i];												// <--- smart
						//all
						box.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(SpriteRenderer))
					{
						SpriteRenderer spriteRenderer = childGO.AddComponent<SpriteRenderer>();
						spriteRenderer.sprite = cc.sprite;
						spriteRenderer.sortingLayerID = cc.sortingLayer;
						
						//all
						spriteRenderer.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(Animator))
					{
						Animator anim = childGO.AddComponent<Animator>();
//						anim.runtimeAnimatorController = cc.animatorController;
						
						//all
						anim.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(SendDamageTrigger))
					{
						Behaviour script = childGO.AddComponent<SendDamageTrigger>();
						//all
						script.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(ItemCollectorScript))
					{
						Behaviour script = childGO.AddComponent<ItemCollectorScript>();
						//all
						script.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(CloneSpriteScript))
					{
						Behaviour script = childGO.AddComponent<CloneSpriteScript>();
						//all
						script.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(RageTrigger))
					{
						Behaviour script = childGO.AddComponent<RageTrigger>();
						//all
						script.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(PlatformCharacter))
					{
						Behaviour script = childGO.AddComponent<PlatformCharacter>();
						//all
						script.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(PlatformUserControl))
					{
						Behaviour script = childGO.AddComponent<PlatformUserControl>();
						//all
						script.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(PlatformJumperV2))
					{
						Behaviour script = childGO.AddComponent<PlatformJumperV2>();
						//all
						script.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(Shoot))
					{
						Behaviour script = childGO.AddComponent<Shoot>();
						//all
						script.enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(SendDamageTrigger) || 
					        cc.componentType == typeof(ItemCollectorScript) || 
					        cc.componentType == typeof(CloneSpriteScript) ||
					        cc.componentType == typeof(RageTrigger) ||
					        cc.componentType == typeof(PlatformCharacter) ||
					        cc.componentType == typeof(PlatformUserControl) ||
					        cc.componentType == typeof(PlatformJumperV2) ||
					        cc.componentType == typeof(Shoot) ||
					        cc.componentType == typeof(Bot) ||
					        cc.componentType == typeof(Rage) ||
					        cc.componentType == typeof(RageModus) ||
					        cc.componentType == typeof(Shield))
					{
						Behaviour script = childGO.AddComponent<Shield>();
						//all
						script.enabled = cc.enabled;
					}

					if( cc.componentType == null)
					{
						Debug.LogError( childGO.name + "cc.componentType.ToString() = " + cc.componentType.ToString());
					}
					else
					{
						Debug.Log( childGO.name + "cc.componentType.ToString() = " + cc.componentType.ToString());
					}
					

					//current smart Add finish
				}

				//currentComponent finish
			}

			// currentChildGO finish
		}
	}



	public void fillChildData()
	{
		
		float leftPos = -20f;	// TODO inspector
		float rightPos = 20f;	// TODO inspector
		
		Vector3 rootTransformPos = 			Vector3.zero;
		Vector3 centerTransformPos = 		rootTransformPos;
		Vector3 leftTransformPos = 			new Vector3(leftPos,0f,0f);
		Vector3 rightTransformPos = 		new Vector3(rightPos,0f,0f);
		Vector3 headTransformPos = 			new Vector3(0f,0.3f,0f);
		Vector3 feetTransformPos = 			new Vector3(0f,-0.3f,0f);
		Vector3 bodyTransformPos = 			new Vector3(0f,0f,0f);
		Vector3 itemCollectorTransformPos = new Vector3(0f,0f,0f);
		Vector3 powerHitTransformPos = 		new Vector3(0f,0f,0f);
		Vector3 groundStopperTransformPos = new Vector3(0f,-0.25f,0f);
		Vector3 kingTransformPos = 			new Vector3(0f,0.6f,0f);
		
		Vector2 headBoxSize = new Vector2(0.7f,0.25f);
		Vector2 feetBoxSize = new Vector2(0.7f,0.25f);
		Vector2 bodyBoxSize = new Vector2(0.7f,0.8f);
		Vector2 itemCollectorBoxSize = new Vector2(0.7f,0.8f);
		Vector2 powerHitBoxSize = new Vector2(0.7f,0.8f);
		Vector2 groundStopperBoxSize = new Vector2(0.7f,0.5f);
		
		Vector2 colliderOffSetCenter = Vector2.zero;
		Vector2 colliderOffSetLeft = new Vector2(leftPos,0f);
		Vector2 colliderOffSetRight = new Vector2(rightPos,0f);
		
		//			Vector2 headBoxOffset; // use smartOffset
		//			Vector2[] headBoxOffset = new Vector2[3];
		//			headBoxOffset [0] = colliderOffSetCenter;
		//			headBoxOffset [1] = colliderOffSetLeft;
		//			headBoxOffset [2] = colliderOffSetRight;
		Vector2[] smartComponentOffset = new Vector2[3];
		smartComponentOffset [0] = colliderOffSetCenter;
		smartComponentOffset [1] = colliderOffSetLeft;
		smartComponentOffset [2] = colliderOffSetRight;
		
		bool headIsTrigger = true;
		bool feetIsTrigger = true;
		bool bodyIsTrigger = false;
		bool itemCollectorIsTrigger = true;
		bool powerHitAreaIsTrigger = true;
		bool groundStopperIsTrigger = false;


		//ChildData root = new ChildData (Tags.name_player,
		// root
//		SpriteRenderer renderer = characterGO.AddComponent<SpriteRenderer>();	//TODO	layer, sprite
//		//renderer.sprite = 
//		//renderer.sortingLayerID = 
//		//renderer.sortingLayerName = 
//		//renderer.sortingOrder = 
//		Rigidbody2D rb2d = characterGO.AddComponent<Rigidbody2D>();	//TODO  gravityscale = 0, fixedAngle
//		rb2d.gravityScale = 0.0f;
//		rb2d.fixedAngle = true;
//		Animator anim = characterGO.AddComponent<Animator>();		//TODO	animatorController, rootMotion=false
//		//anim.runtimeAnimatorController = 
//		anim.applyRootMotion = false;
//		characterGO.AddComponent<PlatformUserControl>();
//		characterGO.AddComponent<PlatformCharacter>();
//		characterGO.AddComponent<PlatformJumperV2>();
//		Bot bot = characterGO.AddComponent<Bot>();
//		bot.enabled = false;
//		characterGO.AddComponent<Rage>();
//		characterGO.AddComponent<Shoot>();
//		characterGO.AddComponent<Shield>();
//		NetworkedPlayer netPlayer = characterGO.AddComponent<NetworkedPlayer>();
//		AudioSource audioSource = characterGO.AddComponent<AudioSource>();	//TODO (loop off, onawake off)
//		audioSource.playOnAwake = false;
//		audioSource.loop = false;
//		NetworkView networkView = characterGO.AddComponent<NetworkView>();	//TODO
//		networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
//		networkView.observed = netPlayer;
		
		// Clone Left
		ChildData child = new ChildData (Tags.name_cloneLeft, Tags.tag_player, Layer.playerLayerName, leftTransformPos);
		child.Add(typeof(SpriteRenderer), true, smwCharacter.charIdleSprites[0], Color.white);
		child.Add(typeof(CloneSpriteScript), true);
		childs.Add (child);
		
		// Clone Right
		child = new ChildData (Tags.name_cloneRight, Tags.tag_player, Layer.playerLayerName, rightTransformPos);
		child.Add(typeof(SpriteRenderer), true, smwCharacter.charIdleSprites[0], Color.white);
		child.Add(typeof(CloneSpriteScript), true);
		childs.Add (child);
		
		// Head (cloned)
		child = new ChildData (Tags.name_head, Tags.tag_head, Layer.headLayerName, headTransformPos);
		child.Add(typeof(BoxCollider2D), true, headBoxSize, smartComponentOffset, headIsTrigger, 3);
		childs.Add (child);
		
		// Feet (cloned)
		child = new ChildData (Tags.name_feet, Tags.tag_player, Layer.feetLayerName, feetTransformPos);
		child.Add(typeof(BoxCollider2D), true, feetBoxSize, smartComponentOffset, feetIsTrigger, 3);
		child.Add(typeof(SendDamageTrigger),true);
		childs.Add (child);
		
		// Body (cloned)
		child = new ChildData (Tags.name_body, Tags.tag_body, Layer.bodyLayerName, bodyTransformPos);
		child.Add(typeof(BoxCollider2D), true, bodyBoxSize, smartComponentOffset, bodyIsTrigger, 3);
		childs.Add (child);
		
		// ItemCollector (cloned)
		child = new ChildData (Tags.name_itemCollector, Tags.tag_itemCollector, Layer.itemLayerName, itemCollectorTransformPos);
		child.Add(typeof(BoxCollider2D), true, itemCollectorBoxSize, smartComponentOffset, itemCollectorIsTrigger, 3);
		child.Add(typeof(ItemCollectorScript),true);
		childs.Add (child);
		
		// PowerHitArea (cloned)
		child = new ChildData (Tags.name_powerUpHitArea, Tags.tag_powerUpHitArea, Layer.powerUpLayerName, powerHitTransformPos);
		child.Add(typeof(BoxCollider2D), true, powerHitBoxSize, smartComponentOffset, powerHitAreaIsTrigger, 3);
		child.Add(typeof(RageTrigger),true);
		childs.Add (child);
		
		// GroundStopper
		child = new ChildData (Tags.name_groundStopper, Tags.tag_groundStopper, Layer.groundStopperLayerName, groundStopperTransformPos);
		child.Add(typeof(BoxCollider2D), true, groundStopperBoxSize, smartComponentOffset, groundStopperIsTrigger, 1);
		childs.Add (child);
		
		// King
		child = new ChildData (Tags.name_king, Tags.tag_body, Layer.defaultLayerName, kingTransformPos);
		child.Add(typeof(SpriteRenderer), false, MyCharacter.kingSprite, Color.white);
		childs.Add (child);
		
		// CurrentEstimatedPosOnServer
		child = new ChildData (Tags.name_CurrentEstimatedPosOnServer, Tags.tag_CurrentEstimatedPosOnServer, Layer.defaultLayerName, centerTransformPos);
		child.Add(typeof(SpriteRenderer), true, smwCharacter.charIdleSprites[0], color_currentEstimatedPosOnServer);
		childs.Add (child);
		
		// LastRecvedPos
		child = new ChildData (Tags.name_lastReceivedPos, Tags.tag_lastReceivedPos, Layer.defaultLayerName, centerTransformPos);
		child.Add(typeof(SpriteRenderer), true, smwCharacter.charIdleSprites[0], color_LastRecvedPos);
		childs.Add (child);
		
		// PredictedPosSimulatedWithLastInput
		child = new ChildData (Tags.name_PredictedPosSimulatedWithLastInput, Tags.tag_PredictedPosSimulatedWithLastInput, Layer.defaultLayerName, centerTransformPos);
		child.Add(typeof(SpriteRenderer), true, smwCharacter.charIdleSprites[0], color_PredictedPosSimulatedWithLastInput);
		childs.Add (child);
		
		// PredictedPosCalculatedWithLastInput
		child = new ChildData (Tags.name_PredictedPosCalculatedWithLastInput, Tags.tag_PredictedPosCalculatedWithLastInput, Layer.defaultLayerName, centerTransformPos);
		child.Add(typeof(SpriteRenderer), true, smwCharacter.charIdleSprites[0], color_PredictedPosCalculatedWithLastInput);
		childs.Add (child);
		
		// IceWalled
		child = new ChildData (Tags.name_iceWalled, Tags.tag_iceWalled, Layer.defaultLayerName, centerTransformPos);
		child.Add(typeof(SpriteRenderer), true, iceWandSprite, Color.white);
		child.Add(typeof(Animator), true, iceWandAnimatorController);
		childs.Add (child);
	}






















	void ManualSliceSprite(TextureImporter importer)
	{
		if(importer != null && spritesheet != null)
		{
			SpriteAssetInfo(importer);

			// kopiere original SpriteSheet
			string copyPath =  System.IO.Path.GetDirectoryName(importer.assetPath) + "/" +
				System.IO.Path.GetFileNameWithoutExtension(importer.assetPath) + " sliced.asset";
			Debug.Log("copy Path = " + copyPath);
			AssetDatabase.CopyAsset(importer.assetPath, copyPath);
//			importer.SaveAndReimport();	

			// lade kopiertes SpriteSheet
			Sprite newSprite = AssetDatabase.LoadAssetAtPath(copyPath, typeof(Sprite)) as Sprite;
//			TextureImporter copyImporter = UnityEditor.TextureImporter.GetAtPath ( AssetDatabase.GetAssetPath(newSprite) ) as TextureImporter ;
//			SpriteAssetInfo(copyImporter);
//			AssetDatabase.CreateAsset(newSprite, importer.assetPath + " sliced");

			slicedSprite = new Sprite[subSpritesCount];
//	ok		Debug.Log("slicedSprite Array länge: " + slicedSprite.Length);

			// Slice SpriteSheet
			for(int i=0; i<slicedSprite.Length; i++)
			{
//	ok			Debug.Log("Loop i = " + i);
//	ok			Debug.Log("Sprite rect = " + unslicedSprite.rect);
				// left, top, width, height
				Rect rect = new Rect(i*pixelSizeWidth, 	0, pixelSizeWidth, 	pixelSizeHeight);

				// setze Pivot für jedes Sprite
				Vector2 pivot = new Vector2 (0.5f, 0.5f);

				// erhalte sliced Texture
				slicedSprite[i] = Sprite.Create(spritesheet.texture, rect, pivot, pixelPerUnit);

				// setze name 
				slicedSprite[i].name = System.IO.Path.GetFileNameWithoutExtension(importer.assetPath) + "_" + i; 
			}

			if(slicedSprite[0] != null)
			{
				if(slicedSprite[0].texture != null)
				{

					for (int i=subSpritesCount-1; i>=0; i--)
					{
						AssetDatabase.AddObjectToAsset(slicedSprite[i], newSprite);
					}

					// ist nicht gleiches format wie mit Sprite Editor geschnitten
					//AssetDatabase.CreateAsset( slicedSprite[0], importer.assetPath + "_sliced_" + ".asset");
//					AssetDatabase.AddObjectToAsset(slicedSprite[1], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[2], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[3], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[4], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[5], unslicedSprite);

					// geht nicht: unslicedSprite ist schon als Asset gespeichert meldet Console
//					AssetDatabase.CreateAsset( unslicedSprite, importer.assetPath + "_sliced_" + ".asset");
//					AssetDatabase.AddObjectToAsset(slicedSprite[0], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[1], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[2], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[3], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[4], unslicedSprite);
//					AssetDatabase.AddObjectToAsset(slicedSprite[5], unslicedSprite);
					//AssetDatabase.AddObjectToAsset(slicedSprite[1], importer.assetPath + " sliced.asset");
					
					// Reimport the asset after adding an object.
					// Otherwise the change only shows up when saving the project
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newSprite));
				}
			}

		}
		else
		{
			Debug.LogError("importer oder unslicedSprite == null");
		}
	}

	void SetupSprite(TextureImporter importer)
	{
		importer.spritePixelsPerUnit = pixelPerUnit;
		importer.spriteImportMode = SpriteImportMode.Multiple;
		importer.SaveAndReimport();									// sobald am Asset was geändert wurde speichern
	}

	void SpriteAssetInfo(TextureImporter importer)
	{
		// Spriteinfo ausgeben
		Debug.Log("Path = " + importer.assetPath );
		Debug.Log("Import Mode = " + importer.spriteImportMode.ToString() );
		Debug.Log("Pixel Per Unit = " + importer.spritePixelsPerUnit.ToString() );
	}

}

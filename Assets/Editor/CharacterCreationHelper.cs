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

	public SmwCharacterGenerics smwCharacterGenerics;
	public SmwCharacterList smwCharacterList;
	private int viewIndex = 1;

	// properties for all characters
	public AnimationClip spawnAnimClip;
	public AnimationClip protectionAnimClip;
	public AnimationClip rageAnimClip;

	public Sprite kingSprite;
//	public Sprite iceWandSprite;
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
	public string rootRendererSortingLayerName = SortingLayer.name_CharacterBackground;
	public int rootCloneRendererSortingLayer;
	public string rootCloneRendererSortingLayerName = SortingLayer.name_CharacterBackground;
	public int kingRendererSortingLayer;
	public string kingRendererSortingLayerName = SortingLayer.name_CharacterKing;
	public int iceWalledRendererSortingLayer;
	public string iceWalledRendererSortingLayerName = SortingLayer.name_CharacterForeground;
	public int currentEstimatedPosOnServerSortingLayer;
	public string currentEstimatedPosOnServerSortingLayerName = SortingLayer.name_CharacterForeground;
	public int lastRecvdPosRendererSortingLayer;
	public string lastRecvdPosRendererSortingLayerName = SortingLayer.name_CharacterForeground;
	public int preSimPosRendererSortingLayer;
	public string preSimPosRendererSortingLayerName = SortingLayer.name_CharacterForeground;
	public int preCalclastRecvdPosRendererSortingLayer;
	public string preCalclastRecvdPosRendererSortingLayerName = SortingLayer.name_CharacterForeground;
	// Get the sorting layer names
	//int popupMenuIndex;//The selected GUI popup Index
	public string[] GetSortingLayerNames()
	{
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		string[] sortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
//		foreach (string layer in sortingLayers)
//		{
//			Debug.Log(layer);
//		}
		return sortingLayers;
	}
	string[] sortingLayerNames;//we load here our Layer names to be displayed at the popup GUI
	int[] sortingLayersUniqueIDs;//we load here our Layer names to be displayed at the popup GUI

	static string SMWCharacterListPath = "SMWCharacterListPath";
	static string SMWCharacterGenericsPath = "SMWCharacterGenericsPath";

	/// <summary>
	/// Raises the enable event. We use it to set some references and do some initialization. I don`t figured out how to make a variable persistent in Unity Editor yet so most of the codes here can useless
	/// </summary>
	void OnEnable()
	{
		// load last used List
		if(EditorPrefs.HasKey(SMWCharacterListPath))
		{
			string objectPath = EditorPrefs.GetString(SMWCharacterListPath);
			smwCharacterList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(SmwCharacterList)) as SmwCharacterList;
		}
		// load last used character generics
		if(EditorPrefs.HasKey(SMWCharacterGenericsPath))
		{
			string objectPath = EditorPrefs.GetString(SMWCharacterGenericsPath);
			smwCharacterGenerics = AssetDatabase.LoadAssetAtPath(objectPath, typeof(SmwCharacterGenerics)) as SmwCharacterGenerics;
		}

		UpdateSortingLayers();
	}

	// Get the unique sorting layer IDs -- tossed this in for good measure
	public int[] GetSortingLayerUniqueIDs() {
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
		int[] sortingLayersUniqueIDs = (int[]) sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
//		foreach (int layerId in sortingLayersUniqueIDs)
//		{
//			Debug.Log(layerId);
//		}
		return sortingLayersUniqueIDs;
	}

	public string GetSortingLayerName(int sortingLayerID)
	{
		if(sortingLayersUniqueIDs.Length != sortingLayerNames.Length)
			return "Default";
		
		for (int i = 0; i<sortingLayersUniqueIDs.Length; i++) //here we initialize our popupMenuIndex with the current Sort Layer Name
		{
			if (sortingLayersUniqueIDs[i] == sortingLayerID)
				return sortingLayerNames[i];
		}
		Debug.LogError("Sorting Layer " + sortingLayerID + " nicht gefunden");
		return "Default";
	}

	public int GetSortingLayerNumber(string sortingLayerName)
	{
		if(sortingLayersUniqueIDs.Length != sortingLayerNames.Length)
			return 0;

		for (int i = 0; i<sortingLayerNames.Length;i++) //here we initialize our popupMenuIndex with the current Sort Layer Name
		{
			if (sortingLayerNames[i] == sortingLayerName)
				return sortingLayersUniqueIDs[i];
		}
		Debug.LogError("Sorting Layer " + sortingLayerName + " nicht gefunden");
		return 0;
	}

//	public int GetSortingLayerNumber(string sortingLayerName)
//	{
//		for (int i = 0; i<sortingLayerNames.Length;i++) //here we initialize our popupMenuIndex with the current Sort Layer Name
//		{
//			if (sortingLayerNames[i] == sortingLayerName)
//				return i;
//		}
//		Debug.LogError("Sorting Layer " + sortingLayerName + " nicht gefunden");
//		return 0;
//	}

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

	

	void OnFocus()
	{
		// wenn fester wieder aktiv wird //TODO sortingLayer neu einlesen und alles andere auch am besten
		UpdateSortingLayers();
	}

	void UpdateSortingLayers()
	{
		sortingLayerNames = GetSortingLayerNames(); //First we load the name of our layers
		sortingLayersUniqueIDs = GetSortingLayerUniqueIDs(); //First we load the name of our layers

		// int OptionValue, string ist Option => Option muss aktualisiert werden
		// sonst wird vorherige eingabe überschrieben... vorherige eingabe (value) kann jettz falsch sein!! (sortinglayer umsortiert umbenannt gelöscht...)
		// TODO
		// keine AHNUNG eigentlich muss IntPopup in StringPopup gewechselt werden und value wird xxxSortingLayerName
		// geht aber aufs gleiche raus, wenn SortinLayerName in UnityEngine geändert wird  (sortinglayer umsortiert umbenannt gelöscht...) kann es sein das es ebenfalls nicht mehr existiert
		// TODO SortingLayer.xxxName stimmt dann auch nicht mehr!!

		rootRendererSortingLayerName  = GetSortingLayerName(rootRendererSortingLayer);
		rootCloneRendererSortingLayerName = GetSortingLayerName(rootCloneRendererSortingLayer);
		
		kingRendererSortingLayerName = GetSortingLayerName(kingRendererSortingLayer);
		iceWalledRendererSortingLayerName  = GetSortingLayerName(iceWalledRendererSortingLayer);
		
		currentEstimatedPosOnServerSortingLayerName  = GetSortingLayerName(currentEstimatedPosOnServerSortingLayer);
		lastRecvdPosRendererSortingLayerName  = GetSortingLayerName(lastRecvdPosRendererSortingLayer);
		preSimPosRendererSortingLayerName  = GetSortingLayerName(preSimPosRendererSortingLayer);
		preCalclastRecvdPosRendererSortingLayerName  = GetSortingLayerName(preCalclastRecvdPosRendererSortingLayer);

		//TODO check
//		return;
//		rootRendererSortingLayer = GetSortingLayerNumber(rootRendererSortingLayerName);
//		rootCloneRendererSortingLayer = GetSortingLayerNumber(rootCloneRendererSortingLayerName);
//		
//		kingRendererSortingLayer = GetSortingLayerNumber(kingRendererSortingLayerName);
//		iceWalledRendererSortingLayer = GetSortingLayerNumber(iceWalledRendererSortingLayerName);
//		
//		currentEstimatedPosOnServerSortingLayer = GetSortingLayerNumber(currentEstimatedPosOnServerSortingLayerName);
//		lastRecvdPosRendererSortingLayer = GetSortingLayerNumber(lastRecvdPosRendererSortingLayerName);
//		preSimPosRendererSortingLayer = GetSortingLayerNumber(preSimPosRendererSortingLayerName);
//		preCalclastRecvdPosRendererSortingLayer = GetSortingLayerNumber(preCalclastRecvdPosRendererSortingLayerName);
	}

	void OpenCharacterList()
	{
		string absPath = EditorUtility.OpenFilePanel ("Select Character List", "", "");
		if (absPath.StartsWith(Application.dataPath))
		{
			string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
			smwCharacterList = AssetDatabase.LoadAssetAtPath (relPath, typeof(SmwCharacterList)) as SmwCharacterList;
			if (smwCharacterList)
			{
				EditorPrefs.SetString(SMWCharacterListPath, relPath);
			}
		}
	}

	void OpenCharacterGenerics()
	{
		string absPath = EditorUtility.OpenFilePanel ("Select Character Generics", "", "");
		if (absPath.StartsWith(Application.dataPath))
		{
			string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
			smwCharacterGenerics = AssetDatabase.LoadAssetAtPath (relPath, typeof(SmwCharacterGenerics)) as SmwCharacterGenerics;
			if (smwCharacterGenerics)
			{
				EditorPrefs.SetString(SMWCharacterGenericsPath, relPath);
			}
		}
	}

	void CreateNewCharacterList()
	{
		viewIndex = 1;
		smwCharacterList = CreateSmwCharacterList.Create();
		if(smwCharacterList)
		{
			string relPath = AssetDatabase.GetAssetPath(smwCharacterList);
			EditorPrefs.SetString(SMWCharacterListPath, relPath);
		}
	}

	void CreateNewCharacterGenerics()
	{
		smwCharacterGenerics = CreateSmwCharacterGenerics.Create();
		if(smwCharacterGenerics)
		{
			string relPath = AssetDatabase.GetAssetPath(smwCharacterGenerics);
			EditorPrefs.SetString(SMWCharacterGenericsPath, relPath);
		}
	}

	void AddCharacter()
	{
		SmwCharacter character = CreateSmwCharacter.CreateAssetAndSetup();
		character.name = "New Character";
		smwCharacterList.characterList.Add (character);
		viewIndex = smwCharacterList.characterList.Count;
	}

	void DeleteCharacter(int index)
	{
		smwCharacterList.characterList.RemoveAt (index);
	}

	void OnGUI ()
	{

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Character Generics", EditorStyles.boldLabel);
		
		if(smwCharacterGenerics != null)
			GUI.enabled = true;
		else
			GUI.enabled = false;
		if (GUILayout.Button("Show Generics", GUILayout.ExpandWidth(false)))
		{
			// ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = smwCharacterGenerics;
		}
		
		GUI.enabled = true;
		if (GUILayout.Button("Open Existing Generics", GUILayout.ExpandWidth(false)))
		{
			OpenCharacterGenerics();
		}
		
		if(smwCharacterGenerics == null)
			GUI.enabled = true;
		else
			GUI.enabled = false;
		if (GUILayout.Button("Create New Generics", GUILayout.ExpandWidth(false)))
		{
			CreateNewCharacterGenerics();
		}
		GUILayout.EndHorizontal ();

		if(smwCharacterGenerics != null)
		{
			GUI.enabled = true;
			GUILayout.BeginVertical ();
			GUILayout.Label ("Generic Animations");
			spawnAnimClip = EditorGUILayout.ObjectField("Spawn Animation", spawnAnimClip, typeof(AnimationClip), false) as AnimationClip;
			protectionAnimClip = EditorGUILayout.ObjectField("Protection Animation", protectionAnimClip, typeof(AnimationClip), false) as AnimationClip;
			rageAnimClip = EditorGUILayout.ObjectField("Rage Animation", rageAnimClip, typeof(AnimationClip), false) as AnimationClip;
			
			GUILayout.Label ("Special Sprites with Animator Controller");
			kingSprite = EditorGUILayout.ObjectField("King Sprite", kingSprite, typeof(Sprite), false) as Sprite;
			//		iceWandSprite = EditorGUILayout.ObjectField("Ice Wand Sprite", iceWandSprite, typeof(Sprite), false) as Sprite;
			iceWandAnimatorController = EditorGUILayout.ObjectField("Ice Wand AnimatorController", iceWandAnimatorController, typeof(RuntimeAnimatorController), false) as RuntimeAnimatorController;
			//iceWandAnimator = EditorGUILayout.ObjectField("Ice Wand AnimatorController", iceWandAnimator, typeof(Runti), false) as AnimatorController;
			
			
			GUILayout.Label ("SpriteRenderer");
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("root", EditorStyles.foldout);
			GUILayout.EndHorizontal ();
			rootRendererSortingLayer  = EditorGUILayout.IntPopup("Sorting Layer", rootRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs);//The popup menu is displayed simple as that
			color_rootRenderer = EditorGUILayout.ColorField("Color", color_rootRenderer);
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("root clones", EditorStyles.foldout);
			GUILayout.EndHorizontal ();
			rootCloneRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", rootCloneRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs);//The popup menu is displayed simple as that
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("king", EditorStyles.foldout);
			GUILayout.EndHorizontal ();
			kingRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", kingRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs);//The popup menu is displayed simple as that
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("icewall", EditorStyles.foldout, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal ();
			iceWalledRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", iceWalledRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs, GUILayout.ExpandWidth(true));//The popup menu is displayed simple as that
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("current estim server Po", EditorStyles.foldout, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal ();
			currentEstimatedPosOnServerSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", currentEstimatedPosOnServerSortingLayer, sortingLayerNames, sortingLayersUniqueIDs, GUILayout.ExpandWidth(true));//The popup menu is displayed simple as that
			color_currentEstimatedPosOnServer = EditorGUILayout.ColorField("Color", color_currentEstimatedPosOnServer, GUILayout.ExpandWidth(true));
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("last recvd Pos", EditorStyles.foldout, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal ();
			lastRecvdPosRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", lastRecvdPosRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs, GUILayout.ExpandWidth(true));//The popup menu is displayed simple as that
			color_LastRecvedPos = EditorGUILayout.ColorField("Color", color_LastRecvedPos, GUILayout.ExpandWidth(true));
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("predicted Pos sim", EditorStyles.foldout);
			GUILayout.EndHorizontal ();
			preSimPosRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", preSimPosRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs);//The popup menu is displayed simple as that
			color_PredictedPosSimulatedWithLastInput = EditorGUILayout.ColorField("Color", color_PredictedPosSimulatedWithLastInput);
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("predicted Pos calc", EditorStyles.foldout);
			GUILayout.EndHorizontal ();
			preCalclastRecvdPosRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", preCalclastRecvdPosRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs);//The popup menu is displayed simple as that
			color_PredictedPosCalculatedWithLastInput = EditorGUILayout.ColorField("Color", color_PredictedPosCalculatedWithLastInput);
			
			GUILayout.EndVertical ();
		}


		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Character Library", EditorStyles.boldLabel);

		if(smwCharacterList != null)
			GUI.enabled = true;
		else
			GUI.enabled = false;
		if (GUILayout.Button("Show Character List", GUILayout.ExpandWidth(false)))
		{
			// ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = smwCharacterList;
		}

		GUI.enabled = true;
		if (GUILayout.Button("Open Existing Character List", GUILayout.ExpandWidth(false)))
		{
			OpenCharacterList();
		}

		if(smwCharacterList == null)
			GUI.enabled = true;
		else
			GUI.enabled = false;
		if (GUILayout.Button("Create New Character List", GUILayout.ExpandWidth(false)))
		{
			CreateNewCharacterList();
		}
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		GUI.enabled = true;
		smwCharacterList = EditorGUILayout.ObjectField(smwCharacterList, typeof(SmwCharacterList), false, GUILayout.ExpandWidth(false)) as SmwCharacterList;
		GUILayout.EndHorizontal ();
		
		if(smwCharacterList != null)
		{
			// character liste existiert...
			// lese charactere aus
			GUILayout.BeginHorizontal ();

			GUILayout.Space(10);
			if(GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
			{
				if(viewIndex > 1)
					viewIndex--;
			}
			GUILayout.Space(5);
			if(GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
			{
				if(viewIndex < smwCharacterList.characterList.Count)
					viewIndex++;
			}
			GUILayout.Space(60);
			if(GUILayout.Button("Add Character", GUILayout.ExpandWidth(false)))
			{
				AddCharacter();
			}
			if(GUILayout.Button("Delete Character", GUILayout.ExpandWidth(false)))
			{
				DeleteCharacter(viewIndex - 1);
			}
			GUILayout.EndHorizontal ();
		}





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

		if (GUILayout.Button("create RuntimeAnimatorController"))
		{
			// create Prefab
			CharacterAnimator.Create(smwCharacter);
		}
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
//		GameObject tempObj = new GameObject(charName);

		// build character
		GameObject createdCharacterGO = SmartCreate();

		if( createdCharacterGO != null)
		{
			// save GO in prefab
			PrefabUtility.ReplacePrefab(createdCharacterGO, emptyObj, ReplacePrefabOptions.ConnectToPrefab);
		}
		else
		{
			Debug.LogError("created CharacterGO ist NULL!!!");
		}

	}

	ChildData root;
	List<ChildData> childs;

	public GameObject SmartCreate()
	{
		// erzeuge rootGO
//		GameObject characterGO = new GameObject();	// wird in ChildData root erzeugt (root.gameObject)

		// erzeuge Child Liste
		childs = new List<ChildData> ();

		// fülle root und Child Liste
		fillRootAndChildData();

		// lese Child Liste aus und erzeuge childGO's
		foreach(ChildData child in childs)
		{
			//connect childGO with characterGO
			child.gameObject.transform.SetParent(root.gameObject.transform);

			// currentChildGO finish
		}

		return root.gameObject;
	}



	public void fillRootAndChildData()
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


		// root
		root = new ChildData (smwCharacter.charName, Tags.tag_player, Layer.playerLayerName, centerTransformPos);		//TODO Achtung PrefabName und Name können isch unterscheieden!!!
		root.Add(root.gameObject.AddComponent<SpriteRenderer>(), true, smwCharacter.charIdleSprites[0], color_rootRenderer, rootRendererSortingLayer);
		root.Add(root.gameObject.AddComponent<Animator>(), true, null);		//TODO inspector
		root.Add(root.gameObject.AddComponent<Rigidbody2D>(), 0f, true); 	//TODO inspector
		root.Add(root.gameObject.AddComponent<AudioSource>(), true);
		root.Add(root.gameObject.AddComponent<PlatformUserControl>(), true);
		root.Add(root.gameObject.AddComponent<PlatformCharacter>(), true);
		root.Add(root.gameObject.AddComponent<PlatformJumperV2>(), true);
		root.Add(root.gameObject.AddComponent<Rage>(), true);
		root.Add(root.gameObject.AddComponent<Shoot>(), true);
		root.Add(root.gameObject.AddComponent<Shield>(), true);
		NetworkedPlayer netPlayerScript = root.gameObject.AddComponent<NetworkedPlayer>();
		root.Add(netPlayerScript, true);
		root.Add(root.gameObject.AddComponent<NetworkView>(), true, netPlayerScript);
		root.Add(root.gameObject.AddComponent<PushSkript>(), false);
		root.Add(root.gameObject.AddComponent<Bot>(), false);

		
		// Clone Left
		ChildData child = new ChildData (Tags.name_cloneLeft, Tags.tag_player, Layer.playerLayerName, leftTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, smwCharacter.charIdleSprites[0], color_rootCloneRenderer, rootCloneRendererSortingLayer);
		child.Add(child.gameObject.AddComponent<CloneSpriteScript>(), true);
		childs.Add (child);
		
		// Clone Right
		child = new ChildData (Tags.name_cloneRight, Tags.tag_player, Layer.playerLayerName, rightTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, smwCharacter.charIdleSprites[0], color_rootCloneRenderer, rootCloneRendererSortingLayer);
		child.Add(child.gameObject.AddComponent<CloneSpriteScript>(), true);
		childs.Add (child);
		
		// Head (cloned)
		child = new ChildData (Tags.name_head, Tags.tag_head, Layer.headLayerName, headTransformPos);
		child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, headBoxSize, smartComponentOffset, headIsTrigger, 3);
		childs.Add (child);
		
		// Feet (cloned)
		child = new ChildData (Tags.name_feet, Tags.tag_player, Layer.feetLayerName, feetTransformPos);
		child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, feetBoxSize, smartComponentOffset, feetIsTrigger, 3);
		child.Add(child.gameObject.AddComponent<SendDamageTrigger>(),true);
		childs.Add (child);
		
		// Body (cloned)
		child = new ChildData (Tags.name_body, Tags.tag_body, Layer.bodyLayerName, bodyTransformPos);
		child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, bodyBoxSize, smartComponentOffset, bodyIsTrigger, 3);
		childs.Add (child);
		
		// ItemCollector (cloned)
		child = new ChildData (Tags.name_itemCollector, Tags.tag_itemCollector, Layer.itemLayerName, itemCollectorTransformPos);
		child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, itemCollectorBoxSize, smartComponentOffset, itemCollectorIsTrigger, 3);
		child.Add(child.gameObject.AddComponent<ItemCollectorScript>(),true);
		childs.Add (child);
		
		// PowerHitArea (cloned)
		child = new ChildData (Tags.name_powerUpHitArea, Tags.tag_powerUpHitArea, Layer.powerUpLayerName, powerHitTransformPos);
		child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, powerHitBoxSize, smartComponentOffset, powerHitAreaIsTrigger, 3);
		child.Add(child.gameObject.AddComponent<RageTrigger>(),true);
		childs.Add (child);
		
		// GroundStopper
		child = new ChildData (Tags.name_groundStopper, Tags.tag_groundStopper, Layer.groundStopperLayerName, groundStopperTransformPos);
		child.Add(child.gameObject.AddComponent<BoxCollider2D>(), true, groundStopperBoxSize, smartComponentOffset, groundStopperIsTrigger, 1);
		childs.Add (child);
		
		// King
		child = new ChildData (Tags.name_king, Tags.tag_body, Layer.defaultLayerName, kingTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), false, kingSprite, color_kingRenderer, kingRendererSortingLayer);
		childs.Add (child);
		
		// CurrentEstimatedPosOnServer
		child = new ChildData (Tags.name_CurrentEstimatedPosOnServer, Tags.tag_CurrentEstimatedPosOnServer, Layer.defaultLayerName, centerTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, smwCharacter.charIdleSprites[0], color_currentEstimatedPosOnServer, currentEstimatedPosOnServerSortingLayer);
		childs.Add (child);
		
		// LastRecvedPos
		child = new ChildData (Tags.name_lastReceivedPos, Tags.tag_lastReceivedPos, Layer.defaultLayerName, centerTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, smwCharacter.charIdleSprites[0], color_LastRecvedPos, lastRecvdPosRendererSortingLayer);
		childs.Add (child);
		
		// PredictedPosSimulatedWithLastInput
		child = new ChildData (Tags.name_PredictedPosSimulatedWithLastInput, Tags.tag_PredictedPosSimulatedWithLastInput, Layer.defaultLayerName, centerTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, smwCharacter.charIdleSprites[0], color_PredictedPosSimulatedWithLastInput, preSimPosRendererSortingLayer);
		childs.Add (child);
		
		// PredictedPosCalculatedWithLastInput
		child = new ChildData (Tags.name_PredictedPosCalculatedWithLastInput, Tags.tag_PredictedPosCalculatedWithLastInput, Layer.defaultLayerName, centerTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, smwCharacter.charIdleSprites[0], color_PredictedPosCalculatedWithLastInput, preCalclastRecvdPosRendererSortingLayer);
		childs.Add (child);
		
		// IceWalled
		child = new ChildData (Tags.name_iceWalled, Tags.tag_iceWalled, Layer.defaultLayerName, centerTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, null, color_iceWallRenderer, iceWalledRendererSortingLayer);
		child.Add(child.gameObject.AddComponent<Animator>(), true, iceWandAnimatorController);
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

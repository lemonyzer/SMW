using UnityEngine;
using System.Collections;

using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
//using UnityEditor.TextureImporter;

using System;
using System.IO;
using System.Collections.Generic;
//using System.Xml;

public class CharacterCreationHelper : EditorWindow {

	[MenuItem ("Window/Character Editor %#e")]
	static void Init () {
		GetWindow (typeof (CharacterCreationHelper));
	}

	public SmwCharacterGenerics window_SmwCharacterGenerics;
	public SmwCharacterList window_SmwCharacterList;
	private int viewIndex = 1;

	// CharacterGenerics
//	// properties for all characters
//	public AnimationClip spawnAnimClip;
//	public AnimationClip protectionAnimClip;
//	public AnimationClip rageAnimClip;
//
//	public Sprite kingSprite;
////	public Sprite iceWandSprite;
//	//public AnimatorController iceWandAnimatorController;
//	public RuntimeAnimatorController iceWandAnimatorController;
//
//	public Color color_rootRenderer 						= new Color(1f,1f,1f,1f);		// ALL (ROOT SpriteRenderer)
//	public Color color_rootCloneRenderer 					= new Color(1f,1f,1f,1f);		// ALL
//	public Color color_kingRenderer		 					= new Color(1f,1f,1f,1f);		// ALL
//	public Color color_iceWallRenderer	 					= new Color(1f,1f,1f,1f);		// ALL
//	public Color color_currentEstimatedPosOnServer 			= new Color(1f,1f,1f,1f);	// localplayer Character's	only
//	public Color color_LastRecvedPos 						= new Color(1f,1f,1f,0.25f);	// all other Character's	vergangene Position
//	public Color color_PredictedPosSimulatedWithLastInput 	= new Color(1f,1f,1f,0.25f);	// all other Character's	vergangene Position
//	public Color color_PredictedPosCalculatedWithLastInput 	= new Color(1f,1f,1f,0.25f);	// all other Character's	vergangene Position
//	
//	public int rootRendererSortingLayer;
//	public string rootRendererSortingLayerName = SortingLayer.name_CharacterBackground;
//	public int rootCloneRendererSortingLayer;
//	public string rootCloneRendererSortingLayerName = SortingLayer.name_CharacterBackground;
//	public int kingRendererSortingLayer;
//	public string kingRendererSortingLayerName = SortingLayer.name_CharacterKing;
//	public int iceWalledRendererSortingLayer;
//	public string iceWalledRendererSortingLayerName = SortingLayer.name_CharacterForeground;
//	public int currentEstimatedPosOnServerSortingLayer;
//	public string currentEstimatedPosOnServerSortingLayerName = SortingLayer.name_CharacterForeground;
//	public int lastRecvdPosRendererSortingLayer;
//	public string lastRecvdPosRendererSortingLayerName = SortingLayer.name_CharacterForeground;
//	public int preSimPosRendererSortingLayer;
//	public string preSimPosRendererSortingLayerName = SortingLayer.name_CharacterForeground;
//	public int preCalclastRecvdPosRendererSortingLayer;
//	public string preCalclastRecvdPosRendererSortingLayerName = SortingLayer.name_CharacterForeground;
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

	string EP_last_ShowGenericsFoldoutBool = "EP_last_ShowGenericsFoldoutBool";

	/// <summary>
	/// Raises the enable event. We use it to set some references and do some initialization. I don`t figured out how to make a variable persistent in Unity Editor yet so most of the codes here can useless
	/// </summary>
	void OnEnable()
	{
		// load last used List
		if(EditorPrefs.HasKey(SMWCharacterListPath))
		{
			string objectPath = EditorPrefs.GetString(SMWCharacterListPath);
			window_SmwCharacterList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(SmwCharacterList)) as SmwCharacterList;
		}
		// load last used character generics
		if(EditorPrefs.HasKey(SMWCharacterGenericsPath))
		{
			string objectPath = EditorPrefs.GetString(SMWCharacterGenericsPath);
			window_SmwCharacterGenerics = AssetDatabase.LoadAssetAtPath(objectPath, typeof(SmwCharacterGenerics)) as SmwCharacterGenerics;
		}
//		if(EditorPrefs.HasKey(EP_AutoImportPath))
//		{
//			autoImportPath = EditorPrefs.GetString(EP_AutoImportPath);
//		}
		if(EditorPrefs.HasKey(EP_lastBatchImportFolder))
		{
			batch_LastWorkingImportPath = EditorPrefs.GetString(EP_lastBatchImportFolder);
		}

		if(EditorPrefs.HasKey(EP_last_ShowGenericsFoldoutBool))
		{
			showGenerics = EditorPrefs.GetBool(EP_last_ShowGenericsFoldoutBool);
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

	public SmwCharacter window_SmwCharacter;

	public SpriteAlignment spriteAlignment = SpriteAlignment.Center;
	public Vector2 customOffset = new Vector2(0.5f, 0.5f);

	public Sprite spritesheet;
	public Sprite[] slicedSprite;

	public int subSpritesCount = 6;
	public int pixelPerUnit = 32;
	public int pixelSizeWidth = 32;
	public int pixelSizeHeight = 32;


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

		if(window_SmwCharacterGenerics == null)
		{
			return;
		}

		sortingLayerNames = GetSortingLayerNames(); //First we load the name of our layers
		sortingLayersUniqueIDs = GetSortingLayerUniqueIDs(); //First we load the name of our layers

		// int OptionValue, string ist Option => Option muss aktualisiert werden
		// sonst wird vorherige eingabe überschrieben... vorherige eingabe (value) kann jettz falsch sein!! (sortinglayer umsortiert umbenannt gelöscht...)
		// TODO
		// keine AHNUNG eigentlich muss IntPopup in StringPopup gewechselt werden und value wird xxxSortingLayerName
		// geht aber aufs gleiche raus, wenn SortinLayerName in UnityEngine geändert wird  (sortinglayer umsortiert umbenannt gelöscht...) kann es sein das es ebenfalls nicht mehr existiert
		// TODO SortingLayer.xxxName stimmt dann auch nicht mehr!!

		window_SmwCharacterGenerics.rootRendererSortingLayerName  = GetSortingLayerName(window_SmwCharacterGenerics.rootRendererSortingLayer);
		window_SmwCharacterGenerics.rootCloneRendererSortingLayerName = GetSortingLayerName(window_SmwCharacterGenerics.rootCloneRendererSortingLayer);
		
		window_SmwCharacterGenerics.kingRendererSortingLayerName = GetSortingLayerName(window_SmwCharacterGenerics.kingRendererSortingLayer);
		window_SmwCharacterGenerics.iceWalledRendererSortingLayerName  = GetSortingLayerName(window_SmwCharacterGenerics.iceWalledRendererSortingLayer);
		
		window_SmwCharacterGenerics.currentEstimatedPosOnServerSortingLayerName  = GetSortingLayerName(window_SmwCharacterGenerics.currentEstimatedPosOnServerSortingLayer);
		window_SmwCharacterGenerics.lastRecvdPosRendererSortingLayerName  = GetSortingLayerName(window_SmwCharacterGenerics.lastRecvdPosRendererSortingLayer);
		window_SmwCharacterGenerics.preSimPosRendererSortingLayerName  = GetSortingLayerName(window_SmwCharacterGenerics.preSimPosRendererSortingLayer);
		window_SmwCharacterGenerics.preCalclastRecvdPosRendererSortingLayerName  = GetSortingLayerName(window_SmwCharacterGenerics.preCalclastRecvdPosRendererSortingLayer);

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
			window_SmwCharacterList = AssetDatabase.LoadAssetAtPath (relPath, typeof(SmwCharacterList)) as SmwCharacterList;
			if (window_SmwCharacterList)
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
			window_SmwCharacterGenerics = AssetDatabase.LoadAssetAtPath (relPath, typeof(SmwCharacterGenerics)) as SmwCharacterGenerics;
			if (window_SmwCharacterGenerics)
			{
				EditorPrefs.SetString(SMWCharacterGenericsPath, relPath);
			}
		}
	}

	void SaveBoolInEditorPrefs(bool value, string key)
	{
		EditorPrefs.SetBool(key, value);
	}

	void CreateNewCharacterList()
	{
		viewIndex = 1;
		window_SmwCharacterList = CreateSmwCharacterList.Create();
		if(window_SmwCharacterList)
		{
			string relPath = AssetDatabase.GetAssetPath(window_SmwCharacterList);
			EditorPrefs.SetString(SMWCharacterListPath, relPath);
		}
	}

	void CreateNewCharacterGenerics()
	{
		window_SmwCharacterGenerics = CreateSmwCharacterGenerics.Create();
		if(window_SmwCharacterGenerics)
		{
			string relPath = AssetDatabase.GetAssetPath(window_SmwCharacterGenerics);
			EditorPrefs.SetString(SMWCharacterGenericsPath, relPath);
		}
	}

	void AddCharacter()
	{
		SmwCharacter character = CreateSmwCharacter.CreateAssetAndSetup();
		character.name = "New Character";
		window_SmwCharacterList.Add (character);
		viewIndex = window_SmwCharacterList.Count;
	}

	void DeleteCharacter(int index)
	{
		window_SmwCharacterList.RemoveAt (index);
	}

	void OnGUI_Generics()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Character Generics", EditorStyles.boldLabel);
		
		if(window_SmwCharacterGenerics != null)
			GUI.enabled = true;
		else
			GUI.enabled = false;
		if (GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
		{
			// ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
			EditorUtility.SetDirty(window_SmwCharacterGenerics);
		}
		
		if(window_SmwCharacterGenerics != null)
			GUI.enabled = true;
		else
			GUI.enabled = false;
		if (GUILayout.Button("Show Generics", GUILayout.ExpandWidth(false)))
		{
			// ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = window_SmwCharacterGenerics;
		}
		
		GUI.enabled = true;
		if (GUILayout.Button("Open Existing Generics", GUILayout.ExpandWidth(false)))
		{
			OpenCharacterGenerics();
		}
		
		if(window_SmwCharacterGenerics == null)
			GUI.enabled = true;
		else
			GUI.enabled = false;
		if (GUILayout.Button("Create New Generics", GUILayout.ExpandWidth(false)))
		{
			CreateNewCharacterGenerics();
		}
		GUILayout.EndHorizontal ();
		
		if(window_SmwCharacterGenerics != null)
		{
			GUI.enabled = true;
			GUILayout.BeginVertical ();
			GUILayout.Label ("Generic Animations");
			window_SmwCharacterGenerics.spawnAnimClip = EditorGUILayout.ObjectField("Spawn Animation", window_SmwCharacterGenerics.spawnAnimClip, typeof(AnimationClip), false) as AnimationClip;
			window_SmwCharacterGenerics.protectionAnimClip = EditorGUILayout.ObjectField("Protection Animation", window_SmwCharacterGenerics.protectionAnimClip, typeof(AnimationClip), false) as AnimationClip;
			window_SmwCharacterGenerics.rageAnimClip = EditorGUILayout.ObjectField("Rage Animation", window_SmwCharacterGenerics.rageAnimClip, typeof(AnimationClip), false) as AnimationClip;
			
			GUILayout.Label ("Special Sprites with Animator Controller");
			window_SmwCharacterGenerics.kingSprite = EditorGUILayout.ObjectField("King Sprite", window_SmwCharacterGenerics.kingSprite, typeof(Sprite), false) as Sprite;
			//		iceWandSprite = EditorGUILayout.ObjectField("Ice Wand Sprite", iceWandSprite, typeof(Sprite), false) as Sprite;
			window_SmwCharacterGenerics.iceWandAnimatorController = EditorGUILayout.ObjectField("Ice Wand AnimatorController", window_SmwCharacterGenerics.iceWandAnimatorController, typeof(RuntimeAnimatorController), false) as RuntimeAnimatorController;
			//iceWandAnimator = EditorGUILayout.ObjectField("Ice Wand AnimatorController", iceWandAnimator, typeof(Runti), false) as AnimatorController;
			
			
			GUILayout.Label ("SpriteRenderer");
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("root", EditorStyles.foldout);
			GUILayout.EndHorizontal ();
			window_SmwCharacterGenerics.rootRendererSortingLayer  = EditorGUILayout.IntPopup("Sorting Layer", window_SmwCharacterGenerics.rootRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs);//The popup menu is displayed simple as that
			window_SmwCharacterGenerics.color_rootRenderer = EditorGUILayout.ColorField("Color", window_SmwCharacterGenerics.color_rootRenderer);
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("root clones", EditorStyles.foldout);
			GUILayout.EndHorizontal ();
			window_SmwCharacterGenerics.rootCloneRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", window_SmwCharacterGenerics.rootCloneRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs);//The popup menu is displayed simple as that
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("king", EditorStyles.foldout);
			GUILayout.EndHorizontal ();
			window_SmwCharacterGenerics.kingRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", window_SmwCharacterGenerics.kingRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs);//The popup menu is displayed simple as that
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("icewall", EditorStyles.foldout, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal ();
			window_SmwCharacterGenerics.iceWalledRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", window_SmwCharacterGenerics.iceWalledRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs, GUILayout.ExpandWidth(true));//The popup menu is displayed simple as that
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("current estim server Po", EditorStyles.foldout, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal ();
			window_SmwCharacterGenerics.currentEstimatedPosOnServerSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", window_SmwCharacterGenerics.currentEstimatedPosOnServerSortingLayer, sortingLayerNames, sortingLayersUniqueIDs, GUILayout.ExpandWidth(true));//The popup menu is displayed simple as that
			window_SmwCharacterGenerics.color_currentEstimatedPosOnServer = EditorGUILayout.ColorField("Color", window_SmwCharacterGenerics.color_currentEstimatedPosOnServer, GUILayout.ExpandWidth(true));
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("last recvd Pos", EditorStyles.foldout, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal ();
			window_SmwCharacterGenerics.lastRecvdPosRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", window_SmwCharacterGenerics.lastRecvdPosRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs, GUILayout.ExpandWidth(true));//The popup menu is displayed simple as that
			window_SmwCharacterGenerics.color_LastRecvedPos = EditorGUILayout.ColorField("Color", window_SmwCharacterGenerics.color_LastRecvedPos, GUILayout.ExpandWidth(true));
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("predicted Pos sim", EditorStyles.foldout);
			GUILayout.EndHorizontal ();
			window_SmwCharacterGenerics.preSimPosRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", window_SmwCharacterGenerics.preSimPosRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs);//The popup menu is displayed simple as that
			window_SmwCharacterGenerics.color_PredictedPosSimulatedWithLastInput = EditorGUILayout.ColorField("Color", window_SmwCharacterGenerics.color_PredictedPosSimulatedWithLastInput);
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("predicted Pos calc", EditorStyles.foldout);
			GUILayout.EndHorizontal ();
			window_SmwCharacterGenerics.preCalclastRecvdPosRendererSortingLayer = EditorGUILayout.IntPopup("Sorting Layer", window_SmwCharacterGenerics.preCalclastRecvdPosRendererSortingLayer, sortingLayerNames, sortingLayersUniqueIDs);//The popup menu is displayed simple as that
			window_SmwCharacterGenerics.color_PredictedPosCalculatedWithLastInput = EditorGUILayout.ColorField("Color", window_SmwCharacterGenerics.color_PredictedPosCalculatedWithLastInput);
			
			GUILayout.EndVertical ();
		}
	}

	void OnGUI_CharacterList()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Character Library", EditorStyles.boldLabel);
		
		if(window_SmwCharacterList != null)
			GUI.enabled = true;
		else
			GUI.enabled = false;
		if (GUILayout.Button("Set CharacterIDs", GUILayout.ExpandWidth(false)))
		{
			window_SmwCharacterList.SetCharacterIDs();
		}
		if (GUILayout.Button("Show Character List", GUILayout.ExpandWidth(false)))
		{
			// ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = window_SmwCharacterList;
		}
		
		GUI.enabled = true;
		if (GUILayout.Button("Open Existing Character List", GUILayout.ExpandWidth(false)))
		{
			OpenCharacterList();
		}
		
		if(window_SmwCharacterList == null)
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
		window_SmwCharacterList = EditorGUILayout.ObjectField(window_SmwCharacterList, typeof(SmwCharacterList), false, GUILayout.ExpandWidth(false)) as SmwCharacterList;
		GUILayout.EndHorizontal ();
		
		if(window_SmwCharacterList != null)
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
				if(viewIndex < window_SmwCharacterList.Count)
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
	}

	void OnGUI_CharacterEditor ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("SMW Character Editor", EditorStyles.boldLabel);
		
		if (window_SmwCharacter != null)
		{
			// wenn ein SMW Character gesetzt ist
			if (GUILayout.Button("Show SMW Character SO"))
			{
				// ... kann man die Datei im ProjectWindow (Datei Explorer) öffnen
				EditorUtility.FocusProjectWindow();
				Selection.activeObject = window_SmwCharacter;
			}
		}
		if (GUILayout.Button("New Character"))
		{
			// neuen Character erzeugen
			window_SmwCharacter = CreateSmwCharacter.CreateAssetAndSetup();
		}
		GUILayout.EndHorizontal ();
		
		window_SmwCharacter = EditorGUILayout.ObjectField("SMW Character SO", window_SmwCharacter, typeof(SmwCharacter), false) as SmwCharacter;
	}

//	string EP_AutoImportPath = "AutoImportPathString";
//	string autoImportPath = "";

//	string OpenAutoImportFolderDialog(string relStartPath)
//	{
//		string absStartPath = Application.dataPath + relStartPath.Substring("Assets".Length);
//		//Debug.Log(absStartPath);
//		
//		string absPath = EditorUtility.OpenFolderPanel ("Select Folder with Sprites", absStartPath, "");
//		if (absPath.StartsWith(Application.dataPath))
//		{
//			string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
//			if (!string.IsNullOrEmpty(relPath))
//			{
//				EditorPrefs.SetString(EP_AutoImportPath, relPath);
//			}
//			return relPath;
//		}
//		return null;
//	}

//	string OpenAutoImportFolderDialog_Resources(string relStartPath)
//	{
//		string absStartPath = Application.dataPath + relStartPath.Substring("Assets".Length);
//		//Debug.Log(absStartPath);
//		
//		string absPath = EditorUtility.OpenFolderPanel ("Select Folder with Sprites", absStartPath, "");
//		if (absPath.StartsWith(Application.dataPath))
//		{
//			string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
//			if (!string.IsNullOrEmpty(relPath))
//			{
//				EditorPrefs.SetString(EP_AutoImportPath, relPath);
//			}
//
//			//char[] divid = new char[10] ;
//			//divid = (char[]) "/Resources";
//			char[] divid = new char[] { '/','R','e','s','o','u','r','c','e','s' } ;
//			string[] splitt = relPath.Split(divid);
//			string resPath = splitt[splitt.Length - 1];
//			Debug.Log(resPath + " splitt.Length=" + splitt.Length);
//
//			return resPath;
//		}
//		return null;
//	}

//	UnityEngine.Object[] importObjects;
//	void OnGUI_AutoImport()
//	{
//		GUILayout.Label ("Auto Import", EditorStyles.boldLabel);
//		GUILayout.BeginHorizontal ();
//		GUILayout.Label ("Path = " + autoImportPath, GUILayout.ExpandWidth(false));
//		if (GUILayout.Button("Select Import Folder", GUILayout.ExpandWidth(false)))
//		{
//			// open folder dialog
//			//autoImportPath = Application.dataPath + "/" + OpenAutoImportFolderDialog (autoImportPath);// + "/";		// AssetsDatabase kann nur auf Assets/.. zugreifen
//			autoImportPath = OpenAutoImportFolderDialog (autoImportPath);// + "/";
//			if (!string.IsNullOrEmpty(autoImportPath))
//			{
//				//importObjects = AssetDatabase.LoadAllAssetsAtPath(autoImportPath);
//				importObjects = Resources.LoadAll(autoImportPath);
//				if(importObjects != null)
//				{
//					Debug.Log("Found " + importObjects.Length + " importObjects @ relPath " + autoImportPath);
//				}
//			}
//		}
//
//		if(importObjects != null)
//		{
//			GUILayout.Label ( "Found " + importObjects.Length + " importObjects @ relPath " + autoImportPath, GUILayout.ExpandWidth(false));
//			foreach(UnityEngine.Object obj in importObjects)
//			{
//				GUILayout.Label (obj.name , GUILayout.ExpandWidth(false));
//			}
//		}
//		else
//		{
//			GUILayout.Label ("importedObjects == NULL! @ relPath " + autoImportPath, GUILayout.ExpandWidth(false));
//		}
//		
//		GUILayout.EndHorizontal ();
//	}

	public string GetCharNameFromFileName(string fileName)
	{
		string[] splitted = SplittFileName(fileName);

		if(splitted == null)
		{
			Debug.LogError(fileName + " SpittFileName == null");
		}
		if(splitted.Length == 3)
		{
			if (!string.IsNullOrEmpty(splitted[0]))
				return splitted[0];
		}
		else if(splitted.Length == 4)
		{
			if (!string.IsNullOrEmpty(splitted[1]))
				return splitted[1];
		}

		Debug.LogError(fileName + " konnte Character namen nicht extrahieren");
		return null;
	}

	public string[] SplittFileName(string fileName)
	{
		string[] result;
		char[] charSeparators = new char[] {'_'};
		//string[] stringSeparators = new string[] {"[stop]"};
		result = fileName.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
		return result;
	}

	void StartBatchImport(SmwCharacterList charList, SmwCharacterGenerics characterGenerics, bool clearListBeforeImport, string importPath)
	{

		if(string.IsNullOrEmpty(importPath))
		{
			Debug.LogError ("importPath == \"\" oder null !!!");
			return;
		}

		//TODO DONE ordner auf existenz prüfen
		FileInfo[] info = GetFileList(importPath);

		if(info == null)
		{
			Debug.LogError ("FileInfo[] == null !!!");
			return;
		}

		if (charList == null) {
			Debug.LogError ("SmwCharacterList == null !!!");
			return;
		}

		if (characterGenerics == null) {
			Debug.LogError ("characterGenerics == null !!!");
			return;
		}

		if(clearListBeforeImport)
		{
			Debug.LogWarning("SmwCharacterList -> Clear()");
			charList.Clear();
		}

		if(info != null)
		{
			foreach (FileInfo f in info)
			{
				//				Debug.Log("Found " + f.Name);
				//				Debug.Log("f.DirectoryName=" + f.DirectoryName);
				//				Debug.Log("f.FullName=" + f.FullName);
				//				Debug.Log("modified=" + f.FullName.Substring(Application.dataPath.Length - "Assets".Length));
				
				
				// relative pfad angabe
				string currentSpritePath = f.FullName.Substring(Application.dataPath.Length - "Assets".Length);
				GUILayout.Label ("Found " + currentSpritePath, GUILayout.ExpandWidth(false));


				TextureImporter spriteImporter = null;
				spriteImporter = TextureImporter.GetAtPath (currentSpritePath) as TextureImporter ;
				if(spriteImporter == null)
				{
					Debug.LogError( currentSpritePath + " TextureImporter == null ");
					continue;		// skip this character
				}
				else
				{
					// PerformMetaSlice
					PerformMetaSlice(spriteImporter);
				}


				//TODO character name extrahieren (string.splitt by _)
				string charName = GetCharNameFromFileName(f.Name);

				if(string.IsNullOrEmpty(charName))
					charName = f.Name;

				// Character ScriptableObject erstellen	(Ordner und name)
				SmwCharacter currentCharacter = CreateSmwCharacter.CreateAssetWithPathAndName("Assets/Test", charName);		//TODO ordner erstellen falls nicht vorhanden

				//überprüfe ob scriptableObject hinzugefügt wurde
				if(currentCharacter == null)
				{
					Debug.LogError(f.Name + " currentCharacter == null");
					continue;		// skip this character
				}

				AddSpritesheetToSmwCharacterSO(currentCharacter, currentSpritePath);

				//überprüfe ob spritesheet hinzugefügt wurde //TODO inhalt ebenfalls prüfen!
				if(currentCharacter.charSpritesheet == null)
				{
					Debug.LogError(f.Name + " currentCharacter.charSpritesheet == null");
					continue;		// skip this character
				}


				//runtimeAnimatorController erstellen
				RuntimeAnimatorController animController = CharacterAnimator.Create(window_SmwCharacterGenerics, currentCharacter);

				//überprüfe ob runtimeAnimatorController hinzugefügt wurde
				if(currentCharacter.runtimeAnimatorController == null)					//TODO in welchem pfad wird das asset runtimeAnimatorController gespeichert???
				{
					Debug.LogError(f.Name + " currentCharacter.runtimeAnimatorController == null");

					currentCharacter.runtimeAnimatorController = animController;
					Debug.LogError(f.Name + " currentCharacter.runtimeAnimatorController Manuel hinzugefügt");
					if(currentCharacter.runtimeAnimatorController == null)
					{
						continue;		// skip this character
					}
					else
					{
						Debug.LogError(f.Name + " currentCharacter.runtimeAnimatorController Manuel hinzugefügt: Erfolgreich");
					}
				}

				//prefab erstellen
				GameObject currentPrefab = CreateCharacterPrefab(currentCharacter, characterGenerics, batch_KeepBatchCreatedPrefabsInScene);

				//prefab in ScriptableObject referenzieren
				currentCharacter.SetUnityNetworkPrefab(currentPrefab);

				//fertigen SmwCharacter änderungen speichern
				currentCharacter.Save();

				Debug.LogWarning(currentCharacter.charName + " wurde erfolgreich erstellt.");

				//fertigen SmwCharacter in liste speichern
				charList.Add (currentCharacter);
				//viewIndex = window_SmwCharacterList.characterList.Count;
			}
			charList.Save();	//schleife fertig, gefüllte liste speichern
			charList.SetCharacterIDs();		// set characterIds
		}
	}
	bool window_KeepCreatedPrefabsInScene = true;

	string batch_LastWorkingImportPath = "";
	string batch_ImportPath = "";
	bool batch_KeepBatchCreatedPrefabsInScene = false;
	bool clearAndBatchImport = true;

	Vector2 scrollPosition = Vector2.zero;

	string EP_lastBatchImportFolder = "EP_lastBatchImportFolder";
//	string lastBatchImportFolder = "";

//	DirectoryInfo dir = null;
	FileInfo[] window_Batch_FileInfo = null;

	FileInfo[] GetFileList (string absPath)
	{
		if (!string.IsNullOrEmpty(absPath))
		{
			DirectoryInfo dir = new DirectoryInfo(absPath);
			FileInfo[] info = dir.GetFiles("*.png");
			
			
			// Einmalige ausgabe auf Console
			foreach (FileInfo f in info)
			{
				//				Debug.Log("Found " + f.Name);
				//				Debug.Log("f.DirectoryName=" + f.DirectoryName);
				//				Debug.Log("f.FullName=" + f.FullName);
				//				Debug.Log("modified=" + f.FullName.Substring(Application.dataPath.Length - "Assets".Length));
				// relative pfad angabe
				string currentSpritePath = f.FullName.Substring(Application.dataPath.Length - "Assets".Length);
				Debug.Log("currentSpritePath=" + currentSpritePath);

				string charName = GetCharNameFromFileName(f.Name);
				if(charName != null)
				{
					Debug.Log(charName);
				}
				else
				{
					Debug.LogError(f.Name + " konnte Character Name nicht extrahieren");
				}
			}
			return info;
		}
		else
		{
			Debug.LogError("absPath == \"\" or NULL ");
			return null;
		}
	}

	string AbsolutPathToUnityProjectRelativePath(string absPath)
	{
		if (absPath.StartsWith(Application.dataPath))
		{
			string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
			Debug.Log(absPath);
			Debug.Log(relPath);

			return relPath;
		}
		return null;
	}

	string window_batch_fileCount = "";

	void OnGUI_AutoImport()
	{
		GUILayout.Label ("Auto Import", EditorStyles.boldLabel);
		GUILayout.Label ("Path = " + batch_ImportPath, GUILayout.ExpandWidth(false));
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button("Select Import Folder", GUILayout.ExpandWidth(false)))
		{
			// open folder dialog
			batch_ImportPath = EditorUtility.OpenFolderPanel ("Select Import Folder with Sprites", batch_LastWorkingImportPath, "");
			if(!string.IsNullOrEmpty(batch_ImportPath))
			{
				batch_LastWorkingImportPath = batch_ImportPath;
				//absolutenPath in EditorPrefs speichern 
				EditorPrefs.SetString(EP_lastBatchImportFolder, batch_LastWorkingImportPath);
				window_Batch_FileInfo = GetFileList(batch_ImportPath);
			}
			else
			{
				//WITCHTIG!!!!!!!!!!
				batch_ImportPath = "";
				window_Batch_FileInfo = null;

			}

		}
		if (GUILayout.Button("Open Folder", GUILayout.ExpandWidth(false)))
		{
			// open folder dialog
			if(!string.IsNullOrEmpty(batch_LastWorkingImportPath))
			{
				string relPath = AbsolutPathToUnityProjectRelativePath(batch_LastWorkingImportPath);
				if(relPath != null)
				{
					EditorUtility.FocusProjectWindow();
					UnityEngine.Object folder = AssetDatabase.LoadAssetAtPath (relPath,typeof(UnityEngine.Object));
					Selection.activeObject = folder;
				}
			}
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginVertical ();
		if (window_Batch_FileInfo != null)
			GUILayout.Label ( window_Batch_FileInfo.Length + " gefundene *.png im Ordner " + batch_ImportPath, GUILayout.ExpandWidth(false));

//		if (string.IsNullOrEmpty(batch_LastWorkingImportPath))
//		{
//			GUI.enabled = false;
//		}
		if (window_Batch_FileInfo != null && window_Batch_FileInfo.Length > 0)
		{
			window_batch_fileCount = "" + window_Batch_FileInfo.Length;
		}

		if (window_SmwCharacterGenerics != null)
		{
			GUI.enabled = true;
			// TODO// TODO// TODO// TODO// TODO// TODO// TODO// TODO// TODO// TODO// TODO// TODO			window_SmwCharacterGenerics muss komplett eingestellt sein
			if(true)//window_SmwCharacterGenerics.allPropertysSet)
			{
				GUI.enabled = true;
			}
			else
			{
				GUILayout.Label ("window_SmwCharacterGenerics muss komplett eingestellt sein", GUILayout.ExpandWidth(false));
				GUI.enabled = false;
			}
		}
		else
		{
			GUILayout.Label ("window_SmwCharacterGenerics muss geladen sein", GUILayout.ExpandWidth(false));
			GUI.enabled = false;
		}

		if(window_SmwCharacterList == null)
		{
			GUILayout.Label ("window_SmwCharacterList muss geladen sein", GUILayout.ExpandWidth(false));
			GUI.enabled = false;
		}
		clearAndBatchImport = GUILayout.Toggle(clearAndBatchImport, "Clear Character List before bacth import?");
		batch_KeepBatchCreatedPrefabsInScene = GUILayout.Toggle(batch_KeepBatchCreatedPrefabsInScene, "Keep created Prefabs In Scene?");
		if(string.IsNullOrEmpty(batch_ImportPath))
		{
			GUI.enabled = false;
		}
		if (GUILayout.Button("Start Import " + window_batch_fileCount, GUILayout.ExpandWidth(false)))
		{
			StartBatchImport(window_SmwCharacterList, window_SmwCharacterGenerics, clearAndBatchImport, batch_ImportPath);		// TODO absOrdnerPfad angeben und erneut einlesen im BacthImport!!!!!
		}

		if (window_Batch_FileInfo != null && window_Batch_FileInfo.Length > 0)
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			// aktuelle gefundenen Daten ausgeben
			foreach (FileInfo f in window_Batch_FileInfo)
			{
				// relative pfad angabe
				string currentSpritePath = f.FullName.Substring(Application.dataPath.Length - "Assets".Length);
				GUILayout.Label ("Found " + currentSpritePath, GUILayout.ExpandWidth(false));
			}
			EditorGUILayout.EndScrollView();
		}

		GUILayout.EndVertical ();

	}

	protected static bool showGeneralSettings = true; //declare outside of function
	protected static bool showGenerics = true;
	protected static bool showAutoImport = true;
	protected static bool showCharacterList = true;
	protected static bool showCharacterEditor = true;

	GUIStyle myFoldoutStyle;
	

	void InitFoldStyle()
	{
		myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
		myFoldoutStyle.fontStyle = FontStyle.Bold;
		myFoldoutStyle.fontSize = 14;
		Color myStyleColor = Color.red;
		myFoldoutStyle.normal.textColor = myStyleColor;
		myFoldoutStyle.onNormal.textColor = myStyleColor;
		myFoldoutStyle.hover.textColor = myStyleColor;
		myFoldoutStyle.onHover.textColor = myStyleColor;
		myFoldoutStyle.focused.textColor = myStyleColor;
		myFoldoutStyle.onFocused.textColor = myStyleColor;
		myFoldoutStyle.active.textColor = myStyleColor;
		myFoldoutStyle.onActive.textColor = myStyleColor;
	}

	void OnGUI ()
	{
		InitFoldStyle ();	// TODO put in OnEnable

//		showGeneralSettings = EditorGUILayout.Foldout(showGeneralSettings, "General Settings", myFoldoutStyle);
//		if (!showGeneralSettings)
//			return;

		EditorGUI.BeginChangeCheck ();
		showGenerics = EditorGUILayout.Foldout(showGenerics, "Generics", myFoldoutStyle);
		if( EditorGUI.EndChangeCheck () )
		{
			SaveBoolInEditorPrefs(showGenerics, EP_last_ShowGenericsFoldoutBool);
		}
		if(showGenerics)
			OnGUI_Generics();

		showCharacterList = EditorGUILayout.Foldout(showCharacterList, "Character List", myFoldoutStyle);
		if(showCharacterList)
			OnGUI_CharacterList();

		showAutoImport = EditorGUILayout.Foldout(showAutoImport, "AutoImport", myFoldoutStyle);
		if(showAutoImport)
			OnGUI_AutoImport();


		showCharacterEditor = EditorGUILayout.Foldout(showCharacterEditor, "Single Character Editor", myFoldoutStyle);
		if(showCharacterEditor)
			OnGUI_CharacterEditor();


		GUILayout.Label ("Spritesheet Editor", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal ();
		spritesheet = EditorGUILayout.ObjectField("Unsliced Sprite", spritesheet, typeof(Sprite), false) as Sprite;
		GUILayout.EndHorizontal ();
//		bool canSetupSprite = false;
		TextureImporter myImporter = null;
		if(spritesheet != null)
		{
			myImporter = TextureImporter.GetAtPath (AssetDatabase.GetAssetPath(spritesheet)) as TextureImporter;
			if(myImporter != null)
			{
				if(SpriteIsPrepared(myImporter))
				{
					GUILayout.Label("Sprite is prepared! Subsprites = " + myImporter.spritesheet.Length);
				}
				else
				{
					GUILayout.Label("Sprite is not prepared! Subsprites = " + myImporter.spritesheet.Length);
				}

				// GUI SubSpriteCount
				subSpritesCount = EditorGUILayout.IntSlider("Sub Sprites #", subSpritesCount, 1, 6);

				// GUI pixelPerUnit
				pixelPerUnit = EditorGUILayout.IntField("Pixel per Unit", pixelPerUnit);

				GUILayout.BeginHorizontal ();
				// GUI: Pivot
				bool current = GUI.enabled;
				spriteAlignment = (SpriteAlignment) EditorGUILayout.EnumPopup("Pivot", spriteAlignment);
				if (spriteAlignment != SpriteAlignment.Custom) {
					// deaktiviere custom Offset
					GUI.enabled = false;
				}
				// GUI: Custom Pivot
				EditorGUILayout.Vector2Field("Custom Offset", customOffset);
				GUI.enabled = current;
				GUILayout.EndHorizontal ();


				GUILayout.BeginHorizontal ();
				if (GUILayout.Button("Sprite Info"))
				{
					// Spriteinfo ausgeben
					SpriteAssetInfo(myImporter);
				}
				if (GUILayout.Button("meta. Slice"))
				{
					//Grid Slice
					PerformMetaSlice(myImporter);
				}
				GUILayout.EndHorizontal ();
			}
			else
			{
				GUILayout.Label("Error: select other Sprite");
			}
		}


		//TODO
		//TODO aktuell wird nicht direkt das Sprite [multiple] als Asset übergeben!!!
		//TODO
		if(myImporter != null &&
		   myImporter.spritesheet.Length == 6)
		{
			GUI.enabled = true;
			GUILayout.Label("Sprite ist vorbereitet. :)");
		}
		else
		{
			GUI.enabled = false;
			GUILayout.Label("Sprite muss vorbereitet werden!");
		}
		if (GUILayout.Button("Add Spritesheet to Character"))
		{
			AddSpritesheetToSmwCharacterSO(window_SmwCharacter, myImporter.assetPath);
		}




		if(allowedToCreateAnimatorController())
			GUI.enabled = true;
		else
			GUI.enabled = false;
		if (GUILayout.Button("create RuntimeAnimatorController"))
		{
			// create Prefab
			CharacterAnimator.Create(window_SmwCharacterGenerics, window_SmwCharacter);
		}
		networked = EditorGUILayout.Toggle("for Network", networked);


		if(allowedToCreateCharacterPrefab())
			GUI.enabled = true;
		else
			GUI.enabled = false;
		window_KeepCreatedPrefabsInScene = GUILayout.Toggle(window_KeepCreatedPrefabsInScene, "Keep created prefab In Scene?");
			
		if (GUILayout.Button("create Prefab"))
		{
			// create Prefab
			CreateCharacterPrefab(window_SmwCharacter, window_SmwCharacterGenerics, window_KeepCreatedPrefabsInScene);
        }
    }

	bool allowedToMetaSliceSprite()
	{
		//window_SmwCharacter != null
		//window_SmwCharacterGenerics != null
		//AnimClips set != null?
		//Spritesheet sliced ?
		
		if(window_SmwCharacterGenerics != null && 
		   window_SmwCharacterGenerics.protectionAnimClip != null &&
		   window_SmwCharacterGenerics.rageAnimClip != null &&
		   window_SmwCharacterGenerics.spawnAnimClip != null &&
		   window_SmwCharacter != null &&
		   window_SmwCharacter.charSpritesheet.Length == 6)
			return true;
		else
			return false;
	}


	bool allowedToCreateAnimatorController()
	{
		//window_SmwCharacter != null
		//window_SmwCharacterGenerics != null
		//AnimClips set != null?
		//Spritesheet sliced ?

		if(window_SmwCharacterGenerics != null && 
		   window_SmwCharacterGenerics.protectionAnimClip != null &&
		   window_SmwCharacterGenerics.rageAnimClip != null &&
		   window_SmwCharacterGenerics.spawnAnimClip != null &&
		   window_SmwCharacter != null &&
		   window_SmwCharacter.charSpritesheet.Length == 6)
			return true;
		else
			return false;
	}

	bool allowedToCreateCharacterPrefab()
	{
		//window_SmwCharacter != null
		//window_SmwCharacterGenerics != null

		//Spritesheet sliced ?
		//RuntimeAnimator set != null ?

		if(window_SmwCharacterGenerics != null && 
		   window_SmwCharacter != null &&
		   window_SmwCharacter.runtimeAnimatorController != null)
			return true;
		else
			return false;
	}


	private bool AddSpritesheetToSmwCharacterSO(SmwCharacter currentCharacter, string relSpritePath)
	{
//		Debug.Log("Loading Sprites @ " + relSpritePath);
		//					slicedSprite = AssetDatabase.LoadAllAssetRepresentationsAtPath (myImporter.assetPath) as Sprite[];
		//slicedSprite = ((Sprite)AssetDatabase.LoadAllAssetsAtPath(myImporter.assetPath)) //.Of //OfType<Sprite>().ToArray();
		
		UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(relSpritePath);

		Sprite[] slicedSprites = null;
		if(assets != null)
		{
			if(assets.Length > 1)
			{
				Debug.Log("SubAssets Anzahl = " + assets.Length);
				slicedSprites = new Sprite[assets.Length -1 ];							// generate Sprite[] aus asset
				for(int i=1; i< assets.Length; i++)
				{
					slicedSprites[i-1] = assets[i] as Sprite;
				}
			}
			else
			{
				Debug.LogError("SubAssets Anzahl = " + assets.Length);
			}
		}
		

		if(slicedSprites != null)
		{
			Debug.Log("slicedSprites SubAssets Anzahl = " + slicedSprites.Length);
			currentCharacter.SetCharSpritesheet(slicedSprites);								// add to SmwCharacter
			EditorUtility.SetDirty(currentCharacter);										// save ScriptableObject
			return true;
		}
		else
		{
			Debug.LogError("slicedSprites == null!!!");
			return false;
		}
	}


//	[SerializeField] private TextAsset xmlAsset;
//	public TextureImporter importer;

	// thx to http://www.toxicfork.com/154/importing-xml-spritesheet-into-unity3d

	private void PerformMetaSlice(TextureImporter spriteImporter)
	{
		if(spriteImporter != null)
		{
			Debug.Log("PerformMetaSlice: " + spriteImporter.assetPath);
//			TextureImporter myImporter = null;
//			myImporter = TextureImporter.GetAtPath (AssetDatabase.GetAssetPath(sprite)) as TextureImporter ;

			bool failed = false;
			List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();

			//TODO abfragen ob sprite <multiple> ist
			//TODO abfragen ob größe stimmt, überspringen ???

//			Debug.Log("SpriteMode geladen: " + spriteImporter.spriteImportMode.ToString());
//			Debug.Log("SpriteMetaData länge geladen: " + spriteImporter.spritesheet.Length);
//			if(spriteImporter.spriteImportMode == SpriteImportMode.Multiple)
//			{
//				spriteImporter.spriteImportMode = SpriteImportMode.Single;
//				UnityEditor.EditorUtility.SetDirty(myImporter);
//			}
//			Debug.Log("SpriteMode (umgestellt): " + spriteImporter.spriteImportMode.ToString());
//			Debug.Log("SpriteMetaData länge (umgestellt): " + spriteImporter.spritesheet.Length);
			
			// falls multiple 


//			slicedSprite = new Sprite[subSpritesCount];
			// Calculate SpriteMetaData (sliced SpriteSheet)
			for(int i=0; i<subSpritesCount; i++)
			{
				try {

					SpriteMetaData spriteMetaData = new SpriteMetaData
					{
						alignment = (int)spriteAlignment,
						border = new Vector4(),
						name = System.IO.Path.GetFileNameWithoutExtension(spriteImporter.assetPath) + "_" + i,
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
			
			if (!failed) {
				spriteImporter.spritePixelsPerUnit = pixelPerUnit;					// setze PixelPerUnit
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
				SpriteAssetInfo(spriteImporter);
			}
		}
		else
		{
			Debug.LogError( " sprite == null");
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

	GameObject CreateCharacterPrefab(SmwCharacter characterSO, SmwCharacterGenerics generics, bool keepTempCreatedGoInScene)
	{
		string charName = characterSO.charName;
		if(characterSO.charName == "")
		{
			charName = "unnamedChar";
			Debug.LogError("character.charName == \"\" (leer)");
		}
		Debug.Log(this.ToString() + " Create () " + charName);
		

		string pathRelativeToAssetsPath = "";

		if(networked)
			pathRelativeToAssetsPath = "Resources/AutoGen Characters/UnityNetwork";
		else
			pathRelativeToAssetsPath = "Prefabs/AutoGen Characters";

		if (!CharacterAnimator.CreateFolder (pathRelativeToAssetsPath))
		{
			Debug.LogError("Ordner " + pathRelativeToAssetsPath + " konnte nicht erstellt werden");
			return null;
		}

//		string pathRelativeToProject = "Assets/" + pathRelativeToAssetsPath;
		string prefabPathRelativeToProject = "Assets/" + pathRelativeToAssetsPath + "/" + charName + ".prefab";

		UnityEngine.Object emptyObj = PrefabUtility.CreateEmptyPrefab (prefabPathRelativeToProject);
        
		//GameObject tempObj = GameObject.CreatePrimitive(prim);
		//GameObject tempObj = new GameObject(BodyPartComponents.components.ToArray());

		// create empty
//		GameObject tempObj = new GameObject(charName);

		// build character
		GameObject createdCharacterGO = SmartCreate(characterSO, generics);

		if( createdCharacterGO != null)
		{
			// save created GO in prefab
			GameObject createdCharacterPrefab = PrefabUtility.ReplacePrefab(createdCharacterGO, emptyObj, ReplacePrefabOptions.ConnectToPrefab);

			// destroy created GO in Scene
			if(!keepTempCreatedGoInScene)
				DestroyImmediate(createdCharacterGO);

			// return prefab
			return createdCharacterPrefab;
		}
		else
		{
			Debug.LogError("created CharacterGO ist NULL!!!");
			return null;
		}

		return null;
	}

	ChildData root;
	List<ChildData> childs;

	/// <summary>
	/// Smarts the create. Create special Body Parts MultipleTimes
	/// </summary>
	/// <returns>The create.</returns>
	/// <param name="characterSO">Character S.</param>
	/// <param name="charGenerics">Char generics.</param>
	public GameObject SmartCreate(SmwCharacter characterSO, SmwCharacterGenerics charGenerics)
	{
//		Debug.Log(this.ToString() + " Create smwCharacter Name= " + characterSO.name);
		
		// erzeuge rootGO
//		GameObject characterGO = new GameObject();	// wird in ChildData root erzeugt (root.gameObject)

		// erzeuge Child Liste
		childs = new List<ChildData> ();

		// fülle root und Child Liste
		fillRootAndChildData(characterSO, charGenerics);

		// lese Child Liste aus und erzeuge childGO's
		foreach(ChildData child in childs)
		{
			//connect childGO with characterGO
			child.gameObject.transform.SetParent(root.gameObject.transform);

			// currentChildGO finish
		}

		return root.gameObject;
	}



	public void fillRootAndChildData(SmwCharacter characterSO, SmwCharacterGenerics charGenerics)
	{
		
		float leftPos = -20f;	// TODO inspector
		float rightPos = 20f;	// TODO inspector
		float topPos = 15f;	// TODO inspector
		float bottomPos = -15f;	// TODO inspector
		
		Vector3 rootTransformPos = 			Vector3.zero;
		Vector3 centerTransformPos = 		rootTransformPos;
		Vector3 leftTransformPos = 			new Vector3(leftPos,0f,0f);
		Vector3 rightTransformPos = 		new Vector3(rightPos,0f,0f);
		Vector3 topTransformPos = 			new Vector3(0f,topPos,0f);
		Vector3 bottomTransformPos = 		new Vector3(0f,bottomPos,0f);
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
		root = new ChildData (characterSO.charName, Tags.tag_player, Layer.playerLayerName, centerTransformPos);		//TODO Achtung PrefabName und Name können isch unterscheieden!!!
		root.Add(root.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.charIdleSprites[0], charGenerics.color_rootRenderer, charGenerics.rootRendererSortingLayer);
		root.Add(root.gameObject.AddComponent<Animator>(), true, characterSO.runtimeAnimatorController);		//TODO inspector
		root.Add(root.gameObject.AddComponent<Rigidbody2D>(), 0f, true); 	//TODO inspector
		root.Add(root.gameObject.AddComponent<AudioSource>(), true);
		root.Add(root.gameObject.AddComponent<RealOwner>(), true);
		root.Add(root.gameObject.AddComponent<PlatformUserControl>(), true);
		PlatformCharacter platformCharScript = root.gameObject.AddComponent<PlatformCharacter>();
		// build > 0.701
		// add CharSO to CharPrefab
		platformCharScript.SetSmwCharacterSO(characterSO);
		root.Add(platformCharScript, true);
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
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.charIdleSprites[0], charGenerics.color_rootCloneRenderer, charGenerics.rootCloneRendererSortingLayer);
		child.Add(child.gameObject.AddComponent<CloneSpriteScript>(), true);
		childs.Add (child);
		
		// Clone Right
		child = new ChildData (Tags.name_cloneRight, Tags.tag_player, Layer.playerLayerName, rightTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.charIdleSprites[0], charGenerics.color_rootCloneRenderer, charGenerics.rootCloneRendererSortingLayer);
		child.Add(child.gameObject.AddComponent<CloneSpriteScript>(), true);
		childs.Add (child);

		// Clone Top
		child = new ChildData (Tags.name_CloneTop, Tags.tag_player, Layer.playerLayerName, topTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.charIdleSprites[0], charGenerics.color_rootCloneRenderer, charGenerics.rootCloneRendererSortingLayer);
		child.Add(child.gameObject.AddComponent<CloneSpriteScript>(), true);
		childs.Add (child);

		// Clone Bottom
		child = new ChildData (Tags.name_CloneBottom, Tags.tag_player, Layer.playerLayerName, bottomTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.charIdleSprites[0], charGenerics.color_rootCloneRenderer, charGenerics.rootCloneRendererSortingLayer);
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
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), false, charGenerics.kingSprite, charGenerics.color_kingRenderer, charGenerics.kingRendererSortingLayer);
		childs.Add (child);
		
		// CurrentEstimatedPosOnServer
		child = new ChildData (Tags.name_CurrentEstimatedPosOnServer, Tags.tag_CurrentEstimatedPosOnServer, Layer.defaultLayerName, centerTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.charIdleSprites[0], charGenerics.color_currentEstimatedPosOnServer, charGenerics.currentEstimatedPosOnServerSortingLayer);
		childs.Add (child);
		
		// LastRecvedPos
		child = new ChildData (Tags.name_lastReceivedPos, Tags.tag_lastReceivedPos, Layer.defaultLayerName, centerTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.charIdleSprites[0], charGenerics.color_LastRecvedPos, charGenerics.lastRecvdPosRendererSortingLayer);
		childs.Add (child);
		
		// PredictedPosSimulatedWithLastInput
		child = new ChildData (Tags.name_PredictedPosSimulatedWithLastInput, Tags.tag_PredictedPosSimulatedWithLastInput, Layer.defaultLayerName, centerTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.charIdleSprites[0], charGenerics.color_PredictedPosSimulatedWithLastInput, charGenerics.preSimPosRendererSortingLayer);
		childs.Add (child);
		
		// PredictedPosCalculatedWithLastInput
		child = new ChildData (Tags.name_PredictedPosCalculatedWithLastInput, Tags.tag_PredictedPosCalculatedWithLastInput, Layer.defaultLayerName, centerTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, characterSO.charIdleSprites[0], charGenerics.color_PredictedPosCalculatedWithLastInput, charGenerics.preCalclastRecvdPosRendererSortingLayer);
		childs.Add (child);
		
		// IceWalled
		child = new ChildData (Tags.name_iceWalled, Tags.tag_iceWalled, Layer.defaultLayerName, centerTransformPos);
		child.Add(child.gameObject.AddComponent<SpriteRenderer>(), true, null, charGenerics.color_iceWallRenderer, charGenerics.iceWalledRendererSortingLayer);
		child.Add(child.gameObject.AddComponent<Animator>(), true, charGenerics.iceWandAnimatorController);
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

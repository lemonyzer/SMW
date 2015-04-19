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
	#endregion

	#region Main Methods

	[MenuItem("SMW/Tileset Create")]
	public static void Create()
	{
		Tileset newTilesetAsset = ScriptableObject.CreateInstance<Tileset>();

		AssetDatabase.CreateAsset(newTilesetAsset, "Assets/Maps/newCTiletsetSO.asset");
		AssetDatabase.SaveAssets();
		
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = newTilesetAsset;
	}

	[MenuItem("SMW/Tileset Window")]
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

	int w_TilesetPixelToUnit = 32;
	int w_SubTilePosX = 0;
	int w_SubTilePosY = 0;

	Sprite w_subTile = null;

	void OnGUI()
	{
		//EditorGUILayout.Space(10);
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		currentTileset = EditorGUILayout.ObjectField("Tileset ", currentTileset, typeof(Tileset) ,false) as Tileset;
		GUILayout.Label("SMW Tileset Properties", EditorStyles.boldLabel);

		if (GUILayout.Button("Create Tileset SO", GUILayout.ExpandWidth(false)))
		{
			Create();
		}

		w_TilesetPixelToUnit = EditorGUILayout.IntField("Tileset Pixel to Unit", w_TilesetPixelToUnit);

		w_SubTilePosX = EditorGUILayout.IntField("iCol x", w_SubTilePosX);
		w_SubTilePosY = EditorGUILayout.IntField("iRow y", w_SubTilePosY);

		if (GUILayout.Button("Show Tile (SubSprite)", GUILayout.ExpandWidth(false)))
		{
			CreateGameObjectWithSubSprite(w_SubTilePosX, w_SubTilePosY);
		}
		if (GUILayout.Button("Show Tile (NewSprite)", GUILayout.ExpandWidth(false)))
		{
			CreateGameObjectWithNewSprite(w_SubTilePosX, w_SubTilePosY);
		}

		GUILayout.EndVertical();
		GUILayout.BeginHorizontal();


		Repaint();
	}

	void CreateGameObjectWithSubSprite(int x, int y)
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
	#endregion

	string EP_LastWorkingMapImportPath = "EP_LastWorkingMapImportPath";
	string m_LastWorkingMapImportPath = "";
	string m_LastMapPath = "";
	bool m_FileOpened = false;

	bool OnGUI_OpenFile(out string absPath)
	{
		// open folder dialog
		absPath = EditorUtility.OpenFilePanel ("Select SMW Map", m_LastWorkingMapImportPath, "map");
		if(!string.IsNullOrEmpty(absPath))
		{
			m_LastWorkingMapImportPath = absPath;
			//absolutenPath in EditorPrefs speichern 
			EditorPrefs.SetString(EP_LastWorkingMapImportPath, m_LastWorkingMapImportPath);

			return true;
		}
		else
		{
			return false;
			
		}
	}

}

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

	[MenuItem("SMW/Tileset Create Empty")]
	public static Tileset CreateEmpty()
	{
		Tileset newTilesetAsset = ScriptableObject.CreateInstance<Tileset>();

		AssetDatabase.CreateAsset(newTilesetAsset, "Assets/tileset_NewEmptySO.asset");
		AssetDatabase.SaveAssets();
		
		//		EditorUtility.FocusProjectWindow();			// <-- Satck Overflow
		Selection.activeObject = newTilesetAsset;
		return newTilesetAsset;
	}

	public Tileset Create(Sprite tilesetSprite, Vector2 tilePivot)
	{
		Tileset newTilesetAsset = ScriptableObject.CreateInstance<Tileset>();
		newTilesetAsset.tilesetName = tilesetSprite.name;
		newTilesetAsset.TilesetSprite = tilesetSprite;
		newTilesetAsset.tilePivot = tilePivot;
		AssetDatabase.CreateAsset(newTilesetAsset, "Assets/Maps/tileset_" + tilesetSprite.name + ".asset"); //TODO dateiname nur gültige zeichen
		AssetDatabase.SaveAssets();
		
		//		EditorUtility.FocusProjectWindow();		// <-- Satck Overflow
		Selection.activeObject = newTilesetAsset;
		return newTilesetAsset;
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

//	Sprite w_subTile = null;

	Sprite w_TilesetSprite = null;
	Vector2 w_TilePivot = Vector2.zero;

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
						w_TilesetSprite = EditorGUILayout.ObjectField("Sprite", w_TilesetSprite, typeof(Sprite), false,  GUILayout.ExpandWidth(true)) as Sprite;
						w_TilePivot = EditorGUILayout.Vector2Field("Tile Pivot", w_TilePivot);
						if(w_TilesetSprite != null)
						{
							GUI.enabled = true;
							GUILayout.Label("Spritename = " + w_TilesetSprite.name);
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
						if (GUILayout.Button("Create Tileset from Sprite", GUILayout.ExpandWidth(false)))
						{
							currentTileset = Create(w_TilesetSprite, w_TilePivot);
//							EditorUtility.FocusProjectWindow();							<-- Satck Overflow
//							Selection.activeObject = currentTileset;
						}
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

				w_TilesetPixelToUnit = EditorGUILayout.IntField("Tileset Pixel to Unit", w_TilesetPixelToUnit);

				w_SubTilePosX = EditorGUILayout.IntField("iCol x", w_SubTilePosX);
				w_SubTilePosY = EditorGUILayout.IntField("iRow y", w_SubTilePosY);

//				if (GUILayout.Button("Show Tile (SubSprite)", GUILayout.ExpandWidth(false)))
//				{
//					CreateGameObjectWithSubSprite(w_SubTilePosX, w_SubTilePosY);
//				}
				if (GUILayout.Button("Show Tile (NewSprite)", GUILayout.ExpandWidth(false)))
				{
					CreateGameObjectWithNewSprite(w_SubTilePosX, w_SubTilePosY);
				}
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
//		Repaint();
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

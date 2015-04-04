using UnityEngine;
using System.Collections;

using UnityEditor;
//using UnityEditor.TextureImporter;

using System;
using System.Collections.Generic;
//using System.Xml;

public class CharacterCreationHelper : EditorWindow {

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

	void OnGUI ()
	{
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

	[System.Serializable]
	public class BodyPartComponent
	{
		// liste die alle componenenten enthält die dem prefab hinzugefügt werden müssen
		public string name = "";
		public string tag = "";
		public string layer = "";
		public Vector3 position;
		public List<Component> components;

		public BodyPartComponent(string name, string tag, string layer)
		{
			this.name = name;
			this.tag = tag;
			this.layer = layer;
			this.position = new Vector3(0,0,0);
			this.components = new List<Component>();
		}
	}
        

	[System.Serializable]
	public class MyCharacter
	{
		// um Prefab zu erstellen muss einfach über Liste childs iteriert werden und die darin enthaltenen Körperteile
		public static List<BodyPartComponent> childs;

		//newBug
//		public static void SetupChilds()
//		{
//			childs = new List<BodyPartComponent>();
//			
//			Vector3 headPos = new Vector3(0f,0.3f,0f);
//			Vector3 feetPos = new Vector3(0f,-0.3f,0f);
//			Vector3 kingPos = new Vector3(0f,0.6f,0f);
//			
//			float leftPos = -20f;
//			float rightPos = 20f;
//			
//			Vector2 headBoxSize = new Vector2(0.7f,0.25f);
//			Vector2 offSetCenter = Vector2.zero;
//			Vector2 offSetLeft = new Vector2(leftPos,0f);
//			Vector2 offSetRight = new Vector2(rightPos,0f);
//			
//			
//			// Clone Left
//			BodyPartComponent child = new BodyPartComponent(Tags.cloneLeft, Tags.cloneLeft, Layer.playerLayerName);
//			child.position.x = leftPos;
//			child.components.Add(new SpriteRenderer());
//			child.components.Add(new CloneSpriteScript());
//			//add
//			childs.Add(child);
//			
//			
//			// Clone Right
//			child = new BodyPartComponent(Tags.cloneRight, Tags.cloneRight, Layer.playerLayerName);
//			child.position.x = rightPos;
//			child.components.Add(new SpriteRenderer());
//			child.components.Add(new CloneSpriteScript());
//			//add
//			childs.Add(child);
//			
//			
//			// Head
//			child = new BodyPartComponent(Tags.head, Tags.head, Layer.headLayerName);
//			child.position = headPos;
//			
//			//center
//			BoxCollider2D childComponent = new BoxCollider2D();
//			childComponent.isTrigger = true;
//			childComponent.size = headBoxSize;
//			childComponent.offset = Vector2.zero;
//			child.components.Add(childComponent);
//			
//			//left
//			childComponent = new BoxCollider2D();
//			childComponent.isTrigger = true;
//			childComponent.size = headBoxSize;
//			childComponent.offset = offSetLeft;
//			child.components.Add(childComponent);
//			
//			//right
//			childComponent = new BoxCollider2D();
//			childComponent.isTrigger = true;
//			childComponent.size = headBoxSize;
//			childComponent.offset = offSetRight;
//			child.components.Add(childComponent);
//			//add
//			childs.Add(child);
//			
//			//			GameObject c = new GameObject();
//			//			c.transform.position = feetPos;
//			
//			// Feet
//			child = new BodyPartComponent(Tags.feet, Tags.feet, Layer.feetLayerName);
//			child.position = feetPos;
//			
//			//center
//			childComponent = new BoxCollider2D();
//			childComponent.isTrigger = true;
//			childComponent.size = headBoxSize;
//			childComponent.offset = Vector2.zero;
//			child.components.Add(childComponent);
//			
//			//left
//			childComponent = new BoxCollider2D();
//			childComponent.isTrigger = true;
//			childComponent.size = headBoxSize;
//			childComponent.offset = offSetLeft;
//			child.components.Add(childComponent);
//			
//			//right
//			childComponent = new BoxCollider2D();
//			childComponent.isTrigger = true;
//			childComponent.size = headBoxSize;
//			childComponent.offset = offSetRight;
//			child.components.Add(childComponent);
//			//add
//			childs.Add(child);
//			
//			
//		}

		public static void SetupChilds(GameObject characterGO)
		{
			Vector3 headPos = new Vector3(0f,0.3f,0f);
			Vector3 feetPos = new Vector3(0f,-0.3f,0f);
			Vector3 kingPos = new Vector3(0f,0.6f,0f);

			float leftPos = -20f;
			float rightPos = 20f;
			Vector3 centerTransformPos = Vector3.zero;
			Vector3 leftTransformPos = new Vector3(leftPos,0f,0f);
			Vector3 rightTransformPos = new Vector3(rightPos,0f,0f);

			Vector2 headBoxSize = new Vector2(0.7f,0.25f);
			Vector2 feetBoxSize = new Vector2(0.7f,0.25f);
			Vector2 bodyBoxSize = new Vector2(0.7f,0.8f);
			Vector2 itemCollectorAreaBoxSize = new Vector2(0.7f,0.8f);
			Vector2 powerHitAreaBoxSize = new Vector2(0.7f,0.8f);
			Vector2 groundStopperBoxSize = new Vector2(0.7f,0.8f);
			
			Vector2 offSetCenter = Vector2.zero;
			Vector2 offSetLeft = new Vector2(leftPos,0f);
			Vector2 offSetRight = new Vector2(rightPos,0f);


			// Clone Left
			GameObject childGO = new GameObject(Tags.cloneLeft);
			childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
			childGO.transform.position = leftTransformPos;					// setze offSet Position
			childGO.tag = Tags.cloneLeft;									// setze tag
			childGO.layer = LayerMask.NameToLayer(Layer.playerLayerName);	// setze layer
			// Componenten
			SpriteRenderer renderer = childGO.AddComponent<SpriteRenderer>();
			CloneSpriteScript cloneScript = childGO.AddComponent<CloneSpriteScript>();


			// Clone Right
			childGO = new GameObject(Tags.cloneRight);
			childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
			childGO.transform.position = leftTransformPos;					// setze offSet Position
			childGO.tag = Tags.cloneRight;									// setze tag
			childGO.layer = LayerMask.NameToLayer(Layer.playerLayerName);	// setze layer
			// Componenten
			renderer = childGO.AddComponent<SpriteRenderer>();
			cloneScript = childGO.AddComponent<CloneSpriteScript>();

			// Head
			childGO = new GameObject(Tags.head);
			childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
			childGO.transform.position = headPos;					// setze offSet Position
			childGO.tag = Tags.head;									// setze tag
			childGO.layer = LayerMask.NameToLayer(Layer.headLayerName);	// setze layer
			// Componenten
			//center
			BoxCollider2D box = childGO.AddComponent<BoxCollider2D>();
			box.isTrigger = true;
			box.size = headBoxSize;
			box.offset = Vector2.zero;
			//left
			box = childGO.AddComponent<BoxCollider2D>();
			box.isTrigger = true;
			box.size = headBoxSize;
			box.offset = offSetLeft;
			//right
			box = childGO.AddComponent<BoxCollider2D>();
			box.isTrigger = true;
			box.size = headBoxSize;
			box.offset = offSetRight;


			// Feet
			childGO = new GameObject(Tags.feet);
			childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
			childGO.transform.position = feetPos;					// setze offSet Position
			childGO.tag = Tags.feet;									// setze tag
			childGO.layer = LayerMask.NameToLayer(Layer.feetLayerName);	// setze layer
			// Componenten
			//center
			box = childGO.AddComponent<BoxCollider2D>();
			box.isTrigger = true;
			box.size = feetBoxSize;
			box.offset = Vector2.zero;
			//left
			box = childGO.AddComponent<BoxCollider2D>();
			box.isTrigger = true;
			box.size = feetBoxSize;
			box.offset = offSetLeft;
			//right
			box = childGO.AddComponent<BoxCollider2D>();
			box.isTrigger = true;
			box.size = feetBoxSize;
			box.offset = offSetRight; 
        }
        
//        public static void BuildCharacter(GameObject characterGO)
//		{
//			SetupChilds();
//			Transform parentTransform = characterGO.transform;
//
//			foreach(BodyPartComponent child in childs)
//			{
//				GameObject childGO = new GameObject(child.name);
//
//				// verbinde childGO mit CharacterGO
//				childGO.transform.SetParent(parentTransform);
//
//				// setze tag
//				childGO.tag = child.tag;
//
//				// setze layer
//				Debug.Log(child.layer + " = " + LayerMask.NameToLayer(child.layer));
//				childGO.layer = LayerMask.NameToLayer(child.layer);
//
//				// füge vorbereitete componenten hinzu
//				foreach(Component component in child.components)
//				{
//					Debug.Log("aktuelle Componente ist vom Typ " + component.GetType()); 
//					childGO.AddComponent(component.GetType());
//				}
//
//				//
//			}
//
//		}
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
		MyCharacter.SetupChilds(tempObj);


		// save GO in prefab
		PrefabUtility.ReplacePrefab(tempObj, emptyObj, ReplacePrefabOptions.ConnectToPrefab);

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

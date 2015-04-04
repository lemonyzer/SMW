using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//	[System.Serializable]
//	public class BodyPartComponent
//	{
//		// liste die alle componenenten enthält die dem prefab hinzugefügt werden müssen
//		public string name = "";
//		public string tag = "";
//		public string layer = "";
//		public Vector3 position;
//		public List<Component> components;
//
//		public BodyPartComponent(string name, string tag, string layer)
//		{
//			this.name = name;
//			this.tag = tag;
//			this.layer = layer;
//			this.position = new Vector3(0,0,0);
//			this.components = new List<Component>();
//		}
//	}


[System.Serializable]
public class MyCharacterSHIT
{
	// um Prefab zu erstellen muss einfach über Liste childs iteriert werden und die darin enthaltenen Körperteile
	//		public static List<BodyPartComponent> childs;
	
	//EDIT //TODO//TODO//TODO//TODO//TODO//TODO//TODO
	//		// Open tag manager
	//		SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
	//		SerializedProperty tagsProp = tagManager.FindProperty("tags");
	//		
	//		// For Unity 5 we need this too
	//		SerializedProperty layersProp = tagManager.FindProperty("layers");
	//		
	//		// Adding a Tag
	//		string s = "the_tag_i_want_to_add";
	//		
	//		// First check if it is not already present
	//		bool found = false;
	//		for (int i = 0; i < tagsProp.arraySize; i++)
	//		{
	//			SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
	//			if (t.stringValue.Equals(s)) { found = true; break; }
	//		}
	//		
	//		// if not found, add it
	//		if (!found)
	//		{
	//			tagsProp.InsertArrayElementAtIndex(0);
	//			SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
	//			n.stringValue = s;
	//		}
	//		
	//		// Setting a Layer (Let's set Layer 10)
	//		string layerName = "the_name_want_to_give_it";
	//		
	//		// --- Unity 4 ---
	//		SerializedProperty sp = tagManager.FindProperty("User Layer 10");
	//		if (sp != null) sp.stringValue = layerName;
	//		
	//		// --- Unity 5 ---
	//		SerializedProperty sp = layersProp.GetArrayElementAtIndex(10);
	//		if (sp != null) sp.stringValue = layerName;
	//		
	//		// and to save the changes
	//		tagManager.ApplyModifiedProperties();
	
	
	public static void SetupChilds(GameObject characterGO)
	{
		
		
		Vector3 headPos = 			new Vector3(0f,0.3f,0f);
		Vector3 feetPos = 			new Vector3(0f,-0.3f,0f);
		Vector3 bodyPos = 			new Vector3(0f,0f,0f);
		Vector3 itemCollectorPos = 	new Vector3(0f,0f,0f);
		Vector3 powerHitPos = 		new Vector3(0f,0f,0f);
		Vector3 groundStopperPos = 	new Vector3(0f,0f,0f);
		Vector3 kingPos = 			new Vector3(0f,0.6f,0f);
		
		float leftPos = -20f;
		float rightPos = 20f;
		Vector3 centerTransformPos = Vector3.zero;
		Vector3 leftTransformPos = new Vector3(leftPos,0f,0f);
		Vector3 rightTransformPos = new Vector3(rightPos,0f,0f);
		
		Vector2 headBoxSize = new Vector2(0.7f,0.25f);
		Vector2 feetBoxSize = new Vector2(0.7f,0.25f);
		Vector2 bodyBoxSize = new Vector2(0.7f,0.8f);
		Vector2 itemCollectorBoxSize = new Vector2(0.7f,0.8f);
		Vector2 powerHitBoxSize = new Vector2(0.7f,0.8f);
		Vector2 groundStopperBoxSize = new Vector2(0.7f,0.5f);
		
		Vector2 groundStopperBoxOffset = new Vector2(0.0f,-0.25f);
		
		Vector2 offSetCenter = Vector2.zero;
		Vector2 offSetLeft = new Vector2(leftPos,0f);
		Vector2 offSetRight = new Vector2(rightPos,0f);
		
		// root
		SpriteRenderer renderer = characterGO.AddComponent<SpriteRenderer>();	//TODO	layer, sprite
		//renderer.sprite = 
		//renderer.sortingLayerID = 
		//renderer.sortingLayerName = 
		//renderer.sortingOrder = 
		Rigidbody2D rb2d = characterGO.AddComponent<Rigidbody2D>();	//TODO  gravityscale = 0, fixedAngle
		rb2d.gravityScale = 0.0f;
		rb2d.fixedAngle = true;
		Animator anim = characterGO.AddComponent<Animator>();		//TODO	animatorController, rootMotion=false
		//anim.runtimeAnimatorController = 
		anim.applyRootMotion = false;
		characterGO.AddComponent<PlatformUserControl>();
		characterGO.AddComponent<PlatformCharacter>();
		characterGO.AddComponent<PlatformJumperV2>();
		Bot bot = characterGO.AddComponent<Bot>();
		bot.enabled = false;
		characterGO.AddComponent<Rage>();
		characterGO.AddComponent<Shoot>();
		characterGO.AddComponent<Shield>();
		NetworkedPlayer netPlayer = characterGO.AddComponent<NetworkedPlayer>();
		AudioSource audioSource = characterGO.AddComponent<AudioSource>();	//TODO (loop off, onawake off)
		audioSource.playOnAwake = false;
		audioSource.loop = false;
		NetworkView networkView = characterGO.AddComponent<NetworkView>();	//TODO
		networkView.stateSynchronization = NetworkStateSynchronization.Unreliable;
		networkView.observed = netPlayer;
		
		
		// Clone Left
		GameObject childGO = new GameObject(Tags.name_cloneLeft);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = leftTransformPos;					// setze offSet Position
		childGO.tag = Tags.tag_cloneLeft;									// setze tag
		childGO.layer = LayerMask.NameToLayer(Layer.playerLayerName);	// setze layer
		// Componenten
		renderer = childGO.AddComponent<SpriteRenderer>();
		CloneSpriteScript cloneScript = childGO.AddComponent<CloneSpriteScript>();
		//renderer.sprite = //TODO sprite for king 
		//renderer.sortingLayerName = //TODO kingRendererSortLayer 
		
		// Clone Right
		childGO = new GameObject(Tags.name_cloneRight);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = rightTransformPos;					// setze offSet Position
		childGO.tag = Tags.tag_cloneRight;									// setze tag
		childGO.layer = LayerMask.NameToLayer(Layer.playerLayerName);	// setze layer
		// Componenten
		renderer = childGO.AddComponent<SpriteRenderer>();
		cloneScript = childGO.AddComponent<CloneSpriteScript>();
		//renderer.sprite = //TODO sprite for king 
		//renderer.sortingLayerName = //TODO kingRendererSortLayer 
		
		// Head
		childGO = new GameObject(Tags.name_head);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = headPos;					// setze offSet Position
		childGO.tag = Tags.tag_head;									// setze tag
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
		childGO = new GameObject(Tags.name_feet);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = feetPos;					// setze offSet Position
		childGO.tag = Tags.tag_feet;									// setze tag
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
		SendDamageTrigger feetScript = childGO.AddComponent<SendDamageTrigger>();
		
		
		// Body
		childGO = new GameObject(Tags.name_body);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = bodyPos;					// setze offSet Position
		childGO.tag = Tags.tag_body;									// setze tag
		childGO.layer = LayerMask.NameToLayer(Layer.bodyLayerName);	// setze layer
		// Componenten
		//center
		box = childGO.AddComponent<BoxCollider2D>();
		box.isTrigger = false;
		box.size = bodyBoxSize;
		box.offset = Vector2.zero;
		//left
		box = childGO.AddComponent<BoxCollider2D>();
		box.enabled = false;	//TODO off
		box.isTrigger = false;
		box.size = bodyBoxSize;
		box.offset = offSetLeft;
		//right
		box = childGO.AddComponent<BoxCollider2D>();
		box.enabled = false;	//TODO off
		box.isTrigger = false;
		box.size = bodyBoxSize;
		box.offset = offSetRight; 
		
		
		// ItemCollector
		childGO = new GameObject(Tags.name_itemCollector);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = itemCollectorPos;					// setze offSet Position
		childGO.tag = Tags.tag_itemCollector;									// setze tag
		childGO.layer = LayerMask.NameToLayer(Layer.itemLayerName);	// setze layer
		// Componenten
		//center
		box = childGO.AddComponent<BoxCollider2D>();
		box.isTrigger = true;
		box.size = itemCollectorBoxSize;
		box.offset = Vector2.zero;
		//left
		box = childGO.AddComponent<BoxCollider2D>();
		box.isTrigger = true;
		box.size = itemCollectorBoxSize;
		box.offset = offSetLeft;
		//right
		box = childGO.AddComponent<BoxCollider2D>();
		box.isTrigger = true;
		box.size = itemCollectorBoxSize;
		box.offset = offSetRight; 
		ItemCollectorScript collectorScript = childGO.AddComponent<ItemCollectorScript>();
		
		
		// PowerHitArea
		childGO = new GameObject(Tags.name_powerUpHitArea);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = powerHitPos;					// setze offSet Position
		childGO.tag = Tags.tag_powerUpHitArea;									// setze tag
		childGO.layer = LayerMask.NameToLayer(Layer.powerUpLayerName);	// setze layer
		// Componenten
		//center
		box = childGO.AddComponent<BoxCollider2D>();
		box.isTrigger = true;
		box.size = powerHitBoxSize;
		box.offset = Vector2.zero;
		//left
		box = childGO.AddComponent<BoxCollider2D>();
		box.isTrigger = true;
		box.size = powerHitBoxSize;
		box.offset = offSetLeft;
		//right
		box = childGO.AddComponent<BoxCollider2D>();
		box.isTrigger = true;
		box.size = powerHitBoxSize;
		box.offset = offSetRight; 
		RageTrigger powerHitAreaScript = childGO.AddComponent<RageTrigger>();
		
		
		// GroundStopper
		childGO = new GameObject(Tags.name_groundStopper);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = groundStopperPos;					// setze offSet Position
		childGO.tag = Tags.tag_groundStopper;									// setze tag
		childGO.layer = LayerMask.NameToLayer(Layer.groundStopperLayerName);	// setze layer
		// Componenten
		//center
		box = childGO.AddComponent<BoxCollider2D>();
		box.isTrigger = true;
		box.size = groundStopperBoxSize;
		box.offset = groundStopperBoxOffset;
		//			//left
		//			box = childGO.AddComponent<BoxCollider2D>();
		//			box.isTrigger = true;
		//			box.size = powerHitBoxSize;
		//			box.offset = offSetLeft;
		//			//right
		//			box = childGO.AddComponent<BoxCollider2D>();
		//			box.isTrigger = true;
		//			box.size = powerHitBoxSize;
		//			box.offset = offSetRight; 
		
		// King
		childGO = new GameObject(Tags.name_king);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = kingPos;					// setze offSet Position
		//			childGO.tag = ;									// setze tag
		childGO.layer = LayerMask.NameToLayer(Layer.defaultLayerName);	// setze layer
		// Componenten
		//center
		renderer = childGO.AddComponent<SpriteRenderer>();
		//renderer.sprite = //TODO sprite for king 
		//renderer.sortingLayerName = //TODO kingRendererSortLayer 
		
		
		// CurrentEstimatedPosOnServer
		childGO = new GameObject(Tags.name_CurrentEstimatedPosOnServer);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = Vector3.zero;					// setze offSet Position
		//			childGO.tag = ;									// setze tag
		childGO.layer = LayerMask.NameToLayer(Layer.defaultLayerName);	// setze layer
		// Componenten
		//center
		renderer = childGO.AddComponent<SpriteRenderer>();
		//renderer.color = //TODO color and transparenz
		//renderer.sprite = //TODO sprite 
		//renderer.sortingLayerName = //TODO kingRendererSortLayer
		
		// LastRecvedPos
		childGO = new GameObject(Tags.name_lastReceivedPos);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = Vector3.zero;					// setze offSet Position
		//			childGO.tag = ;									// setze tag
		childGO.layer = LayerMask.NameToLayer(Layer.defaultLayerName);	// setze layer
		// Componenten
		//center
		renderer = childGO.AddComponent<SpriteRenderer>();
		//renderer.color = //TODO color and transparenz
		//renderer.sprite = //TODO sprite 
		//renderer.sortingLayerName = //TODO kingRendererSortLayer
		
		//PredictedPosSimulatedWithLastInput
		childGO = new GameObject(Tags.name_PredictedPosSimulatedWithLastInput);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = Vector3.zero;					// setze offSet Position
		//			childGO.tag = ;									// setze tag
		childGO.layer = LayerMask.NameToLayer(Layer.defaultLayerName);	// setze layer
		// Componenten
		//center
		renderer = childGO.AddComponent<SpriteRenderer>();
		//renderer.color = //TODO color and transparenz
		//renderer.sprite = //TODO sprite 
		//renderer.sortingLayerName = //TODO kingRendererSortLayer
		
		//PredictedPosCalculatedWithLastInput
		childGO = new GameObject(Tags.name_PredictedPosCalculatedWithLastInput);
		childGO.transform.SetParent(characterGO.transform);				// setze Verbindung zum characterGO
		childGO.transform.position = Vector3.zero;					// setze offSet Position
		//			childGO.tag = ;									// setze tag
		childGO.layer = LayerMask.NameToLayer(Layer.defaultLayerName);	// setze layer
		// Componenten
		//center
		renderer = childGO.AddComponent<SpriteRenderer>();
		//renderer.color = //TODO color and transparenz
		//renderer.sprite = //TODO sprite 
		//renderer.sortingLayerName = //TODO kingRendererSortLayer
	}
	
	
	//		public class CustomChild
	//		{
	//			public GameObject gameObject;
	//			//child
	//			public string name;
	//			public string tag;
	//			public string layerName;
	//			public Vector3 position;
	//			
	//			//child components
	//			public List<CustomComponent> customComponents;
	//			
	//			public CustomChild(string name, string tag, string layerName, Vector3 position)
	//			{
	//				gameObject = new GameObject(name);
	//				this.name = name;
	//				this.tag = tag;
	//				this.layerName = layerName;
	//				this.position = position;
	//				
	//				customComponents = new List<CustomComponent>();
	//			}
	//		}
	
	//		public class CustomComponent : Component
	//		{
	//			public UnityEngine.Object genericComponent;
	//			public string realComponent;
	//			
	//			//			//smart
	//			//			public bool smartCloneEnabled;
	//			//			public int smartCloneCount;
	//			
	//			//all
	//			public bool enabled;
	//			
	//			//Boxcollider2D
	//			public Vector2 size;
	//			public Vector2 offset;
	//			//			public Vector2 size;
	//			public bool isTrigger = false;
	//			
	//			//SpriteRenderer
	//			public Sprite sprite;
	//			public Color color; 
	//			public int sortingLayer; 
	//			
	//			public CustomComponent(string realComponent, bool enabled, Vector2 size, Vector2 offset, bool isTrigger)
	//			{
	//				if (realComponent == "BoxCollider2D")
	//				{
	//					this.realComponent = realComponent;
	//					this.enabled = enabled;
	//					this.size = size;
	//					this.offset = offset;
	//					this.isTrigger = isTrigger;
	//				}
	//			}
	//			
	//			public CustomComponent(string realComponent, bool enabled, Sprite sprite, Color color)
	//			{
	//				if (realComponent == "SpriteRenderer")
	//				{
	//					this.enabled = enabled;
	//					this.sprite = sprite;
	//					this.color = color;
	//				}
	//			}
	//		}
	
	
	public static Sprite kingSprite;
	public static Sprite iceWallSprite;
	public static AnimatorController iceWallAnimatorController;
	
	public class ComponentData
	{
		public System.Type componentType;
		public UnityEngine.Object genericComponent;
		
		//smart
		public int smartCloneCount;
		
		//all
		public bool enabled;
		
		//Boxcollider2D
		public Vector2 size;
		public Vector2[] smartOffset;
		public bool isTrigger = false;
		
		//SpriteRenderer
		public Sprite sprite;
		public Color color; 
		public int sortingLayer; 
		
		//Animator
		public AnimatorController animatorController;
		
		
		/**
			 * 	
			 **/
		public ComponentData(System.Type componentType, bool enabled, Vector2 size, Vector2[] smartOffset, bool isTrigger, int smartCloneCount)
		{
			if (componentType == typeof(BoxCollider2D))
			{
				//					this.realComponent = realComponent;
				this.enabled = enabled;
				this.size = size;
				this.smartOffset = smartOffset;
				this.isTrigger = isTrigger;
				
				this.smartCloneCount = smartCloneCount;
			}
		}
		
		public ComponentData(System.Type componentType, bool enabled, Sprite sprite, Color color)
		{
			if (componentType == typeof(SpriteRenderer))
			{
				this.componentType = componentType;
				this.enabled = enabled;
				this.sprite = sprite;
				this.color = color;
			}
		}
		
		public ComponentData(System.Type componentType, bool enabled, AnimatorController animatorController)
		{
			if(componentType == typeof(Animator))
			{
				this.componentType = componentType;
				this.enabled = enabled;
				this.animatorController = animatorController;
			}
		}
		
		public ComponentData(System.Type componentType, bool enabled)
		{
			this.componentType = componentType;
			this.enabled = enabled;
		}
	}
	
	public class ChildData
	{
		//			public GameObject gameObject;		//TODO weitere möglichkeit! gameObject muss kein parent gesetzt werden, nur vorbereiteter boxcollider/spriterenderer wird benötigt
		//child
		public string name;
		public string tag;
		public string layerName;
		public Vector3 position;
		
		//child components
		public List<ComponentData> components;
		
		public ChildData(string name, string tag, string layerName, Vector3 position)
		{
			//				gameObject = new GameObject(name);
			this.name = name;
			this.tag = tag;
			this.layerName = layerName;
			this.position = position;
			
			components = new List<ComponentData>();
		}
		
		/**
			 * 	generic?
			 **/
		public void Add(System.Type componentType, bool enabled, Vector2 size, Vector2[] smartOffset, bool isTrigger, int smartCloneCount)
		{
			components.Add (new ComponentData(componentType,enabled,size,smartOffset,isTrigger, smartCloneCount));
		}
		
		public void Add(System.Type componentType, bool enabled, Sprite sprite, Color color)
		{
			components.Add (new ComponentData(componentType,enabled,sprite,color));
		}
		
		public void Add(System.Type componentType, bool enabled, AnimatorController animatorController)
		{
			components.Add (new ComponentData(componentType, enabled, animatorController));
		}
		
		public void Add(System.Type componentType, bool enabled)
		{
			components.Add (new ComponentData(componentType,enabled));
		}
	}
	
	public void fillData(GameObject characterGO, SmwCharacter smwCharacter)
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
		
		Color color_currentEstimatedPosOnServer 		= new Color(0f,0f,0f,0.25f);	// localplayer Character's	only
		Color color_interpolatedPos						= new Color(1f,1f,1f,1f);		// all other Character's ROOT SpriteRenderer		//TODO
		Color color_LastRecvedPos 						= new Color(1f,1f,1f,0.25f);	// all other Character's	vergangene Position
		Color color_PredictedPosSimulatedWithLastInput 	= new Color(1f,1f,1f,0.25f);	// all other Character's	vergangene Position
		Color color_PredictedPosCalculatedWithLastInput = new Color(1f,1f,1f,0.25f);	// all other Character's	vergangene Position
		
		
		
		//			Vector2[] groundStopperBoxOffset = new Vector2[1] { new Vector2(0.0f,-0.25f) };	// wie alle anderen Collider auch wurde jetzt TransformPos verschoben
		
		
		List<ChildData> childs = new List<ChildData> ();
		
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
		childs.Add (child);
		
		// PowerHitArea (cloned)
		child = new ChildData (Tags.name_powerUpHitArea, Tags.tag_powerUpHitArea, Layer.powerUpLayerName, powerHitTransformPos);
		child.Add(typeof(BoxCollider2D), true, powerHitBoxSize, smartComponentOffset, powerHitAreaIsTrigger, 3);
		childs.Add (child);
		
		// GroundStopper
		child = new ChildData (Tags.name_groundStopper, Tags.tag_groundStopper, Layer.groundStopperLayerName, groundStopperTransformPos);
		child.Add(typeof(BoxCollider2D), true, groundStopperBoxSize, smartComponentOffset, groundStopperIsTrigger, 1);
		childs.Add (child);
		
		// King
		child = new ChildData (Tags.name_body, Tags.tag_body, Layer.defaultLayerName, kingTransformPos);
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
		child.Add(typeof(SpriteRenderer), true, iceWallSprite, Color.white);
		child.Add(typeof(Animator), true, iceWallAnimatorController);
		childs.Add (child);
	}
	
	
	public static void SmartCreate(GameObject characterGO)
	{
		List<ChildData> childs = new List<ChildData> ();
		
		foreach(ChildData child in childs)
		{
			GameObject childGO = new GameObject(child.name);
			//set as childGO
			childGO.transform.SetParent(characterGO.transform);
			//set childs offset Position
			childGO.transform.position = child.position;
			
			foreach(ComponentData cc in child.components)
			{
				for(int i=0; i < cc.smartCloneCount; i++)
				{
					//cc.genericComponent.GetType() test = characterGO.AddComponent(cc.genericComponent.GetType());
					//							Behaviour currentComponent;
					// sinnlos, Component hat keine zusammenfassenden eigenschaften die ich bearbeiten muss 
					//currentComponent.enabled <--- //TODO gibts nicht
					
					// Dynamisch Objekte unterschiedlicher Typen erzeugen	cc.componentType (System.Type variable soll classe angeben -> geht nicht)
					//cc.componentType component;
					
					if(cc.componentType == typeof(BoxCollider2D))
					{
						// geht bringt aber nichts
						//								currentComponent = childGO.AddComponent<BoxCollider2D>();
						//								((BoxCollider2D)(currentComponent)).isTrigger = cc.isTrigger;
						//								((BoxCollider2D)(currentComponent)).size = cc.size;
						//								((BoxCollider2D)(currentComponent)).offset = cc.offset;
						//								((BoxCollider2D)(currentComponent)).enabled = cc.enabled;
					}
					else if(cc.componentType == typeof(SpriteRenderer))
					{
						// geht nicht
						//								currentComponent = childGO.AddComponent<SpriteRenderer>();
						//								((SpriteRenderer)(currentComponent)).enabled = cc.enabled;
						//								((SpriteRenderer)(currentComponent)).sprite = cc.sprite;
						//								((SpriteRenderer)(currentComponent)).sortingLayerID = cc.sortingLayer;
						//renderer.enabled = cc.enabled;
					}
					
					if(cc.componentType == typeof(BoxCollider2D))
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
						SpriteRenderer renderer = childGO.AddComponent<SpriteRenderer>();
						//all
						renderer.enabled = cc.enabled;
					}
				}
			}
		}
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
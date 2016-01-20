using UnityEngine;
using System.Collections;

/**
 *  Ein GameObject (Tag: UserControls) mit diesem UserControlScript in JEDER Scene
 *  
 *  Vorteil:
 *  Mehrere GameObjects benötigen nur ein AnalogStick  
 *  BackButton funktioniert überall und muss nur einmal geschrieben werden
 *  GUI elemente müssen nicht destroyed werden, nur deaktiviert wenn Character wechsel/entfernt  
 *   
 *  Nachteil:
 *  FindGameObjectWithTag aufruf in Awake()    
 *  Brotgrümel erzeugen
 *  oder Scenenamen abhängige funktion 
 *
 **/    

public class PlatformUserControl : MonoBehaviour {
	
	// check who is the owner of the current character
	// allow input if local photonnetwork.player == realOwner.owner
	private RealOwner realOwner;
	
	/** 
	 * Combined Input
	 **/

	[SerializeField]
	bool useThreeStateMovement = true;
	float threeStateMovementSchwellwert = 0.5f;		// schwellwert
//	float threeStateMovementHystereseCenter = 0f;		// center of hysterese

	[SerializeField]
	private float inputHorizontal = 0f;

	public float GetInputHorizontal()
	{
		if(useThreeStateMovement)
		{
			if(inputLeft)
				return -1.0f;
			else if(inputRight)
				return 1.0f;
			else
				return 0f;
		}
		else
		{
			return inputHorizontal;
		}
	}

	public void SetInputHorizontal(bool left, bool right)
	{
		if(left)
		{
			inputHorizontal = -1f;
			inputLeft = true;
			inputRight = false;
		}
		else if(right)
		{
			inputHorizontal = 1f;
			inputLeft = false;
			inputRight = true;
		}
		else
		{
			inputHorizontal = 0f;
			inputLeft = false;
			inputRight = false;
		}
	}

	public void SetInputHorizontal(float value)
	{
		inputHorizontal = value;
		// limit combination to [-1,1]
		Mathf.Clamp(inputHorizontal, -1, +1);    // kein cheaten möglich mit touch+keyboard steuerung

		if( inputHorizontal < -threeStateMovementSchwellwert)
		{
			if(useThreeStateMovement)
				inputHorizontal = -1f;
			inputLeft = true;
			inputRight = false;
		}
		else if( inputHorizontal > threeStateMovementSchwellwert)
		{
			if(useThreeStateMovement)
				inputHorizontal = 1f;
			inputLeft = false;
			inputRight = true;
		}
		else
		{
			if(useThreeStateMovement)
				inputHorizontal = 0f;
			inputLeft = false;
			inputRight = false;
		}
	}

//	public void SetInputHorizontal(float value)
//	{
//		if(threeStateMovement)
//		{
//			if(inputHorizontal < threeStateMovementHysterese)
//			{
//				inputHorizontal = -1f;
//				inputLeft = true;
//				inputRight = false;
//			}
//			else if(inputHorizontal > threeStateMovementHysterese)
//			{
//				inputHorizontal = 1f;
//				inputLeft = false;
//				inputRight = true;
//			}
//			else
//			{
//				inputHorizontal = 0f;
//				inputLeft = false;
//				inputRight = false;
//			}
//		}
//		else
//		{
//			inputHorizontal = value;
//			// limit combination to [-1,1]
//			Mathf.Clamp(inputHorizontal, -1, +1);    // kein cheaten möglich mit touch+keyboard steuerung
//		}
//	}

	public bool inputLeft;
	public bool inputRight;
	public bool inputUp;
	public bool inputDown;

	//	[System.NonSerialized]
	public float inputVertical = 0f;
	
	//	[System.NonSerialized]
	public bool inputJump = false;

	//	[System.NonSerialized]
	public bool inputPower = false;

	// jump needs single klick, or key pressing
	private bool keyPressed = false;
	
	/**
	* Input Touch
	**/    
	
	private float inputTouchHorizontal = 0f;
	private float inputTouchVertical = 0f;
	private bool inputTouchJump = false;
	
	/**
	 * Input Flags (Jump Button)
	 **/
	//	int buttonTouchID=-1;			// ID of current jump touch (right screen)
	int buttonATapCount=0;			// tap count current jump touch (right screen)
	bool buttonAIsPressed = false;	// flag if player presses jump 		
	bool buttonAIsTapped = false;	// flag if player presses jump again		

	/**
	 * Input Flags (Jump Button)
	 **/
	//	int buttonTouchID=-1;			// ID of current jump touch (right screen)
	int buttonBTapCount=0;			// tap count current jump touch (right screen)
	bool buttonBIsPressed = false;	// flag if player presses jump 		
	bool buttonBIsTapped = false;	// flag if player presses jump again		
	
	/**
	 * Input Flags (Analog Stick)
	 **/
	//	Touch analogStick;
	int analogStickTouchID=-1;
	bool analogStickTouchBegan = false;
	bool analogStickIsStillPressed = false;
	
	float touchBeganPositionX;
	float touchBeganPositionY;
	
	
	public GUITexture prefabAnalogStickTexture;
	public GUITexture prefabStickTexture;
	GUITexture analogStickTexture;
	GUITexture stickTexture;
	float analogStickTextureWidth;
	float analogStickTextureHeight;
	float stickTextureWidth;
	float stickTextureHeight;
	
	float textureSizeWithSaveZoneX;
	float textureSizeWithSaveZoneY;
	
	/**
	* Input Keyboard
	**/
	private bool useRawKeyboardInput = true;
//	private float inputKeyboardHorizontal = 0f;
	private float inputKeyboardHorizontalSmooth = 0f;
	private float inputKeyboardHorizontalRaw = 0f;
//	private float inputKeyboardVertical = 0f;
	private float inputKeyboardVerticalSmooth = 0f;
	private float inputKeyboardVerticalRaw = 0f;
	private bool inputKeyboardJump = false;                    
	private bool inputKeyboardPower = false;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		realOwner = GetComponent<RealOwner>();
	}

	void Start() {

		//TODO //TODO //TODO achtung jetzt darf es nicht mehr destroyed werden!

		//TODO aktuell 2 Find: ... versuchen auf ein Fin zu reduzieren (GO und Child GO)
		//TODO ohne Find: in Scene bereits instanziieren und direkt über inspector public zuweisen
		
		//TODO static analogStick ?? muss nur einmal für diese klasse festgelegt werden
		//TODO static stick ?? muss nur einmal für diese klasse festgelegt werden
		
		GameObject analogStickGO = GameObject.FindGameObjectWithTag(Tags.guiAnalogStick);
		if(analogStickGO == null)
		{
			analogStickTexture = (GUITexture) Instantiate(prefabAnalogStickTexture);		// needed? pre-instantiete in hierachie?!
			analogStickGO = analogStickTexture.gameObject;
		}
		else
		{
			analogStickTexture = analogStickGO.GetComponent<GUITexture>();
		}
		analogStickTextureWidth = analogStickTexture.pixelInset.width;
		analogStickTextureHeight = analogStickTexture.pixelInset.height;

		stickTexture = analogStickGO.transform.FindChild(Tags.guiStick).GetComponent<GUITexture>();
		
//		GameObject stickGO = GameObject.Find("Stick");
//		if(stickGO == null)
//		{
//			stickTexture = (GUITexture) Instantiate(prefabStickTexture);					// needed? pre-instantiete in hierachie?!
//		}
//		else
//		{
//			stickTexture = stickGO.GetComponent<GUITexture>();
//		}
		stickTextureWidth = stickTexture.pixelInset.width;
		stickTextureHeight = stickTexture.pixelInset.height;
		
		// Analog Stick ausblenden (aus sichtfeld verschieben)
		analogStickTexture.pixelInset = new Rect(0,
		                                         0,
		                                         0,
		                                         0);
		// Stick ausblenden (aus sichtfeld verschieben)
		stickTexture.pixelInset = new Rect(0,
		                                   0,
		                                   0,
		                                   0);
	}
	
//	// Use this for initialization
//	/// <summary>
//	/// Start this instance.
//	/// </summary>
//	void Start() {
//		
//		analogStickTexture = (GUITexture) Instantiate(prefabAnalogStickTexture);		// needed? pre-instantiete in hierachie?!
//		stickTexture = (GUITexture) Instantiate(prefabStickTexture);					// needed? pre-instantiete in hierachie?!
//		analogStickTextureWidth = analogStickTexture.pixelInset.width;
//		analogStickTextureHeight = analogStickTexture.pixelInset.height;
//		stickTextureWidth = stickTexture.pixelInset.width;
//		stickTextureHeight = stickTexture.pixelInset.height;
//		// Analog Stick ausblenden (aus sichtfeld verschieben)
//		analogStickTexture.pixelInset = new Rect(0,
//		                                         0,
//		                                         0,
//		                                         0);
//		stickTexture.pixelInset = new Rect(0,
//		                                   0,
//		                                   0,
//		                                   0);
//	}

	/// <summary>
	/// 
	/// </summary>
	void ApplicationPlatformInputCheck()
	{
		/**
		 * not on Mobile Devices (Android / IOs)
		 **/
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			Keyboard();
			Touch();
		}
		else if (Application.platform == RuntimePlatform.OSXEditor)
		{
			Keyboard();
			Touch();
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			Keyboard();
			Touch();			
		}
		else if (Application.platform == RuntimePlatform.WindowsWebPlayer)
		{
			Keyboard();	
			Touch();		
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			//Keyboard();   // nur bei angeschlossesner USB/Bluetooth Tastatur
			Touch();			
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Touch();			
		}
		else
		{
			Keyboard();
			Touch();
			Debug.LogWarning(this.name + ": disabled!!!");
		}
	}

	public bool simulate = false;

	// Update is called once per frame
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
		// Wenn jeder Character ein UserControl script hat muss abgefragt werden ob der Character dem lokalen Spieler gehört
		if(Network.peerType == NetworkPeerType.Disconnected || Network.player == realOwner.owner)
		{
			if(simulate)
				return;
//			Debug.LogWarning(this.ToString() +": is reading local controls input!!!");
			ApplicationPlatformInputCheck();		//
			CombineInput(); 						// kombiniert alle abgefragten Eingabemöglichkeiten (Keyboard, Touchpad, Mouse...)
			// dannach stehen die Eingabedaten in inputHorizontal und inputJump
		}
		else
		{
			// Update darf input Variablen nicht überschreiben, da NetworkedPlayer diese ebenfals verwendet
			this.enabled = false;
		}
	}

	/// <summary>
	/// Combines the input of Touch and Keyboard.
	/// </summary>
	void CombineInput()
	{
		if(keyPressed)
		{
			if(buttonAIsPressed || inputKeyboardJump)
			{
				inputJump = true;
			}
			else
			{
				inputJump = false;
			}

			if(buttonBIsPressed || inputKeyboardPower)
			{
				inputPower = true;
			}
			else
			{
				inputPower = false;
			}
		}
		else
		{
			if(buttonAIsTapped || inputKeyboardJump)
			{
				inputJump = true;
			}
			else
			{
				inputJump = false;
			}

			if(buttonBIsTapped || inputKeyboardPower)
			{
				inputPower = true;
			}
			else
			{
				inputPower = false;
			}
		}

//		// keyboard movement fix
//		if(inputKeyboardHorizontal < 0 &&
//		   inputKeyboardHorizontal > -1)
//		{
//			inputKeyboardHorizontal = 0f;
//		}
//		else if(inputKeyboardHorizontal < 1 &&
//		        inputKeyboardHorizontal > 0)
//		{
//			inputKeyboardHorizontal = 0f;
//		}

		// combine the horizontal input
		if(useRawKeyboardInput)
		{
			inputHorizontal = inputTouchHorizontal + inputKeyboardHorizontalRaw;
			
		}
		else
		{
			inputHorizontal = inputTouchHorizontal + inputKeyboardHorizontalSmooth;
		}


		SetInputHorizontal(inputHorizontal);
		

	}

	/// <summary>
	/// Keyboard this instance.
	/// </summary>
	void Keyboard() {
		inputKeyboardHorizontalSmooth = Input.GetAxis ("Horizontal");
		inputKeyboardHorizontalRaw = Input.GetAxisRaw ("Horizontal");
		inputKeyboardVerticalSmooth = Input.GetAxis ("Vertical");
		inputKeyboardVerticalRaw = Input.GetAxisRaw ("Vertical");
		if(keyPressed)
		{
			inputKeyboardJump = Input.GetKey (KeyCode.Space);
			inputKeyboardPower = Input.GetKey (KeyCode.E);
		}
		else
		{
			inputKeyboardJump = Input.GetKeyDown (KeyCode.Space);
			inputKeyboardPower = Input.GetKeyDown (KeyCode.E);
		}
	}

	/// <summary>
	/// Touch this instance.
	/// </summary>
	void Touch()
	{
		AnalogStickAndButton();
	}

	/// <summary>
	/// Analogs the stick and button.
	/// </summary>
	void AnalogStickAndButton() {
		// muss auf false gesetzt werden, da schleife beendet wird wenn touch gefunden
		buttonAIsPressed = false;
		buttonAIsTapped = false;
		buttonBIsPressed = false;
		buttonBIsTapped = false;
		analogStickIsStillPressed = false;
		foreach (Touch touch in Input.touches)
		{
			if(!buttonAIsTapped)	// Button (rechte Seite) muss nur einmal gefunden werden
			{
				if(touch.position.x > (Screen.width * 0.75f))
				{
					//debugmsg += "Jump found\n";
					//					buttonTouchID = touch.fingerId;			// ID des Touches speichern um beim nächsten durchlauf TapCount des Touches kontrollieren zu können
					if(buttonATapCount < touch.tapCount) {	// Spieler muss Taste immer wieder erneut drücken, um Aktion auszulösen
						buttonATapCount = touch.tapCount;	
						buttonAIsTapped = true;				
						buttonAIsPressed = true;
					}
					else
					{
						buttonAIsTapped = false;
						buttonAIsPressed = true;
					}
				}
			}

			if(!buttonBIsTapped)	// Button (rechte Seite) muss nur einmal gefunden werden
			{
				if(touch.position.x > (Screen.width * 0.5f) &&
				   touch.position.x < (Screen.width * 0.75f))
				{
					//debugmsg += "Jump found\n";
					//					buttonTouchID = touch.fingerId;			// ID des Touches speichern um beim nächsten durchlauf TapCount des Touches kontrollieren zu können
					if(buttonBTapCount < touch.tapCount) {	// Spieler muss Taste immer wieder erneut drücken, um Aktion auszulösen
						buttonBTapCount = touch.tapCount;	
						buttonBIsTapped = true;				
						buttonBIsPressed = true;
					}
					else
					{
						buttonBIsTapped = false;
						buttonBIsPressed = true;
					}
				}
			}
			
			if(!analogStickIsStillPressed)
			{
				/*
			 * Touch nach Touchphase auswerten:
			 * 	1. Began
			 *  2. Moved
			 *  3. Stationary
			 *  4. Ended
			 * */
				switch (touch.phase) {
					/* 1. */
				case TouchPhase.Began:
					//				Steuerung reagiert schlecht!
					//
					//				if(touch.position.x > (Screen.width * 0.5f))
					//				{
					//					buttonIsPressed=true;
					//				}
					if(touch.position.x < (Screen.width * 0.5f))
					{
						//debugmsg += "AnalogStick began()\n";
						// Analog Stick gefunden (Touch auf linker Bildschirmhälfte)
						//						analogStick = touch;
						analogStickTouchID = touch.fingerId;
						analogStickTouchBegan = true;
						analogStickIsStillPressed = true;
						
						// Screen.width/(2*2*2) = Screen.width*0.125
						float texturesizeX = analogStickTextureWidth * 0.5f;
						float texturesizeY = analogStickTextureHeight * 0.5f;
						float savezoneX = texturesizeX*0.5f;
						float savezoneY = texturesizeY*0.5f;
						textureSizeWithSaveZoneX = texturesizeX + savezoneX;
						textureSizeWithSaveZoneY = texturesizeY + savezoneY;
						
						/* X Position checken
						 * 
						 * 
						 * */
						if((touch.position.x > textureSizeWithSaveZoneX) && (touch.position.x < ((Screen.width*0.5)-textureSizeWithSaveZoneX)))
						{
							// X position korrekt (ohne SaveZone)
							touchBeganPositionX = touch.position.x;
						}
						else if(touch.position.x > ((Screen.width*0.5f)-textureSizeWithSaveZoneX))
						{
							// zu weit rechts am Rand
							// X position muss korrigiert werden (ohne SaveZone)
							touchBeganPositionX = ((Screen.width*0.5f)-textureSizeWithSaveZoneX);
						}
						else if(touch.position.x < textureSizeWithSaveZoneX)
						{
							// zu weit links am Rand
							// X position muss korrigiert werden (ohne SaveZone)
							touchBeganPositionX = textureSizeWithSaveZoneX;
						}
						
						/* Y Position checken
						 * 
						 * 
						 * */
						if((touch.position.y > textureSizeWithSaveZoneY) && (touch.position.y < (Screen.height)-textureSizeWithSaveZoneY))
						{
							// alles perfekt
							touchBeganPositionY = touch.position.y;
						}
						else if(touch.position.y > ((Screen.height)-textureSizeWithSaveZoneY))
						{
							// Problem: zu nah am oberen Rand, OFFSET (ohne SaveZone)!
							touchBeganPositionY = Screen.height-textureSizeWithSaveZoneY;
						}
						else if(touch.position.y < textureSizeWithSaveZoneY)
						{
							// Problem: zu nah am unteren Rand, OFFSET (ohne SaveZone)!
							touchBeganPositionY = textureSizeWithSaveZoneY;
						}
						
						
						// Analogstick um TouchBeganPosition (Mittelpunkt) zeichnen
						analogStickTexture.pixelInset = new Rect(touchBeganPositionX - analogStickTextureWidth*0.5f,	// left
						                                         touchBeganPositionY - analogStickTextureHeight*0.5f,	// top
						                                         analogStickTextureWidth,								// width
						                                         analogStickTextureHeight);								// height
						
						// Stick um Touch Position zeichnen
						stickTexture.pixelInset = new Rect(touch.position.x - stickTextureWidth*0.5f,		// left
						                                   touch.position.y - stickTextureHeight*0.5f,		// top
						                                   stickTextureWidth,								// width
						                                   stickTextureHeight);								// height
					}
					break;
					/* 2. */
				case TouchPhase.Moved:
					if(touch.fingerId == analogStickTouchID) 			/// needed??, for now yes! switch case geht über ganzen bildschirm
					{
						//debugmsg += "AnalogStick moved()\n";
						analogStickIsStillPressed = true;
						float stickPosX=0;
						float stickPosY=0;
						
						// Analogstick um TouchBeganPosition (Mittelpunkt) zeichnen
						if(touch.position.x > touchBeganPositionX + analogStickTextureWidth*0.5f)
							stickPosX = touchBeganPositionX + analogStickTextureWidth*0.5f;				// touch x-pos außerhalb des analogsticks (rechts)
						
						else if(touch.position.x < touchBeganPositionX - analogStickTextureWidth*0.5f)
							stickPosX = touchBeganPositionX - analogStickTextureWidth*0.5f;				// touch x-pos außerhalb des analogsticks (links)
						
						else
							stickPosX = touch.position.x;												// touch x-pos innerhalb des analogsticks
						
						if(touch.position.y > touchBeganPositionY + analogStickTextureHeight*0.5f)
							stickPosY = touchBeganPositionY + analogStickTextureHeight*0.5f;				// touch y-pos außerhalb des analogsticks (oben)
						
						else if(touch.position.y < touchBeganPositionY - analogStickTextureHeight*0.5f)
							stickPosY = touchBeganPositionY - analogStickTextureHeight*0.5f;				// touch y-pos außerhalb des analogsticks (unten)
						
						else
							stickPosY = touch.position.y;												// touch y-pos innerhalb des analogsticks
						
						// Stick um Touch Position zeichnen
						stickTexture.pixelInset = new Rect(stickPosX - stickTextureWidth*0.5f,	// left
						                                   stickPosY - stickTextureHeight*0.5f,	// top
						                                   stickTextureWidth,								// width
						                                   stickTextureHeight);								// height
						
						// Entfernung zum Analogstick Mittelpunkt berechnen (x-Ache)
						inputTouchHorizontal = (touch.position.x - touchBeganPositionX)/(analogStickTextureWidth*0.5f);
						if(inputTouchHorizontal > 1.0f)
							inputTouchHorizontal = 1.0f;
						else if(inputTouchHorizontal < -1.0f)
							inputTouchHorizontal = -1.0f;
						
						// Entfernung zum Analogstick Mittelpunkt berechnen (y-Ache)
						inputTouchVertical = (touch.position.y - touchBeganPositionY)/(analogStickTextureHeight*0.5f);
						if(inputTouchVertical > 1.0f)
							inputTouchVertical = 1.0f;
						else if(inputTouchVertical < -1.0f)
							inputTouchVertical = -1.0f;
						
					}
					break;
					
					/* 3. */
				case TouchPhase.Stationary:
					if(touch.fingerId == analogStickTouchID) 
					{
						//debugmsg += "AnalogStick stationary()\n";
						analogStickIsStillPressed = true;
					}
					break;
					
					/* 4. */
				case TouchPhase.Ended:
					if(touch.fingerId == analogStickTouchID) 
					{
						//debugmsg += "AnalogStick ended()\n";
						// Analog Stick ausblenden (aus sichtfeld verschieben)
						analogStickTexture.pixelInset = new Rect(0,
						                                         0,
						                                         0,
						                                         0);
						stickTexture.pixelInset = new Rect(0,
						                                   0,
						                                   0,
						                                   0);
						
						// Analog Stick als nicht aktiv setzen
						analogStickTouchBegan = false;
						analogStickIsStillPressed = false;
						analogStickTouchID = -1;
					}
					break;
				}
			}
		}
		
		if(!buttonAIsPressed)
		{
			//debugmsg += "kein Button gefunden\n";
			//kein Button in der Schleife oben gefunden, zurücksetzen
			//			buttonTouchID = -1;
			buttonATapCount = 0;
		}
		if(!buttonBIsPressed)
		{
			//debugmsg += "kein Button gefunden\n";
			//kein Button in der Schleife oben gefunden, zurücksetzen
			//			buttonTouchID = -1;
			buttonBTapCount = 0;
		}
		
		if(!analogStickTouchBegan)
		{
			//debugmsg += "kein AnalogStick gefunden (analogStickTouchBegan)\n";
			//kein AnalogStick in der Schleife oben gefunden, zurücksetzen
			inputTouchHorizontal = 0f;
			inputTouchVertical = 0f;
		}
		
		if(!analogStickIsStillPressed)
		{
			//debugmsg += "kein AnalogStick gefunden (analogStickIsStillPressed)\n";
			//kein AnalogStick in der Schleife oben gefunden, zurücksetzen
			inputTouchHorizontal = 0f;
			inputTouchVertical = 0f;
		}
		
		//		if(debugging != null)
		//			debugging.text = debugmsg;
		
	}

	//TODO //TODO //TODO achtung jetzt darf es nicht mehr destroyed werden!
	// Wenn jeder Character ein UserControlScript hat müssen die benutzten AnalogSticks dieses Script 
	// beim entfernen mitzerstört werden
	// GUITexture sind nicht dem CharacterGameObject untergeordnet (Child), da sich dessen Position ändert und 
	// im child die position immer auf Vector3.zero gesetzt werden müsste um die GUITexture korrekt auf dem Display
	// anzeigen zu lassen.
	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy()
	{
		//TODO //TODO //TODO achtung jetzt darf es nicht mehr destroyed werden!
//		if(analogStickTexture != null)
//			Destroy(analogStickTexture);
//		
//		if(stickTexture != null)
//			Destroy(stickTexture);
	}
}

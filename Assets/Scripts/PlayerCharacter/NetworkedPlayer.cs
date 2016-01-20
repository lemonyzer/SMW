using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

//[System.Serializable]
//public struct NetworkedInput{
//	public bool left;
//	public bool right;
//	public bool jump;
//	public bool power;
//}

public class NetworkedPlayer : MonoBehaviour
{
	// how far back to rewind interpolation?
	private double InterpolationBackTime = 0.1f;				// double trouble fix (was previously float!)
	private double ExtrapolationLimit = 0.5f;				// double trouble fix (was previously float!)

	public bool interpolation = true;
	public bool extrapolation = true;
	
	// a snapshot of values received by server from character owner client over the network
	private struct networkState
	{
		public Vector3 Position;
		public double Timestamp;
		public double tripTime;

		public float InputHorizontal;
		public bool InputJump;
		
		public networkState( Vector3 pos, double time, float inputHorizontal, bool inputJump )
		{
			this.InputHorizontal = inputHorizontal;
			this.InputJump = inputJump;
			this.Position = pos;
			this.Timestamp = time;		// time when client sent the State
			this.tripTime = Network.time - this.Timestamp;	// Network.time == time when Server received State, tripTime is the duration
		}
	}

	public class Power {
		public float maxSpeed;
	}

	public class Normal : Power {
	}

	// represents a move command sent to the server
	private struct move
	{
//		public float HorizontalAxis;
		public bool left;
		public bool right;
		public bool jump;
		public bool power;
		public double Timestamp;
		public Vector3 Position;

		
		public move (bool left, bool right, bool jump, bool power, double timestamp)
		{
//			this.HorizontalAxis = horiz;
			this.left = left;
			this.right = right;
			this.jump = jump;
			this.power = power;
			this.Timestamp = timestamp;
			this.Position = new Vector3(0f,0f,0f);
		}
	}
	
	// we'll keep a buffer of 20 states
	networkState[] stateBuffer = new networkState[ 20 ];
	int stateCount = 0; // how many states have been recorded
	
	// a history of move commands sent from the client to the server
	List<move> moveHistory = new List<move>();
	
	PlatformUserControl inputScript;
	PlatformCharacter characterScript;
	RealOwner ownerScript;
	NetworkView myNetworkView;

//	Transform characterLastRecvedPosBoxCollider;
//	Transform characterLastRecvedPos;
//	Transform characterPredictedPosBoxCollider;

	void Awake()
	{
		ownerScript = GetComponent<RealOwner> ();
		characterScript = GetComponent<PlatformCharacter> ();
		inputScript = GetComponent<PlatformUserControl> ();
		myNetworkView = GetComponent<NetworkView>();
//		characterLastRecvedPosBoxCollider = transform.Find("BoxCollider");		// looks in transform childs
//										//transform.FindChild
//																		// GameObject.Find looks in compete Scene!
//		characterLastRecvedPos = transform.Find("LastRecvedPos");
//		characterPredictedPosBoxCollider = transform.Find("PredictedBoxCollider");
	}

	bool initialComplete = false;

	[SerializeField] private bool useUnityPhysics = true;
	

	void Start()
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			showConnectionInfo = false;
			return;
		}

		if(Network.isServer)
		{
			showConnectionInfo = false;
			return;
		}

		if( ownerScript.owner == Network.player )
		{
			//TODO: wird nicht ausgeführt, da owner noch nicht gesetzt ist!!
			// mit setzen des owners, UI Elemente erstellen bzw initialisieren
			InitConnectionInfo();
			initialComplete = true;
		}
	}

	bool correctedPosition = false;
	int skipInputFrameCount = 3;
	int skippedInputFrametimes = 0;

	// simulate movement local
	// send input and calculated position to server / masterclient
	public bool localPlayerUnityEnginePhysicsUsed = false;
	bool frameRPCsended = false;
	float inputToRPCDelay = 0f;
	void FixedUpdate()
	{
		if (useUnityPhysics)
		{
			return;
		}

		if(Network.peerType == NetworkPeerType.Disconnected)
			return;

		if( ownerScript.owner == Network.player )
		{
			if(characterScript.GetPower() != null)
			{
				characterScript.GetPower().TriggeredUpdate();
			}
			if (moveHistory.Count > 0)
			{
				// ein FixedUpdate() aufruf später abschicken (position wird von physic engine korrigiert falls collision stattfindet)	// send 20ms delay!!!
				if(localPlayerUnityEnginePhysicsUsed)
				{
					if(!frameRPCsended)
					{
						//Debug.Log("Input-Physics-Send lag: " + ((int)((Time.time - inputToRPCDelay)*1000)) + " ms");
						move lastMoveStateWithPhysics = moveHistory[0];
						lastMoveStateWithPhysics.Position = this.transform.position;
						moveHistory.RemoveAt(0);
						moveHistory.Insert (0, lastMoveStateWithPhysics);

						if(Network.isClient)
						{
							//TODO works BUT BIG
							//myNetworkView.RPC( "ProcessInput", RPCMode.Server, lastMoveStateWithPhysics.left, lastMoveStateWithPhysics.right, lastMoveStateWithPhysics.jump, this.transform.position.x, this.transform.position.y );

							//TODO Sytem.char NOT supported
//							char inputChar = ConvertInputToOneCharByte(lastMoveStateWithPhysics.left, lastMoveStateWithPhysics.right, lastMoveStateWithPhysics.jump, lastMoveStateWithPhysics.power);
//							myNetworkView.RPC( "ProcessInput", RPCMode.Server, inputChar, this.transform.position.x, this.transform.position.y );

							//TODO Sytem.byte NOT supported
							//byte inputByte = ConvertInputToByte(lastMoveStateWithPhysics.left, lastMoveStateWithPhysics.right, lastMoveStateWithPhysics.jump, lastMoveStateWithPhysics.power);
							//myNetworkView.RPC( "ProcessInput", RPCMode.Server, inputByte, this.transform.position.x, this.transform.position.y );

							byte[] inputByteArray = new byte[1];
							inputByteArray[0] = ConvertInputToByte(lastMoveStateWithPhysics.left, lastMoveStateWithPhysics.right, lastMoveStateWithPhysics.jump, lastMoveStateWithPhysics.power);
							myNetworkView.RPC( "ProcessInput", RPCMode.Server, inputByteArray, this.transform.position.x, this.transform.position.y );
						}
						else
						{
							// Server muss nichts simulieren
						}
//						if(characterScript.canUsePowerButton)
//						{
//							if(inputScript.inputPower)
//							{
//////////////////////////////////characterScript.item.coolDown();
//								myNetworkView.RPC( "ProcessPowerRequest", RPCMode.Server );
//							}
//						}
					}
				}
			}

			if(localPlayerUnityEnginePhysicsUsed)
			{
				frameRPCsended = false;
				inputToRPCDelay = Time.time;
			}

			if(correctedPosition)
			{
				Debug.Log("corrected Position, skip input for one frame");
				correctedPosition = false;
				return;
			}
			// wird nur auf anderen characteren ausgeführt!
			extrapolation = false;

			// simulate
			characterScript.Simulate();

			// this is my character
			// get current move state
			move moveState = new move( inputScript.inputLeft, inputScript.inputRight, inputScript.inputJump, inputScript.inputPower, Network.time );

			if(!localPlayerUnityEnginePhysicsUsed)
			{
				// information required to locate the own boxcollider (should be 100 ms behind the charactersprite)
				moveState.Position = this.transform.position;		// not corrected by physics engine!!! could be realy wrong!!!
			}

			// buffer move state
			moveHistory.Insert( 0, moveState );
			
			// cap history at 200
			if( moveHistory.Count > 200 )
			{
				moveHistory.RemoveAt( moveHistory.Count - 1 );
			}

			// send state to server
			if(Network.isClient && !localPlayerUnityEnginePhysicsUsed)
			{
//				if(clientNeedsToSendNewInput)
//				{
//					myNetworkView.RPC( "ClientACKPositionCorrection", RPCMode.Server );
//					clientNeedsToSendNewInput = false;
//				}
				//TODO works BUT BIG
//				myNetworkView.RPC( "ProcessInput", RPCMode.Server, moveState.left, moveState.right, moveState.jump, this.transform.position.x, this.transform.position.y);

				//TODO NOT supported System.char
				//char inputChar = ConvertInputToOneCharByte(moveState.left, moveState.right, moveState.jump, moveState.power);
				//myNetworkView.RPC( "ProcessInput", RPCMode.Server, inputChar, this.transform.position.x, this.transform.position.y );

				//TODO NOT supported System.byte
				//byte inputByte = ConvertInputToByte(moveState.left, moveState.right, moveState.jump, moveState.power);
				//myNetworkView.RPC( "ProcessInput", RPCMode.Server, inputByte, this.transform.position.x, this.transform.position.y );

				byte[] inputByteArray = new byte[1];
				inputByteArray[0] = ConvertInputToByte(moveState.left, moveState.right, moveState.jump, moveState.power);
				myNetworkView.RPC( "ProcessInput", RPCMode.Server, inputByteArray, this.transform.position.x, this.transform.position.y );

//				if(characterScript.canUsePowerButton)
//				{
//					if(inputScript.inputPower)
//					{
//						characterScript.PowerPredictedAnimation();	// instant reaktion, feels better!
//						myNetworkView.RPC( "ProcessPowerRequest", RPCMode.Server );
//					}
//				}
			}
			else if(Network.isServer)
			{
				// cant send from server to server!
				if(characterScript.canUsePowerButton)
				{
					if(inputScript.inputPower)
					{
						ProcessPowerRequest();
					}
				}
			}
		}
		else
		{

		}
	}

	[RPC]
	public void ProcessPowerRequest()
	{
		if(!Network.isServer)
		{
			return;
		}

		if(characterScript.GetPower() != null)
		{
			//power like shooting bullets dont need extra RPC to clients. server will instantiate bullet object, clients can see it.
			//other powers like shield need animation on client (all clients, server aswell) so RPC to start ShieldAnimation.
			//TODO polymorphismus
			// class Items.power() with correct execution/rpc.
			characterScript.Power();
		}

//		if(characterScript.hasItem)
//		{
//			//power like shooting bullets dont need extra RPC to clients. server will instantiate bullet object, clients can see it.
//			//other powers like shield need animation on client (all clients, server aswell) so RPC to start ShieldAnimation.
//			//TODO polymorphismus
//			// class Items.power() with correct execution/rpc.
//			characterScript.Power();
//		}
	}

	// server
	public uint serverCorrectsClientPositionCount;
	public List<Vector3> deltaPositions = new List<Vector3>();
	public Vector3 lastPositionDifference = Vector3.zero;
	public Vector3 avgPositionDifference = Vector3.zero;
	public bool waitingForNewClientInput = false;

//	[RPC]
//	void ClientACKPositionCorrection()
//	{
//		waitingForNewClientInput = false;		// wenn diese RPC ausgeführt wurde ließt und wertet Server wieder ProcessInput RPC's aus
//	}

	[RPC]
// works BUT VERY BIG			void ProcessInput( bool recvedLeft, bool recvedRight, bool recvedInputJump, Vector3 recvedPosition, NetworkMessageInfo info )
// works BUT BIG				void ProcessInput( bool recvedLeft, bool recvedRight, bool recvedInputJump, float recvedPositionX, float recvedPositionY, NetworkMessageInfo info )
// System.char NOT SUPPORTED	void ProcessInput( char inputCharByte, float recvedPositionX, float recvedPositionY, NetworkMessageInfo info )
// System.byte NOT SUPPORTED	void ProcessInput( byte inputByte, float recvedPositionX, float recvedPositionY, NetworkMessageInfo info )
	void ProcessInput( byte[] inputByteArray, float recvedPositionX, float recvedPositionY, NetworkMessageInfo info )
	{
//		Debug.Log(this.ToString() + ": ProcessInput");
		// aktuell gehören photonviews dem masterclient
		//		if( photonView.isMine )
		//			return;
		if (ownerScript.owner == Network.player)
		{
			// this character is owned by local player... don't run simulation
			// master client muss sich selbst nicht kontrollieren
			return;
		}
		
		if( !Network.isServer )
		{
			// nur master client bekommt input und kontrolliert andere spieler
			return;
		}

		// was no bug... server accepted and applied input only position was wrong so server sents correctposition, till client is sync
//		if(waitingForNewClientInput)
//		{
//			// client position prediction was wrong, drop all packages newInput == false
//			// waiting for newInput after client recvs correctposition rpc and replys with input
//
//			// gameplay FIX
//			inputScript.inputHorizontal = 0;
//			inputScript.inputJump = false;
//			characterScript.Simulate();
//			return;
//		}

		// execute input

		// TODO System.char NOT supported
//		bool[] recvedBools = ConvertInputCharToBoolArray(inputCharByte);
//		inputScript.SetInputHorizontal(recvedBools[0], recvedBools[1]);
//		inputScript.inputJump = recvedBools[2];

		// TODO System.char NOT supported
//		bool[] recvedBools = ConvertInputByteToBoolArray(inputByte);
//		inputScript.SetInputHorizontal(recvedBools[0], recvedBools[1]);
//		inputScript.inputJump = recvedBools[2];

		byte inputByte = inputByteArray[0];
		bool[] recvedBools = ConvertInputByteToBoolArray(inputByte);
		inputScript.SetInputHorizontal(recvedBools[0], recvedBools[1]);
		inputScript.inputJump = recvedBools[2];

		characterScript.Simulate();

		if(characterScript.isDead)	// no position correction needed!
		{
			Debug.LogWarning("character is Dead. Input reveiced but, no positioncorrection are checked and sent");
			return;
		}

		// authorative movement, without anti cheat check!
		//this.transform.position = recvedPosition;
		//return;
																									// berücksichtigt alle

		Vector3 recvedPosition = new Vector3(recvedPositionX, recvedPositionY, 0f);
		if( Vector3.Distance( this.transform.position, recvedPosition ) > 0.2f )
		{
			// error is too big, tell client to rewind and replay								// berücksichtigt die, die zu stark abweichen
			serverCorrectsClientPositionCount++;
			myNetworkView.RPC( "CorrectState", info.sender, this.transform.position.x, this.transform.position.y, characterScript.moveDirection.x, characterScript.moveDirection.y );
//			waitingForNewClientInput = true;															// drop already send ProcessInput RPC's from Client
			// compare results
			deltaPositions.Insert(0, (this.transform.position - recvedPosition));						

			avgPositionDifference = Vector3.zero;
			for(int i=0;i<deltaPositions.Count;i++)
			{
				avgPositionDifference += deltaPositions[i];
			}
			avgPositionDifference = (1.0f/deltaPositions.Count) * avgPositionDifference;
			lastPositionDifference = deltaPositions[0];

			// cap history at 1000
			if( deltaPositions.Count > 1000 )
			{
				deltaPositions.RemoveAt( deltaPositions.Count - 1 );
			}
		}
		else
		{
			// authorative movement, with anti cheat check!
			// less correction send, becauze clients calculations will be accepted and applied! 
			this.transform.position = recvedPosition;
			return;
		}

//		if(timestamp > lastCorrectionSend.Timstamp &&
//		   timestamp < lastcorrectionsend.timestamp + 1sec)
//		{
//																							// brücksichtigt die, die auf eine korrigierung zu stark abweichen
//		}
	}

//	double timeDiff = 0;

	// local client character only
	public uint correctPositionCount = 0;
//	public bool clientNeedsToSendNewInput = false;
	[RPC]
	void CorrectState( float correctPositionX, float correctPositionY, float moveDirectionX, float moveDirectionY, NetworkMessageInfo info )
	{
		correctPositionCount++;
//		clientNeedsToSendNewInput = true;
		// find past state based on timestamp
		int pastState = -1;											// FIX?: -1	//replay begins with 0<=pastState, if array is empty can't access element 0!!!
		for( int i = 0; i < moveHistory.Count; i++ )
		{
			if( moveHistory[ i ].Timestamp <= info.timestamp )
			{
				// wenn gebufferter eintrag älter als ankommendes correctPosition paket
				// tritt bei zwei aufeinander folgende correctPosition RPC's nicht mehr ein, da moveHistory.clear aufgerufen wird!
				pastState = i;
				break;
			}
		}

		Vector3 tempPositionBeforeCorrection = this.transform.position;

		// rewind position
		if(true)
		{
			//v1 3 frames to smooth correction
			//a) Coroutine triggers every deltaFixedUpdateTime
			//b) FixedUpdate
			this.transform.position = new Vector3(correctPositionX, correctPositionY, 0f);
			characterScript.moveDirection = new Vector3(moveDirectionX, moveDirectionY, 0f);				// FIX, less desync?

		}


		Debug.Log("pastState ID: " + pastState);

		// replay
		if(true)
		{
			// replay already sent controlInput (was with wrong predicted position, but server accepted input!!!!)
			// because the movement commands are already sent to server!		// BUG: with wrong predicted position!! ->> server ACCEPTS Input Sends correct position!
			for( int i = 0; i <= pastState; i++ )
			{
				// gameplay FIX
//				if(replayMode==false)	// replay = false
//				{
//					// gameplay FIX
//					inputScript.inputHorizontal = 0f;
//					inputScript.inputJump = false;
//					characterScript.Simulate();
//				}
				inputScript.SetInputHorizontal(moveHistory[ i ].left, moveHistory[ i ].right);
				inputScript.inputJump = moveHistory[ i ].jump;
				characterScript.Simulate();
			}
		}

		Vector3 tempPositionAfterCorrectionAndInputReplay = this.transform.position;

		// try
//		transform.position = Vector3.Lerp(tempPositionBeforeCorrection, tempPositionAfterCorrectionAndInputReplay, 0.5f);
		
		// clear
		moveHistory.Clear();

//		FixedUpdate();				// to get atleast on element in moveHistory!!! ... next correctPosition is coming, need rewind and replay states
//		correctedPosition = true;
	}
	


	int GetPastState(double timeStamp)
	{
		// moveHistory[0] immer aktuellster State, wenn nicht leer!
		int pastState = -1;											
		for( int i = 0; i < moveHistory.Count; i++ )
		{
			if( moveHistory[ i ].Timestamp <= timeStamp )
			{
				pastState = i;
				break;
			}
		}
		return pastState;
	}

	// raw position data stored in moveHisory, no backsimulation needed
//	// local Character BoxCollider draw on 100ms back position
//	void SimulationBack()
//	{
//	}

	void SetLastRecvdPos()
	{

	}

	void SetTransformToCalculatedEstiminatedPositionOnServer(Transform transform)
	{

	}

	void SetTransformToLastRecevedPosition(Transform transform)
	{
		if(stateCount > 0)
		{
			//characterScript.lastReceivedPos.position = stateBuffer[0].Position;		// stateBuffer[0]
			transform.position = stateBuffer[0].Position;		// stateBuffer[0]
        }
    }


	void LastPosAndPrediction()
	{
		avgTripTime = 0;
		for(int i=0; i< stateCount; i++)
		{
			if(stateBuffer[i].tripTime > maxTripTime)
			{
				maxTripTime = stateBuffer[i].tripTime;
			}
			avgTripTime += stateBuffer[i].tripTime;
		}
		avgTripTime = (double)(avgTripTime/stateCount);

		if(ownerScript.owner == Network.player)
		{
			// shows my characterposition 100ms in the past (Network Time - 100 ms)... is almost equal to the authorativ position from server

			// its not 100ms !!!
			// it is Network.time - lastTripTime		( client sends input and predicted position to server, server answers only if position is bad)
			//double serverTime = Network.time - InterpolationBackTime;
			//double serverTime = Network.time - (2*Network.GetAveragePing(Network.connections[0]))/1000.0;

			// NetworkAveragePing cant belive this value...
			// need continously pakageflow to read ping(rtt)/triptime

			//T O D O avgTripTime has to be calculated in this method, not in OnGUI!
			//DONE
			//TODO avgTripTime in paketreceivemethod
			
			double serverTime = Network.time - (2*avgTripTime);

//			Debug.Log(Network.time - InterpolationBackTime);
//			Debug.Log(Network.time - (2*Network.GetAveragePing(Network.connections[0]))/1000.0);

			int pastState = GetPastState( serverTime );
//			Debug.Log("BoxCollider pastState = " + pastState);
			if(pastState >= 0)
			{
				// buffer moveHistory[0] immer aktuellste
				//				for(int i=0, i<=pastState)
				//				{
				//					SimulationBack();
				//				}

				// setzte berechnete position
				characterScript.currentEstimatedPosOnServer.transform.position = moveHistory[pastState].Position;			// moveHistory[0]

				// setzte letzte erhaltene position
				if(stateCount > 0)
				{
					characterScript.lastReceivedPos.position = stateBuffer[0].Position;											// stateBuffer[0]
				}
				else
				{
					//TODO
				}
#if UNITY_EDITOR
				Debug.DrawLine(this.transform.position, moveHistory[pastState].Position, Color.red, 5f);
#endif				
			}
			else
			{
				// keinen past state gefunden
				// TODO: calculate triptime of correctPosition message and check if player is grounded if not -> collider is moved triptime toward ground

				// setzte letzte erhaltene position
				if(stateCount > 0)
				{
					characterScript.lastReceivedPos.position = stateBuffer[0].Position;											// stateBuffer[0]

					// keine berechnung möglich, auf letzte erhaltene pos (diese ist triptime alt)
					characterScript.currentEstimatedPosOnServer.transform.position = stateBuffer[0].Position;	
				}
				else
				{
					//TODO
				}


				//characterLastRecvedPosBoxCollider.transform.position = this.transform.position;
				//characterLastRecvedPos.position = this.transform.position;
				//				characterBoxCollider.SetActive(false);
			}
		}
		else
		{
			// Auf Client: other Players position
			/***
			 * 
			 *	PREDICTION			SIMPLE (only Player Input from latest received netUpdate (OnSerialzationView)
			 *
			 **/
			// predicts characterposition (Network Time + 100ms) 

			if(stateCount >= 2)
			{
				//Lerping zwischen den zwei aktuellsten Positionen
				//characterLastRecvedPos.position = Vector3.Lerp(stateBuffer[1].Position,stateBuffer[0].Position, (float)((Network.time-stateBuffer[0].Timestamp)/(2*stateBuffer[0].tripTime)));
				characterScript.lastReceivedPos.position = Vector3.Lerp(stateBuffer[1].Position,stateBuffer[0].Position, (float)((Network.time-stateBuffer[0].Timestamp)/(stateBuffer[0].Timestamp-stateBuffer[1].Timestamp)));
			}
			else if(stateCount == 1)
				characterScript.lastReceivedPos.position = stateBuffer[0].Position;

			if(stateCount > 0)
			{
//				Vector3 predictedPosition = new Vector3(transform.position.x,transform.position.y,transform.position.z);
				Vector3 moveDirectionPredicted = new Vector3(stateBuffer[0].InputHorizontal * characterScript.getMaxSpeed() * Time.fixedDeltaTime,0f,0f);

				//TODO
				// look how many stateBuffer[0].triptime
				// getPastStateCount
				//TODO int steps = GetPastState(Network.time - stateBuffer[0].tripTime) + 1;
				int steps = (int) (stateBuffer[0].tripTime/Time.fixedDeltaTime);

				// OnGUI
//				Debug.Log("TripTime (FromServer)= " + stateBuffer[0].tripTime + "\nresulting prediction Steps =" + steps);
//				Debug.Log("Each Step takes Time.fixedDeltaTime: " + Time.fixedDeltaTime + " ms)");

				// vorher position wieder auf ZULETZT ERHALTENE Position setzen, dann kann vorrausberechnet werden
				characterScript.predictedPosCalculatedWithLastInput.position = stateBuffer[0].Position;
				for(int i=0; i < steps; i++)
				{
					characterScript.predictedPosCalculatedWithLastInput.Translate( moveDirectionPredicted );
				}
			}
		}
	}
	
	// Server and other clients characters
	public uint extrapolationCount = 0;
	// private float updateTimer = 0f;		// server send position updates every x seconds
											// time based sending, not framerate based - more precise on various machines!
											// NetworkView RPC's are always reliable
											// not used because i'm now using OnSerilizeNetworkView (send 15 times per second) and is unreliable
	void Update()
	{

		if(Network.peerType == NetworkPeerType.Disconnected)
			return;

		UpdateConnectionInfo();
		LastPosAndPrediction();

		// in OnSerializeView() --- unreliable/reliable posibility! ...
		// Server owns all Character GameObjects therefore he knows the correct authorative position ( with pingdelay behind controlling client ) 

		// is this the server? send out position updates every 1/10 second
//		if( Network.isServer )
//		{
//			updateTimer += Time.deltaTime;
//			if( updateTimer >= 0.1f )
//			{
//				updateTimer = 0f;
//				myNetworkView.RPC( "netUpdate", RPCMode.Others, transform.position );
//			}
//		}

		if(Network.isServer)
		{
//			Debug.Log(stateCount); 		// always 0  -> noData to interpolate!
			return;						// Server doesn't need interpolation!
		}

		// NetworkView of character is always owned by server!
//		if( myNetworkView.isMine ) return; 						// don't run interpolation on the local object
		if (ownerScript.owner == Network.player)
		{
			return;  // don't run interpolation on the local object
		}

		if( stateCount == 0 ) return; // no states to interpolate
		
		double currentTime = Network.time;
		double interpolationTime = currentTime - InterpolationBackTime;

		// the latest packet is newer than interpolation time - we have enough packets to interpolate
		if( stateBuffer[ 0 ].Timestamp > interpolationTime )
		{
			for( int i = 0; i < stateCount; i++ )
			{
				// find the closest state that matches network time, or use oldest state
				if( stateBuffer[ i ].Timestamp <= interpolationTime || i == stateCount - 1 )
				{
					// the state closest to network time
					networkState lhs = stateBuffer[ i ];
					
					// the state one slot newer
					//networkState rhs = stateBuffer[ Mathf.Max( i - 1, 0 ) ];
					networkState rhs = stateBuffer[ 0 ];								// position should be more precise
					
					// use time between lhs and rhs to interpolate
					double length = rhs.Timestamp - lhs.Timestamp;
					float t = 0f;
					if( length > 0.0001 )
					{
						t = (float)( ( interpolationTime - lhs.Timestamp ) / length );		// needs fix
					}

					if(Vector3.Distance(lhs.Position,rhs.Position) > 5)
					{
						// position changed dramatically (beam)
						transform.position = rhs.Position;
					}
					else
					{
                        // Vector3.Lerp ( from, to, fraction )
                        //Linearly interpolates between two vectors.
                        //Interpolates between from and to by the fraction t.
                        //This is most commonly used to find a point some fraction
                        //of the way along a line between two endpoints (eg, to move
                        //an object gradually between those points). This fraction is
                        //clamped to the range [0...1]. When t = 0 returns FROM.
                        //When t = 1 returns TO. When t = 0.5 returns the point midway between from and to.
						transform.position = Vector3.Lerp( lhs.Position, rhs.Position, t );
					}
					break;
				}
			}
		}
		else
		{
			// Extrapolation (since 100 ms no update received!)
//			Debug.Log("Extrapolation");
			if(extrapolation)
			{
				extrapolationCount++;
				networkState latest = stateBuffer[0];
				float extrapolationLength = (float)(interpolationTime - latest.Timestamp);		// (Network.time-100ms) - (lastest.time), mit latest.time < network.time-100ms
																								// ergebnis immer positiv, größer 0
																								// extrapolationLenght > 0

				// Don't extrapolation for more than 500 ms, you would need to do that carefully
				if (extrapolationLength < ExtrapolationLimit)
				{
					Vector3 moveDirection = new Vector3(latest.InputHorizontal * characterScript.getMaxSpeed() * Time.fixedDeltaTime,0f,0f);
					transform.position = latest.Position + moveDirection * extrapolationLength;
				}
			}
		}


		PredictionWithLastInput();
	}

	/// <summary>
	/// Predictions Position with using last move Input
	/// </summary>
	void PredictionWithLastInput()
	{

		/**
			 * 
			 * !!!!!!!!!! 	Prediction / "Extrapolation"		!!!!!!	
			 * 
			 **/
		// this character is from other player
		if(stateCount > 0)
		{
			//				// wir haben mindestens ein paket
			networkState last = stateBuffer[0];
			Vector3 tempPosition = transform.position;
			
			inputScript.SetInputHorizontal(last.InputHorizontal);	// show animation
			//inputScript.inputJump = last.InputJump;
			characterScript.Simulate();
			
			transform.position = tempPosition;	// reset position
			characterScript.predictedPosSimulatedWithLastInput.position = transform.position;

			//TODO replace Time.fixedDeltaTime
			// what is GetPastState looking for, 
			//int count = GetPastState(Network.time - avgTripTime) + 1 ;
			for(int i=0; i< ((int)((avgTripTime)/Time.fixedDeltaTime)); i++)			// TODO calculate prediction steps with avgTripTime in receiving method!!
			{
				inputScript.SetInputHorizontal(last.InputHorizontal);	// predict that user is still moving in same direction.
				//inputScript.inputJump = last.InputJump;
				characterScript.Simulate();
			}
			Vector3 predictedPosition = characterScript.predictedPosSimulatedWithLastInput.position;		// stateBuffer[0].pos + predictiontime*stateBuffer[0].inputH could be wrong?
			transform.position = tempPosition;
			characterScript.predictedPosSimulatedWithLastInput.position = predictedPosition;
#if UNITY_EDITOR
			Debug.DrawLine(tempPosition, predictedPosition);
#endif
		}
	}

	bool oneTimeInfo = true;
	// Authorative & unreliable replaced - Bookmethod [RPC]
	// buffers only on Clients, on Characters not owned by local player
	public uint olderPackageReceivedCount =0;
//	public bool unreliableConnection = true;
	[RPC]
	void netUpdate( Vector3 position, float inputHorizontal, bool inputJump, NetworkMessageInfo info )
	{
		// hier kann einiges optimiert werden

		// The short keyword is used to define a variable of type short. Short is an integer (ie no decimals are allowed). Range of short: –32,768 through 32,767. 
		// positionen ebenfalls  (wird sowieso dazwischen interpoliert)
		// position über vector2 - 2d game!
		// inputHorizontal: 1 char/2 booleam (left/right)/short/int
		// input all in one char [jumpBit, powerUpBit, leftBit, rightBit, X, X, X, X]

		//Problem: aktuell gehören photonViews dem MasterClient
		if( ownerScript.owner != Network.player )
		{
			// dieser Character gehört nicht lokalem Spieler
			if(myNetworkView.stateSynchronization == NetworkStateSynchronization.Unreliable)
			{
				if((info.timestamp > stateBuffer[0].Timestamp))
				{
					// this package has new information and will be buffered
					bufferState( new networkState( position, info.timestamp, inputHorizontal, inputJump ) );
				}
				else
				{
					// this package is older than the latest buffered package
					// dont buffer (drop it)
					Debug.LogWarning("unreliable Connection, older UDP package received and dropped.");
					olderPackageReceivedCount++;
				}
			}
			else if(GetComponent<NetworkView>().stateSynchronization == NetworkStateSynchronization.ReliableDeltaCompressed)
			{
				// reliable Connection - receiving in correct Order (always latest package is received)
				bufferState( new networkState( position, info.timestamp, inputHorizontal, inputJump ) );
			}
			else if(GetComponent<NetworkView>().stateSynchronization == NetworkStateSynchronization.Off)
			{
				// this is an RPC (always reliable in Unity 4.5 NetworkViews)
				bufferState( new networkState( position, info.timestamp, inputHorizontal, inputJump ) );
			}

		}
		else
		{
			bufferState( new networkState( position, info.timestamp, inputHorizontal, inputJump ) );					// buffers local players character (to use TripTime to show boxcollider position on server) 
			// Dieser Character gehört lokalem Spieler
			if(oneTimeInfo)
			{
				oneTimeInfo=false;
				Debug.Log(this.ToString() + ": my Character");
			}
		}
	}
	
	// save new state to buffer
	void bufferState( networkState state )
	{
		// shift buffer contents to accomodate new state
		for( int i = stateBuffer.Length - 1; i > 0; i-- )
		{
			stateBuffer[ i ] = stateBuffer[ i - 1 ];
		}
		
		// save state to slot 0
		stateBuffer[ 0 ] = state;
		
		// increment state count (up to buffer size)
		stateCount = Mathf.Min( stateCount + 1, stateBuffer.Length );
	}

	void NetworkDataOptimizationAndCompression(BitStream stream)
	{
		int dataCount = 3;

		for(int i=0; i < dataCount; i++)
		{

		}
	}


	
//	chars[0] = 'X';        // Character literal
//	chars[1] = '\x0058';   // Hexadecimal
//	chars[2] = (char)88;   // Cast from integral type
//	chars[3] = '\u0058';   // Unicode

	private char ConvertInputToOneCharByte(bool left, bool right, bool jump, bool power)
	{
		char result = '0';
		// This assumes the array never contains more than 8 elements!
		
		if (left)
			result |= (char)(1 << (0));
		
		if (right)
			result |= (char)(1 << (1));
		
		if (jump)
			result |= (char)(1 << (2));
		
		if (power)
			result |= (char)(1 << (3));
		
		return result;
	}

	private byte ConvertInputToByte(bool left, bool right, bool jump, bool power)
	{
		byte result = 0;
		// This assumes the array never contains more than 8 elements!

		if (left)
			result |= (byte)(1 << (0));

		if (right)
			result |= (byte)(1 << (1));

		if (jump)
			result |= (byte)(1 << (2));

		if (power)
			result |= (byte)(1 << (3));

		return result;
	}

	private static bool[] ConvertInputByteToBoolArray(byte b)
	{
		// prepare the return result
		bool[] result = new bool[8];
		
		// check each bit in the byte. if 1 set to true, if 0 set to false
		for (int i = 0; i < 8; i++)
			result[i] = (b & (1 << i)) == 0 ? false : true;
		
		return result;
	}

	private static bool[] ConvertInputCharToBoolArray(char b)
	{
		// prepare the return result
		bool[] result = new bool[8];
		
		// check each bit in the byte. if 1 set to true, if 0 set to false
		for (int i = 0; i < 8; i++)
			result[i] = (b & (1 << i)) == 0 ? false : true;
		
		return result;
	}



//	private byte ConvertBoolArrayToByte(bool[] source)
//	{
//		byte result = 0;
//		// This assumes the array never contains more than 8 elements!
//		int index = 8 - source.Length;
//		
//		// Loop through the array
//		foreach (bool b in source)
//		{
//			// if the element is 'true' set the bit at that position
//			if (b)
//				result |= (byte)(1 << (7 - index));
//			
//			index++;
//		}
//		
//		return result;
//	}
//
//	private static bool[] ConvertByteToBoolArray(byte b)
//	{
//		// prepare the return result
//		bool[] result = new bool[8];
//		
//		// check each bit in the byte. if 1 set to true, if 0 set to false
//		for (int i = 0; i < 8; i++)
//			result[i] = (b & (1 << i)) == 0 ? false : true;
//		
//		// reverse the array
//		Array.Reverse(result);
//		
//		return result;
//	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{

		float authoritativePosZ = 0f;

		// Code runs on NetworkView Owner
		// if Owner is Server, send to all Clients
		// if Owner is Client, send to all Players (not only to Server!!!)
		if (stream.isWriting)
		{
			Vector3 authoritativePos = transform.position;				// authorative calculated position
			float authoritativePosX = authoritativePos.x;
			float authoritativePosY = authoritativePos.y;
//2D -> Z=0	//float authoritativePosZ = authoritativePos.z;
			//float receivedHorizontal = inputScript.GetInputHorizontal();		// received input from the character owner send to all clients (for prediction use and animation)
//			bool receivedInputJump = inputScript.inputJump;				// received input from the character owner send to all clients (for prediction use and animation)
			char receivedInput = ConvertInputToOneCharByte(inputScript.inputLeft, inputScript.inputRight, inputScript.inputJump, inputScript.inputPower);
//			stream.Serialize(ref authoritativePos);
			stream.Serialize(ref authoritativePosX);
			stream.Serialize(ref authoritativePosY);
			stream.Serialize(ref receivedInput);
//			stream.Serialize(ref receivedHorizontal);
//			stream.Serialize(ref receivedInputJump);
		}
		// Read data from Server
		else
		{
			bool left = false;
			bool right = false;
			bool jump = false;
			bool power = false;
			Vector3 authoritativePos = Vector3.zero;
			float authoritativePosX = 0f;
			float authoritativePosY = 0f;
//			byte receivedHorizontal = 0f;
//			bool receivedInputJump = false;
			char receivedInput = '\x0000';
//			stream.Serialize(ref authoritativePos);
			stream.Serialize(ref authoritativePosX);
			stream.Serialize(ref authoritativePosY);
			stream.Serialize(ref receivedInput);
//			stream.Serialize(ref receivedHorizontal);
//			stream.Serialize(ref receivedInputJump);

			bool[] booleans = ConvertInputCharToBoolArray(receivedInput);
			left = booleans[0];
			right = booleans[1];
			jump = booleans[2];
			power = booleans[3];

			authoritativePos = new Vector3(authoritativePosX, authoritativePosY, authoritativePosZ);

			// Update: netUpdates buffers now also on local player character to get current TripTime :://TODO DONE and latest pos ( lastrecvdPos

			//netUpdate(pos, info);			// will buffer state only on characters not controlled by local player
			netUpdate(authoritativePos, GetInputHorizontal(left,right), jump, info);			// will buffer state only on characters not controlled by local player
		}
	}

	public float GetInputHorizontal(bool left, bool right)
	{
		if(left)
			return -1.0f;
		else if(right)
			return 1.0f;
		else
			return 0f;
	}


//	void OnGUI()
//	{
//		// OnGUI
//		//				Debug.Log("TripTime (FromServer)= " + stateBuffer[0].tripTime + "\nresulting prediction Steps =" + steps);
//		//				Debug.Log("Each Step takes Time.fixedDeltaTime: " + Time.fixedDeltaTime + " ms)");
//
//		// beste wär wenn server tripTime von anderem Client ebenfalls sendet: otherClient -> server -> this client
//		// aktuell nur server -> this client
//		if( Network.isServer)
//			return;
//		if( ownerScript.owner == Network.player)
//		{
//			GUILayout.Space(160);
//			GUILayout.FlexibleSpace();
//			GUILayout.Box("LastTripTime = " + ((double)(stateBuffer[0].tripTime)*1000).ToString("#### ms") + "\nresulting prediction Steps =" + ((double)(stateBuffer[0].tripTime/Time.fixedDeltaTime)).ToString("#.#"));
//			GUILayout.Box("LastTripTime = " + ((double)(stateBuffer[0].tripTime)*1000).ToString("#### ms") + "\nresulting prediction Steps =" + (GetPastState(Network.time - stateBuffer[0].tripTime)+1));
//			// kann sein das LastTripTime nicht im avgTripTime eingerechnet wurde da der avg zum anderen zeitpunkt berechnet wird und da das neuste paket noch nicht angekommen ist. 
//			//TODO // lösung -> avg immer bei ankommendem paket berechnen!
//			GUILayout.Box("avgTripTime = " + ((double)(avgTripTime*1000)).ToString("#### ms") + "\nresulting avg prediction Steps =" + ((double)(avgTripTime/Time.fixedDeltaTime)).ToString("#.#"));
//			GUILayout.Box("avgTripTime = " + ((double)(avgTripTime*1000)).ToString("#### ms") + "\nresulting avg prediction Steps =" + ((GetPastState(Network.time - avgTripTime)+1)));
//			GUILayout.Box("maxTripTime = " + maxTripTime.ToString("0.###") + "\nresulting avg prediction Steps =" + ((double)(maxTripTime/Time.fixedDeltaTime)).ToString("#"));
//			GUILayout.Box("maxTripTime = " + maxTripTime.ToString("0.###") + "\nresulting avg prediction Steps =" + (GetPastState(Network.time - maxTripTime)+1));
//			GUILayout.FlexibleSpace();
//		}
//	}
	double avgTripTime = 0;
	double maxTripTime = 0;

	public GameObject prefabConnectionInfo;
	public GameObject goCanvasConnectionInfo;
	public Text txtLastTripTime;
	public Text txtLastTripTimeSteps;
	public Text txtLastTripTimeSteps2;
	public Text txtAvarageTripTime;
	public Text txtAvarageTripTimeSteps;
	public Text txtAvarageTripTimeSteps2;
	public Text txtMaxTripTime;
	public Text txtMaxTripTimeSteps;
	public Text txtMaxTripTimeSteps2;

	public bool showConnectionInfo = true;

	void InitConnectionInfo()
	{
		if(prefabConnectionInfo != null)
		{
			goCanvasConnectionInfo = Instantiate(prefabConnectionInfo, Vector3.zero, Quaternion.identity) as GameObject;

			string parentGO = "ConnectionInfo/";

			txtLastTripTime = goCanvasConnectionInfo.transform.FindChild(parentGO+"txtLastTripTime").GetComponent<Text>();
			txtLastTripTimeSteps = goCanvasConnectionInfo.transform.FindChild(parentGO+"txtLastTripTimeSteps").GetComponent<Text>();
			txtLastTripTimeSteps2 = goCanvasConnectionInfo.transform.FindChild(parentGO+"txtLastTripTimeSteps2").GetComponent<Text>();

			txtAvarageTripTime = goCanvasConnectionInfo.transform.FindChild(parentGO+"txtAvarageTripTime").GetComponent<Text>();
			txtAvarageTripTimeSteps = goCanvasConnectionInfo.transform.FindChild(parentGO+"txtAvarageTripTimeSteps").GetComponent<Text>();
			txtAvarageTripTimeSteps2 = goCanvasConnectionInfo.transform.FindChild(parentGO+"txtAvarageTripTimeSteps2").GetComponent<Text>();

			txtMaxTripTime = goCanvasConnectionInfo.transform.FindChild(parentGO+"txtMaxTripTime").GetComponent<Text>();
			txtMaxTripTimeSteps = goCanvasConnectionInfo.transform.FindChild(parentGO+"txtMaxTripTimeSteps").GetComponent<Text>();
			txtMaxTripTimeSteps2 = goCanvasConnectionInfo.transform.FindChild(parentGO+"txtMaxTripTimeSteps2").GetComponent<Text>();
		}
	}

	void UpdateConnectionInfo()
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
			return;
		if(!showConnectionInfo)
			return;

		if(!initialComplete)
		{
			//TODO !!!
			// initialisierung der UI Ebene nach instanzierung von character!!!! SONST gibts hier exceptions!!
			Start ();
		}

		// beste wär wenn server tripTime von anderem Client ebenfalls sendet: otherClient -> server -> this client
		// aktuell nur server -> this client

		if( Network.isServer)
			return;

		if( ownerScript.owner == Network.player)
		{
			txtLastTripTime.text = "LTT:\n" + ((double)(stateBuffer[0].tripTime)*1000).ToString("#### ms");
			txtLastTripTimeSteps.text = "LTTS:\n" + ((double)(stateBuffer[0].tripTime/Time.fixedDeltaTime)).ToString("#.#");
			txtLastTripTimeSteps2.text = "LTTS2:\n" +  (GetPastState(Network.time - stateBuffer[0].tripTime)+1).ToString();
			// kann sein das LastTripTime nicht im avgTripTime eingerechnet wurde da der avg zum anderen zeitpunkt berechnet wird und da das neuste paket noch nicht angekommen ist. 
			//TODO // lösung -> avg immer bei ankommendem paket berechnen!
			//TODO // LÖSUNG v2: static avgTripTime ... eine TripTime für alle NetworkedPlayer!

			txtAvarageTripTime.text = "avgTT:\n" + ((double)(avgTripTime*1000)).ToString("#### ms");
			txtAvarageTripTimeSteps.text = "avgTTS:\n" + ((double)(avgTripTime/Time.fixedDeltaTime)).ToString("#.#");
			txtAvarageTripTimeSteps2.text = "avgLTTS2:\n" + ((GetPastState(Network.time - avgTripTime)+1)).ToString();

			txtMaxTripTime.text = "maxTT:\n" + maxTripTime.ToString("0.###");
			txtMaxTripTimeSteps.text = "maxTTS:\n" + ((double)(maxTripTime/Time.fixedDeltaTime)).ToString("#");
			txtMaxTripTimeSteps2.text = "maxTTS2:\n" + (GetPastState(Network.time - maxTripTime)+1).ToString();
		}
	}

	void OnDestroy()
	{

		if(goCanvasConnectionInfo != null)
		{
			Destroy (goCanvasConnectionInfo);
		}
	}

}
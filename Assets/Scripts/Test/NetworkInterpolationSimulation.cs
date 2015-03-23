using UnityEngine;
using System.Collections;

public class NetworkInterpolationSimulation : MonoBehaviour {

	Transform characterPredictionBoxCollider;
	Transform characterLastRecvedPos;

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
			this.Timestamp = time;
			this.tripTime = 0.02;
		}
	}


	// we'll keep a buffer of 20 states
	networkState[] simulatedStatesBuffer = new networkState[ 20 ];
	networkState[] stateBuffer = new networkState[ 20 ];

	int stateCount = 0; // how many states have been recorded


	void InitializeNetworkStates()
	{
		float x=0f, y=0f, z=0f; 
		double sendTime = 0;
		float inputHorizontal = 0;
		bool inputJump = false;

		double sendUpdateRate = (double)(1.0 / 20.0); 

		for(int i=0; i< simulatedStatesBuffer.Length; i++)
		{
			SimulateMove();
			simulatedStatesBuffer [i] = new networkState (transform.position, sendTime+i*sendUpdateRate, inputHorizontal, inputJump);
		}

		// stimmen zeitabstände mit position überein?

		StopSimulation();

//		stateBuffer [0] = new networkState (new Vector3 (x,y,z),	0.950, 0f, false);
//		stateBuffer [1] = new networkState (new Vector3 (x,y,z),	0.900, 0f, false);
//		stateBuffer [2] = new networkState (new Vector3 (x,y,z),	0.850, 0f, false);
//		stateBuffer [3] = new networkState (new Vector3 (x,y,z),	0.800, 0f, false);
//		stateBuffer [4] = new networkState (new Vector3 (x,y,z),	0.750, 0f, false);
//		stateBuffer [5] = new networkState (new Vector3 (x,y,z),	0.700, 0f, false);
//		stateBuffer [6] = new networkState (new Vector3 (x,y,z),	0.650, 0f, false);
//		stateBuffer [7] = new networkState (new Vector3 (x,y,z),	0.600, 0f, false);
//		stateBuffer [8] = new networkState (new Vector3 (x,y,z),	0.550, 0f, false);
//		stateBuffer [9] = new networkState (new Vector3 (x,y,z),	0.500, 0f, false);
//		stateBuffer [10] = new networkState (new Vector3 (x,y,z),	0.450, 0f, false);
//		stateBuffer [11] = new networkState (new Vector3 (x,y,z),	0.400, 0f, false);
//		stateBuffer [12] = new networkState (new Vector3 (x,y,z),	0.350, 0f, false);
//		stateBuffer [13] = new networkState (new Vector3 (x,y,z),	0.300, 0f, false);
//		stateBuffer [14] = new networkState (new Vector3 (x,y,z),	0.250, 0f, false);
//		stateBuffer [15] = new networkState (new Vector3 (x,y,z),	0.200, 0f, false);
//		stateBuffer [16] = new networkState (new Vector3 (x,y,z), 	0.150, 0f, false);
//		stateBuffer [17] = new networkState (new Vector3 (x,y,z), 	0.100, 0f, false);
//		stateBuffer [18] = new networkState (new Vector3 (x,y,z), 	0.050, 0f, false);
//		stateBuffer [19] = new networkState (new Vector3 (x,y,z),	0.000, 0f, false);
	}

	PlatformUserControl inputScript;
	PlatformCharacter characterScript;

	void SimulateMove()
	{
		inputScript.inputHorizontal = 1f;
		inputScript.inputJump = false;
		characterScript.Simulate();
	}

	void StopSimulation()
	{
		inputScript.inputJump = false;
		inputScript.inputHorizontal = 0f;
	}

	// Use this for initialization
	void Start () {
		characterScript = GetComponent<PlatformCharacter> ();
		inputScript = GetComponent<PlatformUserControl> ();
		inputScript.simulate = true;								// disables input reading

		characterPredictionBoxCollider = transform.Find("BoxCollider");		// looks in transform childs
		//transform.FindChild
		// GameObject.Find looks in compete Scene!
		characterLastRecvedPos = transform.Find("LastRecvedPos");
	}

	void ShowNetworkStates()
	{
		for(int i=0; i< stateBuffer.Length; i++)
		{
			Debug.Log("stateBuffer[" + i + "] " + stateBuffer[i].Position +","+stateBuffer[i].Timestamp+","+stateBuffer[i].tripTime);
		}
	}

	bool firstUpdate = true;
	bool useExtrapolation = true;
	int extrapolationCount = 0;
	double InterpolationBackTime = 0.05f;
	double ExtrapolationLimit = 0.5f;

	// Update is called once per frame
	void Update () {
		if(firstUpdate)
		{
			InitializeNetworkStates ();
			ShowNetworkStates ();
			firstUpdate = false;
			transform.position = Vector3.zero;
		}

		if( stateCount == 0 ) return; // no states to interpolate
		
		double currentTime = updateTime;
		double interpolationTime = currentTime - InterpolationBackTime;
		currentInterpolationTime = interpolationTime;
		characterLastRecvedPos.position = stateBuffer[0].Position;
		
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
					currLHS = lhs;
					
					// the state one slot newer
					//networkState rhs = stateBuffer[ Mathf.Max( i - 1, 0 ) ];
					networkState rhs = stateBuffer[ 0 ];								// position should be more precise
					currRHS = rhs;

					// use time between lhs and rhs to interpolate
					double length = rhs.Timestamp - lhs.Timestamp;
					float t = 0f;
					if( length > 0.0001 )
					{
						t = (float)( ( interpolationTime - lhs.Timestamp ) / length );		// needs fix
						lerping = true;
					}
					else
					{
						lerping = false;
					}
					
//					if(Vector3.Distance(lhs.Position,rhs.Position) > 5)
//					{
//						// position changed dramatically (beam)
//						transform.position = rhs.Position;
//					}
//					else
//					{
						// Vector3.Lerp ( from, to, fraction )
						//Linearly interpolates between two vectors.
						//Interpolates between from and to by the fraction t.
						//This is most commonly used to find a point some fraction
						//of the way along a line between two endpoints (eg, to move
						//an object gradually between those points). This fraction is
						//clamped to the range [0...1]. When t = 0 returns FROM.
						//When t = 1 returns TO. When t = 0.5 returns the point midway between from and to.
						transform.position = Vector3.Lerp( lhs.Position, rhs.Position, t );
//					}
					break;
				}
			}
		}
//		else
//		{
//			// Extrapolation (since 100 ms no update received!)
//			//			Debug.Log("Extrapolation");
//			if(useExtrapolation)
//			{
//				extrapolationCount++;
//				networkState latest = stateBuffer[0];
//				float extrapolationLength = (float)(interpolationTime - latest.Timestamp);		// (Network.time-100ms) - (lastest.time), mit latest.time < network.time-100ms
//				// ergebnis immer positiv, größer 0
//				// extrapolationLenght > 0
//				
//				// Don't extrapolation for more than 500 ms, you would need to do that carefully
//				if (extrapolationLength < ExtrapolationLimit)
//				{
//					Vector3 moveDirection = new Vector3(latest.InputHorizontal * characterScript.getMaxSpeed() * Time.fixedDeltaTime,0f,0f);
//					transform.position = latest.Position + moveDirection * extrapolationLength;
//				}
//			}
//		}


	}


	private float updateTime = 0.1f;			// test with updateTime < interpolationTime => -x
	private float deltaUpdateTime = 0.01f;
	private double currentInterpolationTime;

	private networkState currLHS;
	private networkState currRHS;

	public bool lerping = false;

	void OnGUI()
	{
		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		if(GUILayout.Button("Next Update()"))
		{
			updateTime += deltaUpdateTime;
		}

		if(GUILayout.Button("Last Update()"))
		{
			updateTime -= deltaUpdateTime;
		}
		GUILayout.EndVertical();

		GUILayout.Box("UpdateTime = " + updateTime + "\n" +
		              "InterpolationTime = " + currentInterpolationTime);

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		if(GUILayout.Button("Next OnNetworkSerializUpdate()"))
		{
			if(stateCount < stateBuffer.Length)
			{
				//stateCount ++;
				//int index = simulatedStatesBuffer.Length - stateCount;
				int index = stateCount;
				netUpdate(simulatedStatesBuffer[index].Position,
				          simulatedStatesBuffer[index].InputHorizontal,
				          simulatedStatesBuffer[index].InputJump,
				          updateTime);
			}
		}

		if(GUILayout.Button("Last OnNetworkSerializUpdate()"))
		{
			if(stateCount > 0)
				stateCount --;
		}
		GUILayout.EndVertical();
		GUILayout.Box("stateCount = " + stateCount + "\n" +
		              "stateBuffer.Length = " + stateBuffer.Length );

		if(true)//currLHS != null)
		{
			GUILayout.Box("LHS:\n " + currLHS.Timestamp);
		}
		if(true)//currRHS != null)
		{
			GUILayout.Box("RHS:\n " + currRHS.Timestamp);
		}

		GUILayout.Box("Lerping: " + lerping);

		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}



	void netUpdate( Vector3 position, float inputHorizontal, bool inputJump, double timestamp )
	{
		// this package has new information and will be buffered
		bufferState( new networkState( position, timestamp, inputHorizontal, inputJump ) );

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

}

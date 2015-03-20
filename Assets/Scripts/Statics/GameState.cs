using UnityEngine;
using System.Collections;

public class GameState {

	public enum States {
		Pause,
		Initializing,				// Init									// check playerDictionary != null
		LateInitializing,			// Late Init (dependencies from Init)	// read playerDictionary		MasterClient: which Prefabs need to get Spawned, wait for all to LoadGameScene
		PrepareingStart,			// Camera: Level Introduction
		Starting,					// Camera: Player Introduction / 3,2,1 Animation
		Running,					// Camera: GameCamMode                  // read playerDictionary (OnGUI stats)
		PrepareingEnd,				// Calculate Scoretable, summerize actions (statistica who, whom, how & how many time )
		Ending,						// Show Winner and Statistics
		Destroying					// Destroy the level
	}

	public static States currentState = States.Running;
	
	public static void ChangeState(States stateTo) {
		if(currentState == stateTo)
			return;
		currentState = stateTo;
	}
	
	public static bool IsState(States stateTo) {
		if(currentState == stateTo)
			return true;
		return false;
	}
	
	public static bool IsRunning {
		get {
			return IsState(States.Running);
		}
	}
	
	public static bool IsPaused {
		get {
			return IsState(States.Pause);
		}
	}
	
	// You can still do this but will need GameState.Running = true;
	// ChangeState is more atomic...
	public static bool Running {
		get {
			return IsState(States.Pause);
		}
		set {
			if(value)
                ChangeState(States.Running);
        }
    }
    // ...
}

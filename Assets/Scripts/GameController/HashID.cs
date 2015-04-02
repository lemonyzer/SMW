using UnityEngine;
using System.Collections;

public class HashID : MonoBehaviour {

	// Parameter
	public static string p_hSpeed = "hSpeed";
	public static string p_vSpeed = "vSpeed";
	public static string p_grounded = "Grounded";
	public static string p_walled = "Walled";
	public static string p_hitted = "Hitted";
	public static string p_gameOver = "GameOver";
	public static string p_headJumped = "HeadJumped";
	public static string p_spawn = "Spawn";
	public static string p_dead = "Dead";
	public static string p_hitTrigger = "HitTrigger";
	public static string p_spawnProtection = "SpawnProtection";
	public static string p_changeRunDirectionTrigger = "ChangeRunDirection";
	public static string p_rageTrigger = "RageTrigger";
	public static string p_rageModus = "RageModus";
	public static string p_nextStateTrigger = "NextStateTrigger";

	// States
	public static string s_Idle = "Idle";
	public static string s_JumpAndFall = "JumpAndFall";
	public static string s_Run = "Run";
	public static string s_ChangeRunDirection = "ChangeRunDirection";

	public static string s_Hitted = "Hitted";
	public static string s_HeadJumped = "HeadJumped";
	public static string s_Dead = "Dead";
	public static string s_GameOver = "GameOver";

	public static string s_Generic_Spawn = "_Generic_Spawn";
	public static string s_Generic_SpawnProtection = "_Generic_SpawnProtection";

	public static string s_RageMode = "_Generic_RageMode";


	/**
	 * Platform Character
	 **/
	public static int hittedState;
	public static int deadState;
	public static int dieState;
	public static int headJumpedState;
	public static int spawnState;
	public static int spawnProtectionState;
	public static int idleState;
	public static int runState;
	public static int changeRunDirectionState;
	public static int jumpState;
	
	public static int hSpeedFloat;
	public static int vSpeedFloat;
	public static int changeRunDirectionTrigger;
	
	public static int groundedBool;
	public static int walledBool;
	
	public static int hittedBool;
	public static int gameOverBool;
	public static int headJumpedBool;
	public static int spawnBool;
	public static int deadBool;
	public static int hitTrigger;
	public static int spawnProtectionBool;
	public static int nextStateTrigger;

	/**
	 * Breakable IceBlock
	 **/
	public static int icedTrigger;
	public static int iceBlockBreakTrigger;
	public static int iceBlockMeltTrigger;

	
	/**
	 * Platform PowerUpBlock
	 **/
	public static int powerUpBlockReleaseTrigger;
	public static int powerUpBlockLoadedTrigger;
	public static int hasPowerUpBool;
	
	/**
	 * CountDown
	 **/
	public static int startCountDownTrigger;
	public static int countDownEnabledBool;
	
	void Awake() {
		
		/**
		 * Platform Character
		 **/
		hittedState =			Animator.StringToHash("Base Layer.Hitted");
		deadState = 			Animator.StringToHash("Base Layer.Dead");
		dieState = 				Animator.StringToHash("Base Layer.Die");
		headJumpedState = 		Animator.StringToHash("Base Layer.HeadJumped");
		spawnState = 			Animator.StringToHash("Base Layer._Generic_Spawn");
		spawnProtectionState = 	Animator.StringToHash("Base Layer._Generic_SpawnProtection");
		idleState = 			Animator.StringToHash("Base Layer.Idle");
		runState = 				Animator.StringToHash("Base Layer.Run");
		changeRunDirectionState = Animator.StringToHash("Base Layer.ChangeRunDirection");
		jumpState = 			Animator.StringToHash("Base Layer.JumpAndFall");
		
		hSpeedFloat = Animator.StringToHash("hSpeed");
		vSpeedFloat = Animator.StringToHash("vSpeed");
		changeRunDirectionTrigger = Animator.StringToHash("ChangeRunDirection");
		
		groundedBool = Animator.StringToHash("Grounded");
		walledBool = Animator.StringToHash("Walled");
		
		hittedBool = Animator.StringToHash("Hitted");
		gameOverBool = Animator.StringToHash("GameOver");
		headJumpedBool = Animator.StringToHash("HeadJumped");
		spawnBool = Animator.StringToHash("Spawn");
		deadBool = Animator.StringToHash("Dead");
		hitTrigger = Animator.StringToHash("HitTrigger");
		spawnProtectionBool = Animator.StringToHash("SpawnProtection");
		nextStateTrigger = Animator.StringToHash("NextStateTrigger");
		
		/**
		 * Platform PowerUpBlock
		 **/
		hasPowerUpBool = Animator.StringToHash("hasPowerUp");
		powerUpBlockReleaseTrigger = Animator.StringToHash("PowerUpBlockReleaseTrigger");
		powerUpBlockLoadedTrigger = Animator.StringToHash("PowerUpBlockLoadedTrigger");
		
		/**
		 * CountDown
		 **/
		startCountDownTrigger = Animator.StringToHash("startCountDown");
		countDownEnabledBool = Animator.StringToHash("CountDownEnabled");

		/**
		 * Breakable IceBlock
		 **/
		iceBlockBreakTrigger = Animator.StringToHash("Break");
		iceBlockMeltTrigger = Animator.StringToHash("Melt");
		icedTrigger = Animator.StringToHash("Iced");
		
	}
}

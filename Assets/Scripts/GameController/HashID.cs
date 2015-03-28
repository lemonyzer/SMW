using UnityEngine;
using System.Collections;

public class HashID : MonoBehaviour {
	
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
		
	}
}

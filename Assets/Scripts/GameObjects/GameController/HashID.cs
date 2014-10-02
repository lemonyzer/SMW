using UnityEngine;
using System.Collections;

public class HashID : MonoBehaviour {
	
	/**
	 * Platform Character
	 **/
	public int hittedState;
	public int deadState;
	public int dieState;
	public int headJumpedState;
	public int spawnState;
	public int spawnProtectionState;
	public int idleState;
	public int runState;
	public int changeRunDirectionState;
	public int jumpState;
	
	public int hSpeedFloat;
	public int vSpeedFloat;
	public int changeRunDirectionTrigger;
	
	public int groundedBool;
	public int walledBool;
	
	public int hittedBool;
	public int gameOverBool;
	public int headJumpedBool;
	public int spawnBool;
	public int deadBool;
	public int hitTrigger;
	public int spawnProtectionBool;
	public int nextStateTrigger;
	
	/**
	 * Platform PowerUpBlock
	 **/
	public int powerUpBlockReleaseTrigger;
	public int powerUpBlockLoadedTrigger;
	public int hasPowerUpBool;
	
	/**
	 * CountDown
	 **/
	public int startCountDownTrigger;
	public int countDownEnabledBool;
	
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

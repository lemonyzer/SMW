using UnityEngine;
using System.Collections;

public class Tags : MonoBehaviour {

	public const string guiAnalogStick = "AnalogStick";
	public const string guiStick = "Stick";

	/**
	 * Body Parts (Parent and Children)
	 **/
	public const string player = "Player";
	public const string cloneLeft = "Clone Left";
	public const string cloneRight = "Clone Right";
	public const string head = "Head";
	public const string body = "Body";
	public const string feet = "Feet";
	public const string itemCollector = "ItemCollector";
	public const string powerUpHitArea = "PowerUpHitArea";
	public const string groundStopper = "GroundStopper";

	//public const string boxCollider = "BoxCollider";
	public const string lastReceivedPos = "LastRecvedPos";
	public const string CurrentEstimatedPosOnServer = "CurrentEstimatedPosOnServer";
	public const string PredictedPosSimulatedWithLastInput = "PredictedPosSimulatedWithLastInput";
	public const string PredictedPosCalculatedWithLastInput = "PredictedPosCalculatedWithLastInput";
	public const string PredictedPosV3 = "PredictedPosV3";
	public const string iceWalled = "IceWalled";

	/**
	 * Scene, Level Parts
	 **/
	public const string gameController = "GameController";
//	public const string itemLibrary = "ItemLibrary";
	public const string background = "Background";
	public const string invincibleSound = "InvincibleSound";
	public const string powerUp = "PowerUp";
	public const string fader = "Fader";
	public const string countDown = "CountDown";

	/**
	 * Character Selection
	 **/
	public const string character = "Character";
	public const string ai = "AI";
	public const string death = "Death";
	public const string enemy = "Enemy";

	
	/**
	 * Team Selection
	 **/
	public const string Team1 = "Team1";
	public const string TeamRed = "TeamRed";

	public const string Team2 = "Team2";
	public const string TeamGreen = "TeamGreen";

	public const string Team3 = "Team3";
	public const string TeamYellow = "TeamYellow";

	public const string Team4 = "Team4";
	public const string TeamBlue = "TeamBlue";

	
}

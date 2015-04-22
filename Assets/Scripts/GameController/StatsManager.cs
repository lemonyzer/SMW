using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour {

	public delegate void OnPvPKill(Player killer, Player victim);
	public static event OnPvPKill onPvPKill;

	void OnEnable()
	{

		PlatformCharacter.onHeadJump += HeadJump;
		PlatformCharacter.onRageKill += InvincibleAttack;
		PlatformCharacter.onProjectileHit += BulletHit;
		PlatformCharacter.onBulletHit += BulletHit;
		PlatformCharacter.onBombHit += BombHit;

//		SendDamageTrigger.onHeadJump += HeadJump;
//		RageTrigger.onRageKill += InvincibleAttack;
//		AuthoritativeFireBall.onBulletHit += BulletHit;
//		AuthoritativeBomb.onBombHit += BombHit;
	}
	
	void OnDisable()
	{

		PlatformCharacter.onHeadJump -= HeadJump;
		PlatformCharacter.onRageKill -= InvincibleAttack;
		PlatformCharacter.onProjectileHit -= BulletHit;
		PlatformCharacter.onBulletHit -= BulletHit;
		PlatformCharacter.onBombHit -= BombHit;

//		SendDamageTrigger.onHeadJump -= HeadJump;
//		RageTrigger.onRageKill -= InvincibleAttack;
//		AuthoritativeFireBall.onBulletHit -= BulletHit;
//		AuthoritativeBomb.onBombHit -= BombHit;
		
	}

	public bool gameRunning = false;
	public bool gameHasWinner = false;
	private GameObject gameWinner;

	public static GameMode currentGameMode = GameMode.Classic;
	public int gameModePointLimit = 10;
	
	public enum GameMode
	{
		Classic,
		ClassicReverse,
		CatchMe,
		CTF,
		Tagging,
		RaceCheckPoints,
		RaceRaskulls,
		FeedYoshi,
		Survival,
		FlappyBird
	}

	private int damageValueHeadJump = 1;
	private int pointValueHeadJump = 1;
	
	private int slotsCount;
	private int teamsCount;

	private GameObject backGround;
	public GameObject winEffect;
	public GameObject winEffect2;
	public AudioClip winSound;


	NetworkView myNetworkView;

	void Awake()
	{

		backGround = GameObject.FindGameObjectWithTag(Tags.tag_background);
		if(backGround == null)
		{
			Debug.LogError("no Background found!");
		}

//		currentGameMode = GameMode.Classic;

		gameHasWinner = false;
		gameRunning = true;

		myNetworkView = GetComponent<NetworkView>();
	}

	bool PointLimitReached(int currentPoints)
	{
//		if(currentGameMode == GameMode.Classic)
//		{
			if(currentPoints >= gameModePointLimit)
			{
				return true;
			}
//		}
		return false;
	}

	void GameHasAWinner(GameObject player)
	{
		gameRunning = false;
		gameHasWinner = true;
		gameWinner = player;
		WinnerEffect();
		//gameWinner = spawnScript.playerDictonary[player].getCharacter().getPrefab();
	}

	void WinnerEffect()
	{
//		Debug.Log("WinnerEffect started");
//		backGround.renderer.enabled = false;
		if(winEffect != null)
		{
			winEffect.SetActive(true);
			winEffect.transform.position = gameWinner.transform.position;
			winEffect.transform.parent = gameWinner.transform;
			winEffect.GetComponent<Renderer>().sortingLayerID = SortingLayer.int_GUI;
		}
		if(winEffect2 != null)
		{
			winEffect2.SetActive(true);
			winEffect2.transform.position = gameWinner.transform.position;
			winEffect2.transform.parent = gameWinner.transform;
			foreach(Transform child in winEffect2.transform)
			{
				child.GetComponent<Renderer>().sortingLayerID = SortingLayer.int_GUI;
			}
		}
		if(winSound != null) {
			transform.GetComponent<AudioSource>().Stop();
			AudioSource.PlayClipAtPoint(winSound,transform.position,1);
		}
	}


	void AddKill(Player killer, Player victim)
	{
		//if(!combo)
		//AddPoint(killer, pointValueHeadJump);
		if(currentGameMode == GameMode.Classic)
		{
			killer.addPoints(pointValueHeadJump);
			killer.addKill();
			victim.LostLife();
		}
		else if(currentGameMode == GameMode.ClassicReverse)
		{
			killer.addPoints(pointValueHeadJump);
			killer.addKill();
//			victim.addHealth(-damageValueHeadJump);
			victim.LostLife();
		}

		if(onPvPKill != null)
			onPvPKill(killer, victim);
	}

	public void HeadJump(GameObject attacker, GameObject victim)
	{
		if(myNetworkView != null)
		{
			if(!Network.isServer)
			{
				return;
			}
		}
		// wird nur von PhotonMasterClient ausgeführt....

		if(GameState.currentState == GameState.States.Running)
		{
			NetworkPlayer attackersRealOwner = attacker.GetComponent<RealOwner>().owner;
			NetworkPlayer victimsRealOwner = victim.GetComponent<RealOwner>().owner;

			myNetworkView.RPC("SyncHeadJump", RPCMode.AllBuffered, attackersRealOwner, victimsRealOwner);
		}
		else
		{
			Debug.LogWarning("current GameState = " + GameState.currentState.ToString() + " HeadJump zählt nicht!");
		}
	}

	[RPC]
	public void SyncHeadJump(NetworkPlayer attackersRealOwner, NetworkPlayer victimsRealOwner)
	{
		//if(GameState.currentState == GameState.States.Running)
        //{
		if(attackersRealOwner != null &&
		   victimsRealOwner != null)
		{
			Player playerAttacker;
			Player playerVictim;

			bool foundAttacker = false;
			bool foundVictim = false;

			if (PlayerDictionaryManager._instance.TryGetPlayer(attackersRealOwner, out playerAttacker))
			{
				foundAttacker = true;
			}
			else
			{
				Debug.LogError("attackersRealOwner = " + attackersRealOwner.ToString() + " existiert nicht (mehr) in playerDictionary");
			}
			if (PlayerDictionaryManager._instance.TryGetPlayer(victimsRealOwner, out playerVictim))
			{
				foundVictim = true;
				
			}
			else
			{
				Debug.LogError("victimsRealOwner = " + victimsRealOwner.ToString() + " existiert nicht (mehr) in playerDictionary");
			}
		
			//AnimatorController victimsAnimationController = playerVictim.getCharacter().getGameObject().GetComponent<AnimatorController>();
			//victimsAnimationController.HeadJumpAnimation();
//TODO			PlatformCharacter victimCharacterScript = playerVictim.getCharacter().getGameObject().GetComponent<PlatformCharacter>();
//TODO			victimCharacterScript.HeadJumpVictim();

			if (foundVictim)
			{
				if (playerVictim.platformCharacterScript == null)
				{
					Debug.LogError("playerVictim " + victimsRealOwner.ToString() + " platformCharacterScript == NULL!");
				}
				else
					playerVictim.platformCharacterScript.HeadJumpVictim();
			}

			AddKill(playerAttacker, playerVictim);
		}
		else
		{
			Debug.LogError("Character ohne RealOwner beteiligt! BOT?");
		}
		//}
    }

	public void InvincibleAttack(GameObject attacker, GameObject victim)
	{
		if(myNetworkView != null)
		{
			if(!Network.isServer && Network.peerType != NetworkPeerType.Disconnected)
			{
				return;
			}
		}
		if(Network.peerType == NetworkPeerType.Disconnected)
			return;
		// wird nur von PhotonMasterClient ausgeführt....
		if(GameState.currentState == GameState.States.Running)
		{
			NetworkPlayer attackersRealOwner = attacker.GetComponent<RealOwner>().owner;
			NetworkPlayer victimsRealOwner = victim.GetComponent<RealOwner>().owner;

			if(server())
				myNetworkView.RPC("SyncInvincibleAttack", RPCMode.AllBuffered, attackersRealOwner, victimsRealOwner);

			if(offline())
			{
//				AnimatorController victimsAnimationController = victim.GetComponent<AnimatorController>();
//				victimsAnimationController.InvincibleAttackAnimation();
//				AddKill(attacker,victim);
			}
        }
		else
		{
			Debug.LogWarning("current GameState = " + GameState.currentState.ToString() + " InvincibleAttack zählt nicht!");
		}
	}


	[RPC]
	public void SyncInvincibleAttack(NetworkPlayer attackersRealOwner, NetworkPlayer victimsRealOwner)
	{
		//if(GameState.currentState == GameState.States.Running)
		//{
		Player playerAttacker;
		Player playerVictim;
		PlayerDictionaryManager._instance.TryGetPlayer(attackersRealOwner, out playerAttacker);
		PlayerDictionaryManager._instance.TryGetPlayer(victimsRealOwner, out playerVictim);
			
		//AnimatorController victimsAnimationController = playerVictim.getCharacter().getGameObject().GetComponent<AnimatorController>();
		//victimsAnimationController.InvincibleAttackAnimation();

//TODO		PlatformCharacter victimCharacterScript = playerVictim.getCharacter().getGameObject().GetComponent<PlatformCharacter>();
//TODO		victimCharacterScript.InvincibleAttackVictim();
		playerVictim.platformCharacterScript.InvincibleAttackVictim();
		
		AddKill(playerAttacker, playerVictim);
        //}
    }


	
	public void BulletHit(GameObject bulletOwner, GameObject victim)					// wenn spieler scene verlässt oder character wechselt, müssen alle bullets entfernt werden!!!
	{
		if(myNetworkView != null || Network.peerType != NetworkPeerType.Disconnected)
		{
			if(!Network.isServer)
			{
				return;
			}
		}
		if(myNetworkView == null)
		{
			Debug.Log("keine NetworkView!!!!");
			return;
		}
		// wird nur von PhotonMasterClient ausgeführt....
		
		if(GameState.currentState == GameState.States.Running)
		{
			NetworkPlayer attackersRealOwner = bulletOwner.GetComponent<RealOwner>().owner;
			NetworkPlayer victimsRealOwner = victim.GetComponent<RealOwner>().owner;
			
			myNetworkView.RPC("SyncBulletHitAnimation", RPCMode.All, attackersRealOwner, victimsRealOwner);
			myNetworkView.RPC("SyncBulletHit", RPCMode.AllBuffered, attackersRealOwner, victimsRealOwner);
		}
		else
		{
			Debug.LogWarning("current GameState = " + GameState.currentState.ToString() + " BulletHit zählt nicht!");
		}
	}

	public void BombHit(GameObject bulletOwner, GameObject victim)					// wenn spieler scene verlässt oder character wechselt, müssen alle bullets entfernt werden!!!
	{
		if(myNetworkView != null || Network.peerType != NetworkPeerType.Disconnected)
		{
			if(!Network.isServer)
			{
				return;
			}
		}
		if(myNetworkView == null)
		{
			Debug.Log("keine NetworkView!!!!");
			return;
		}
		// wird nur von PhotonMasterClient ausgeführt....
		
		if(GameState.currentState == GameState.States.Running)
		{
			NetworkPlayer attackersRealOwner = bulletOwner.GetComponent<RealOwner>().owner;
			NetworkPlayer victimsRealOwner = victim.GetComponent<RealOwner>().owner;
			
			myNetworkView.RPC("SyncBulletHitAnimation", RPCMode.All, attackersRealOwner, victimsRealOwner);
			myNetworkView.RPC("SyncBulletHit", RPCMode.AllBuffered, attackersRealOwner, victimsRealOwner);
		}
		else
		{
			Debug.LogWarning("current GameState = " + GameState.currentState.ToString() + " BulletHit zählt nicht!");
		}
	}

	[RPC]
	public void SyncBulletHitAnimation(NetworkPlayer attackersRealOwner, NetworkPlayer victimsRealOwner)
	{
		//if(GameState.currentState == GameState.States.Running)
		//{
//			TODO animation für attacker (combos, nemesis, blood...)
//			Player playerAttacker = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary.GetPlayer(attackersRealOwner);
		Player playerVictim;
		PlayerDictionaryManager._instance.TryGetPlayer(victimsRealOwner, out playerVictim);
			
			//AnimatorController victimsAnimationController = playerVictim.getCharacter().getGameObject().GetComponent<AnimatorController>();
			//victimsAnimationController.InvincibleAttackAnimation();

//TODO		PlatformCharacter victimCharacterScript = playerVictim.getCharacter().getGameObject().GetComponent<PlatformCharacter>();
//TODO		victimCharacterScript.InvincibleAttackVictim();
		playerVictim.platformCharacterScript.InvincibleAttackVictim();
			
			//AddKill(playerAttacker, playerVictim);
		//}
	}

	[RPC]
	public void SyncBulletHit(NetworkPlayer attackersRealOwner, NetworkPlayer victimsRealOwner)
	{
		Player playerVictim;
		PlayerDictionaryManager._instance.TryGetPlayer(victimsRealOwner, out playerVictim);

		Player playerAttacker;
		PlayerDictionaryManager._instance.TryGetPlayer(attackersRealOwner, out playerAttacker);
//		Player playerAttacker = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary.GetPlayer(attackersRealOwner);
//		Player playerVictim = PlayerDictionaryManager.syncedLocalPersistentPlayerDictionary.GetPlayer(victimsRealOwner);
		AddKill(playerAttacker, playerVictim);
	}


	bool isAuthoritativeHost()
	{
		if(offline ())
			return true;
		
		if(server ())
			return true;
		
		return false;
		
	}
	
	bool offline()
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
			return true;
		
		return false;
	}
	
	bool server()
	{
		if(Network.isServer)
			return true;
		
		return false;
	}
}

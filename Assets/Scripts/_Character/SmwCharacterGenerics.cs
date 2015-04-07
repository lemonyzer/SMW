using UnityEngine;
using System.Collections;

[System.Serializable]
public class SmwCharacterGenerics : ScriptableObject {

	// properties for all characters
	public AnimationClip spawnAnimClip;
	public AnimationClip protectionAnimClip;
	public AnimationClip rageAnimClip;

	// können nicht im inspector zugewiesen werden!
//	public SpawnStateScript spawnStateScript;
//	public SpawnDelayStateScript spawnDelayStateScript;
	
	public Sprite kingSprite;
	public RuntimeAnimatorController iceWandAnimatorController;
	
	public Color color_rootRenderer 						= new Color(1f,1f,1f,1f);		// ALL (ROOT SpriteRenderer)
	public Color color_rootCloneRenderer 					= new Color(1f,1f,1f,1f);		// ALL
	public Color color_kingRenderer		 					= new Color(1f,1f,1f,1f);		// ALL
	public Color color_iceWallRenderer	 					= new Color(1f,1f,1f,1f);		// ALL
	public Color color_currentEstimatedPosOnServer 			= new Color(1f,1f,1f,0.1f);	// localplayer Character's	only
	public Color color_LastRecvedPos 						= new Color(1f,1f,1f,0.1f);	// all other Character's	vergangene Position
	public Color color_PredictedPosSimulatedWithLastInput 	= new Color(1f,1f,1f,0.1f);	// all other Character's	vergangene Position
	public Color color_PredictedPosCalculatedWithLastInput 	= new Color(1f,1f,1f,0.1f);	// all other Character's	vergangene Position
	
	public int rootRendererSortingLayer;
	public string rootRendererSortingLayerName = SortingLayer.name_CharacterBackground;
	public int rootCloneRendererSortingLayer;
	public string rootCloneRendererSortingLayerName = SortingLayer.name_CharacterBackground;
	public int kingRendererSortingLayer;
	public string kingRendererSortingLayerName = SortingLayer.name_CharacterKing;
	public int iceWalledRendererSortingLayer;
	public string iceWalledRendererSortingLayerName = SortingLayer.name_CharacterForeground;
	public int currentEstimatedPosOnServerSortingLayer;
	public string currentEstimatedPosOnServerSortingLayerName = SortingLayer.name_CharacterForeground;
	public int lastRecvdPosRendererSortingLayer;
	public string lastRecvdPosRendererSortingLayerName = SortingLayer.name_CharacterForeground;
	public int preSimPosRendererSortingLayer;
	public string preSimPosRendererSortingLayerName = SortingLayer.name_CharacterForeground;
	public int preCalclastRecvdPosRendererSortingLayer;
	public string preCalclastRecvdPosRendererSortingLayerName = SortingLayer.name_CharacterForeground;

}

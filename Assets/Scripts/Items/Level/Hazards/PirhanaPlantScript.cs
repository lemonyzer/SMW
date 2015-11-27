using UnityEngine;
using System.Collections;

public class PirhanaPlantScript : MonoBehaviour {

	public SpriteRenderer hazardRenderer;
	public BoxCollider2D hazardCollider;
	public Hazard hazard;

	public void CreateHazard (Hazard hazard)
	{
		hazardCollider = this.gameObject.AddComponent <BoxCollider2D> ();
		hazardRenderer = this.gameObject.AddComponent <SpriteRenderer> ();
		hazardRenderer.sprite = hazard.previewSprite;
		hazardRenderer.sortingLayerName = "MapDebug";
		this.hazard = hazard;
	}
}

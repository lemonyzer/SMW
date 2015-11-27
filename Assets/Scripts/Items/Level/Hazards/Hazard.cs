using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Hazard {

	[SerializeField]
	public HazardType type;
	[SerializeField]
	public List<Sprite> projectile;

	[SerializeField]
	public Sprite previewSprite;
}

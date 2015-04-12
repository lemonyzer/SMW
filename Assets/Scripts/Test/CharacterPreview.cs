using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//TODO ist default enabled = false!

[RequireComponent (typeof (SpriteRenderer))]
public class CharacterPreview : MonoBehaviour {

	public Transform myTransform;
	public GameObject myGO;
	public Sprite[] run;
	public NetworkPlayer netPlayerOwner;

	[SerializeField]
	private Button btnUserNameAndColor;
	[SerializeField]
	private Text txtUserName;

	int currentSpriteIndex = 0;
	SpriteRenderer spriteRenderer;

	float switchSpriteTime = 0.1f;
	float currentSpriteTime = 0f;


	//Awake:
	// just after a prefab is instantiated (even if this script is NOT enabled!
	// This function is always called before any Start functions and also just after a prefab is instantiated.
	//(If a GameObject is inactive during start up Awake is not called until it is made active, or a function in any script attached to it is called.)
	void Awake()
	{
//		Debug.LogWarning(this.ToString() + " Awake() I'm enabled = " + this.enabled.ToString());
		myTransform = this.transform;
		myGO = this.gameObject;
		spriteRenderer = this.GetComponent<SpriteRenderer>();
	}

	// Start: Start is called before the first frame update only if the script instance is enabled.
	void Start ()
	{
		if(run == null)
		{
			this.enabled = false;
		}
	}

	public void SetUserName(string userName)
	{
		if (btnUserNameAndColor != null)
		{
			btnUserNameAndColor.enabled = true;
			ColorBlock cb = btnUserNameAndColor.colors;
			cb.normalColor = Color.white;					// TODO color of team

			txtUserName.text = userName;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
		if(currentSpriteTime > switchSpriteTime)
		{
			currentSpriteTime = 0f;

			spriteRenderer.sprite = run[currentSpriteIndex++];
		}

		currentSpriteTime += Time.deltaTime;
		currentSpriteIndex = currentSpriteIndex % 2;
	}
}

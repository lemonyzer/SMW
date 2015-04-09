using UnityEngine;
using System.Collections;

public class ConnectButtonScript : MonoBehaviour {

	//TODO public delegate void ClickAction(string ip, int port);
	//TODO done in the Listener
	public delegate void ClickAction(ConnectButtonScript clickedButton);
	public static event ClickAction OnClicked;


	public string hostIp;
	public int hostPort;
	public string hostGuId;
	public bool useNat;
//	public bool useUid;

//	ConnectToGame ctg;
//
//	void Awake()
//	{
//		ctg = GameObject.Find("GUI").GetComponent<ConnectToGame>();
//	}

	public void Connect_Button()
	{
		if(OnClicked != null)
		{
			// we have event listeners
			OnClicked(this);
			//Debug.LogError(this.ToString() + "OnClicked() " + this.hostGuId + " " + this.hostIp);
			
			//TODO switch between direct connect and connect through MasterServer
			//TODO done in the Listener
//			if(useNat)
//			{
//				Debug.Log("connected with NAT");
//				ctg.ConnectWithGUID_Button(guid);
//			}
//			else
//			{
//				Debug.Log("connected without NAT, directly!");
//				ctg.ConnectWithIp_Button(hostip, hostport);
//			}
		}
		else
		{
			Debug.LogError(this.ToString() + "no OnClicked() listeners");
		}
	}
}

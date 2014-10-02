using UnityEngine;
using System.Collections;

public class RequireNetwork : MonoBehaviour
{
	void OnGUI()
	{
		if( Network.peerType == NetworkPeerType.Disconnected )
		{
			if( GUILayout.Button("Start Server (20 Client Slots + 'Serverslot')", GUILayout.Width(Screen.width), GUILayout.Height(Screen.height)))
			{
				Network.InitializeServer( 20, 25005, true );
			}
		}
	}
}
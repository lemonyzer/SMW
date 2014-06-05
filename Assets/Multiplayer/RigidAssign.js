﻿#pragma strict

function OnNetworkInstantiate (msg : NetworkMessageInfo) {
	if (networkView.isMine)
	{
		var _NetworkRigidbody : NetworkRigidbody = GetComponent("NetworkRigidbody");
		_NetworkRigidbody.enabled = false;
	}
	else
	{
		name += "Remote";
		var _NetworkRigidbody2 : NetworkRigidbody =	GetComponent("NetworkRigidbody");
		_NetworkRigidbody2.enabled = true;
	}
}
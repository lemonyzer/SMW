﻿#pragma strict

private var server_port : int = 5000;
private var server_ip : String;

// multicast
private var startup_port : int = 5100;
private var group_address : System.Net.IPAddress = System.Net.IPAddress.Parse ("224.0.0.224");
private var udp_client : System.Net.Sockets.UdpClient;
private var remote_end : System.Net.IPEndPoint;

function Start ()
{
//    // loaded elsewhere
//    if (station_id == "GameServer")
//        StartGameServer ();
//    else
//        StartGameClient ();
}

function StartGameServer ()
{
    // the Unity3d way to become a server
    var init_status = Network.InitializeServer (10, server_port, false);
    Debug.Log ("status: " + init_status);

    StartBroadcast ();
}

function StartGameClient ()
{
    // multicast receive setup
    remote_end = System.Net.IPEndPoint(System.Net.IPAddress.Any, startup_port);
    udp_client = System.Net.Sockets.UdpClient(remote_end);
    udp_client.JoinMulticastGroup (group_address);

    // async callback for multicast
    udp_client.BeginReceive (new System.AsyncCallback(ServerLookup), null);

    MakeConnection ();
}

function MakeConnection ()
{
    // continues after we get server's address
    while (!server_ip)
        yield;

    while (Network.peerType == NetworkPeerType.Disconnected)
    {
        Debug.Log ("connecting: " + server_ip +":"+ server_port);

        // the Unity3d way to connect to a server
        var error : NetworkConnectionError;
        error = Network.Connect (server_ip, server_port);

        Debug.Log ("status: " + error);
        yield WaitForSeconds (1);
    }
}

/******* broadcast functions *******/
function ServerLookup (ar : System.IAsyncResult)
{
    // receivers package and identifies IP
    var receiveBytes = udp_client.EndReceive (ar, remote_end);

    server_ip = remote_end.Address.ToString ();
    Debug.Log ("Server: " + server_ip);
}

function StartBroadcast ()
{
    // multicast send setup
    var udp_client = System.Net.Sockets.UdpClient();
    udp_client.JoinMulticastGroup (group_address);
    var remote_end = System.Net.IPEndPoint(group_address, startup_port);

    // sends multicast
    while (true)
    {
        var buffer = System.Text.Encoding.ASCII.GetBytes("GameServer");
        udp_client.Send(buffer, buffer.Length, remote_end);

        yield WaitForSeconds (1);
    }
}
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
	[Tooltip("The Ui Panel to let the user enter name, connect and play")]
	[SerializeField]
	private GameObject controlPanel;
	[Tooltip("The UI Label to inform the user that the connection is in progress")]
	[SerializeField]
	private GameObject progressLabel;
	
	[Tooltip(
		"The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
	[SerializeField]
	private byte maxPlayersPerRoom = 4;
	
	/// <summary>
	/// This client's version number. Users are separated from each other by _gameVersion
	/// </summary>
	private string _gameVersion = "1";

	private void Awake()
	{
		// #Critical
		// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	private void Start()
	{
		progressLabel.SetActive(false);
		controlPanel.SetActive(true);
	}


	/// <summary>
	/// Start the connection process.
	/// - If already connected, we attempt joining a random room
	/// - if not yet connected, Connect this application instance to Photon Cloud Network
	/// </summary>
	public void Connect()
	{
		progressLabel.SetActive(true);
		controlPanel.SetActive(false);
		
		// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
		if (PhotonNetwork.IsConnected)
		{
			// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
			PhotonNetwork.JoinRandomRoom();
		}
		else
		{
			// #Critical, we must first and foremost connect to Photon Online Server.
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = _gameVersion;
		}
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

		PhotonNetwork.JoinRandomRoom();
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		progressLabel.SetActive(false);
		controlPanel.SetActive(true);
		
		Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
	}
}

using UnityEngine;
using System.Collections;

//172.31.36.176 is the concordia ip address for local connection

public class NetworkManager : MonoBehaviour
{
	//public GameObject lobbyPlayerPrefab;
	public string playerName { get; set; }
	public int playerCount { get; set; }
	public GameObject[] playerPrefabs = new GameObject[2];
	private bool isChoice;
	private bool isOnline;

	public TextMesh mesh;
	public Font font;

	//Online Multiplayer Variables
	private const string typeName = "COMP476Network";
	private string gameName;
	private HostData[] hostList;						//list of servers

	//Local Multiplayer Variables
	private int portNumber;
	private string ipAddress;
	private string portNumber_string;

	void Awake() {
		DontDestroyOnLoad (this);
	}

	void Start() {
		ipAddress = "Enter IP Address";			//Put IPv4 address here
		portNumber_string = "35000";
		playerName = "Enter Player Name Here";
		gameName = "Enter Server Name Here";
		playerCount = 1;
	}

	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if(isChoice == false) {

				GUIStyle style2 = new GUIStyle();
				style2.alignment = TextAnchor.MiddleCenter;
				style2.fontSize = 96;
				style2.font = font;

				GUI.Label(new Rect(Screen.width/6, Screen.height/3 - 60, Screen.width/1.5f, 30), "<Color=red>" + mesh.text + "</Color>", style2);

				if(GUI.Button(new Rect(Screen.width/2.5f, Screen.height/3 + 30, 250, 30), "Online Multiplayer")) {
					isChoice = true;
					isOnline = true;
				}
				
				if(GUI.Button(new Rect(Screen.width/2.5f,Screen.height/3 + 60 ,250,30),"Local Multiplayer")) {
					isChoice = true;
					isOnline = false;
				}
			}
			
			//Online Multiplayer Menu
			if(isChoice && isOnline) {

				playerName = GUI.TextField(new Rect(Screen.width/2.5f,Screen.height/6,250,30),playerName);

				gameName = GUI.TextField(new Rect(Screen.width/2.5f,Screen.height/6 + 50, 250, 30), gameName);

				if (GUI.Button(new Rect(Screen.width/2.5f, Screen.height/6 + 100, 250, 30), "Start Server"))
					StartOnlineServer();
				
				if (GUI.Button(new Rect(Screen.width/2.5f, Screen.height/6 + 150, 250, 50), "Refresh Hosts"))
					RefreshHostList();
				
				if (hostList != null)
				{
					for (int i = 0; i < hostList.Length; i++)
					{
						if (GUI.Button(new Rect(Screen.width/1.5f, Screen.height/6 + 350, 250, 50), hostList[i].gameName))
							JoinServer(hostList[i]);
					}
				}
			}

			//Local Multiplayer Menu
			else if(isChoice && isOnline == false) {

				playerName = GUI.TextField(new Rect(Screen.width/2.5f,Screen.height/6,250,30),playerName);

				if (GUI.Button(new Rect(Screen.width/6, Screen.height/2.5f + 50, 250, 30), "Start Server"))
					StartLocalServer();
				
				ipAddress = GUI.TextField(new Rect(Screen.width/2.5f,Screen.height/6 + 100,250,30),ipAddress);
				
				portNumber_string = GUI.TextField(new Rect(Screen.width/2.5f,Screen.height/6 + 150,250,30),portNumber_string);
				
				if(GUI.Button(new Rect(Screen.width/1.5f,Screen.height/6 + 200,250,30), "Join"))
					JoinIP(ipAddress,portNumber_string);
			}
			
		}
	}

	//Server Functions
	public void StartOnlineServer()
	{
		Debug.Log ("Online Server created.");
		
		Network.InitializeServer(4, 35000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}
	
	
	public void StartLocalServer()
	{
		Debug.Log ("Local Server created.");
		
		Network.InitializeServer(4, 35000, !Network.HavePublicAddress());
	}
	
	void OnServerInitialized()
	{
		Application.LoadLevel(1);
		bgin ();
		bgin2 ();
	}


	//Client Functions
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	public void JoinServer(HostData hostData)
	{
		Debug.Log ("Joining Server.");
		Network.Connect(hostData);
	}

	public void JoinIP(string ip, string port) {
		portNumber = int.Parse(portNumber_string);
		Network.Connect (ip, portNumber);
	}

	void OnConnectedToServer()
	{
		Debug.Log ("Connected to server.");

		Network.SetSendingEnabled(0, false);	
		Network.isMessageQueueRunning = false; //disabled because it's not in correct scene yet.

		Application.LoadLevel (1);
	}
	
	//spawns player after level is loaded
	void OnLevelWasLoaded(int level) {
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0, true);	
	}

	private Player SpawnLobbyPlayer()
	{
		Player[] players = GameObject.FindObjectsOfType<Player> ();
		int playerNumber = players.Length + 1;
		Vector3 startPosition = Vector3.zero;
		GameObject player = (GameObject)Network.Instantiate(playerPrefabs[players.Length], startPosition, Quaternion.identity, 0)as GameObject;
		player.transform.name = "Player " + playerNumber;
		Player pl = player.GetComponent<Player> ();
		pl.transform.position = pl.startPosition;
		pl.playerName= playerName;
		pl.playerNumber = playerNumber;
		pl.gameObject.tag = "Player " + playerNumber;
		networkView.RPC ("setPlayerInfo", RPCMode.OthersBuffered, new object[]{pl.networkView.viewID,pl.playerNumber,pl.playerName,pl.startPosition});
		return pl;
	}

	[RPC] void setPlayerInfo(NetworkViewID id, int playerNumber,string playerName, Vector3 startPosition) {
		Player player = NetworkView.Find (id).GetComponent<Player> ();
		player.playerNumber = playerNumber;
		player.playerName = playerName;
		player.startPosition = startPosition;
		player.gameObject.tag = "Player " + playerNumber;
		player.transform.name = "Player " + playerNumber;
	}

	void OnPlayerConnected(NetworkPlayer player) {
		++playerCount;
		Debug.Log ("Player " + (playerCount) + " connected from " +
		           player.ipAddress + ":" + player.port);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player) {
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (Network.isServer) {
			Debug.Log ("Local server connection lost");
		} 
		else {
			if(info == NetworkDisconnection.LostConnection) {
				Debug.Log ("Lost connection to the server");
			}
			else {
				Debug.Log ("Successfully disconnected from the server");
			}
		}
	}

	void bgin() {
		if(Network.isServer) {
			Player player = SpawnLobbyPlayer();
			networkView.RPC("spawnPlayer", RPCMode.OthersBuffered, null);
		}
	}
	
	void bgin2() {
		
		if(networkView.isMine) {
			playerName = GameObject.Find ("NetworkManager").GetComponent<NetworkManager> ().playerName;
			networkView.RPC("setName", RPCMode.OthersBuffered, playerName);
		}
	}
	
	[RPC] void setName(string s) {
		playerName = s;
	}
	
	[RPC] void spawnPlayer() {
		Player player = SpawnLobbyPlayer();
		player.gameObject.tag = "Player " + player.playerNumber;
	}
}

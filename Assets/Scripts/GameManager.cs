using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	// Network
	public Sender sender;
	public Client client;
	public Lobby lobby;
	public State state;

	public Dictionary<string,UserController> PlayerControllerList = new Dictionary<string,UserController>();
	public Dictionary<string,Player> PlayerDataList = new Dictionary<string, Player>();
	public Dictionary<string,string> SpawnIDList = new Dictionary<string, string> ();

	// MainPlayer Data
	public UserController MainUserController;
	public Player player;
	public GameObject PlayerObject;
	public Canvas EndGameCanvas;
	public Text HighRankText,PlayerHighestRank;
	public string spawnId;
	public bool isDied = false;

	int highestRank;
	bool isJoined;
	bool isSendForward = false;

	// Last Data
	int lastLevel;
	string lastPosition;
	string lastRotation;

	/*  
		Command Type (id,type,msg)
		spawn: playerid,name,level,pos_x:pos_y:pos_z,rotation_x:rotation_y:rotation_z
		movement: forwardVect,sideVect
		jump: jumpLogic
		isEaten: opLevel
		eat: type,opLevel
	*/

	private float startTime;
	private float journeyLength;
	public float speed = 10f;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {

		highestRank = 10000;
		isSendForward = true;
		System.Random random = new System.Random ();
		int randomPosX = random.Next (0, 50);
		int randomPosZ = random.Next (0, 50);
		string position = randomPosX.ToString () + ":0:" + randomPosZ.ToString ();

		client.SetOnJoin ((string id, string name) => {
			state.id = id;
			state.Register ("level", "10000");
			state.Register ("position", position);
			state.Register ("rotation", "0:0:0");

			isJoined = true;

			// Initial on Client Data
			spawnId = Convert.ToBase64String (Guid.NewGuid ().ToByteArray ());

			// Command Spawn 
			lastLevel = 10000;
			lastPosition = position;
			lastRotation = "0:0:0";
			SpawnPlayer (spawnId, id , name ,new Vector3(randomPosX,0f,randomPosZ),new Vector3(0f,0f,0f), 10000,true);

		});

		state.SetOnSync (OnSyncPlayer);


		lobby.SetOnCmd (onCmd);

		StartCoroutine (Join());
		StartCoroutine (Interval ());
		StartCoroutine (SendForwardInterval ());

	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.X))
			onCmd (spawnId + "?eat?false,10000");
		else if (Input.GetKeyDown (KeyCode.Z))
			onCmd ("1555?jump?true");
		else if (Input.GetKeyDown (KeyCode.B))
			onCmd ("1555?movement?0.5,0");
		else if (Input.GetKeyDown (KeyCode.G))
			onCmd (spawnId + "?isEaten?10000");

		if (!PlayerControllerList.ContainsKey (spawnId) && isJoined)
			EndGame ();
		else
			OnUpdateRanking ();

	}

	IEnumerator Join () {
		while (true) {
			if (sender.isReady () && !isJoined)
				client.Join (PlayerPrefs.GetString ("PlayerName"));
			yield return new WaitForSeconds (2f);
		}
	}

	IEnumerator Interval () {
		while (true && (!isJoined || PlayerControllerList.ContainsKey(spawnId))) {
			state.Sync ();
			startTime = Time.time;
			yield return new WaitForSeconds (2f);
		}
	}

	IEnumerator SendForwardInterval() {
		while (true) {
			isSendForward = true;

			if (PlayerControllerList.ContainsKey (spawnId)) {
				string currentPosition = ConvertVectorToString (PlayerControllerList [spawnId].transform.position);
				if (currentPosition != lastPosition) {
					state.Change ("position", lastPosition, currentPosition);
					lastPosition = currentPosition;
				}

				string currentRotation = ConvertVectorToString (PlayerControllerList [spawnId].transform.rotation.eulerAngles);
				if (currentRotation != lastRotation) {
					state.Change ("rotation", lastRotation, currentRotation);
					lastRotation = currentRotation;
				}
			}

			yield return new WaitForSeconds (0.4f);
		}
	}
		

	void EndGame() {
		// Assume Endgame
		PlayerHighestRank.text = "Highest Ranking: " + highestRank.ToString();
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		CancelInvoke();
		EndGameCanvas.enabled = true;
		sender.End ();
	}

	void SpawnPlayer(string id, string thisPlayerId , string name,Vector3 spawnPosition, Vector3 spawnRotation, int level , bool mainCharacter) {
		GameObject NewPlayer = (GameObject) Instantiate(PlayerObject,spawnPosition,Quaternion.Euler(spawnRotation));
		NewPlayer.name = name;

		// Add User Controller
		UserController resource = NewPlayer.GetComponent<UserController> ();
		resource.mainCharacter = mainCharacter;
		if (!mainCharacter) {
			// Remove MainCharactor From other person
			foreach (Transform child in NewPlayer.transform)
				if (child.name == "Main Camera")
					Destroy (child.gameObject);	
		
			SpawnIDList [thisPlayerId] = id;
		} else {
			MainUserController = resource;
			// Send Spawn
			SendSpawn(id,thisPlayerId,name,level,spawnPosition,spawnRotation);
			SpawnIDList [thisPlayerId] = id;
		}

		PlayerControllerList [id] = resource;

		// Add Player Data
		Player player = NewPlayer.GetComponent<Player>();
		player.name = name;
		player._id = id;
		player.level = level;
		player.nameText.text = name;
		resource.changeScale (level / 10000f);
		PlayerDataList [id] = player;

	}


	void OnSyncPlayer(string id, List<string> keys, List<string> values)  {
		
		string currentSpawnID = SpawnIDList [id];

		// LV 
		if (keys.IndexOf ("level") != -1) {
			PlayerDataList[currentSpawnID].level = int.Parse(values[keys.IndexOf("level")]);
			PlayerControllerList [currentSpawnID].changeScale (PlayerDataList[currentSpawnID].level/10000f);
		}

			
		//  Sync Position
		if (keys.IndexOf ("position") != -1) {
			Vector3 syncPosition = ConvertToVector (values [keys.IndexOf ("position")]);
			journeyLength = Vector3.Distance (PlayerControllerList [currentSpawnID].transform.position, new Vector3 (syncPosition.x, syncPosition.y, syncPosition.z));
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			PlayerControllerList [currentSpawnID].SyncPosition = syncPosition;
			PlayerControllerList [currentSpawnID].fracJourney = fracJourney;
			PlayerControllerList [currentSpawnID].onSync = true;
		}

		// Sync Rotation
		if (keys.IndexOf ("rotation") != -1) {
			Vector3 syncRotation = ConvertToVector (values [keys.IndexOf ("rotation")]);
			PlayerControllerList [currentSpawnID].transform.Rotate (syncRotation);
		}

	}

	void OnUpdateRanking() {
		HighRankText.text = "High Rank:\n";
		Dictionary<string,int> ranking;

		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();

		foreach (KeyValuePair<string,Player> p in PlayerDataList) {
			list.Add(new KeyValuePair<string, int>(p.Value.name,p.Value.level));
		}
			
		list.Sort ((a1,a2) => a2.Value.CompareTo(a1.Value));

		int count = 0;
		foreach (KeyValuePair<string, int> l in list) {
			if (count > 6)
				break;
			
			HighRankText.text += l.Key.ToString() + " " + l.Value.ToString() + "\n";
			count++;
		}
	}
		
	Vector3 ConvertToVector(string data) {
		string[] vect = data.Split (':');
		float posX = float.Parse (vect [0]);
		float posY = float.Parse (vect [1]);
		float posZ = float.Parse (vect [2]);

		return new Vector3 (posX, posY, posZ);
	}

	String ConvertVectorToString(Vector3 data) {
		return data [0].ToString () + ":" + data [1].ToString () + ":" + data [2].ToString ();
	}
		

	void onCmd(string command) {
		Debug.Log (command);
		// Input String Command
		string[] cmd = command.Split('?');
		ControlPlayer(cmd[0],cmd[1],cmd[2]);
	}

	void ControlPlayer(string ownerid, string type, string msg) {

		if (!PlayerControllerList.ContainsKey (ownerid) && type != "spawn")
			return;

		string[] data;
		Player player;
		UserController userControl;

		switch (type) {
		case "spawn":
			data = msg.Split (',');
			int level = int.Parse (data [2]);
			Vector3 spawnPosition = ConvertToVector (data [3]);
			Vector3 spawnRotation = ConvertToVector (data [4]);
			SpawnPlayer (ownerid, data[0] , data [1], spawnPosition, spawnRotation, level , false);
			break;
		case "movement":
			data = msg.Split (',');
			// Prepare send to UserController
			float forwardVect = float.Parse (data [0]);
			float sideVect = float.Parse (data [1]);
			userControl = PlayerControllerList[ownerid];
			userControl.GetInput(forwardVect,sideVect,false,false);
			break;
		case "isEaten":
			// Player was eaten
			player = PlayerDataList [ownerid];
			player.eaten(int.Parse(msg));

			if (ownerid == spawnId) {
				if (lastLevel != PlayerDataList [spawnId].level) {
					Debug.Log ("Update!");
					state.Change ("level", lastLevel.ToString(),PlayerDataList [spawnId].level.ToString());
					lastLevel = PlayerDataList [spawnId].level;
				}
			}

			break;
		case "eat":
			// Up 1 Level
			data = msg.Split (',');
			player = PlayerDataList [ownerid];
			player.eat (bool.Parse (data [0]), int.Parse (data [1]));

			break;
		case "jump":
			userControl = PlayerControllerList[ownerid];
			userControl.GetInput(0f,0f,bool.Parse(msg),false);
			break;
		}
	}

	public void LoadNewGame() {
		Debug.Log ("Load New Scene");
		SceneManager.LoadScene (0);
	}


	// Send CMD
	public void SendEatToServer(bool NPC, int opLevel) {
		// Send Level to highscore
		highestRank = PlayerDataList[spawnId].level > highestRank?PlayerDataList[spawnId].level:highestRank;
		string cmd = spawnId + "?eat?" + NPC.ToString () + "," + opLevel.ToString ();
//		state.Change("level",opLevel.ToString(),opNextLevel.ToString ());

		if (lastLevel != PlayerDataList [spawnId].level) {
			Debug.Log ("Update!");
			state.Change ("level", lastLevel.ToString(),PlayerDataList [spawnId].level.ToString());
			lastLevel = PlayerDataList [spawnId].level;
		}

		lobby.Cmd(cmd);
	}

	public void SendIsEatenToServer(string opId,int opLevel) {
		string cmd = opId + "?isEaten?" + opLevel.ToString ();
		lobby.Cmd(cmd);
	}

	public void SendMove(float forwardMove,float sideMove) {
		if (isSendForward) {
			isSendForward = false;
			string cmd = spawnId + "?movement?" + forwardMove.ToString () + "," + sideMove.ToString ();
			lobby.Cmd (cmd);
		}
	}

	public void SendJump(bool isJump) {
		string cmd = spawnId + "?jump?" + isJump.ToString();
		lobby.Cmd(cmd);
	}

	public void SendSpawn(string id,string playerID,string pname,int lv,Vector3 position, Vector3 rotation) {
		string cmd = id + "?spawn?" + playerID + "," + pname + "," + lv.ToString () + "," + position.x.ToString () + ":" + position.y.ToString () + ":" + position.z.ToString () + "," + rotation.x.ToString () + ":" + rotation.y.ToString () + ":" + rotation.z.ToString ();
		lobby.Cmd(cmd);
	}

}

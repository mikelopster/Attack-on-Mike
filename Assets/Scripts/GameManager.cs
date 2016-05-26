using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public Dictionary<string,UserController> PlayerControllerList = new Dictionary<string,UserController>();
	public Dictionary<string,Player> PlayerDataList = new Dictionary<string, Player>();
	public Dictionary<string,string> SpawnIDList = new Dictionary<string, string> ();

	// MainPlayer Data
	public UserController MainUserController;
	public Player player;
	public GameObject PlayerObject;
	public Canvas EndGameCanvas;
	public string spawnId;

	/*  
		Command Type (id,type,msg)
		spawn: name,playerid,level,pos_x:pos_y:pos_z,rotation_x:rotation_y:rotation_z
		movement: forwardVect,sideVect
		isEaten: opLevel
		eat: type,opLevel
	*/

	private float startTime;
	private float journeyLength;
	public float speed = 0.5f;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		Debug.Log (PlayerPrefs.GetString ("PlayerName"));
		// Set New Time Interval
		InvokeRepeating("onSync", 0, 2);
		spawnId = Convert.ToBase64String (Guid.NewGuid ().ToByteArray ());
		SpawnPlayer (spawnId, "P1234" ,"Earth",new Vector3(0f,0f,0f),new Vector3(0f,0f,0f),true);

		// Register on Sync Table (lv,position,rotation)
		// lv,1
		// position, 1:1:1
		// rotation, 1:1:1

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.X))
			onCmd (spawnId + "?eat?false,10000");
		else if (Input.GetKeyDown (KeyCode.Z))
			onCmd ("1555?jump?true");
//		else if (Input.GetKeyDown (KeyCode.C))
//			OnSyncPlayer ("P1234", "1000,-0.71:0.88:-1.22,0:0:0");
		else if (Input.GetKeyDown (KeyCode.V))
			onCmd ("1555?spawn?P444,Mike,2,1.1:1.1:1.1,0:0:0");
		else if (Input.GetKeyDown (KeyCode.B))
			onCmd ("1555?movement?0,0");
		else if (Input.GetKeyDown (KeyCode.G))
			onCmd (spawnId + "?isEaten?10000");

		if (MainUserController == null) 
			EndGame ();

	}

	void onSync() {
		startTime = Time.time;
		// Check Sync
	}

	void EndGame() {
		// Assume Endgame
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		CancelInvoke();
		EndGameCanvas.enabled = true;
	}

	void SpawnPlayer(string id, string thisPlayerId , string name,Vector3 spawnPosition, Vector3 spawnRotation, bool mainCharacter) {
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
			SendSpawn(id,thisPlayerId,name,1,spawnPosition,spawnRotation);
			SpawnIDList [thisPlayerId] = id;
		}

		PlayerControllerList [id] = resource;

		// Add Player
		Player player = NewPlayer.GetComponent<Player>();
		player.name = name;
		player._id = id;
		PlayerDataList [id] = player;

	}


	void OnSyncPlayer(string playerId, Dictionary<string,string> msg) {
//		SpawnIDList[playerId]
//
//
//		// Move by Sync Position
//		journeyLength = Vector3.Distance(transform.position, new Vector3(syncPosition.x,syncPosition.y,syncPosition.z));
//		float distCovered = (Time.time - startTime) * speed;
//		float fracJourney = distCovered / journeyLength;
//		choosenPlayer.position = Vector3.Lerp(transform.position, new Vector3(syncPosition.x,syncPosition.y,syncPosition.z), fracJourney);
//		// Rotation
//		choosenPlayer.Rotate(syncRotation);
	}
		
	Vector3 ConvertToVector(string data) {
		string[] vect = data.Split (':');
		float posX = float.Parse (vect [0]);
		float posY = float.Parse (vect [1]);
		float posZ = float.Parse (vect [2]);

		return new Vector3 (posX, posY, posZ);
	}
		
	// Input String Command
	void onCmd(string command) {
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
			SpawnPlayer (ownerid, data[0] , data [1], spawnPosition, spawnRotation, false);
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
		string cmd = spawnId + "?eat?" + NPC.ToString () + "," + opLevel.ToString ();
		Debug.Log (cmd);
	}

	public void SendIsEatenToServer(string opId,int opLevel) {
		string cmd = opId + "?isEaten?" + opLevel.ToString ();
		Debug.Log (cmd);
	}

	public void SendMove(float forwardMove,float sideMove) {
		string cmd = spawnId + "?movement?" + forwardMove.ToString() + "," + sideMove.ToString();
//		Debug.Log (cmd);
	}

	public void SendJump(bool isJump) {
		string cmd = spawnId + "?jump?" + isJump.ToString();
		Debug.Log (cmd);
	}

	public void SendSpawn(string id,string playerID,string pname,int lv,Vector3 position, Vector3 rotation) {
		string cmd = id + "?spawn?" + playerID + "," + pname + "," + lv.ToString () + "," + position.x.ToString () + ":" + position.y.ToString () + ":" + position.z.ToString () + "," + rotation.x.ToString () + ":" + rotation.y.ToString () + ":" + rotation.z.ToString ();
		Debug.Log (cmd);
	}

}

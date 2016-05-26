using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public Dictionary<string,UserController> PlayerControllerList = new Dictionary<string,UserController>();
	// MainPlayer Data
	public UserController MainUserController;
	public GameObject PlayerObject;
	public Canvas EndGameCanvas;

	/*  
		Command Type (id,type,msg)
		spawn: name,level,pos_x:pos_y:pos_z,rotation_x:rotation_y:rotation_z
		movement: forwardVect,sideVect
		isEaten: ""
		eat: ""
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
		SpawnPlayer ("1232", "Earth",new Vector3(0f,0f,0f),new Vector3(0f,0f,0f),true);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.X))
			ControlPlayer ("1232", "isEaten", "");
		else if (Input.GetKeyDown (KeyCode.Z))
			ControlPlayer ("1555", "movement", "0.5,0");
		else if (Input.GetKeyDown (KeyCode.C))
			OnSyncPlayer ("1232", "1000,-0.71:0.88:-1.22,0:0:0");
		else if (Input.GetKeyDown (KeyCode.V))
			ControlPlayer ("1555", "spawn", "Mike,2,1.1:1.1:1.1,0:0:0");
		else if (Input.GetKeyDown (KeyCode.B))
			ControlPlayer ("1555", "movement", "0,0");
		else if (Input.GetKeyDown (KeyCode.E)) {
			// Assume Endgame
			Destroy(MainUserController.gameObject);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			CancelInvoke();
			EndGameCanvas.enabled = true;
		}
	}

	void onSync() {
		startTime = Time.time;
		Debug.Log (startTime);
		// Check Sync
	}

	void SpawnPlayer(string id, string name,Vector3 spawnPosition, Vector3 spawnRotation, bool mainCharacter) {
		GameObject NewPlayer = (GameObject) Instantiate(PlayerObject,spawnPosition,Quaternion.Euler(spawnRotation));
		NewPlayer.name = name;
		UserController resource = NewPlayer.GetComponent<UserController> ();
		resource.mainCharacter = mainCharacter;
		if (!mainCharacter) {
			// Remove MainCharactor From other person
			foreach (Transform child in NewPlayer.transform) 
				if (child.name == "Main Camera") 
					Destroy (child.gameObject);	
		} else 
			MainUserController = resource;
		
		PlayerControllerList [id] = resource;

	}

	void EatenPlayer(string ownerid) {
		// Eaten Method

		if (true) {
			// Must Died
			UserController destroyed = PlayerControllerList [ownerid];
			Destroy (destroyed.gameObject);
		}

	}

	void IncreasingLevelPlayer(string ownerid,string msg) {
		Debug.Log (msg);
	}

	void OnSyncPlayer(string ownerid, string msg) {
		string[] vect = msg.Split (',');
		int level = int.Parse (vect [0]);
		Vector3 syncPosition = ConvertToVector(vect[1]);
		Vector3 syncRotation = ConvertToVector (vect [2]);

		Transform choosenPlayer = PlayerControllerList [ownerid].gameObject.transform;

		// Move by Sync Position
		journeyLength = Vector3.Distance(transform.position, new Vector3(syncPosition.x,syncPosition.y,syncPosition.z));
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyLength;
		choosenPlayer.position = Vector3.Lerp(transform.position, new Vector3(syncPosition.x,syncPosition.y,syncPosition.z), fracJourney);
		// Rotation
		choosenPlayer.Rotate(syncRotation);
	}
		
	Vector3 ConvertToVector(string data) {
		string[] vect = data.Split (':');
		float posX = float.Parse (vect [0]);
		float posY = float.Parse (vect [1]);
		float posZ = float.Parse (vect [2]);

		return new Vector3 (posX, posY, posZ);
	}
		

	void ControlPlayer(string ownerid, string type, string msg) {

		if (!PlayerControllerList.ContainsKey (ownerid) && type != "spawn")
			return;
		
		switch (type) {
		case "spawn":
			string[] data = msg.Split (',');
			int level = int.Parse (data [1]);
			Vector3 spawnPosition = ConvertToVector (data [2]);
			Vector3 spawnRotation = ConvertToVector (data [3]);
			SpawnPlayer (ownerid, data [0], spawnPosition, spawnRotation, false);
			break;
		case "movement":
			string[] vect = msg.Split (',');
			// Prepare send to UserController
			float forwardVect = float.Parse (vect [0]);
			float sideVect = float.Parse (vect [1]);
			UserController userControl = PlayerControllerList[ownerid];
			userControl.GetInput(forwardVect,sideVect,false,false);
			break;
		case "isEaten":
			// Player was eaten
			EatenPlayer (ownerid);
			break;
		case "eat":
			// Up 1 Level
			IncreasingLevelPlayer(ownerid,msg);
			break;
		}
	}

	public void LoadNewGame() {
		Debug.Log ("Load New Scene");
		SceneManager.LoadScene (0);
	}

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public Dictionary<string,PlayerResource> PlayerList = new Dictionary<string,PlayerResource>();
	public GameObject PlayerObject;

	/*
		{
			id: 1232
			value: {
				level: 12,
				position: "1,2,3",
				
			}	
		}
	*/

	private float startTime;
	private float journeyLength;
	public float speed = 0.5f;

	// Use this for initialization
	void Start () {
		// Set New Time Interval
		InvokeRepeating("onSync", 0, 2);
		SpawnPlayer ("1232", "Earth");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.X))
			ControlPlayer ("1232", "onEaten", "");
		else if (Input.GetKeyDown (KeyCode.Z))
			ControlPlayer ("1232", "walk", "0.2,0.8");
		else if (Input.GetKeyDown (KeyCode.C))
			OnSyncPlayer ("1232", "1000,-0.71,0.88,-1.22");
	}

	void onSync() {
		startTime = Time.time;
		Debug.Log (startTime);
		// Check Sync
	}

	void SpawnPlayer(string id, string name) {
		GameObject NewPlayer = (GameObject)Instantiate(PlayerObject);
		NewPlayer.name = name;
		PlayerResource resource = NewPlayer.GetComponent<PlayerResource> ();
		resource.SetName (name);
		PlayerList [id] = resource;

		Debug.Log (PlayerList [id].GetName ());
	}

	void EatenPlayer(string ownerid) {
		// Eaten Method

		if (true) {
			// Must Died
			PlayerResource destroyed = PlayerList [ownerid];
			Destroy (destroyed.gameObject);
		}

	}

	void IncreasingLevelPlayer(string ownerid,string msg) {
		Debug.Log (msg);
	}

	void OnSyncPlayer(string ownerid, string msg) {
		string[] vect = msg.Split (',');
		int level = int.Parse (vect [0]);
		Vector3 syncPosition = new Vector3 (float.Parse (vect [1]), float.Parse (vect [2]), float.Parse (vect [3]));
		Transform choosenPlayer = PlayerList [ownerid].gameObject.transform;

		// Move by Sync
		journeyLength = Vector3.Distance(transform.position, new Vector3(syncPosition.x,syncPosition.y,syncPosition.z));
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyLength;
		choosenPlayer.position = Vector3.Lerp(transform.position, new Vector3(syncPosition.x,syncPosition.y,syncPosition.z), fracJourney);
		
	}
		
		

	void ControlPlayer(string ownerid, string type, string msg) {
		switch (type) {
			case "walk":
				string[] vect = msg.Split (',');
				// Prepare send to UserController
				float forwardVect = float.Parse(vect[0]);
				float sideVect = float.Parse(vect[1]);

				break;
			case "onEaten":
				// Player was eaten
				EatenPlayer (ownerid);
				break;
			case "onEat":
				// Up 1 Level
				IncreasingLevelPlayer(ownerid,msg);
				break;
		}
	}

}

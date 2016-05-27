using UnityEngine;
using System.Collections;

public class Environment : MonoBehaviour {

	public static Environment instance;
	public GameObject NPCObject;
	public int MaxNPC = 8;
	public int countSpawn = 0;

	// Use this for initialization

	void Awake() {
		instance = this;
	}

	void Start () {

		for (int i = 0; i < MaxNPC; i++)
			SpawnNPC ();
	
		InvokeRepeating ("SpawnNPC", 0, 5f);
	}


	void SpawnNPC() {

		if (countSpawn < MaxNPC) {
			System.Random random = new System.Random ();
			int randomPosX = random.Next (0, 50);
			int randomPosZ = random.Next (0, 50);

			Instantiate (NPCObject, new Vector3 (randomPosX, 5f, randomPosZ), Quaternion.identity);	
			countSpawn++;
		}
	}
}

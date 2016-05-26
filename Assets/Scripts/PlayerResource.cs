using UnityEngine;
using System.Collections;

public class PlayerResource : MonoBehaviour {

	int lv;
	string name;


	void Start () {
	
	}

	void Update () {
		
	}

	public void Move(int x_direction,int y_direction) {
		// Move
		Debug.Log("Move");
	}

	public int GetLevel() {
		return lv;
	}

	public void SetLevel(int level) {
		lv = level;
	}

	public string GetName() {
		return name;
	}

	public void SetName(string n) {
		name = n;
	}

}

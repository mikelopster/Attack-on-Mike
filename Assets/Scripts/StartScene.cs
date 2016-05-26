using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class StartScene : MonoBehaviour {

	public Text NameInput;
	string PlayerName;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void SetName() {
		PlayerName = NameInput.text;
		PlayerPrefs.SetString ("PlayerName", PlayerName);
		// Load MainScene
		SceneManager.LoadScene (1);
	}
}

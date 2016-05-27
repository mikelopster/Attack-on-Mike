using System;
using System.Collections;
using UnityEngine;

public class Client : MonoBehaviour {

	public Sender sender;

	Action<string, string> onJoinCallback;


	// Start cycle


	void Start () {
		sender.SetOnSend (Sender.PacketType.join, OnJoin);
	}


	// Implement methods


	public void Join (string name) {
		sender.Send (Sender.PacketType.join, name);
	}

	public void SetOnJoin (Action<string, string> callback) {
		onJoinCallback = callback;
	}


	// Implement listeners


	void OnJoin (string message) {
		string[] messages = message.Split ('|');
		string id = messages [0];
		string name = messages [1];

		onJoinCallback (id, name);
	}
}

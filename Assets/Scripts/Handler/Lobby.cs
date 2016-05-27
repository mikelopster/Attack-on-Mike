using System;
using System.Collections;
using UnityEngine;

public class Lobby : MonoBehaviour {

	public Sender sender;

	Action<string> onCmdCallback;


	// Start cycle


	void Start () {
		sender.SetOnSend (Sender.PacketType.cmd, (string message) => {
			onCmdCallback (message);
		});
	}


	// Implement methods


	public void Cmd (string message) {
		sender.Send (Sender.PacketType.cmd, message);
	}

	public void SetOnCmd (Action<string> callback) {
		onCmdCallback = callback;
	}
}

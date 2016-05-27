using System;
using UnityEngine;

public class Sender : MonoBehaviour {

	public Socket socket;

	public enum PacketType {
		join,
		cmd,
		sync
	}

	Action<string> onJoinCallback;
	Action<string> onCmdCallback;
	Action<string> onSyncCallback;


	// Start cycle


	void Start () {
		socket.SetOnMessage (OnSend);
	}


	// Implement methods


	public bool isReady () {
		return socket.isReady;
	}

	public void End () {
		socket.isFinished = true;
		socket.Close ();
	}

	public void Send (PacketType type, string message) {
		Packet packet = new Packet ();
		packet.type = (int)type;
		packet.message = message;

		message = JsonUtility.ToJson (packet);
		socket.Message (message);
	}

	public void SetOnSend (PacketType type, Action<string> callback) {
		switch (type) {

		case PacketType.join:
			onJoinCallback = callback;
			break;

		case PacketType.cmd:
			onCmdCallback = callback;
			break;

		case PacketType.sync:
			onSyncCallback = callback;
			break;
		}
	}


	// Implement listeners


	void OnSend (string message) {
		Packet packet = JsonUtility.FromJson<Packet> (message);
		switch ((PacketType)packet.type) {

		case PacketType.join:
			onJoinCallback (packet.message);
			break;

		case PacketType.cmd:
			onCmdCallback (packet.message);
			break;

		case PacketType.sync:
			onSyncCallback (packet.message);
			break;
		}
	}
}

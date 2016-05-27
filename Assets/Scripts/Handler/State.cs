using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour {

	public string id;
	public Sender sender;

	[Serializable]
	struct StateTable {
		public List<string> keys;
		public List<string> values;
	}

	StateTable stateTable;
	List<string> cloneValues;
	Action<string, List<string>, List<string>> onSyncCallback;


	// Start cycle


	void Start () {
		stateTable = new StateTable ();
		stateTable.keys = new List<string> ();
		stateTable.values = new List<string> ();
		cloneValues = new List<string> ();

		sender.SetOnSend (Sender.PacketType.sync, onSync);
	}


	// Implement methods


	public void Register (string key, string value) {
		string clone = value + id;
		stateTable.keys.Add (key);
		stateTable.values.Add (value);
		cloneValues.Add (clone);
	}

	public bool Change (string key, string value, string newValue) {
		int index = stateTable.keys.IndexOf (key);
		string clone = cloneValues [index].Replace (id, "");

		if (clone == value) {
			stateTable.values [index] = newValue;
			cloneValues [index] = newValue + id;
			return true;
		}
		return false;
	}

	public void Sync () {
		string message = JsonUtility.ToJson (stateTable);
		sender.Send (Sender.PacketType.sync, message);
	}

	public void SetOnSync (Action<string, List<string>, List<string>> callback) {
		onSyncCallback = callback;
	}


	// Implement listeners


	void onSync (string message) {
		string[] messages = message.Split ('|');
		string id = messages [0];
		string stage = messages [1];

		StateTable stateTable = JsonUtility.FromJson<StateTable> (stage);
		onSyncCallback (id, stateTable.keys, stateTable.values);
	}
}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NPC {

	// Talk List
	Dictionary<int, Dictionary<int,string>> talkQuests = new Dictionary<int, Dictionary<int,string>>();
	string talkDefault;

	// Attr
	string name;

	public NPC(string name) {
		this.name = name;
	}

	public string GetName() {
		return this.name;
	}
		

	public string GetTalkDefault() {
		return talkDefault;
	}

	public string GetTalkQuest(int quest_index,int talk_index) {
		return talkQuests [quest_index][talk_index];
	}

	// Talk Method
	public void SetTalk(string talkD,Dictionary<int, Dictionary<int,string>> talkQs) {
		this.talkDefault = talkD;
		this.talkQuests = talkQs;
	}

	public void SetQuest() {
		
	}

}

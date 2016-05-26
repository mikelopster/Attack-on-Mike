﻿using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour {

	Vector3 TargetPosition;
	float distance;
	bool onWalk = false;
	float speed = 0.05f;
	float posX,posZ;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (onWalk) {
			transform.position += new Vector3(posX * speed,transform.position.y,posZ * speed);
		} else {
			if (GameManager.instance.MainUserController != null) {
				TargetPosition = GameManager.instance.MainUserController.gameObject.transform.position;
				distance = Vector3.Distance (TargetPosition, transform.position);
				if (distance < 3.0f) {
					posX = TargetPosition.x / TargetPosition.x;
					posZ = TargetPosition.z / TargetPosition.z;

					if (TargetPosition.x == 0)
						posX = 0;
					if (TargetPosition.z == 0)
						posZ = 0;
					
					onWalk = true;
				}
			}
		}

	}
}

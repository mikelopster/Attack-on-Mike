using UnityEngine;
using System.Collections;

public class SyncMovement : MonoBehaviour {


	public Transform endMarker;
	public float speed = 1.0F;
	private float startTime;
	private float journeyLength;

	// Use this for initialization
	void Start () {
		startTime = Time.time;

	}
	
	// Update is called once per frame
	void Update () {
//		InterpolationSync ();
	}

	void InterpolationSync() {
//		journeyLength = Vector3.Distance(transform.position, new Vector3(endMarker.position.x,transform.position.y,endMarker.position.z));
//		float distCovered = (Time.time - startTime) * speed;
//		float fracJourney = distCovered / journeyLength;
//		transform.position = Vector3.Lerp(transform.position, new Vector3(endMarker.position.x,transform.position.y,endMarker.position.z), fracJourney);
//
	}


}

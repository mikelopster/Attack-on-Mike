using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class CameraController : MonoBehaviour {

	public float sensitivityY = 1;
	public float minimumY = 0;
	public float maximumY = 30;
	float rotationY = 0;
	Quaternion originalRotation;
	Vector3 old_position;

	void Update ()
	{
//		if (transform.parent.GetComponent<PlayerResource> ().HP > 0) {
			rotationY += Input.GetAxis ("Mouse Y") * sensitivityY;
			rotationY = ClampAngle (rotationY, minimumY, maximumY);
			Quaternion yQuaternion = Quaternion.AngleAxis (-rotationY, Vector3.right);
			transform.localRotation = originalRotation * yQuaternion;
//		}
	}

	void Start ()
	{
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
		originalRotation = transform.localRotation;
	}

	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
}

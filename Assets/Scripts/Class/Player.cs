using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public int level = 10000;
	public float scale = 1.0f;
	public string name;
	public string _id;
	float ratio = 10000;
	UserController userController;

	void Start()
	{
		userController = GetComponent<UserController> ();
	}

	public void eat (bool NPC, int opLevel) {
		if (NPC)
			level += 500;
		else if (level - opLevel >= 5000)
			level += level / 4;
		else
			level += opLevel / 10;

		scale = level / ratio;
		userController.changeScale (scale);
	}

	public void eaten (int opLevel) {
		if (opLevel - level >= 5000)
			died ();
		else 
			level -= opLevel / 10;

		if (level < ratio)
			died ();

		scale = level / ratio;
		userController.changeScale (scale);
	}

	public void died () {
		Destroy (this.gameObject);
	}
}

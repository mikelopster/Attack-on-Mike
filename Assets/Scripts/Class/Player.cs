using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public int level = 10000;
	public float scale = 1.0f;
	public string name;
	public string _id;
	public int score = 0;
	public Text nameText;
	float ratio = 10000;
	UserController userController;
	public float height;


	void Start()
	{
		userController = GetComponent<UserController> ();
		height = scale * 5;
	}

	public void eat (bool NPC, int opLevel) {
		if (NPC)
			level += 500;
		else if (level - opLevel >= 5000)
			level += level / 4;
		else
			level += opLevel / 10;

		scale = level / ratio;
		height = scale;
		height = (float) Math.Round(height * 5, 2);
		userController.changeScale (scale);
		if (score < level)
			score = level;

		GameManager.instance.SendEatToServer(false,opLevel);
	}

	public void eaten (int opLevel) {
		if (opLevel - level >= 5000)
			died ();
		else
			level -= opLevel / 10;

		if (level < ratio)
			died ();

		scale = level / ratio;
		height = scale;
		height = (float) Math.Round(height * 5, 2);
		userController.changeScale (scale);
	}

//	public int burn () {
//		if (level > 10000) {
//			if (level - 100 >= 10000) {
//				level -= 100;
//			}
//			else {
//				level = 10000;
//			}
//
//			scale = level / ratio;
//			height = scale;
//			height = (float) Math.Round(height * 5, 2);
//			userController.changeScale (scale);
//
//			return 2;
//		}
//		else {
//			return 1;
//		}
//
//	}

	public void died () {
		// Remove Key
		GameManager.instance.PlayerDataList.Remove (_id);
		GameManager.instance.PlayerControllerList.Remove (_id);
		Destroy (this.gameObject);
	}
}

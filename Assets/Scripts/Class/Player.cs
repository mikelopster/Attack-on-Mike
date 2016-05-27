using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public int level = 10000;
	public float scale = 1.0f;
	public string name;
	public string _id;
	public Text nameText;
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
		userController.changeScale (scale);

	}

	public void died () {
		// Remove Key
		GameManager.instance.PlayerDataList.Remove (_id);
		GameManager.instance.PlayerControllerList.Remove (_id);
		Destroy (this.gameObject);
	}
}

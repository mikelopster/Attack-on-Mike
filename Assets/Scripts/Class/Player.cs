using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public int level = 10000;
	public float scale = 1.0;
	float ratio = 10000;

	public void eat (bool NPC) {
		if (NPC)
			level += level / 4;
		else
			level += 500;

		scale = level / ratio;
	}

	public void eaten (int opLevel) {
		level -= opLevel / 4;
		if (level < ratio)
			died ();
		scale = level / ratio;
	}

	public void died () {
		Destroy (this.gameObject);
	}
}

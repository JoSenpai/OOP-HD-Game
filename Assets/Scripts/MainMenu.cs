using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Completed{
	
	public class MainMenu : MonoBehaviour {

		//public GameObject gameManager;			//GameManager prefab to instantiate.
		//public GameObject soundManager;			//SoundManager prefab to instantiate.
		//public GameObject Menu;
		public static bool playing;
		public GameObject loader;
		public GameObject StartButton;

		// Use this for initialization
		void Start() {
			if (playing) {
				StartButton.SetActive (false);
			}

		}

		public void StartGame()
		{	
			Instantiate (loader);
			playing = true;
			StartButton.SetActive (false);

		}
			

	}

}
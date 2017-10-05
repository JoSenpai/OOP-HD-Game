using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Completed
{
	public class Pause : MonoBehaviour {

		public GameObject PauseUI;
		public GameObject MenuUI;

		private bool paused = false;

		void Start()
		{
			PauseUI.SetActive (false);
		}

		void Update()
		{
			if (Input.GetButtonDown ("Pause")) {
				paused = !paused;
			}

			if (paused) {
				PauseUI.SetActive (true);
				//setting time to 0 so nothing happens
				Time.timeScale = 0;
			}

			if (!paused) {
				PauseUI.SetActive (false);
				Time.timeScale = 1;
			}
		}

		public void Resume()
		{
			paused = false;
		}

		public void Restart()
		{
			GameManager.instance.level--;
			GameManager.instance.playerHP = 100;
			//MenuUI.SetActive (true);
			//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
			SceneManager.LoadScene(0);
		}
			

		public void Quit()
		{
			Application.Quit ();
		}

	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Completed
{
	public class Melee : MonoBehaviour {
		
		public AudioClip hitSound;
		public AudioClip hitSound2;
		public AudioClip enemyHit;
		// Use this for initialization
		void Start () {
			SoundManager.instance.RandomizeSfx (hitSound, hitSound2);
		}
			
		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.tag == "Enemy") {
				SoundManager.instance.PlaySingle (enemyHit);

				other.gameObject.GetComponent<Enemy> ().enemyHP = other.gameObject.GetComponent<Enemy> ().enemyHP - 2;

				if(other.gameObject.GetComponent<Enemy> ().enemyHP <= 0)
				{
					other.gameObject.SetActive (false);
					other.gameObject.GetComponent<Enemy> ().playerDamage = 0;
					other.gameObject.GetComponent<Enemy> ().blockingLayer = 0;
				}
			}

		}
	}

}
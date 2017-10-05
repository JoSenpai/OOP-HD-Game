using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Completed
{
	public class Bullet : MonoBehaviour {

		public float speed;
		public AudioClip bulletHitSound;

		// Update is called once per frame
		void Update () {
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (2.0f * Input.GetAxis("Mouse X"), 5);
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.tag == "Enemy") {
				SoundManager.instance.RandomizeSfx (bulletHitSound);
				other.gameObject.GetComponent<Enemy> ().enemyHP = other.gameObject.GetComponent<Enemy> ().enemyHP - 1;

				int deadEnemy=1;
				if(other.gameObject.GetComponent<Enemy> ().enemyHP == 0)
				{
					//other.gameObject.SetActive (false);
					other.gameObject.GetComponent<Enemy> ().playerDamage = 0;
					other.gameObject.SetActive (false);
					other.gameObject.GetComponent<Enemy> ().blockingLayer = 0;
				}

			}
			//removes the bullet when collide with enemy
			Destroy (gameObject);
		}
	}
}

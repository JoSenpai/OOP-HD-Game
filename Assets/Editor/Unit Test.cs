using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NUnit.Framework;

namespace Completed{

	[TestFixture]
	public class UnitTest : Player {

		[Test]
		public void FirstTest()
		{
			Debug.Log ("Hello World");
			Assert.AreNotEqual (1, 2);
		}

		[Test]
		public void GameObjectTest()
		{
			var gameObject = new GameObject();

			var newGameObjectName = "My game object";
			gameObject.name = newGameObjectName;

			Assert.AreEqual(newGameObjectName, gameObject.name);
		}

		[Test]
		public void TransformPositionTest()
		{
			var gameObject = new GameObject();
			gameObject.AddComponent<Player> ();
			var player = gameObject.GetComponent<Player> ();
			player.transform.position = new Vector3 (0, 0, 0);
		
			Assert.AreEqual (player.transform.position, new Vector3 (0, 0, 0));
		}
	}
}
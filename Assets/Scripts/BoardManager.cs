using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace Completed
{
	public class BoardManager : MonoBehaviour
	{
		// Using Serializable allows us to embed a class with sub properties in the inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Minimum value for our Count class.
			public int maximum; 			//Maximum value for our Count class.

			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}

		public int columns = 8; 										//Number of columns in our game board.
		public int rows = 8;											//Number of rows in our game board.
		public GameObject exit;											
		public GameObject[] floorTiles;									
		public GameObject[] enemyTiles;									
		public GameObject[] outerWallTiles;								
		public GameObject[] wallTiles;
		public Count wallCount = new Count (6, 10);

		private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.
		private int boss1 = 1;
		private int boss2 = 1;
		
		//Clears our list gridPositions and prepares it to generate a new board.
		private void InitialiseList ()
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();

			for(int y = 1; y < rows-1; y++)
			{
				for(int x = 1; x < columns-1; x++)
				{
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}
		
		//Sets up the outer walls and floor (background) of the game board.
		private void BoardSetup ()
		{
			//Instantiate Board and set boardHolder to its transform.
			boardHolder = new GameObject ("Board").transform;
			
			//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
			for(int x = -1; x < columns + 1; x++)
			{
				//Loop along y axis, starting from -1 to place floor or outerwall tiles.
				for(int y = -1; y < rows + 1; y++)
				{
					//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
					GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
					
					//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
					if(x == -1 || x == columns || y == -1 || y == rows)
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					
					//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
					GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					
					//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
					instance.transform.SetParent (boardHolder);
				}
			}
		}
		
		
		//RandomPosition returns a random position from our list gridPositions.
		private Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];
			
			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);
			
			//Return the randomly selected Vector3 position.
			return randomPosition;
		}

		private Vector3 RandomEnemy ()
		{
			
			int randomIndex = Random.Range (20, gridPositions.Count);

			Vector3 randomPosition = gridPositions[randomIndex];

			gridPositions.RemoveAt (randomIndex);

			return randomPosition;
		}
		
		
		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		private void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum);
			
			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				
				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, 1)];
				
				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}


		private void LayoutEnemiesAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			int objectCount = Random.Range (minimum, maximum);
		
			for(int i = 0; i < objectCount; i++)
			{
				Vector3 randomPosition = RandomEnemy();

				GameObject tileChoice = tileArray[Random.Range (0, 1)];

				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}
		
		
		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetupScene (int level)
		{
			//Creates the outer walls and floor.
			BoardSetup ();
			
			//Reset our list of gridpositions.
			InitialiseList ();
			
			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
			
			//Determine number of enemies based on current level number
			int enemyCount = (int)Mathf.Log(level*10, 2f);

			Vector3 randomEnemy = RandomEnemy();

			//Instantiate the Orc Boss
			if (level % 5 == 0) {
				for (int i = 0; i < boss2; i++) {
					Vector3 randomEnemy1 = RandomEnemy ();
					Instantiate (enemyTiles [2], randomEnemy1, Quaternion.identity);
				}
				boss2++;
			}//Instiate the Skeleton Boss
			else if (level % 3 == 0) {
				for (int i = 0; i < boss2; i++) {
					Vector3 randomEnemy2 = RandomEnemy ();
					Instantiate (enemyTiles [1], randomEnemy2, Quaternion.identity);
				}
				boss1++;
			}
			else
			{
				//Instantiate the skeletons
				LayoutEnemiesAtRandom (enemyTiles, enemyCount, enemyCount);
			}
			
			//Instantiate the exit tile in the upper right hand corner of our game board
			Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
		}
	}
}
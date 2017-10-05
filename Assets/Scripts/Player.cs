using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

namespace Completed
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{
		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
		public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
		public Text HPText;						//UI Text to display current player food total.
		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves
		public AudioClip gameOverSound;				//Audio clip to play when player dies.
		public AudioClip pewpew;

		public Transform firePoint;
		public Transform meleePoint;
		public GameObject bullet;
		public GameObject melee;

		private Animator animator;					//Used to store a reference to the Player's animator component.
		private int PlayerHP;                           //Used to store player food points total during level.

		
		//Start overrides the Start function of MovingObject
		protected override void Start ()
		{
			//Get a component reference to the Player's animator component
			animator = GetComponent<Animator>();

			//Get the current food point total stored in GameManager.instance between levels.
			if (GameManager.instance != null) {
				PlayerHP = GameManager.instance.playerHP;
			} else {
				PlayerHP = 100;
			}
			
			//Set the foodText to reflect the current player food total.
			HPText.text = "HP: " + PlayerHP;
			
			//Call the Start function of the MovingObject base class.
			base.Start ();
		}
		
		//This function is automatically called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			//Allows the player hp to be saved to next scene
			GameManager.instance.playerHP = PlayerHP;
		}
		
		
		private void Update ()
		{
	
			if (GameManager.instance.doingSetup)
				return;

			//If it's not the player's turn, exit the function.
			if(!GameManager.instance.playersTurn) return;
			
			int horizontal = 0;  	//Used to store the horizontal move direction.
			int vertical = 0;		//Used to store the vertical move direction.

			
			//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
			//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
			//Check if moving horizontally, if so set vertical to zero.
			if(horizontal != 0)
			{
				vertical = 0;
			}
				
			//Check if we have a non-zero value for horizontal or vertical
			if(horizontal != 0 || vertical != 0)
			{
				//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
				//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
				transform.position = new Vector3(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y),0);
				AttemptMove<Wall> (horizontal, vertical);


			}
			if (Input.GetKeyDown (KeyCode.Space)) {
				
				animator.SetTrigger ("playerChop");
				Instantiate (melee, meleePoint.position, meleePoint.rotation);
				transform.position = new Vector3(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y),0);
			}

			if (Input.GetMouseButtonDown(0)) {
				SoundManager.instance.PlaySingle (pewpew);
				Instantiate (bullet, firePoint.position, firePoint.rotation);
				transform.position = new Vector3(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y),0);
			}
		}
			
		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
		public override void AttemptMove <T> (int xDir, int yDir)
		{
			//Update food text display to reflect current score.
			HPText.text = "HP: " + PlayerHP;

			base.AttemptMove <T> (xDir, yDir);

			//Hit allows us to reference the result of the Linecast done in Move.
			//A raycast is used to detect objects that lie along the path of a ray and is conceptually like firing a laser beam into the scene and observing which objects are hit by it. 
			//The RaycastHit2D class is used by Physics2D.Raycast and other functions to return information about the objects detected by raycasts.
			RaycastHit2D hit;
			
			//If Move returns true, meaning Player was able to move into an empty space.
			if (Move(xDir, yDir, out hit)) 
			{
				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
			}
			CheckIfGameOver ();
			
			//Set the playersTurn boolean of GameManager to false now that players turn is over.
			GameManager.instance.playersTurn = false;

		}

		
		//OnCantMove overrides the abstract function OnCantMove in MovingObject.
		//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
		protected override void OnCantMove <T> (T component)
		{

			Wall hitWall = component as Wall;

			hitWall.DamageWall (wallDamage);

			animator.SetTrigger ("playerChop");
		}
		
		
		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Check if the tag of the trigger collided with is Exit.
			if(other.tag == "Exit")
			{
				//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
				Invoke ("Restart", restartLevelDelay);
				
				//Disable the player object since level is over.
				enabled = false;
			}

		}
		
		
		//Restart reloads the scene when called.
		public void Restart ()
		{
			//Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
            //and not load all the scene object in the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
		
		
		//LoseHP is called when an enemy attacks the player.
		public void LoseHP (int loss)
		{
			animator.SetTrigger ("playerHit");
			
			//Subtract lost food points from the players total.
			PlayerHP -= loss;
			
			//Update the food display with the new total.
			HPText.text = "-"+ loss + " HP: " + PlayerHP;
			
			//Check to see if game has ended.
			CheckIfGameOver ();
			transform.position = new Vector3(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y),0);
		}
		
		
		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			//Check if food point total is less than or equal to zero.
			if (PlayerHP <= 0) 
			{
				//Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
				SoundManager.instance.PlaySingle (gameOverSound);
				
				//Stop the background music.
				SoundManager.instance.musicSource.Stop();
				
				//Call the GameOver function of GameManager.
				GameManager.instance.GameOver ();
			}
		}
	}
}


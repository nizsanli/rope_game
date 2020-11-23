using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMgr : MonoBehaviour {

	// game states
	public enum GameState
	{
		WAITING,
		GAME
	}
	// current state
	public static GameState STATE;

	// action key
	// used to jump/shoot rope
	private KeyCode actionButton;

	// camera manager
	// moves camera accordingly
	public CameraMgr cameraMgr;

	// ceiling manager
	public CeilingMgr ceilingMgr;

	// world objects
	public Character character;
	public Transform pillar;

	// for reset bug
	private bool charJumped;

	// Use this for initialization
	void Start () {
		actionButton = KeyCode.Space;

		// reset game
		reset();
	}

	public void reset()
	{
		// set initial state
		STATE = GameState.WAITING;

		// reset world
		character.reset();
		ceilingMgr.reset();

		// spawn ceilings
		ceilingMgr.spawn(1000);

		// for reset bug
		charJumped = false;
	}
	
	// Update is called once per frame
	void Update () {
		switch (STATE) {
		case GameState.WAITING:
			if (Input.GetMouseButtonDown(0))
			{
				// jump character and enable its rigidbody
				character.enableRigid();
				character.jump();
				
				charJumped = true;
			}
			else if (Input.GetMouseButtonUp(0) && charJumped)
			{
				// wait till action key up to switch to game state
				STATE = GameState.GAME;
				
				charJumped = false;
			}
			
			break;
		case GameState.GAME:
			if (Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0))
			{
				// shoot rope if action key is currently down
				character.shootRope();
			}
			else
			{
				// pull rope if action key is not down
				character.pullRope();
			}
			
			break;
		}
		
		// camera follows the character
		cameraMgr.followCharacter();
		
		// check reset condition
		checkDeath();
	}

	private void checkDeath()
	{
		if (character.transform.position.y < -1 * Camera.main.orthographicSize - Camera.main.orthographicSize * 0.1f)
		{
			character.eraseRope();
			reset();
		}
	}
}

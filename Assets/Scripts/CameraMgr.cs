using UnityEngine;
using System.Collections;

public class CameraMgr : MonoBehaviour {

	public Character character;

	// Use this for initialization
	void Start () {
	
	}
	
	public void followCharacter()
	{
		Camera.main.transform.position = new Vector3(character.transform.position.x, 0f, Camera.main.transform.position.z);
	}
}

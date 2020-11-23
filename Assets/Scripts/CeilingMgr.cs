using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CeilingMgr : MonoBehaviour {

	private Queue<Ceiling> ceilingQ;
	private float lastX = 0f;
	
	public Ceiling ceilPrefab;
	
	private float widthCeil;
	private float distCeil;
	
	private System.Random rand;
	
	public void reset()
	{
		// set spawn parameters
		widthCeil = 1f;
		distCeil = 4f;

		lastX = 0f;
		
		// new random object
		rand = new System.Random();

		// empty out q
		foreach (Ceiling ob in ceilingQ)
		{
			Destroy(ob.gameObject);
		}
		ceilingQ.Clear();
	}
	
	public void spawn(int numberToSpawn)
	{
		for (int i = 0; i < numberToSpawn; i++)
		{
			// add distCeil to lastX and spawn ceiling at lastX
			lastX += distCeil;
			lastX += (float) rand.NextDouble() * distCeil * 0.5f;
			Ceiling newCeil = (Ceiling) Instantiate(ceilPrefab, new Vector3(lastX, Camera.main.orthographicSize - 0.05f, -1f), Quaternion.identity);
			newCeil.transform.localScale = new Vector3(widthCeil, 0.1f, 1f);

			// add to q
			ceilingQ.Enqueue(newCeil);
		}
	}
	
	void Awake()
	{
		ceilingQ = new Queue<Ceiling>(100);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

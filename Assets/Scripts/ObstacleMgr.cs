using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleMgr : MonoBehaviour {

	private Queue<Obstacle> obstacleQ;
	private float lastX = 0f;

	public Obstacle obPrefab;

	private float heightOb;
	private float distOb;

	private System.Random rand;

	public void reset()
	{
		// set spawn parameters
		heightOb = 1f;
		distOb = 4f;

		// new random object
		rand = new System.Random();

		// empty out q
		foreach (Obstacle ob in obstacleQ)
		{
			Destroy(ob.gameObject);
		}
		obstacleQ.Clear();
	}

	public void spawn(int numberToSpawn)
	{
		for (int i = 0; i < numberToSpawn; i++)
		{
			// spawn obstacle distOb away from lastX
			// with random y location
			float randY = (float) rand.NextDouble() * Camera.main.orthographicSize * 2f - Camera.main.orthographicSize;
			lastX += distOb;

			Obstacle newOb = (Obstacle) Instantiate(obPrefab, new Vector3(lastX, randY, -1f), Quaternion.identity);
			newOb.transform.localScale = new Vector3(newOb.transform.localScale.x, heightOb, 1f);

			// add new obstacle as child of manager
			newOb.transform.parent = transform;

			// add new obstacle to q
			obstacleQ.Enqueue(newOb);
		}
	}

	void Awake()
	{
		obstacleQ = new Queue<Obstacle>(100);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

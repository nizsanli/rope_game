using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rope : MonoBehaviour {

	public Character character;

	private Mesh ropeMesh;

	List<Vector3> verts;
	List<int> indices;
	List<Vector2> uvs;

	private float ropeWidthHalf;

	private float inAmount;
	private float outAmount;

	private float outAngleDegs;
	private float outAngleRads;
	Vector3 ropeTipWorld;

	private bool connected;
	private bool fixPos;

	bool ropeOut;

	public void reset()
	{
		connected = false;
		fixPos = false;
		ropeOut = false;

		outAngleDegs = 65f;
		outAngleRads = outAngleDegs * Mathf.Deg2Rad;

		inAmount = 3f;
		outAmount = 1.5f;

		transform.position = character.transform.position + character.transform.up * (character.transform.localScale.y*0.5f);
		ropeTipWorld = transform.position;

		ropeWidthHalf = 0.02f;
	}
	
	// Use this for initialization
	void Start () {
		verts = new List<Vector3>(4);
		indices = new List<int>(6);
		uvs = new List<Vector2>(4);

		makeRopeMesh();
	}

	public void eraseRope()
	{
		verts[2] = new Vector3(ropeWidthHalf, 0f, 0f);
		verts[3] = new Vector3(-ropeWidthHalf, 0f, 0f);
		ropeMesh.vertices = verts.ToArray();
		ropeMesh.RecalculateBounds();
	}

	public void makeRopeMesh()
	{
		// ccw
		verts.Add(new Vector3(-ropeWidthHalf, 0f, 0f));
		verts.Add(new Vector3(ropeWidthHalf, 0f, 0f));
		verts.Add(new Vector3(ropeWidthHalf, 0f, 0f));
		verts.Add(new Vector3(-ropeWidthHalf, 0f, 0f));

		indices.Add(0);
		indices.Add(3);
		indices.Add(2);
		indices.Add(2);
		indices.Add(1);
		indices.Add(0);

		uvs.Add(new Vector2(0f, 0f));
		uvs.Add(new Vector2(1f, 0f));
		uvs.Add(new Vector2(1f, 1f));
		uvs.Add(new Vector2(0f, 1f));

		ropeMesh = new Mesh();
		ropeMesh.vertices = verts.ToArray();
		ropeMesh.triangles = indices.ToArray();
		ropeMesh.uv = uvs.ToArray();
		ropeMesh.RecalculateBounds();
		ropeMesh.MarkDynamic();
		ropeMesh.Optimize();

		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = ropeMesh;
	}

	public void comeOut()
	{
		if (!connected)
		{
			transform.position = character.transform.position + character.transform.up * (character.transform.localScale.y*0.5f);

			if (!ropeOut)
			{
				ropeTipWorld = character.transform.position + new Vector3(0f, character.transform.localScale.y*0.5f, 0f);
				ropeOut = true;
			}

			/*
			Vector3 outVec = new Vector3(0f, outAmount, 0f);
			
			verts[2] = verts[2] + outVec;
			verts[3] = verts[3] + outVec;
			
			tip += outAmount;
			
			ropeMesh.vertices = verts.ToArray();
			ropeMesh.RecalculateBounds();
			*/

			Vector3 ropeOutVec = new Vector3(Mathf.Cos(outAngleRads), Mathf.Sin(outAngleRads), 0f) * outAmount;
			RaycastHit2D hit = Physics2D.Raycast(ropeTipWorld, new Vector2(Mathf.Cos(outAngleRads), Mathf.Sin(outAngleRads)), outAmount);
			if (hit.collider != null)
			{
				ropeTipWorld = new Vector3(hit.point.x, hit.point.y, 0f);
			
				Vector3 toVec = ropeTipWorld - transform.position;
				transform.localRotation = Quaternion.FromToRotation(Vector3.up, toVec);

				activateHinge();
				character.activateHinge();
				enableRigid();

				connected = true;
				fixPos = true;
			}
			else
			{
				ropeTipWorld += ropeOutVec;

				Vector3 toVec = ropeTipWorld - transform.position;
				transform.localRotation = Quaternion.FromToRotation(Vector3.up, toVec);
			}

			float ropeLength = Vector3.Distance(transform.position, ropeTipWorld);
			
			verts[2] = new Vector3(ropeWidthHalf, ropeLength, 0f);
			verts[3] = new Vector3(-ropeWidthHalf, ropeLength, 0f);
			
			ropeMesh.vertices = verts.ToArray();
			ropeMesh.RecalculateBounds();
		}
		else if (fixPos)
		{
			transform.position = character.transform.position + character.transform.up * (character.transform.localScale.y*0.5f);
			fixPos = false;
		}
	}

	public void comeIn()
	{
		transform.position = character.transform.position + character.transform.up * (character.transform.localScale.y*0.5f);
		float ropeLength = Vector3.Distance(transform.position, ropeTipWorld);

		if (connected)
		{
			connected = false;
			character.deactivateHinge();
			character.fling();
			deactivateHinge();
			disableRigid();
		}

		if (ropeLength != 0f)
		{	
			if (ropeLength - inAmount <= 0f)
			{
				ropeTipWorld = transform.position;
				ropeLength = 0f;
				ropeOut = false;
			}
			else
			{
				Vector3 inVec = (transform.position - ropeTipWorld).normalized * inAmount;
				ropeTipWorld += inVec;
				ropeLength -= inAmount;
			}
			
			Vector3 toVec = ropeTipWorld - transform.position;
			transform.localRotation = Quaternion.FromToRotation(Vector3.up, toVec);
			
			Vector3 lengthVec = new Vector3(0f, ropeLength, 0f);
			verts[2] = lengthVec;
			verts[3] = lengthVec;
			
			ropeMesh.vertices = verts.ToArray();
			ropeMesh.RecalculateBounds();
		}
	}

	public void activateHinge()
	{
		HingeJoint2D joint = GetComponent<HingeJoint2D>();
		joint.enabled = true;

		float ropeLength = Vector3.Distance(transform.position, ropeTipWorld);

		//Vector3 connectedWorldPos = (new Vector3(transform.position.x, transform.position.y, 0f)) + (transform.up * transform.localScale.y*0.5f);
		Vector2 connectedPoint = new Vector2(0f, ropeLength);
		joint.anchor = connectedPoint;
		joint.connectedAnchor = ropeTipWorld;
	}

	public void enableRigid()
	{
		GetComponent<Rigidbody2D>().isKinematic = false;
	}

	public void disableRigid()
	{
		GetComponent<Rigidbody2D>().isKinematic = true;
	}

	public void deactivateHinge()
	{
		HingeJoint2D joint = GetComponent<HingeJoint2D>();
		joint.enabled = false;
	}
}

using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	// character controls rope
	public Rope rope;

	public void reset()
	{
		// disable rigidbody and move to initial position
		disableRigid();
		transform.position = new Vector3(0f, 1.5f, 0f);

		// reset rope
		rope.reset();
	}

	public void eraseRope()
	{
		rope.eraseRope();
	}
	
	void Start()
	{

	}

	public void enableRigid()
	{
		GetComponent<Rigidbody2D>().isKinematic = false;
	}

	public void disableRigid()
	{
		Rigidbody2D rb = GetComponent<Rigidbody2D> ();
		rb.velocity = Vector3.zero;

		rb.isKinematic = true;
	}
	
	public void jump()
	{
		// set jump vec
		float jumpMag = 6;
		float jumpAng = 45f * Mathf.Deg2Rad;
		Vector2 jumpVec = new Vector2(Mathf.Cos(jumpAng), Mathf.Sin(jumpAng)) * jumpMag;

		// add force with jump vec
		GetComponent<Rigidbody2D>().AddForce(jumpVec, ForceMode2D.Impulse);
	}

	public void fling()
	{
		Rigidbody2D charRigid = GetComponent<Rigidbody2D>();
		float flingPower = 5f;
		//charRigid.AddForce(charRigid.velocity.normalized * flingPower, ForceMode2D.Impulse);
		charRigid.AddForce(Vector3.up * flingPower, ForceMode2D.Impulse);
	}

	public void shootRope()
	{
		rope.comeOut();
	}

	public void pullRope()
	{
		rope.comeIn();
	}

	public void activateHinge()
	{
		HingeJoint2D joint = GetComponent<HingeJoint2D>();
		joint.enabled = true;

		joint.anchor = new Vector2(0f, transform.localScale.y*0.5f);
		joint.connectedAnchor = Vector2.zero;
	}

	public void deactivateHinge()
	{
		HingeJoint2D joint = GetComponent<HingeJoint2D>();
		joint.enabled = false;
	}

}

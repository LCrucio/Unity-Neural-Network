using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    public Vector3 BallStartPosition;
    private Rigidbody2D rb;
    public float Speed = 1000;

    public AudioSource Blip;
    public AudioSource Blop;

	// Use this for initialization
	void Start ()
	{
	    rb = GetComponent<Rigidbody2D>();
	    BallStartPosition = this.transform.position;
	    ResetBall();
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "backwall")
        {
            Blop.Play();
        }
        else
        {
            Blip.Play();
        }
    }

    public void ResetBall()
    {
        this.transform.position = BallStartPosition;
        rb.velocity = Vector3.zero;
        Vector3 direction = new Vector3(Random.Range(100, 250), Random.Range(-100, 100), 0).normalized;
        rb.AddForce(direction*Speed);
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown("space"))
	    {
	        ResetBall();
	    }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopperScript : MonoBehaviour
{
    public Vector2[] positions;
    public int position = 0;
    Rigidbody2D rb;
	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();       

	}
	
	// Update is called once per frame
	void Update ()
    {
        GetComponentsInChildren<Animator>()[1].SetFloat("speed", GetComponent<Rigidbody2D>().velocity.magnitude * .1f);
        //GetComponent<Rigidbody2D>().velocity = transform.up * 10;
	}

    public void MoveTo(int position)
    {
        StartCoroutine(OnMove(positions[position]));
    }

    IEnumerator OnMove(Vector2 pos)
    {
        while ((transform.position - (Vector3) pos).magnitude > 1)
        {
            Debug.Log("My pos: " + transform.position + "destination: " + pos +" "+(transform.position - (Vector3)pos).magnitude);

            rb.velocity = (( pos - (Vector2)transform.position).normalized * 10);
            //rb.AddForce((pos - (Vector2)transform.position).normalized * 10 * Time.fixedDeltaTime);
            //yield return null;
            yield return new WaitForFixedUpdate();
        }
        rb.velocity = Vector2.zero;
        //Debug.Log("Halp");
        yield break;
    }


    public void DoSomething(int ssdf)
    {

    }
}

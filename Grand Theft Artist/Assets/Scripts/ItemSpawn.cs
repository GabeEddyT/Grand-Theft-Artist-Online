using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour {

    public bool enableSpawn = true; //for toggling off when Networking

    Item[] items;
    
    int randomNum;
    Onomatopoeia ono;
    
	// Use this for initialization
	void Start ()
    {
        items = Resources.LoadAll<Item>("Prefabs/Items");
        ono = Resources.LoadAll<Onomatopoeia>("Prefabs")[0];
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine( Spawn(collision));
    }

    IEnumerator Spawn(Collision2D collision)
    {
        Vector3 pos = collision.transform.position;
        //Debug.Log(collision.relativeVelocity.magnitude);
        Vector2 vec = collision.rigidbody.GetPointVelocity(collision.contacts[0].point);

        if (collision.rigidbody && !collision.rigidbody.isKinematic && (vec.magnitude > 20) && collision.gameObject.layer != 12)
        {
            // Vector2 vec = collision.contacts[0].point - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            //Vector2 vec = collision.relativeVelocity;
            //Debug.Log( collision.rigidbody.GetPointVelocity(collision.contacts[0].point) + " " +vec);
            //vec.Normalize();
            //vec.Scale(new Vector2(-1, -1));
            // vec.Scale(new Vector2(-10, -10));

            Onomatopoeia newOno = Instantiate(ono);
            newOno.GetComponent<AudioSource>().volume = vec.magnitude / 100;
            newOno.scale = vec.magnitude / 10;
            newOno.transform.localPosition = (Vector3)collision.contacts[0].point + new Vector3(0, 0, -5);

            if (!enableSpawn)
            {
                yield break;
            }

            int itemsToSpawn = 0;
            int val = (int)vec.magnitude;

            if (val > 70)
            {
                itemsToSpawn = 13;
            }
            else if (val > 60)
            {
                itemsToSpawn = 8;
            }
            else if (val > 50)
            {
                itemsToSpawn = 5;
            }
            else if (val > 40)
            {
                itemsToSpawn = 3;
            }
            else if (val > 30)
            {
                itemsToSpawn = 2;
            }
            else
                itemsToSpawn = 1;

            for (int i = 0; i < itemsToSpawn; i ++) 
			{
				randomNum = Random.Range(0, items.Length);

				Item item = Instantiate(items[randomNum], collision.contacts[0].point + vec.normalized * 2, transform.rotation);
				//item.transform.localPosition = item.transform.localPosition + new Vector3(0, 0, -4);
				//item.gameObject.layer;
				item.gameObject.layer = 12;

				CircleCollider2D myCollider = item.gameObject.AddComponent<CircleCollider2D>();
				GameObject child = new GameObject();
				child.transform.parent = item.transform;
				child.layer = 12;
				child.transform.localPosition = Vector2.zero;
				//Rigidbody2D childRB = child.AddComponent<Rigidbody2D>();
				//childRB.isKinematic = true;
				//childRB.useFullKinematicContacts = true;
				//GameObject empty = Instantiate(newObj, item.transform);
				CircleCollider2D childCol = child.gameObject.AddComponent<CircleCollider2D>();
				childCol.radius = 1;
				//Debug.Log(item.gameObject.layer);
				//newObj.layer = item.gameObject.laye;
				//child.layer = item.gameObject.layer;
				//myCollider.radius = 

				myCollider.isTrigger = true;
				item.gameObject.AddComponent<Rigidbody2D>();
				Rigidbody2D rb = item.gameObject.GetComponent<Rigidbody2D>();
				//Debug.Log(item.gameObject.name);
				//Debug.Log(item.gameObject.layer);

				rb.mass = .5f;
				rb.drag = 1f;
				rb.angularDrag = 1f;
				rb.AddTorque(Random.value * 50f - 25f);

				float randomAngle = Random.value * 180 - 90;
				Matrix4x4 rotMat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, randomAngle), Vector3.one);
				
				rb.velocity = rotMat.MultiplyVector(vec);
                //rb.AddForceAtPosition(Vector2.up * 24, rb.transform.position + new Vector3(4, 0));

                //Debug.Log("Torque: " + rb.angularVelocity);
                //childRB.velocity = Vector2.zero;
                // rb.velocity = vec;            
                //Debug.Log(rb.angularVelocity);



                //ono.transform.localScale = Vector2.one * collision.relativeVelocity.magnitude;
                // Debug.Log(item);
                yield return null;
			}
        }
    }
}

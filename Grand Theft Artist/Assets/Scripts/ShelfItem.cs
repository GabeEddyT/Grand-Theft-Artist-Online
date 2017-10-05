using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfItem : MonoBehaviour
{
    public Sprite[] items;
	// Use this for initialization
	void Start ()
    {
        gameObject.GetComponent<Transform>().localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -3);
        gameObject.GetComponent<SpriteRenderer>().sprite = items[Random.Range(0, items.Length)];
    }
	
	// Update is called once per frame
	void Update ()
    {
	}
}

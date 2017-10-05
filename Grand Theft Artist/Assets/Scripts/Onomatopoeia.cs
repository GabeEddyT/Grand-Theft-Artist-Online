using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onomatopoeia : MonoBehaviour
{
    public Sprite[] onomatopoeias;
    public float scale = 1.0f;
	// Use this for initialization
	void Start ()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = onomatopoeias[Random.Range(0, onomatopoeias.Length)];

        // ReScale();
    }

    // Update is called once per frame
    void Update ()
    {
        Destroy(gameObject, gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

    }

}

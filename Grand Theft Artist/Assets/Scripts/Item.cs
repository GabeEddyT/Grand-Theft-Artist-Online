using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float value;
	Player player;
    Player []players;
    public float held = 0f;
    Animator anim;
    bool destroyed = false;
    bool collected = false;
    float time;
    public float delay = .5f;

    void Awake()
    {
        players = FindObjectsOfType<Player>();
        player = players[0];
       
    }

	// Use this for initialization
	void Start ()
    {
        anim = gameObject.GetComponent<Animator>();
        StartCoroutine(OnEndSpawn());
        //gameObject.GetComponent<CircleCollider2D>().enabled = false;
        //Debug.Log(gameObject.layer);
        //Rigidbody2D childRB = gameObject.GetComponentInChildren<Rigidbody2D>();
        //childRB.gameObject.layer = gameObject.layer;
    }

    float DistanceTo(Player ob)
    {
        Vector3 diff = transform.position - ob.transform.position;
        return diff.sqrMagnitude;
    }


    // Update is called once per frame
    void Update ()
    {

        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().angularVelocity);

        if (!Pause.paused)
        {
            anim.speed = 1;
            if (!collected)
                PickMe();
            time += Time.deltaTime;
        

        }
        else
        {
            anim.speed = 0;
            
        }
        
    }

    IEnumerator OnEndSpawn()
    {
        while(anim.GetCurrentAnimatorStateInfo(0).normalizedTime <1)
            yield return 0;
        anim.enabled = false;
    }

    private void LateUpdate()
    {
        if(!Pause.paused)
        {
            if (!collected)
                MoveToPlayer();
        }
    }

    private void MoveToPlayer()
    {        
        //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0]);
        //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && destroyed);
        if (destroyed && anim.GetCurrentAnimatorStateInfo(0).IsName("Stolen") && !collected)
        {
            //Debug.Log(transform.rotation);
            transform.position = player.transform.position + new Vector3(0,0,-2);
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !collected)
            {
                //Debug.Log("Yo");
                collected = true;

                Item me = GetComponent<Item>();
                player.Collect(ref me);

                //Debug.Log("Working");
                //destroyed = false;
            }
        }
        

    }

    void PickMe()
    {

        //if (gameObject.GetComponent<Collider2D>().OverlapPoint(new Vector2(player.transform.position.x, player.transform.position.y)))
        //{
        //    held += Time.deltaTime;
        //    Debug.Log(held);
        //}
        //else
        //   held = 0;
        //Debug.Log(held);
        if (held > .1f)
        {
            foreach (Player item in players)
            {
                if (DistanceTo(item) < DistanceTo(player))
                {
                    player = item;
                }
            }
            //transform.position = player.transform.position;
            anim.enabled = true;
            gameObject.GetComponent<Animator>().Play("Stolen");
            //Debug.Log(gameObject.transform.rotation.eulerAngles);

            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            destroyed = true;

            //gameObject.GetComponent<>
            //Debug.Log(anim.GetCurrentAnimatorStateInfo(0));
            //if (anim.GetCurrentAnimatorStateInfo(0).IsName("Stolen"))
            //{
            //    //Debug.Log("Working");
            //    //destroyed = true;

            //}
            //AnimationClip st0l = gameObject.GetComponent<Animator>().GetComponent<Animation>().GetClip("Stolen");
        }
        

        
    }
    

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player") && time > delay)
        {
            held += Time.fixedDeltaTime;
            //Debug.Log(held);
            Player aplayer = collision.GetComponent<Player>();
            foreach (Player item in players)
            {
                if (item == aplayer)
                {
                    player = item;
                }
            }
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            held = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
     
        if (collision.gameObject.layer == LayerMask.NameToLayer("Employee") && !collected)
        {
            Destroy(gameObject);
        }

       

    }
}

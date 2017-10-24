using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

[ExecuteInEditMode]
public class Employee : MonoBehaviour
{

    public Player player;
    public Player[] players;
    CircleCollider2D circle;
    PolygonCollider2D cone;
    BoxCollider2D box;
    public bool hunting;
    float suspicion = 0;
    public bool losing = false;

    //scout
    float lastAngle;
    Vector2 lastLoc;
    public float suspicionRate = .1f;
    public float suspectThreshold = 1f;
    float defaultRotation;
    public float altRotation = 90.0f;
    
    //turning
    ContactFilter2D wallFilter = new ContactFilter2D();
    Collider2D[] newWall = new Collider2D[1];
    LayerMask wallMask;

    //vision cone
    public float angle = 45.0f;     //how wide the employee can see
    public float raidus = 25.0f;    //how far the employee can see
    float myAngle = 0.0f;
    float myRaidus = 0.0f;
    public float speed = 2.0f;
    float chaseTime = 0.0f;
    public float loseThreshold = 10f;

    State currentState = State.survey;

    //States
    enum State
    {
        survey,
        hunt,
        lose,
        returnToStation
    }

    private void Awake()
    {
        players = FindObjectsOfType<Player>();
    }

    // Use this for initialization
    void Start()
    {
        wallFilter.SetLayerMask(LayerMask.GetMask("Block Player"));
        wallMask = LayerMask.GetMask("Block Player");
        box = gameObject.GetComponentInChildren<BoxCollider2D>();
        player = FindObjectOfType<Player>();
        circle = gameObject.GetComponent<CircleCollider2D>();
        cone = gameObject.GetComponentInChildren<PolygonCollider2D>();
        CalcAngle();
        defaultRotation = transform.rotation.eulerAngles.z;
    }
    float DistanceTo(Player ob)
    {
        Vector3 diff = transform.position - ob.transform.position;
        return diff.sqrMagnitude;
    }
    int index = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Application.isPlaying && !Pause.paused)
        {
            ChangeState();
            gameObject.GetComponent<Animator>().SetFloat("Speed", gameObject.GetComponent<Rigidbody2D>().velocity.magnitude / 10.0f);
        }

        CalcAngle();
        
    }

    private void Update()
    {
        if (players[index] != player)
        {
            if (DistanceTo(players[index]) < DistanceTo(player))
            {
                player = players[index];
            }
        }
        index = (index + 1) % players.Length;
    }

    void ChangeState()
    {
        switch (currentState)
        {
            case State.survey:
                Survey();
                break;
            case State.hunt:
                Hunt();
                break;
            case State.lose:
                Lose();
                break;
            default:
                break;
        }
    }

    int turnstep;
    // Employee is actively chasing the player.
    void Hunt()
    {
        if (circle.IsTouching(player.GetComponent<CircleCollider2D>()))
        {

            if (!Turn())
            {
                if (turnstep < 1)
                {
                    Vector3 difference = player.transform.position - gameObject.transform.position;

                    float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90.0f;

                    lastAngle = rotationZ;
                    Quaternion temp = transform.rotation;

                    transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
                }
                if (turnstep > 0)
                {
                    //turnstep--;
                }

                //if (Turn())
                //    return;

                //gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * speed);

                //huntsteps = 0;
                gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * speed);


            }
            else if (turnstep < 3)
            {
                //turnstep++;
            }
            lastLoc = player.transform.position;

        }
        else
        {
            circle.enabled = false;
            currentState = State.lose;
        }
        //gameObject.transform.LookAt(new Vector3(player.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z));

    }


    float faceTime;
    void Survey()
    {
       
        if (cone.IsTouching(player.GetComponent<CircleCollider2D>()) && (suspicion < suspectThreshold))
        {
            suspicion += Time.deltaTime;
            //Debug.Log(suspicion);
            Vector3 difference = player.transform.position - gameObject.transform.position;

            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90.0f;
            GetComponent<Rigidbody2D>().MoveRotation(rotationZ);
        }
        else if (cone.IsTouching(player.GetComponent<CircleCollider2D>()) || circle.IsTouching(player.GetComponent<CircleCollider2D>()))
        {
            //losing = false;
            //hunting = true;
            suspicion = 0;
            circle.enabled = true;
            currentState = State.hunt;
        }
        else if (suspicion > 0)
        {
            suspicion -= Time.deltaTime;
        }
        else
        {
            suspicion = 0;
            if (faceTime < 4 )
            {
                
                faceTime += Time.deltaTime * UnityEngine.Random.value * 2;
            }
            else
            {
                faceTime = 0;
                int randy = UnityEngine.Random.Range(0,2);
                switch (randy)
                {
                    case 1:
                        transform.rotation = Quaternion.AngleAxis(altRotation, Vector3.forward);
                        break;
                    default:
                        transform.rotation = Quaternion.AngleAxis(defaultRotation, Vector3.forward);
                        break;
                }
            }
        }

    }

    bool reachedLastLoc = false;

    void Lose()
    {

        if (cone.IsTouching(player.GetComponent<CircleCollider2D>()))
        {
            chaseTime = 0.0f;
            reachedLastLoc = false;

            circle.enabled = true;
            currentState = State.hunt;
            return;
        }
        if (chaseTime < loseThreshold)
        {
            if (!Turn())
            {
                gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * speed);
            }
            chaseTime += Time.deltaTime;
            if (((Vector2)transform.position - lastLoc).magnitude < 1)
            {
                reachedLastLoc = true;
            }

        }
        else
        {
            chaseTime = 0.0f;
            reachedLastLoc = false;
            circle.enabled = false;
            currentState = State.survey;
        }
    }

    void Patrol()
    {

    }

    void BeSmart()
    {
        //Debug.Log(cone.Distance(newWall[0]).distance);
        //Debug.Log(wall.ToString());

        if (!Turn())
        {
            // Quaternion temp = transform.rotation;
            //transform.rotation = Quaternion.Euler(0.0f, 0.0f, lastAngle);


            //huntsteps = 0;


            //gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * speed);
            gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * speed);
        }


        //gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * speed * -5);
        //Debug.Log(box.Distance(newWall[0]).distance);


    }


    bool wentRight = true;

    bool needAstar = true;
    Vector2 aPoint;
    bool Turn()
    {
        cone.OverlapCollider(wallFilter, newWall);
        Vector2 dir2Player = (lastLoc - (Vector2)transform.position);
        Debug.DrawLine(transform.position, lastLoc);
        RaycastHit2D[] collisions2Player = new RaycastHit2D[3];
        RaycastHit2D hitRay = Physics2D.Raycast(transform.position, transform.up, Mathf.Infinity, wallMask);
        float hit = Physics2D.Raycast(transform.position, transform.up, Mathf.Infinity, wallMask).distance;
        float left = Physics2D.Raycast(transform.position, transform.right * -1, Mathf.Infinity, wallMask).distance;
        float right = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, wallMask).distance;
        //Debug.Log(hit +  " " + left + " " + right);
        Debug.DrawLine(transform.position, hitRay.point);
        float hits = box.Raycast(dir2Player, wallFilter, collisions2Player, dir2Player.magnitude);
        bool canFollowPlayer = hits < 1;
        string outie = "";
        for (int i = 0; i < hits; i++)
        {
            outie += collisions2Player[i].transform.tag + " ";
        }
        outie += hits;
        //Debug.Log(outie);
        Debug.DrawRay(transform.position, dir2Player);
        //Debug.Log(collisions2Player, this);

        // if (cone.IsTouchingLayers(LayerMask.GetMask("Block Player")) && box.Distance(newWall[0]).distance < 3)



        if (!canFollowPlayer || reachedLastLoc)
        {
            if (hit < 10)
            {
                //if (cone.IsTouchingLayers(LayerMask.GetMask("Block Player"))
                //if (needAstar)
                //{
                //    aPoint = Astar();
                //    Vector3 difference = aPoint - (Vector2)gameObject.transform.position;

                //    float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90.0f;

                //    lastAngle = rotationZ;
                //    Quaternion temp = transform.rotation;

                //    transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
                //    //transform.rotation = Quaternion.LookRotation(aPoint - (Vector2)transform.position);

                //    needAstar = false;
                //}

                //if ()
                //{
                //    transform.Rotate(Vector3.forward, -15);
                //    gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right * 3);
                //    wentRight = true;
                //}
                //else
                //{
                    transform.Rotate(Vector3.forward, 15);
                    //gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right * -3);
                    //    wentRight = false;
                    //}
                    //gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
                    //transform.Rotate(Vector3.forward, 15);
                    return true;
            }
            if (left > 5)
            {
                transform.Rotate(Vector3.forward, -1);
            }

            GetComponent<Rigidbody2D>().AddForce(transform.up * speed);
            return true;
            //turnstep = 0;
            //if (collisions2Player[0].distance < 10)
            //{
            //    turnstep++;
            //    GetComponent<Rigidbody2D>().AddForce(transform.up * speed);

            //    if (wentRight)
            //    {
            //        transform.Rotate(Vector3.forward, 1);
            //    }
            //    else
            //        transform.Rotate(Vector3.forward, -1);

            //    return false;
            //}
            if (((Vector2)transform.position - lastLoc).magnitude > 1)
            {                
                GetComponent<Rigidbody2D>().AddForce(transform.up * speed);
                
            }
            else
            {
                if (left > 10 || left == 0)
                {
                    transform.Rotate(Vector3.forward, -90);
                }
                else
                {
                    transform.Rotate(Vector3.forward, 90);
                }
                GetComponent<Rigidbody2D>().AddForce(transform.up * speed);
                needAstar = true;
                return true;
                
            }
            return true;

        }
        //Vector3 difference = player.transform.position - gameObject.transform.position;

        if (!reachedLastLoc)
        {
            float rotationZ = Mathf.Atan2(dir2Player.y, dir2Player.x) * Mathf.Rad2Deg - 90.0f;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

        }
                
               
        //Debug.Log(currentState, this);

        needAstar = true;
        return false;
            //gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * speed);
            //return true;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            hunting = true;
            Hunt();
            Survey();
        }

        

        //if (collision.gameObject.layer == LayerMask.NameToLayer("Block Player"))
        //{
        //    gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * -5);
        //    transform.Rotate(Vector3.forward, 180);

        //}

    }

    //    void OnTriggerStay2D(Collider2D collided)
    //    {
    //        if (collided.name == "Player")
    //        {
    //            //Hunt();
    //            hunting = true;
    //        }
    //    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsTouchingLayers(LayerMask.NameToLayer("Player")))
        {
            player = collision.GetComponent<Player>();
        }
    }
    //
    //    void OnTriggerExit2D(Collider2D collided)
    //    {
    //        if (collided.name == "Player")
    //        {
    //            //Hunt();
    //            gameObject.GetComponent<CircleCollider2D>().enabled = false;
    //            gameObject.GetComponent<PolygonCollider2D>().enabled = true;
    //            hunting = false;
    //            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 0));
    //        }
    //    }
    //


    void playSound(int sound)
    {
        GetComponents<AudioSource>()[sound].Play();
    }

    int huntsteps = 0;
    

    void CalcAngle()
    {
        if (myAngle == angle / 2.0f && myRaidus == raidus)
        {
            return;
        }

        myAngle = angle / 2.0f;
        myRaidus = raidus;
        //Debug.Log("CHange");
        //GetComponent<PolygonCollider2D>().points[1] = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        //GetComponent<PolygonCollider2D>().points[2] = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), -1 * Mathf.Sin(Mathf.Deg2Rad * angle));
        //circle.radius = raidus;
        float length = raidus / (Mathf.Cos(Mathf.Deg2Rad * myAngle));
        
        Vector2[] myPoints = { cone.points[0], new Vector2(Mathf.Sin(Mathf.Deg2Rad * myAngle) * length, Mathf.Cos(Mathf.Deg2Rad * myAngle) * length), new Vector2(- 1* Mathf.Sin(Mathf.Deg2Rad * myAngle) * length, Mathf.Cos(Mathf.Deg2Rad * myAngle) * length) };
        //gameObject.GetComponent<PolygonCollider2D>().points[1].Set(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        //gameObject.GetComponent<PolygonCollider2D>().points[2].Set(Mathf.Cos(Mathf.Deg2Rad * angle), -1 * Mathf.Sin(Mathf.Deg2Rad * angle));
        cone.points = myPoints;
		//gameObject.GetComponent<PolygonCollider2D>().points.
        //Debug.Log(myPoints[1]);       

    }

    Vector2 Astar()
    {
        Vector2[] points = new Vector2[5];
        points[0] = transform.position;
        
        RaycastHit2D leftpoint = Physics2D.Raycast(transform.position, transform.right * -1, 100, wallMask);
        RaycastHit2D rightpoint = Physics2D.Raycast(transform.position, transform.right * -1, 100, wallMask);



        for (int i = 0; i < 10; i++)
        {
            RaycastHit2D newCast = Physics2D.Raycast((rightpoint.point - (Vector2)transform.position) / 10 * i, transform.up, 10, wallMask);
            if (newCast.distance < 1)
            {
                points[1] = newCast.point;
                break;
            }

        }

        for (int i = 0; i < 10; i++)
        {
            RaycastHit2D newCast = Physics2D.Raycast((leftpoint.point - (Vector2)transform.position) / 10 * i, transform.up, 10, wallMask);
            if (newCast.distance < 1)
            {
                points[2] = newCast.point;
                break;
            }
            
        }

        if ((points[1] - (Vector2) transform.position).magnitude < (points[2] - (Vector2) transform.position).magnitude)
        {
            return points[1];
        }
        else
        {
            return points[2];
        }

    }
    
}

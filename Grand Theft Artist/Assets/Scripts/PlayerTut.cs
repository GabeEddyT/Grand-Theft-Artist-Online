using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerTut : MonoBehaviour
{

    public float maxspeed = 10f;
    public float speed = 0f;
    public Text mps;
    public Text money;
    Rigidbody2D rb;
    float relVel;
    public int cameraMode = 0;
    public Camera miniMap;
    public RawImage uiMap;
    public int mapMode = 0;
    public float cash = 0;
    public Image dollar;
    public float goalAmount = 1000.0f;
    public GameObject cart;
    bool cutscene = true;

    public SpriteRenderer[] cartItems = new SpriteRenderer[4];
    int itemCount = 0;

    // Use this for initialization
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        gameObject.GetComponent<Animator>().SetFloat("speed", speed);
        GetComponent<TrailRenderer>().sortingLayerName = "Background";
        GetComponent<TrailRenderer>().sortingOrder = 1;
    }

    void Update()
    {
        if (!Pause.paused)
        {
            GetComponent<Animator>().speed = 1;

            if (!cutscene)
            {
                CameraControl();
            }
        }
        else
        {
            GetComponent<Animator>().speed = 0;
        }
        if (Input.GetKey(KeyCode.Backspace))
        {
            SceneManager.LoadScene(0);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Pause.paused && !cutscene)
        {
            Move();
        }
    }

    void Move()
    {
        relVel = speed > 0 ? rb.velocity.magnitude / 10.0f : rb.velocity.magnitude / -10.0f;
        Vector3 rotVec = new Vector3(gameObject.transform.rotation.x, gameObject.transform.rotation.y, Input.GetAxis("Horizontal") * -5);

        //gameObject.transform.TransformDirection(new Vector3(gameObject.transform.rotation.x, gameObject.transform.rotation.y, Input.GetAxis("Horizontal")));

        //gameObject.transform.rotation = rotate;
        gameObject.transform.Rotate(rotVec);

        //gameObject.transform.Translate(0, Input.GetAxis("Vertical"), 0, Space.Self);//actually works
        //Vector3 eulie = gameObject.transform.localEulerAngles;

        //Debug.Log();
        //var localDirection = transform.rotation * Vector2.up;

        //localDirection = transform.InverseTransformDirection(Vector2.up);
        //Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        //Debug.Log(rotVec);
        //Vector3 forward = transform.InverseTransformDirection(Vector2.up);

        //Debug.Log(new Vector2(gameObject.transform.localRotation.z, gameObject.transform.localRotation.w));
        //Debug.Log(Input.GetAxis("Vertical"));

        //speed = Input.GetAxis("Vertical");
        float gas = Input.GetAxis("Gas") + Input.GetAxis("Vertical");
        if (gas > 0)
        {
            //rb.mass = 0.01f;
            if (speed < 0)
            {
                speed += Time.deltaTime * gas * 2 * (speed * speed + 2);
            }
            else if (speed < gas * maxspeed)
            {
                speed += Time.deltaTime * gas * (maxspeed / (speed + 1));
            }
            else if (speed > gas * maxspeed)
            {
                speed -= Time.deltaTime * gas;
            }
            if (speed > maxspeed)
            {
                speed = maxspeed;
            }

            // rb.AddForce(transform.up * speed);
        }
        else if (gas < 0)
        {

            if (Mathf.Round(rb.velocity.magnitude) > 0.1f && speed > 0.0f)
            {
                speed += Time.deltaTime * gas * (speed * 2 + 2);
                //rb.mass = Mathf.Abs(Input.GetAxis("Vertical"));
                //if (speed < 0)
                //{
                //    speed = 0.0f;
                //}
            }
            else
            {
                //rb.mass = 0.01f;
                if (speed > maxspeed * -.4f)
                {
                    speed += Time.deltaTime * gas * maxspeed;
                }
                if (speed < maxspeed * -.4f)
                {
                    speed -= Time.deltaTime * gas * maxspeed;
                }
            }

        }
        else if (speed > .1)
        {
            speed -= Time.deltaTime;
        }
        else if (speed < -.1)
        {
            speed += Time.deltaTime;
        }
        else
            speed = 0;

        rb.AddForce(transform.up * speed);

        //rb.velocity = transform.up * speed * 10.0f;



        // speed = Input.GetAxis("Vertical");
        // float outthis = Mathf.Round(speed * 10.0f) / 10.0f;


        float outthis = speed > 0 ? Mathf.Round(rb.velocity.magnitude * 1.0f) / 10.0f : Mathf.Round(rb.velocity.magnitude * 1.0f) / -10.0f;
        mps.text = (outthis % 1 == 0 ? outthis + ".0" : outthis + "") + " MPH";

        //if (Input.GetAxis("Vertical") > .5 && speed < maxspeed)
        //{
        //    speed += maxspeed / speed * Input.GetAxis("Vertical") * Time.deltaTime;
        //}
        //else 
        //    speed = Input.GetAxis("Vertical") * 10;
        //speed = Input.GetAxis("Vertical"10;

        //gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * speed);
        //rb.velocity = transform.up * speed;


        //Debug.Log(rb.velocity.magnitude);
        //rb.AddTorque(Input.GetAxis("Horizontal"), ForceMode2D.Impulse);

        //gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(25, 3));
        //Debug.Log(transform.TransformDirection(transform.InverseTransformDirection(transform.up)));
        if (speed != 0)
        {
            gameObject.GetComponent<Animator>().SetFloat("speed", speed);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetFloat("speed", Input.GetAxis("Horizontal"));
        }
    }

    void CameraControl()
    {
        //if (Input.GetButton("Right Bumper"))
        //    Camera.main.transform.Rotate(Vector2.right);
        //Camera.main.transform.rotation = Quaternion.Euler(new Vector3 (0,0, Input.GetAxis("Camera") * -90 + transform.localEulerAngles.z));
        if (Input.GetButtonDown("Camera"))
        {
            if (Input.GetAxis("Camera") < 0)
                cameraMode++;
            else
            {
                mapMode++;
                //Debug.Log("Derp" + mapMode);
            }
        }


        cameraMode %= 2;
        mapMode %= 4;

        switch (mapMode)
        {
            default:
                uiMap.enabled = true;
                uiMap.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 250);
                uiMap.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 250);
                break;
            case 1:
                uiMap.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 400);
                uiMap.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 400);
                //uiMap.rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case 2:
                uiMap.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 700);
                uiMap.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 700);
                //uiMap.rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case 3:
                uiMap.enabled = false;
                break;


        }

        switch (cameraMode)
        {
            default:
                Camera.main.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
                break;
            case 1:
                Camera.main.transform.rotation = transform.rotation;
                break;
        }
        //Debug.Log(relVel);
        //Debug.Log(1 / (Mathf.Abs(speed) + .1f));
        Camera.main.transform.localPosition = Vector3.MoveTowards(Camera.main.transform.localPosition, new Vector3(0, 10.0f * (relVel / maxspeed), -20), ((Mathf.Abs(speed) + .1f)) * Time.deltaTime);

        //Debug.Log(Input.GetAxis("Camera"));


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        speed = 0;
    }

    int painCount;

    public void Collect(ref Item item)
    {

        item.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Item";
        //Debug.Log(item.transform.rotation);
        cash += item.value;
        dollar.fillAmount = (cash / 1000);
        // Debug.Log(cash % 0.1);
        //Debug.Log(cash);
        dollar.GetComponent<Animator>().SetFloat("money", cash);
        money.text = "$" + cash.ToString("F2");
        //Debug.Log(item.transform.rotation.eulerAngles);
        //Debug.Log(cart.gameObject.transform.rotation);
        //Debug.Log(cart.gameObject.transform.localRotation);

        //Debug.Log(transform.rotation);


        item.GetComponent<SpriteRenderer>().sortingOrder = itemCount;
        //item.transform.rotation = rot;
        Destroy(item.GetComponent<Rigidbody2D>());
        Destroy(item.transform.GetChild(0).gameObject);
        Destroy(item.transform.GetChild(1).gameObject);
        Destroy(item.transform.GetComponent<CircleCollider2D>());

        item.gameObject.transform.SetParent(cart.transform, true);

        item.transform.rotation = Quaternion.Euler(Vector3.up * 75);

        //Debug.Log(item.transform.localRotation = transform.rotation);
        //if (itemCount < 4)
        {
            itemCount++;
        }
        playSound(2);
        if (Mathf.Round(item.value) == 25)
        {
            GetComponent<TrailRenderer>().enabled = true;
            painCount++;
            Color newColor = new Color(Random.value, Random.value, Random.value);
            //Debug.Log(newColor);
            Gradient grad = GetComponent<TrailRenderer>().colorGradient;
            GradientColorKey[] colors = GetComponent<TrailRenderer>().colorGradient.colorKeys;
            GradientAlphaKey[] alphas = GetComponent<TrailRenderer>().colorGradient.alphaKeys;

            grad.SetKeys(new GradientColorKey[]
            {
                new GradientColorKey(colors[3].color, 1),
                new GradientColorKey(colors[2].color, .75f),
                new GradientColorKey(colors[1].color, .5f),
                new GradientColorKey(colors[0].color, .25f),
                new GradientColorKey(newColor, 0),}, alphas);
            GetComponent<TrailRenderer>().colorGradient = grad;
            //GradientAlphaKey[] alphas = GetComponent<TrailRenderer>().
            //Debug.Log(colors.Length);
            //colors[0].color = colors[1].color;
            //colors[1].color = colors[2].color;
            //colors[3].color = colors[4].color;
            //colors[4].color = newColor;
            //Debug.Log("from aray" + colors[4].color);
            //Debug.Log(grad.colorKeys[4]);
            //GetComponent<TrailRenderer>().colorGradient.colorKeys.SetValue(colors[, 4);
            //Debug.Log(colors);
        }


        //GetComponent<TrailRenderer>().colorGradient.colorKeys
    }

    void StealItem()
    {

    }

    void playSound(int sound)
    {
        GetComponents<AudioSource>()[sound].Play();

        //GetComponents<AudioSource>()[sound].
    }

    //void OnTriggerEnter2D(Collider2D collided)
    //{
    //    if (collided.CompareTag("Employee"))
    //    {
    //        collided.GetComponent<Employee>().hunting = true;
    //    }
    //}

    //void OnTriggerStay2D(Collider2D collided)
    //{
    //    if (collided.CompareTag("EmployeeBound"))
    //    {
    //        // collided.GetComponentInParent<Employee>().hunting = false;
    //    }
    //}

    //void OnTriggerExit2D(Collider2D collided)
    //{
    //    if (collided.CompareTag("Employee"))
    //    {
    //        collided.GetComponent<Employee>().hunting = false;
    //    }
    //    else if (collided.CompareTag("EmployeeBound"))
    //    {
    //        collided.GetComponentInParent<Employee>().hunting = true;
    //    }
    //}
}

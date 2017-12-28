using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Startup : MonoBehaviour {
    public Image startEngine;
    public Text pushMe;
    public Text start;
    int dumbCount = 0;
    bool notHeld = false;

	// Use this for initialization
	void Awake ()
    { 
        StartCoroutine(Holdup());
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log(Input.GetAxis("Start"));
        //Debug.Log(Input.GetAxis("Start"));
        if (!notHeld)
        {
            return;
        }
        
        if (Input.GetKeyDown("return"))
        {
            //Debug.Log(Input.GetAxis("Pause"));
            SceneManager.LoadScene(1);
        }
        else if(Input.anyKeyDown)
        {
            dumbCount++;
            switch (dumbCount)
            {
                case 1:
                    start.enabled = true;
                    break;
                case 2:
                    startEngine.enabled = true;
                    break;
                case 3:
                    pushMe.enabled = true;
                    break;
                default:
                    Image img = Instantiate(startEngine);
                    Rect myRekt = startEngine.rectTransform.rect;
                    //img.transform.parent = startEngine.transform.parent;
                    img.transform.SetParent(startEngine.transform.parent, false);
                    img.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, Random.value * 686, 68);
                    img.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Random.value * 1212, 68);
                    //img.rectTransform.localScale = startEngine.rectTransform.localScale;
                    break;

            }
            GetComponent<AudioSource>().Play();
            
        }
	}

    IEnumerator Holdup()
    {
        while(Input.GetAxis("Start") != 0)
        {
            notHeld = false;
            yield return 0;
        }
        notHeld = true;
        yield break;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public static bool paused = false;
    public GameObject pauseText;
    int mash = 0;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetButtonDown("Pause") || Input.GetAxisRaw("Start") != 0 && mash < 1)
        {
            TogglePause();
            mash++;
        }
        else if(!Input.GetButtonDown("Pause") && Input.GetAxisRaw("Start") == 0)
        {
            mash = 0;
        }
        //Debug.Log(paused);
        if (paused)
        {
            Time.timeScale = 0;
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1;
            AudioListener.pause = false;

        }
        pauseText.SetActive(paused);
    }

    public static void TogglePause(int mode = 0)
    {
        switch (mode)
        {
            default:
                paused = !paused;
                break;
            case 1:
                paused = true;
                break;
            case 2:
                paused = false;
                break;
                
        }
    }


    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            TogglePause(1);
        }
    }
}

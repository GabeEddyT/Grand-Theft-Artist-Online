using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System;

public class CinematicControl : MonoBehaviour
{
    public Canvas canvas;
    public VideoPlayer standalone; //standard player
    private VideoPlayer myPlayer; //the player we're using
    private bool paused;
    public bool pauseUnfocus = true;

    private bool ended = false;

    int held = 1;

    private void Awake()
    {
        paused = false;

        myPlayer = standalone;
        
        myPlayer.prepareCompleted += OnPrepared;
        myPlayer.loopPointReached += OnEndReached;
        myPlayer.Prepare();            
    }

    // Use this for initialization
    void Start()
    {
        myPlayer.Play();
    }    

    // Update is called once per frame
    void Update()
    {
        //Debug.Log (vp.frameCount);

    }

    private void OnPrepared(VideoPlayer source)
    {
        Debug.Log("prepared");
        source.Play();
        StartCoroutine(Transistion());
    }

    private IEnumerator PrepareCinematic()
    {
        while (!myPlayer.isPrepared)
        {
            Debug.Log("preparing");
            yield return 0;
        }
        StartCoroutine(Transistion());
        yield break;
    }

    private IEnumerator Transistion()
    {
        myPlayer.Play();
        while (!ended)
        {
                CheckInput();

            yield return 0;            
        }
        
        yield break;
    }

    private void PauseMovie(int mode = 0)
    {
        if (mode == 1)
        {
            myPlayer.Pause();
        }
        else if (mode == 2)
        {
            myPlayer.Play();
        }
        else
        {
            if (myPlayer.isPlaying)
            {
                myPlayer.Pause();
            }
            else if (myPlayer.isPrepared)
            {
                myPlayer.Play();
                
            }
        }
        CheckPause();

    }

    void OnEndReached(VideoPlayer source)
    {
        Debug.Log("Reached end...");
        ended = true;
        SceneManager.LoadScene(5);
    }

    private void CheckPause()
    {
        if (myPlayer.isPlaying || ended)
        {
            paused = false;
            canvas.enabled = false;
        }
        else if (myPlayer.isPrepared)
        {
            paused = true;
            canvas.enabled = true;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && pauseUnfocus)
        {
            PauseMovie(1);
        }
    }
    

    private void CheckInput()
    {       

        if (Input.GetAxis("Start") != 0 || Input.GetButtonDown("Pause"))
        {
            if (held < 1)
            {                
                PauseMovie();
            }
            held++;
        }
        else if (Input.GetAxis("Start") == 0)
        {
            held = 0;
        }


#if !UNITY_WEBGL
        if (Input.GetAxis("Gas") > .9 || Input.GetAxis("Vertical") > .9)
        {
            SceneManager.LoadScene(5);
        }
#endif
        if (Input.GetKeyDown(KeyCode.End))
        {
            SceneManager.LoadScene(5);
        }
    }
}


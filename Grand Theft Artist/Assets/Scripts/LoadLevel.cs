using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class LoadLevel : MonoBehaviour
{
    //public Button defaultButton;

    AudioSource mus;            
	// Use this for initialization
	void Start ()
    {
        //defaultButton.Select();
        mus = GetComponent<AudioSource>();

        StartCoroutine(GetInput());
    }
	
	// Update is called once per frame
	void Update ()
    {
	}

    public void LoadMyLevel(int index)
    {
        StartCoroutine(WaitForQueue(index));        
    }

    

    IEnumerator GetInput()
    {
        while (EventSystem.current)
        {
            //Canvas canvas = GetComponent<Canvas>();
            //UnityEngine.EventSystems.EventSystem myStem = gameObject.GetComponent<
            Button selected = null;
            if (EventSystem.current.currentSelectedGameObject)
            {
                selected = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            }


            if (Input.GetAxis("Start") != 0 || Input.GetAxis("Pause") != 0)
            {
                ExecuteEvents.Execute(selected.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }

            if (Input.GetButtonDown("Cancel"))
            {
                LoadMyLevel(0);
            }
            yield return 0; 
        }
    }

    IEnumerator WaitForQueue(int level)
    {
        Button selected = null;
        if (EventSystem.current.currentSelectedGameObject)
        {
            selected = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        }
        ExecuteEvents.Execute(selected.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.deselectHandler);
        EventSystem.current.enabled = false;
        mus.loop = false;
        yield return new WaitWhile(() => mus.isPlaying);
        SceneManager.LoadScene(level);
        
        yield break;
    }
}

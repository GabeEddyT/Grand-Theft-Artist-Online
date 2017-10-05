using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public GameObject shopper;
    public Text message;
    public GameObject textbox;
    Animator animator;
    public GameObject hint;
    public GameObject rick;
    AudioSource mus;

    State currentState = 0;

    enum State
    {
        ALONE,
        HERE,
        MEET,
        FCUK,
        HUH,
        JEFF,
        RICK
    }

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();

        mus = GetComponents<AudioSource>()[3];

        
        StartCoroutine(WaitForAnimation(animator));
        StartCoroutine(WaitForMus(mus));

        animator.Play("Alone");

        StartCoroutine(OnStateChange(State.HERE));
        
        Alone();
    }

    private IEnumerator OnStateChange(State state)
    {
        while (currentState != state)
        {
            yield return 0;
        }

        switch (state)
        {
            case State.ALONE:
                break;
            case State.HERE:
                StartCoroutine(WaitForInput());
                message.text = "Michelle's";
                StartCoroutine(OnStateChange(State.MEET));
                break;
            case State.MEET:
                shopper.GetComponent<Animator>().Play("Act");
                animator.Play("Meet");
                StartCoroutine(WaitForAnimation(shopper.GetComponent<Animator>()));
                break;
            case State.FCUK:
                animator.Play("Huh");
                message.text = "HUh?";
                StartCoroutine(WaitForAnimation(animator));
                break;
            case State.RICK:
                StartCoroutine(WaitForInput());
                break;
            default:
                break;
        }
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.ALONE:
                //Debug.Log(currentState);
                break;
            case State.HERE:
                break;
            case State.MEET:                
                break;
            case State.FCUK:
                break;
            default:
                break;
        }
    }

    void Alone()
    {
        
    }

    IEnumerator WaitForMus(AudioSource mus)
    {
        mus.time = 7;
        mus.Play();
        
        yield return new WaitWhile(()=>mus.time < 17);
        do
        {
            mus.volume *= .5f;
            yield return null;
        } while (mus.volume > .02);

        Debug.Log(mus.time);
        mus.Stop();
        mus.volume = .1f;
        mus.time = 18f;
        Debug.Log(mus.time);
        yield break;

    }

    IEnumerator WaitForAnimation(Animator anim)
    {
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return 0;
        }
        switch (currentState)
        {
            case State.ALONE:                
                StartCoroutine(WaitForInput());
                Debug.Log("This part works");
                break;
            case State.HERE:
                break;
            case State.MEET:
                while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Act"))
                {
                    yield return null;
                }
                while (anim.GetCurrentAnimatorStateInfo(0).IsName("Act"))
                {
                    yield return null;

                }
                message.text = "Hey, jackass";
                animator.Play("FCUK");
                StartCoroutine(OnStateChange(State.FCUK));
                break;
            case State.RICK:
                animator.SetFloat("pauser", 1.0f);
                break;
            case State.FCUK:
                break;
            case State.JEFF:
                animator.SetFloat("pauser", 1.0f);
                break;
            default:
                break;
        }
        yield break;
    }

    IEnumerator WaitForInput()
    {
        while (Input.GetAxis("Submit") != 0)
        {
            yield return 0;
        }

        while (Input.GetAxis("Submit") == 0)
        {
            yield return 0;
        }        

        switch (currentState)
        {
            case State.ALONE:                
                ChangeState(State.HERE);
                break;
            case State.HERE:
                Debug.Log("pls");
                ChangeState(State.MEET);
                break;
            case State.MEET:
                ChangeState(State.FCUK);
                break;
            default:
                animator.SetFloat("pauser", 1.0f) ;
                break;
        }
        yield break;
    }

    void ChangeState(State state)
    {
        currentState = state;
        Debug.Log(currentState);
    }

    void ChangeText(String text)
    {
        message.text = text;
    }

    void PrepareNext()
    {
        animator.SetFloat("pauser", 0.0f);
        StartCoroutine(WaitForInput());
    }

    void PrepareForJeff()
    {
        animator.SetFloat("pauser", 0.0f);
        ChangeState(State.JEFF);
        shopper.GetComponent<Animator>().Play("Jus walk away");
        StartCoroutine(WaitForAnimation(shopper.GetComponent<Animator>()));
    }

    void CheckForGas()
    {
        StartCoroutine(OnGas());
        hint.SetActive(true);
    }

    IEnumerator OnGas()
    {
        while (Input.GetAxis("Gas") < .9f && Input.GetAxis("Vertical") < .9f)
        {
            
            yield return null;
            
        }
        hint.SetActive(false);

        animator.Play("Sprint");

        yield break;
    }

    void CheckForRick()
    {
        animator.SetFloat("pauser", 0.0f);
        ChangeState(State.RICK);
        rick.GetComponent<Animator>().Play("Greet");
        StartCoroutine(WaitForAnimation(rick.GetComponent<Animator>()));
    }

    void CheckTurn()
    {
        StartCoroutine(OnTurn());
    }

    IEnumerator OnTurn()
    {
        while (Input.GetAxis("Horizontal") == 0)
        {
            hint.GetComponent<Text>().text = "[Turn the wheel]";
            hint.SetActive(true);
            yield return null;
            animator.SetFloat("pauser", 0.0f);
        }
        hint.SetActive(false);
        animator.SetFloat("pauser", 1.0f);
        
        yield break;
    }

    void GoBack()
    {
        SceneManager.LoadScene(5);
    }
}

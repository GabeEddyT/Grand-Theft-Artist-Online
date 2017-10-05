using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {
    Player player;
    BoxCollider2D box;
	// Use this for initialization
	void Start () {
        player = FindObjectOfType<Player>();
        box = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        checkWinstate();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            SceneManager.LoadScene(3);
        }
    }

    private void checkWinstate()
    {
        if (player.cash >= player.goalAmount)
        {
            box.enabled = true;
        }
        else
            box.enabled = false;
    }
}

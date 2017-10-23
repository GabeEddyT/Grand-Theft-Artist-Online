using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    public Player player;
    // Use this for initialization
    void Start()
    {
        //player = FindObjectOfType<Player>();
    }
	
    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Image>().fillAmount = Mathf.Abs(player.speed / player.maxspeed);
        gameObject.GetComponent<Image>().color = new Color(Mathf.Abs(player.speed * 1.75f / player.maxspeed), Mathf.Abs(1 - (player.speed * .75f / player.maxspeed)), 0, .5f);
    }
}

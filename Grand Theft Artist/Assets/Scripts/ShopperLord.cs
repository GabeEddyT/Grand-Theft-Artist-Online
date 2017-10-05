using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopperLord : MonoBehaviour {

    public ShopperScript[] shoppers;
    public int radicat;

	// Use this for initialization
	void Start ()
    {
        shoppers = GetComponentsInChildren<ShopperScript>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Move(float x, float y, int index)
    {
        //shoppers[index].MoveTo(x, y);
    }

    public void DoSomething(Vector2 hack)
    {

    }

    public void DoSomething(GameObject instruction)
    {
        instruction.GetComponent<ShopperInstruction>().Command();
    }

    public void DoEverything(AnimationEvent yellr)
    {
        
    }
}

public class Yeller
{
    
}
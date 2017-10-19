using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.InteropServices;


public class NetworkInput : MonoBehaviour {

    [DllImport("MyFrameworkPlugin")]
    static extern int Startup();

    [DllImport("MyFrameworkPlugin")]
    static extern unsafe char* CStringTest();

    [DllImport("MyFrameworkPlugin")]
    static extern unsafe char* InputTest(char* stuff);

    [DllImport("MyFrameworkPlugin")]
    static extern unsafe char* getTest();
    [DllImport("MyFrameworkPlugin")]
    static extern IntPtr returnToSender(IntPtr delivery);
    // Use this for initialization

    enum Messages
    {
        INPUT,
    }



    public struct InputMessage
    {
        public int id;
        public float vertical;
        public float horizontal;
    }


    void Start () {
        sendAndReceiveStruct();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    unsafe void debugCstring()
    {
        string str = Marshal.PtrToStringAnsi((System.IntPtr)CStringTest());
            Debug.Log(str);
        string mama = "hello there";
        System.Text.StringBuilder tester = new System.Text.StringBuilder(mama);
        // char* the;
        string pStr = "Hello World!";

        InputMessage* pChars = (InputMessage*)Marshal.StringToHGlobalAnsi(pStr).ToPointer();
        char* pSomething = (char*)pChars;
        char* charArray;
        InputMessage* pTerrible;
        InputMessage* pTulips = (InputMessage*)Marshal.StringToHGlobalAnsi(new string(pSomething)).ToPointer();

        fixed (char* p = "hey hello yada")
        {
            //string derp = Marshal.PtrToStringAnsi((System.IntPtr)(p));
            string derp = new string(p);
            Debug.Log(new string(p));
            
        }

        
    }

    unsafe void debugStruct()
    {
        InputMessage tom = (InputMessage) Marshal.PtrToStructure((System.IntPtr)getTest(), typeof( InputMessage)); //this works :)
        
        Debug.Log("tom's id: " + tom.id);

        Debug.Log("tom's horizontal: " + tom.horizontal);
        Debug.Log("tom's vertical: " + tom.vertical);


    }

    void sendAndReceiveStruct()
    {
        InputMessage lambda = new InputMessage();
        lambda.vertical = 17.0f;
        lambda.horizontal = 25.0f;

        IntPtr kappa = Marshal.AllocHGlobal(Marshal.SizeOf(lambda));
        
        Marshal.StructureToPtr(lambda, kappa,false);
        
        kappa = returnToSender(kappa);
        
        InputMessage backHome = (InputMessage)Marshal.PtrToStructure(kappa, typeof(InputMessage));
        Debug.Log("Horizontal: " + backHome.horizontal + " Vertical: " + backHome.vertical + " ID: " + backHome.id);
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;


public class NetworkInput : MonoBehaviour {

    [DllImport("MyFrameworkPlugin")]
    static extern int Startup();

    [DllImport("MyFrameworkPlugin")]
    static extern unsafe char* CStringTest();

    [DllImport("MyFrameworkPlugin")]
    static extern IntPtr InputTest(IntPtr stuff);

    [DllImport("MyFrameworkPlugin")]
    static extern IntPtr getTest();
    [DllImport("MyFrameworkPlugin")]
    static extern IntPtr returnToSender(IntPtr delivery);
    [DllImport("MyFrameworkPlugin")]
    static extern int initNetworking(int serverPort, [MarshalAs(UnmanagedType.LPStr)] String ip);
    [DllImport("MyFrameworkPlugin")]
    static extern unsafe char* getNetworkPacket();
    [DllImport("MyFrameworkPlugin")]
    static extern void sendNetworkPacket(IntPtr packet);
    // Use this for initialization

    

    enum Messages
    {
        MESSAGE = 135,
        INPUT
    }

    public Text ip;
    public Text serverPort;
    bool initFlag = false;

    public struct InputMessage
    {
        public int id;
        public float vertical;
        public float horizontal;
    }

    
    public unsafe struct BetaString
    {
        public int id;
        public fixed char pseudoString[512];
    }

    void Start () {
        //sendAndReceiveStruct();
        Startup();
	}
	
	// Update is called once per frame
	void Update () {
        if (initFlag)
        {
            getPacket();
            //initFlag = false;
        }
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
        InputMessage tom = (InputMessage) Marshal.PtrToStructure(getTest(), typeof( InputMessage)); //this works :)
        
        Debug.Log("tom's id: " + tom.id);

        Debug.Log("tom's horizontal: " + tom.horizontal);
        Debug.Log("tom's vertical: " + tom.vertical);


    }

    void SendAndReceiveStruct()
    {
        InputMessage lambda = new InputMessage();
        lambda.vertical = 115.7f;
        lambda.horizontal = 38.129f;

        IntPtr kappa = Marshal.AllocHGlobal(Marshal.SizeOf(lambda));
        
        Marshal.StructureToPtr(lambda, kappa, false);
        
        kappa = returnToSender(kappa);
        
        InputMessage backHome = (InputMessage)Marshal.PtrToStructure(kappa, typeof(InputMessage));
        Debug.Log("Horizontal: " + backHome.horizontal + " Vertical: " + backHome.vertical + " ID: " + backHome.id);
    }

    public void ConnectToRakNet()
    {
        // = ip.text;
        int port = Int32.Parse(serverPort.text);
        //Debug.Log();
        initNetworking(port, ip.text);
        initFlag = true;
    }

    unsafe void getPacket()
    {
        //char* charPtr = (char*) getNetworkPacket();

        char* packet = getNetworkPacket();
        //if (charPtr == null)
        //{
        //    return;
        //}
        ////if (packet == IntPtr.Zero)
        //{
        //    return;
        //}

        if (packet != null)
        {

        }
        else
            return;

        //Debug.Log(packet[0]);
       // char* cPack = (char*) packet;
        switch ((Messages)packet[0])
        {
            case Messages.MESSAGE:
  
                IntPtr care = (IntPtr) packet;
                BetaString* poi = (BetaString*)care;
                //BetaString bs = *(BetaString*) packet;
                //char* cs = bs.pseudoString;
                //string likeActualString = new string(cs);

                //Debug.Log((*bs.pseudoString).ToString());
                //System.Text.StringBuilder theSood = (System.Text.StringBuilder)Marshal.PtrToStructure((IntPtr)packet, typeof(System.Text.StringBuilder));
                //string makeReal = Marshal.PtrToStringAuto((char*)theSood.ToPointer());

                //Debug.Log(new string(theSood));
                //Debug.Log(theSood);
                //Console.WriteLine(*theSood);
                string outputPls = Marshal.PtrToStringAnsi((IntPtr)poi->pseudoString) ; /*new string(bs.pseudoString) +*/ //new string(poi->pseudoString);
                Debug.Log(outputPls);
                break;
            case Messages.INPUT:
                Debug.Log("I shouldn't be *receiving* input...");
                break;
            default:
                Debug.Log("Message with identifier: " + packet[0]);
                break;
        }
    }
}

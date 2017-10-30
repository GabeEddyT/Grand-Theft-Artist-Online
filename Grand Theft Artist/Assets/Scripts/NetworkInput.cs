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
    static extern void sendNetworkPacket(IntPtr packet, int size);
    [DllImport("MyFrameworkPlugin")]
    static extern unsafe void  sendChatMessage(string message);
    [DllImport("MyFrameworkPlugin")]
    static extern unsafe IntPtr getGUID();
    // Use this for initialization


    enum Messages
    {
        REQUEST_ACCEPTED = 16,
        MESSAGE = 135,
        INPUT = 136,
        GUID
    }

    string guid;
    public Text ip;
    public Text serverPort;
    //public Text chatMess;
    public InputField chatMess;
    bool initFlag = false;
    bool inputFlag = false;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct InputMessage
    {
        public byte id;
        public float vertical;
        public float horizontal;
        public string guid;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BetaString
    {
        public byte id;
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

            //    //initFlag = false;
        }
        if (inputFlag)
        {
            SendInput();
        }
    }

    //unsafe void debugCstring()
    //{
    //    string str = Marshal.PtrToStringAnsi((System.IntPtr)CStringTest());
    //        Debug.Log(str);
    //    string mama = "hello there";
    //    System.Text.StringBuilder tester = new System.Text.StringBuilder(mama);
    //    // char* the;
    //    string pStr = "Hello World!";

    //    InputMessage* pChars = (InputMessage*)Marshal.StringToHGlobalAnsi(pStr).ToPointer();
    //    char* pSomething = (char*)pChars;
    //    char* charArray;
    //    InputMessage* pTerrible;
    //    InputMessage* pTulips = (InputMessage*)Marshal.StringToHGlobalAnsi(new string(pSomething)).ToPointer();

    //    fixed (char* p = "hey hello yada")
    //    {
    //        //string derp = Marshal.PtrToStringAnsi((System.IntPtr)(p));
    //        string derp = new string(p);
    //        Debug.Log(new string(p));
            
    //    }

        
    //}

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
        SetGUID();
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
        switch ((byte)packet[0])
        {
            case (byte)Messages.MESSAGE:
  
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
            case (byte)Messages.INPUT:
                Debug.Log("I shouldn't be *receiving* input...");
                break;
            case (byte)Messages.REQUEST_ACCEPTED:
                Debug.Log("Connection request accepted!");
                
                SendGUID();
                break;
            default:
                Debug.Log("Message with identifier: " + (byte) packet[0]);
                break;
        }
    }

    public unsafe void SendChat()
    {
        if (initFlag)
        {
            string s = chatMess.text;
            sendChatMessage(s);
            Debug.Log("Me: " + chatMess.text);
        }
        ////BetaString bs = new BetaString((int)Messages.MESSAGE);
        //bs.id = (int)Messages.MESSAGE;
        //IntPtr pointer = Marshal.AllocHGlobal(Marshal.SizeOf(*bs.pseudoString));
        //Marshal.StructureToPtr(*bs.pseudoString, pointer, false);


        ////bs->pseudoString = Marshal.StringToHGlobalAnsi( chatMess.text);



        //IntPtr kappa = Marshal.AllocHGlobal(Marshal.SizeOf(bs));

        //Marshal.StructureToPtr(bs, kappa, false);

        //sendNetworkPacket(kappa);
    }

    public unsafe void SendInput()
    {
        InputMessage im = new InputMessage();
        im.id = (byte)Messages.INPUT;
        im.horizontal = Input.GetAxis("Horizontal");
        im.vertical = Input.GetAxis("Vertical");
        im.guid = guid;
        int size = Marshal.SizeOf(im);
        IntPtr myPtr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(im, myPtr, false);
        sendNetworkPacket(myPtr, size);
    }

    public unsafe void SendGUID()
    {
        InputMessage im = new InputMessage();
        im.id = (byte)Messages.GUID;
        im.horizontal = 0.0f;
        im.vertical = 0.0f;
        im.guid = guid;
        int size = Marshal.SizeOf(im);
        IntPtr myPtr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(im, myPtr, false);
        int ptrSize = Marshal.SizeOf(myPtr);
        sendNetworkPacket(myPtr, size);
        inputFlag = true;
    }

    public unsafe void SetGUID()
    {
        guid = Marshal.PtrToStringAnsi(getGUID());
        initFlag = true;
    }
}

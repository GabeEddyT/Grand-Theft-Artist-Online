using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;




public class NetworkInput : MonoBehaviour {

    [DllImport("MyFrameworkPlugin")]
    static extern int Startup();
    [DllImport("MyFrameworkPlugin")]
    static extern int Shutdown();

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
        GUID,
        EVENT,
        GAMESTATE,
    }

    public Transform player;
    string guid;
    public Text ip;
    public Text serverPort;
    public Text chatName;
    //public Text chatMess;
    public InputField chatMess;
    bool initFlag = false;
    bool inputFlag = false;
    public static bool networkedMode = false;


    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 81)]
    public unsafe struct GameState
    {
        public byte id;
        public fixed float playerPosX[4];
        public fixed float playerPosY[4];
        public fixed float playerRotation [4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Vector2[] playerVelocity;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct InputMessage
    {
        public byte id;
        public float vertical;
        public float horizontal;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BetaString
    {
        public byte id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte [] pseudoString;
    }

    void Start () {
        networkedMode = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayOrStop;
#endif
        //sendAndReceiveStruct();
        //Shutdown();
        Startup();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (initFlag)
        {
            getPacket();

            //    //initFlag = false;
        }
    }


#if UNITY_EDITOR
    private void OnPlayOrStop(UnityEditor.PlayModeStateChange obj)
    {
        switch (obj)
        {
            case UnityEditor.PlayModeStateChange.EnteredEditMode:
                break;
            case UnityEditor.PlayModeStateChange.ExitingEditMode:
                break;
            case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                break;
            case UnityEditor.PlayModeStateChange.ExitingPlayMode:                
                    Disconnect();
                break;
            default:
                break;
        }
    }
#else
    void OnApplicationQuit()
    {
        Disconnect();
    }
#endif    

    private void OnDestroy()
    {
        networkedMode = false;
    }

    private void FixedUpdate()
    {
        //if (inputFlag /*&& Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Gas") != 0 ||Input.GetAxis("Vertical") != 0*/)
        //{
        //    SendInput();
        //}
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
        if (initFlag)
        {
            Disconnect();
        }
        // = ip.text;
        int port = Int32.Parse(serverPort.text == "" ? "60" : serverPort.text);
        //Debug.Log();
        initNetworking(port, ip.text == "" ? "127.0.0.1" : ip.text);
        SetGUID();
    }



    void Disconnect()
    {
        if (initFlag)
        {
            IntPtr pac = Marshal.AllocHGlobal(1);
            Marshal.WriteByte(pac, 31);
            sendNetworkPacket(pac, 1);
        }    
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
  
                BetaString bs = (BetaString)Marshal.PtrToStructure((IntPtr) packet, typeof(BetaString));
                //BetaString bs = *(BetaString*) packet;
                //char* cs = bs.pseudoString;
                //string likeActualString = new string(cs);

                //Debug.Log((*bs.pseudoString).ToString());
                //System.Text.StringBuilder theSood = (System.Text.StringBuilder)Marshal.PtrToStructure((IntPtr)packet, typeof(System.Text.StringBuilder));
                //string makeReal = Marshal.PtrToStringAuto((char*)theSood.ToPointer());

                //Debug.Log(new string(theSood));
                //Debug.Log(theSood);
                //Console.WriteLine(*theSood);
                string outputPls = Encoding.ASCII.GetString(bs.pseudoString);
                //string outputPls = ; /*new string(bs.pseudoString) +*/ //new string(poi->pseudoString);
                Debug.Log(outputPls);
                break;
            case (byte)Messages.INPUT:
                Debug.Log("I shouldn't be *receiving* input...");
                break;
            case (byte)Messages.REQUEST_ACCEPTED:
                Debug.Log("Connection request accepted!");
                FindObjectOfType<Canvas>().enabled = false;

                StartCoroutine(SendInput());
                break;
            case (byte)Messages.GAMESTATE:
                IntPtr newData = (IntPtr)packet;
                ReceiveGameState(newData);
                break;
            default:
                Debug.Log("Message with identifier: " + (byte) packet[0]);
                break;
        }
    }

    public unsafe void ReceiveGameState(IntPtr packet)
    {
        Debug.Log("Received new Game State");
        GameState newData = (GameState)Marshal.PtrToStructure(packet, typeof(GameState));

        Vector2 posisiton = player.position;
        posisiton.x = newData.playerPosX[0];
        posisiton.y = newData.playerPosY[0];
        player.position = posisiton;

        Quaternion rot = player.rotation;
        rot.z = newData.playerRotation[0];
        player.rotation = rot;
        Debug.Log(newData.playerPosX[0] + "  " + newData.playerPosY[0] + "  " + newData.playerRotation[0] + " " + newData.playerVelocity[0].ToString() );
    }

    public unsafe void SendChat()
    {
        if (initFlag)
        {
            BetaString bs;
            bs.id = (byte)Messages.MESSAGE;
            bs.pseudoString = ToByte(String.IsNullOrEmpty(chatName.text) ? guid : chatName.text + " says: " + chatMess.text);
            SendPkt(bs);
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

    IEnumerator SendInput()
    {
        while(true)
        {
            InputMessage im = new InputMessage();
            im.id = (byte)Messages.INPUT;
            im.horizontal = Input.GetAxis("Horizontal");
            im.vertical = Input.GetAxis("Gas");
            int size = Marshal.SizeOf(im);
            IntPtr myPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(im, myPtr, false);
            sendNetworkPacket(myPtr, size);
            Marshal.FreeHGlobal(myPtr);
            yield return new WaitForSeconds(.034f);
        }
        
    }

    /* *
     * Generic method to abstract the conversion to IntPtr when sending packet. 
     * */
    void SendPkt<T>(T pkt)
    {
        int size = Marshal.SizeOf(pkt);
        IntPtr myPtr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(pkt, myPtr, false);
        sendNetworkPacket(myPtr, size);
        Marshal.FreeHGlobal(myPtr);
    }

    /* *
    * Abstract the conversion of strings to byte arrays. 
    * */
    byte[] ToByte(string s)
    {
        return Encoding.ASCII.GetBytes(s);
    }

    public unsafe void SetGUID()
    {
        guid = Marshal.PtrToStringAnsi(getGUID());
        initFlag = true;
    }
}

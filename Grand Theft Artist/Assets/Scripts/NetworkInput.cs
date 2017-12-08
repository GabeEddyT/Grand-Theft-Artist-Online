using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;



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
        ITEMSPAWN,
        SHELF,
        WINNER,
    }
    public Canvas netMenu;
    public Player []players;
    ulong guid;
    public Text ip;
    public Text serverPort;
    public Text chatName;
    //public Text chatMess;
    public InputField chatMess;
    bool initFlag = false;
    bool inputFlag = false;
    public static bool networkedMode = false;
    private bool cameraSet = false;
    private Dictionary<string, Transform> shelves;
    public Transform cameraTransform;
    public Material playerMat;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct GameState
    {
        public byte id;
        public fixed float playerPosX[4];
        public fixed float playerPosY[4];
        public fixed float playerRotation [4];
        public fixed ulong playerGuid[4];
        public fixed float playerCash[4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Vector2[] playerVelocity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Vector2[] playerAxes;
        public double timestamp;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ShelfState
    {
        public byte id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        public BetaString[] name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        public Vector2[] shelfPos;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        public Quaternion[] shelfRot;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ItemShip
    {
        public byte id;
        public byte numItems;
        public Vector2 location;
        public fixed byte itemType[13];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public Vector2[] trajectory;
        public fixed float rotVelocity[13];
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
        public override string ToString()
        {
            char[] ignoreThese = { '\0', (char)0 };
            return Encoding.ASCII.GetString(pseudoString).Split(ignoreThese)[0];
        }
        public BetaString(string name)
        {
            id = 0;
            pseudoString = Encoding.ASCII.GetBytes(name);
        }
        public override bool Equals(object obj)
        {
            char[] ignoreThese = { '\0', (char)0 };
            return Encoding.ASCII.GetString(pseudoString).Split(ignoreThese)[0] == Encoding.ASCII.GetString(((BetaString)obj).pseudoString).Split(ignoreThese)[0];            
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WinMessage
    {
        public byte id;
        public UInt64 winner;
    }

    void Start () {
        shelves = new Dictionary<string, Transform>();
        ItemSpawn[] myShelves = FindObjectsOfType<ItemSpawn>();
        foreach (ItemSpawn shelf in myShelves)
        {
            shelves.Add(shelf.name, shelf.transform);
        }
        networkedMode = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayOrStop;
#endif
        //sendAndReceiveStruct();
        //Shutdown();
        Startup();
        players = FindObjectsOfType<Player>();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (initFlag)
        {
            getPacket();
            Camera.main.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
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
                netMenu.enabled = false;

                StartCoroutine(SendInput());
                break;
            case (byte)Messages.GAMESTATE:
                {
                    IntPtr newData = (IntPtr)packet;
                    ReceiveGameState(newData);
                }
                break;
            case (byte)Messages.ITEMSPAWN:
                {
                    IntPtr newData = (IntPtr)packet;
                    ReceiveItems(newData);
                }
                break;
            case (byte)Messages.SHELF:
                {
                    IntPtr newData = (IntPtr)packet;
                    ReceiveShelfState(newData);
                }
                break;
            case (byte)Messages.WINNER:
                IntPtr winnerPkt = (IntPtr)packet;
                ProcessWin(winnerPkt);
                break;
            default:
                Debug.Log("Message with identifier: " + (byte) packet[0]);
                break;
        }
    }

    private void ProcessWin(IntPtr winnerPkt)
    {
        WinMessage wm = (WinMessage)Marshal.PtrToStructure(winnerPkt, typeof(WinMessage));
        if (wm.winner == guid)
        {
            SceneManager.LoadScene("Success");
        }
        else
        {
            SceneManager.LoadScene("Lose");
        }
    }

    public unsafe void ReceiveGameState(IntPtr packet)
    {        
        Debug.Log("Received new Game State");
        GameState newData = (GameState)Marshal.PtrToStructure(packet, typeof(GameState));
        TimeSpan t = DateTime.UtcNow - DateTime.MinValue;
        double dt = t.TotalSeconds - newData.timestamp;
        for (int i = 0; i < 4; i++)
        {
            
            Vector2 position = players[i].transform.position;
            position.x = newData.playerPosX[i];
            position.y = newData.playerPosY[i];

            players[i].transform.position = position + (newData.playerVelocity[i] * (float)dt); // "lerp"
            players[i].GetComponent<Rigidbody2D>().velocity = newData.playerVelocity[i];

            

            Quaternion rot = players[i].transform.rotation;
            rot.z = newData.playerRotation[i];
            players[i].transform.rotation = rot;
            if (newData.playerGuid[i] == guid)
            {
                if (!cameraSet)
                {
                    cameraTransform.parent = players[i].transform;
                    cameraTransform.localPosition = new Vector3(0,0, -10);
                    cameraSet = true;
                    players[i].money.color = new Color(163f/255f,221f/255f,0);
                    players[i].dollar.material = playerMat;
                }
                players[i].playerType = 1;
            }

            players[i].speed = newData.playerAxes[i].y;
            if (players[i].speed == 0)
            {
                players[i].speed = newData.playerAxes[i].x;
            }
            players[i].cash = newData.playerCash[i];

            players[i].money.text = "$" + newData.playerCash[i].ToString("F2");
            players[i].dollar.GetComponent<Animator>().SetFloat("money", newData.playerCash[i]);
        }
        
        Debug.Log(newData.playerPosX[0] + "  " + newData.playerPosY[0] + "  " + newData.playerRotation[0] + " " + newData.playerVelocity[0].ToString() + " " + dt);
    }

    /**
     * Receive shelf info from the server to sync up this client.
     * */
    public unsafe void ReceiveShelfState(IntPtr packet)
    {        
        ShelfState shelfData = (ShelfState)Marshal.PtrToStructure(packet, typeof(ShelfState));
        
        for (int i = 0; i < 14; i++)
        {
            Transform shelf = shelves[shelfData.name[i].ToString()];
            
            shelf.position = shelfData.shelfPos[i];
            shelf.rotation = shelfData.shelfRot[i];
        }
    }

    public unsafe void SendChat()
    {
        if (initFlag)
        {
            BetaString bs;
            bs.id = (byte)Messages.MESSAGE;
            bs.pseudoString = ToByte(String.IsNullOrEmpty(chatName.text) ? guid + "" : chatName.text + " says: " + chatMess.text);
            chatMess.text = "";
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

            if(!chatMess.isFocused)
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

    /* *
     *  Abstract getting strings from byte arrays... I can't believe I 
     *  actually need this goddamn function.
     * */
    string FromByte(byte[] b)
    {
        return Encoding.ASCII.GetString(b);
    }

    public unsafe void SetGUID()
    {
        guid = UInt64.Parse(Marshal.PtrToStringAnsi(getGUID()));
        initFlag = true;
    }

    /**
     * Receive the items from the server and call a random ItemSpawn-er to spawn them client-side.
     * */
    public unsafe void ReceiveItems(IntPtr pkt)
    {
        ItemShip shipment = (ItemShip)Marshal.PtrToStructure(pkt, typeof(ItemShip));
        ItemSpawn spawner = FindObjectOfType<ItemSpawn>();
        byte[] itemType = new byte[shipment.numItems];
        float[] rotVelocity = new float[shipment.numItems];
        Marshal.Copy((IntPtr)shipment.itemType, itemType, 0, shipment.numItems); // thank stack: https://stackoverflow.com/a/17569560
        Marshal.Copy((IntPtr)shipment.rotVelocity, rotVelocity, 0, shipment.numItems); 
        StartCoroutine( spawner.Spawn(shipment.numItems,shipment.location, itemType, shipment.trajectory, rotVelocity));
    }
}

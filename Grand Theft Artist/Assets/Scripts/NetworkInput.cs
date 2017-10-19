using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class NetworkInput : MonoBehaviour {

    [DllImport("MyFrameworkPlugin")]
    static extern int Startup();

    [DllImport("MyFrameworkPlugin")]
    static extern unsafe char* CStringTest();

    [DllImport("MyFrameworkPlugin")]
    static extern unsafe char* InputTest(char* stuff);

    // Use this for initialization



    void Start () {
        debugCstring();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    unsafe void debugCstring()
    {
        string str = Marshal.PtrToStringAnsi((System.IntPtr)CStringTest());
            Debug.Log(str);
        string mama = "hey hello there";
        fixed (char* p = mama)
        {
            Marshal.PtrToStringAnsi((System.IntPtr)InputTest(p));
            Debug.Log(mama);

        }
    }

}

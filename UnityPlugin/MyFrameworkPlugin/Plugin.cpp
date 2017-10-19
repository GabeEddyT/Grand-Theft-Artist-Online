#include "Plugin.h"

#include "../MyFramework/MyFrameworkState.h"
#include "RakNet\RakPeerInterface.h"
#include <string.h>
#include "RakNet/MessageIdentifiers.h"
#include "RakNet/BitStream.h"
#include "RakNet/RakNetTypes.h"  // MessageID
#include <fstream>
#include <sstream>
#include <string>
#include <io.h>
#include <iostream>
#include <fcntl.h>
#include <tchar.h>


MYPLUGIN_SYMBOL MyFrameworkState *theState = 0;

enum Messages
{
	INPUT_ID = ID_USER_PACKET_ENUM + 1,
};

int Startup()
{
	if (theState == 0)
	{
		theState = new MyFrameworkState;
		return 1;
	}
	return 0;
}


int Shutdown()
{
	if (theState != 0)
	{
		delete theState;
		theState = 0;
		return 1;
	}
	return 0;
}

int Foo(int bar)
{
	return (bar * bar);
}

char* CStringTest()
{
	return "hi";
}

char* InputTest(char* stuff)
{
	return stuff;
}

char* getTest()
{
	InputMessage *tom;
	tom->horizontal = 74.0f;
	tom->vertical = 56.0f;
	tom->id = INPUT_ID;
	return (char*)tom;
}

char* returnToSender(char* delivery)
{
	InputMessage *pkg = (InputMessage*)delivery;
	pkg->horizontal /= 2;
	pkg->vertical /= 2;
	return (char*)pkg;
	
}

int initNetworking(int serverPort, char * ip)
{
	theState->init(serverPort, ip);
	return 0;
}

char * getNetworkPacket()
{
	return theState->getPacket();	
}

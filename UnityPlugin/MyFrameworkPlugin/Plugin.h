#ifndef _PLUGIN_H
#define _PLUGIN_H

#include "Lib.h"

#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus

	//	startup and shutdown
	MYPLUGIN_SYMBOL int Startup();
	MYPLUGIN_SYMBOL int Shutdown();

	//	c-style function to be called
	MYPLUGIN_SYMBOL int Foo(int bar);
	MYPLUGIN_SYMBOL char* CStringTest();
	MYPLUGIN_SYMBOL char* InputTest(char* stuff);
	MYPLUGIN_SYMBOL char* getTest();
	MYPLUGIN_SYMBOL char* returnToSender(char* delivery);
	MYPLUGIN_SYMBOL int initNetworking(int serverPort, char* ip);
	MYPLUGIN_SYMBOL char* getNetworkPacket();
	MYPLUGIN_SYMBOL void sendNetworkPacket(char* packet);


#ifdef __cplusplus
}
#endif // __cplusplus


#endif // !_PLUGIN_H

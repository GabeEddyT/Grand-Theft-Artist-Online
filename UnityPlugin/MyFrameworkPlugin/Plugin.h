#ifndef _PLUGIN_H
#define _PLUGIN_H

#include "Lib.h"

#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus

	MYPLUGIN_SYMBOL struct InputMessage
	{
		int id = 0;
		float vertical = 1.0f;
		float horizontal = 3.0f;
	};
	//	startup and shutdown
	MYPLUGIN_SYMBOL int Startup();
	MYPLUGIN_SYMBOL int Shutdown();

	//	c-style function to be called
	MYPLUGIN_SYMBOL int Foo(int bar);
	MYPLUGIN_SYMBOL char* CStringTest();
	MYPLUGIN_SYMBOL char* InputTest(char* stuff);
	MYPLUGIN_SYMBOL char* getTest();
	MYPLUGIN_SYMBOL char* returnToSender(char* delivery);


#ifdef __cplusplus
}
#endif // __cplusplus


#endif // !_PLUGIN_H

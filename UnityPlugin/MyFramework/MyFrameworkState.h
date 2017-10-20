#ifndef _MYFRAMEWORKSTATE_H
#define _MYFRAMEWORKSTATE_H

#include "RakNet/RakPeerInterface.h"
#include "RakNet/MessageIdentifiers.h"
#include "RakNet/RakNetTypes.h"  

struct InputMessage
{
	int id = 0;
	float vertical = 0.0f;
	float horizontal = 0.0f;
};



class MyFrameworkState
{

	RakNet::RakPeerInterface *peer;
public:
	void init(int serverPort, char* ip);
	char * getPacket();
	void sendPacket(char* packet);
	int Networking();
	int StateFoo(int bar);
};


#endif // !_MYFRAMEWORKSTATE_H


#ifndef _MYFRAMEWORKSTATE_H
#define _MYFRAMEWORKSTATE_H

#include "RakNet/RakPeerInterface.h"
#include "RakNet/MessageIdentifiers.h"
#include "RakNet/BitStream.h"
#include "RakNet/RakNetTypes.h"  

struct InputMessage
{
	int id = 0;
	float vertical = 1.0f;
	float horizontal = 3.0f;
};



class MyFrameworkState
{

	RakNet::RakPeerInterface *peer;
public:
	void init(int serverPort, char* ip);
	char * getPacket();
	int Networking();
	int StateFoo(int bar);
};


#endif // !_MYFRAMEWORKSTATE_H


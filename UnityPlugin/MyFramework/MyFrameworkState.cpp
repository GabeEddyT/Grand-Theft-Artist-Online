#include "MyFrameworkState.h"
#include <string.h> // MessageID
#include <fstream>
#include <sstream>
#include <string>
#include <io.h>
#include <iostream>
#include <fcntl.h>
#include <tchar.h>


int MyFrameworkState::StateFoo(int bar)
{
	return (bar * bar);
}

void MyFrameworkState::init(int serverPort, char* ip = "127.0.0.1")
{

	peer = RakNet::RakPeerInterface::GetInstance();
	RakNet::SocketDescriptor sd;
	peer->Startup(1, &sd, 1);
	//serverPort = atoi(str);

	// TODO - Add code body here


	//fgets(str, 512, stdin);



	peer->Connect(ip, serverPort, 0, 0);

	peer->SetTimeoutTime(300000000, RakNet::UNASSIGNED_SYSTEM_ADDRESS);

}
//give more time to avoid the timeout while debugging

char* MyFrameworkState::getPacket()
{

	RakNet::Packet *packet;
	
	packet = peer->Receive();
	if (packet)
	{
		return (char*)packet->data;
	}
	else
	{
		

		return NULL;
	}
	switch (packet->data[0])
	{
		break;
	case ID_REMOTE_DISCONNECTION_NOTIFICATION:
		wprintf(L"Another client has disconnected.\n");
		break;
	case ID_REMOTE_CONNECTION_LOST:
		wprintf(L"Another client has lost the connection.\n");
		break;
	case ID_REMOTE_NEW_INCOMING_CONNECTION:
		wprintf(L"Another client has connected.\n");
		break;
	case ID_NO_FREE_INCOMING_CONNECTIONS:
		wprintf(L"The server is full.\n");
		break;
	case ID_CONNECTION_REQUEST_ACCEPTED:
	{
		system("cls");
	}
	break;
	case ID_NEW_INCOMING_CONNECTION:
	{
		system("cls");
	}
	break;
	case ID_DISCONNECTION_NOTIFICATION:
	{
		wprintf(L"We have been disconnected.\n");

	}
	break;
	case ID_CONNECTION_LOST:
		wprintf(L"Connection lost.\n");
		break;
		

	default:
		wprintf(L"Message with identifier %i has arrived.\n", packet->data[0]);
		break;
	}
	peer->DeallocatePacket(packet);

}

void MyFrameworkState::sendPacket(char * packet)
{
	peer->Send(packet, sizeof(packet), HIGH_PRIORITY, RELIABLE_ORDERED, 0, RakNet::UNASSIGNED_SYSTEM_ADDRESS, true);
}

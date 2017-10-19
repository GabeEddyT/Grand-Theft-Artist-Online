#include "MyFrameworkState.h"
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


int MyFrameworkState::StateFoo(int bar)
{
	return (bar * bar);
}

int MyFrameworkState::Networking()
{
	//SetConsoleTitle(_T("Tic Tac Jac in the Sac!"));
	unsigned short serverPort;

	char str[512];
	RakNet::RakPeerInterface *peer = RakNet::RakPeerInterface::GetInstance();

	bool isHost = false;
	bool isLocal = true;
	bool isTurn;
	bool goesFirst;
	bool validMove = true;
	int winState = -1;
	std::string s;
	_setmode(_fileno(stdout), _O_U16TEXT);


	RakNet::Packet *packet;
	while (1)
	{
		system("CLS");/*
		wprintf(L"Welcome to Tic Tac Jack in the Sack\n");*///gabe's name, not mine (jack)
		//select role
//Game loops made by jack
	//isLocal = false;/
	/*wprintf(L"(G)uest or (H)ost?\n");
	fgets(str, 512, stdin);*/
		RakNet::SocketDescriptor sd;
		peer->Startup(1, &sd, 1);

		//pick port
		wprintf(L"Set Port:\n");
		fgets(str, 512, stdin);
		serverPort = atoi(str);








		// TODO - Add code body here



		wprintf(L"Enter server IP\n");
		fgets(str, 512, stdin);
		if (str[0] == 10) {
			strcpy(str, "127.0.0.1");
		}

		system("cls");
		peer->Connect(str, serverPort, 0, 0);

	}
	//give more time to avoid the timeout while debugging
	peer->SetTimeoutTime(300000000, RakNet::UNASSIGNED_SYSTEM_ADDRESS);
	std::string messy = "";
	bool loopGame = true;
	while (loopGame)
	{

		winState = -1;
		while (winState == -1)
		{
			for (packet = peer->Receive(); packet; peer->DeallocatePacket(packet), packet = peer->Receive())
			{
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
					if (isHost) {
						wprintf(L"A client has disconnected.\n");
					}
					else {
						wprintf(L"We have been disconnected.\n");
					}
				}
				break;
				case ID_CONNECTION_LOST:
					if (isHost) {
						wprintf(L"A client lost the connection.\n");
					}
					else {
						wprintf(L"Connection lost.\n");
					}
					break;



				default:
					wprintf(L"Message with identifier %i has arrived.\n", packet->data[0]);
					break;
				}
			}
		}
	}
	RakNet::RakPeerInterface::DestroyInstance(peer);
	return 0;
}

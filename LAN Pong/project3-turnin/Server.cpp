#include "Server.h"

Server::Server(NetManager* net)
{
	netMgr = net;
	netMgr->addNetworkInfo(PROTOCOL_UDP, NULL, 51215);
	netMgr->startServer();
}

Server::~Server(){
	netMgr->stopServer();
}

void Server::startLobby(){
	netMgr->acceptConnections();
}

void Server::update(Ogre::String message){
	const char* cmsg = message.c_str();
	//std::cout << cmsg << "\n"; 
	netMgr->messageClients(PROTOCOL_UDP, cmsg, strlen(cmsg));
}

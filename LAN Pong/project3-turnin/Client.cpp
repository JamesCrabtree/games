#include "Client.h"

Client::Client(NetManager* net, Ogre::String host_ip)
{
	netMgr = net;
	netMgr->addNetworkInfo(PROTOCOL_UDP, host_ip.c_str(), 51215);
	netMgr->startClient();
}

Client::~Client(){
	netMgr->stopClient();
}

void Client::update(Ogre::String message){
	const char* cmsg = message.c_str();
	netMgr->messageServer(PROTOCOL_UDP, cmsg, strlen(cmsg));
}

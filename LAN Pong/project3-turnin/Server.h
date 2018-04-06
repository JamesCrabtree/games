#include <Ogre.h>
#include "NetManager.h"
#include "Simulator.h"

class Server{
protected:
	NetManager* netMgr;
public:
	Server(NetManager* net);
	~Server();
	void startLobby();
	void update(Ogre::String message);
};

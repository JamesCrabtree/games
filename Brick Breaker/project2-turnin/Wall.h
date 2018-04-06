#include <Ogre.h>
#include "Simulator.h"

class Wall : public GameObject{
    public:
	Wall(Ogre::SceneManager* scnMgr, Simulator* sim,
		Ogre::Vector3 normal, Ogre::Vector3 up, Ogre::Vector3 pos, 
		Ogre::String name, int size1, int size2);
	void update(float);
};

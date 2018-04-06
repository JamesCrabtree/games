#ifndef __Paddle_h_
#define __Paddle_h_

#include <Ogre.h>
#include "Simulator.h"

class Paddle : public GameObject{
public:
	Paddle(Ogre::String name, 
		Ogre::SceneManager* scnMgr, 
		Simulator* sim, 
		Ogre::Vector3 pos, 
		Ogre::Real rotation);
	Ogre::Real yaw;
	void update(float);
};

#endif // #ifndef __Paddle_h_
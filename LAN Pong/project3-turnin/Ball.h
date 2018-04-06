#ifndef __ball_h_
#define __ball_h_

#include <Ogre.h>
#include "Simulator.h"

class Ball : public GameObject {
protected:
	Ogre::Real bRadius;
public:
	Ball(Ogre::SceneManager* mNode, Simulator* sim, bool server);
	void update(float elapsedTime);
	bool firstHit;
	int score;
};

#endif // #ifndef __ball_h_

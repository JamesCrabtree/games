#include <Ogre.h>
#include "BulletContactCallback.h"
#include "Simulator.h"
class Character{
	public:
		Character(Ogre::SceneManager* scnMgr, Simulator* sim);
		void punchRight();
		void punchLeft();
		void de_punchRight();
		void de_punchLeft();
    protected:
    	Ogre::SceneNode* leftNode;
    	Ogre::SceneNode* rightNode;
    	Paddle* lPaddle;
        Paddle* rPaddle;
    	int lYaw;
    	int rYaw;
};
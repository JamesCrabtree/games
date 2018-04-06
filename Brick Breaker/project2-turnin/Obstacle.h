#include <Ogre.h>
#include "Simulator.h"
#include "SDL/SDL.h"
#include "SDL/SDL_mixer.h"

class Obstacle : public GameObject{
	protected:
		bool deleteIt;
		int hitsToBreak;
		Mix_Chunk* score;
	public:
	 	Obstacle(Ogre::String name,
			Ogre::SceneManager* scnMgr, Simulator* sim,
			Ogre::Vector3 pos, Ogre::Vector3 size, int hits);
	 	void update(float);
};
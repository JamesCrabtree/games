#include "Obstacle.h"

Obstacle::Obstacle(Ogre::String name,
		Ogre::SceneManager* scnMgr, Simulator* sim,
		Ogre::Vector3 pos, Ogre::Vector3 size, int hits):
		GameObject(name, scnMgr, sim), score(0){
	
	geom = scnMgr->createEntity(name, Ogre::SceneManager::PT_CUBE);
	switch(hits){
		case 1:
			geom->setMaterialName("Template/Red");
			break;
		case 2:
			geom->setMaterialName("Template/Blue");
			break;
		default:
			break;
	}
	hitsToBreak = hits;
	deleteIt = false;
	geom->setCastShadows(false);
	rootNode->attachObject(geom);
	rootNode->setPosition(pos.x,pos.y,pos.z);
	rootNode->setScale(size.x, size.y, size.z);
	restitution = 0.6f;
	friction = 0.1f;
	mass = 0.0f;
	shape = new btBoxShape(btVector3(size.x*55, size.y*55, size.z*55));

	//music = Mix_LoadMUS( "party-crowd-daniel_simon.wav" );
    bounce = Mix_LoadWAV( "paddlehit.wav" );
    score = Mix_LoadWAV( "cheer.wav" );
}

void Obstacle::update(float elapsedTime){
	lastTime += elapsedTime;
	simulator->getWorld()->contactTest(body, *cCallBack);
	if(deleteIt){
		lastTime += elapsedTime;
		if(lastTime > 0.05){
			sceneMgr->destroyEntity(geom);
			simulator->removeObject(this);
			if(sound)
				Mix_PlayChannel( -1, score, 0 );
		}
	}
	else if(context->hit && (lastTime > 0.1)) {
		if(--hitsToBreak == 1)
			geom->setMaterialName("Template/Red");
		else
			deleteIt = true;
		if(sound)
			Mix_PlayChannel( -1, bounce, 0 );
		lastTime = 0.0;
	}
	context->hit = false;
}




#include "Paddle.h"

Paddle::Paddle(Ogre::String name, Ogre::SceneManager* scnMgr, 
		Simulator* sim, 
		Ogre::Vector3 pos, 
		Ogre::Real rotation) : GameObject(name,scnMgr,sim)
{
	geom = scnMgr->createEntity(name, Ogre::SceneManager::PT_CUBE);
	geom->setMaterialName("blank");
	geom->setCastShadows(true);
	rootNode->attachObject(geom);
	rootNode->setPosition(pos.x, pos.y, pos.z);
	//rootNode->yaw(Ogre::Degree(rotation));
	rootNode->setScale(2, .75, .5);

	mass = 0.0f;
	restitution = 4.0f;
	friction = 0.0f;
	yaw = 0;
	lastTime = 0.0;

	shape = new btBoxShape(btVector3(2*55, .75*55, .5*55));

    bounce = Mix_LoadWAV( "paddlehit.wav" );
}

void Paddle::update(float elapsedTime){
	lastTime += elapsedTime;
	updateTransform();
	simulator->getWorld()->contactTest(body, *cCallBack);
	if(context->hit && (lastTime > 0.1)){
		if(sound)
			Mix_PlayChannel( -1, bounce, 0 );
		lastTime = 0.0;
	}
	context->hit = false;
}
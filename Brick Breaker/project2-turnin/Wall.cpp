#include "Wall.h"

using namespace Ogre;

Wall::Wall(Ogre::SceneManager* scnMgr, Simulator* sim,
		Ogre::Vector3 normal, Ogre::Vector3 up, Ogre::Vector3 pos, 
		Ogre::String name, int size1, int size2) : GameObject(name, scnMgr, sim) {
	Plane p(normal, 0);
	MeshManager::getSingleton().createPlane(name,
        Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME,
        p, 
        size1, size2, 1, 1, 
        true, 
        1, 5, 5, 
        up);

	geom = scnMgr->createEntity(name);
    geom->setMaterialName("blank");
    rootNode->attachObject(geom);
    rootNode->setPosition(pos.x, pos.y, pos.z);

	mass = 0.0f;
	restitution = 1.0f;
	friction = 0.1f;
	
	if(normal == Ogre::Vector3::UNIT_Y || normal == Ogre::Vector3::NEGATIVE_UNIT_Y)
        shape = new btBoxShape(btVector3(size1*55, 5.0, size2*55));
    else if(normal == Ogre::Vector3::UNIT_Z || normal == Ogre::Vector3::NEGATIVE_UNIT_Z)
        shape = new btBoxShape(btVector3(size1*55, size2*55, 5.0));
    else
        shape = new btBoxShape(btVector3(5.0, size1*55, size2*55));

    bounce = Mix_LoadWAV( "paddlehit.wav" );

}
void Wall::update(float elapsedTime){
    lastTime += elapsedTime;
    simulator->getWorld()->contactTest(body, *cCallBack);
    if(context->hit && (lastTime > 0.5)){
        if(name != "floor" && sound)
            Mix_PlayChannel( -1, bounce, 0 );
        lastTime = 0.0;
    }
    context->hit = false;
}

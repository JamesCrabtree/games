#include "Simulator.h"
#include "BulletContactCallback.h"

Simulator::Simulator() {
 collisionConfiguration = new btDefaultCollisionConfiguration();
 dispatcher = new btCollisionDispatcher(collisionConfiguration);
 overlappingPairCache = new btDbvtBroadphase();
 solver = new btSequentialImpulseConstraintSolver();
 dynamicsWorld = new btDiscreteDynamicsWorld(dispatcher,
 overlappingPairCache,
 solver,
 collisionConfiguration);
 dynamicsWorld->setGravity(btVector3(0.0f, -10.0f, 100.0f));
 dynamicsWorld->applyGravity();
 //TODO:Add collision shapes to reuse among rigid bodies
}
Simulator::~Simulator(){
	if (dynamicsWorld) delete dynamicsWorld;
	if (collisionConfiguration) delete collisionConfiguration;
	if (dispatcher) delete dispatcher;
	if (overlappingPairCache) delete overlappingPairCache;
	if (solver) delete solver;
}

void Simulator::addObject (GameObject* o) {
 objList.push_back(o);
 dynamicsWorld->addRigidBody(o->getBody());
}

bool Simulator::removeObject (GameObject* o) {
 dynamicsWorld->removeRigidBody(o->getBody());
 objList.erase(std::remove(objList.begin(), objList.end(), o), objList.end());
}

void Simulator::stepSimulation(const Ogre::Real elapsedTime,
 	int maxSubSteps, const Ogre::Real fixedTimestep){
	dynamicsWorld->stepSimulation(elapsedTime, maxSubSteps, fixedTimestep);
	for(int i = 0; i < objList.size(); ++i){
		objList[i]->update(elapsedTime);
	}
}

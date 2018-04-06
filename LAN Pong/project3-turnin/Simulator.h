#ifndef __Simulator_h_
#define __Simulator_h_

#include <Ogre.h>
#include "GameObject.h"

class Simulator {
protected:
	 btDefaultCollisionConfiguration* collisionConfiguration;
	 btCollisionDispatcher* dispatcher;
	 btBroadphaseInterface* overlappingPairCache;
	 btSequentialImpulseConstraintSolver* solver;
	 btDiscreteDynamicsWorld* dynamicsWorld;
	 //btAlignedObjectArray<btCollisionShape*> collisionShapes;
	 std::vector<GameObject*> objList;
public:
	 Simulator();
	 ~Simulator();
	 void addObject(GameObject* o);
	 bool removeObject(GameObject* o);
	 btDiscreteDynamicsWorld* getWorld() {return dynamicsWorld;}
	 void stepSimulation(const Ogre::Real elapsedTime,
	 int maxSubSteps = 1, const Ogre::Real fixedTimestep = 1.0f/60.0f);
};

#endif // #ifndef __Simulator_h_

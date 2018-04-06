#ifndef __GameObject_h_
#define __GameObject_h_

#include <Ogre.h>
#include <btBulletDynamicsCommon.h>
#include "BulletContactCallback.h"
#include "OgreMotionState.h"
#include "SDL/SDL.h"
#include "SDL/SDL_mixer.h"

class Simulator;

class GameObject {
protected:
	Ogre::SceneManager* sceneMgr;
	Ogre::SceneNode* rootNode;
	Ogre::Entity* geom;
	OgreMotionState* motionState;

	Simulator* simulator;
	btCollisionShape* shape;
	btRigidBody* body;
	btTransform tr;
	btVector3 inertia;

	btScalar mass;
	btScalar restitution;
	btScalar friction;
	bool kinematic;
	bool needsUpdates;

	CollisionContext* context;
	BulletContactCallback* cCallBack;

	Mix_Chunk* bounce;
	bool sound;

public:
	Ogre::String name;
	GameObject(Ogre::String nym, Ogre::SceneManager* mgr, Simulator* sim);
	~GameObject();
	btRigidBody* getBody(){return body;}
	void updateTransform();
	void addToSimulator();
	Ogre::SceneNode* getNode() { return rootNode; }
	Ogre::Entity* getEntity() { return geom; }
	Ogre::Vector3 getPosition() { return rootNode->getPosition(); }
	OgreMotionState* getMotionState() { return motionState; }
	void turnOffSound();
	void turnOnSound();
	virtual void update(Ogre::Real) {}
	float lastTime;
};

#endif // #ifndef __GameObject_h_

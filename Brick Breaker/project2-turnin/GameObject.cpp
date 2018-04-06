#include "GameObject.h"
#include "Simulator.h"

GameObject::GameObject(Ogre::String nym, Ogre::SceneManager* mgr, Simulator* sim) :
		context(0),
		cCallBack(0),
		geom(0),
		shape(0),
		motionState(0),
		bounce(0)
{
	name = nym;
	sceneMgr = mgr;
	simulator = sim;
	rootNode = sceneMgr->getRootSceneNode()->createChildSceneNode(name);
	shape = NULL;
	tr.setIdentity();
	lastTime = 0.0f;

	mass = 0.0f;
	friction = 0.0f;
	restitution = 0.0f;
	inertia.setZero();

	kinematic = false;
	needsUpdates = false;

	sound = true;

	SDL_Init( SDL_INIT_EVERYTHING );
    Mix_OpenAudio( 22050, MIX_DEFAULT_FORMAT, 2, 4096 );
}

GameObject::~GameObject() {
	//simulator->removeObject(this);
	
	if (shape) delete shape;
	if (body) delete body;
	if (context) delete context;
	if (cCallBack) delete cCallBack;
	if (motionState) delete motionState;
}

void GameObject::updateTransform() {
	Ogre::Vector3 pos = rootNode->getPosition();
	tr.setOrigin(btVector3(pos.x, pos.y, pos.z));
	Ogre::Quaternion qt = rootNode->getOrientation();
	tr.setRotation(btQuaternion(qt.x, qt.y, qt.z, qt.w));
	if (motionState) motionState->updateTransform(tr);
}

void GameObject::addToSimulator() {
	//using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
	updateTransform();
	motionState = new OgreMotionState(tr, rootNode);
	//rigidbody is dynamic if and only if mass is non zero, otherwise static
	if (mass != 0.0f) shape->calculateLocalInertia(mass, inertia);

	btRigidBody::btRigidBodyConstructionInfo rbInfo(mass, motionState, shape, inertia);
	rbInfo.m_restitution = this->restitution;
    rbInfo.m_friction = this->friction;

	body = new btRigidBody(rbInfo);
	body->setUserPointer(this);

	context = new CollisionContext();
    cCallBack = new BulletContactCallback(*body, *context);
	simulator->addObject(this);
}

void GameObject::turnOffSound(){
	sound = false;
}

void GameObject::turnOnSound(){
	sound = true;
}

#ifndef __OgreMotionState_h_
#define __OgreMotionState_h_

#include <btBulletDynamicsCommon.h>
#include <Ogre.h>

class OgreMotionState : public btMotionState {
	protected:
		Ogre::SceneNode* mVisibleobj;
		btTransform mPosl;
	public:
		OgreMotionState(const btTransform&, Ogre::SceneNode*);
		virtual ~OgreMotionState();
		void setNode(Ogre::SceneNode*);
		void updateTransform(btTransform&);
		virtual void getWorldTransform(btTransform&) const;
		virtual void setWorldTransform(const btTransform&);
};

#endif // #ifndef __OgreMotionState_h_
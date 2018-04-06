/*
-----------------------------------------------------------------------------
Filename:    TutorialApplication.h
-----------------------------------------------------------------------------

This source file is part of the
   ___                 __    __ _ _    _
  /___\__ _ _ __ ___  / / /\ \ (_) | _(_)
 //  // _` | '__/ _ \ \ \/  \/ / | |/ / |
/ \_// (_| | | |  __/  \  /\  /| |   <| |
\___/ \__, |_|  \___|   \/  \/ |_|_|\_\_|
      |___/
Tutorial Framework (for Ogre 1.9)
http://www.ogre3d.org/wiki/
-----------------------------------------------------------------------------
*/

#ifndef __TutorialApplication_h_
#define __TutorialApplication_h_

#include "BaseApplication.h"
#include "Ball.h"
#include "PlayingField.h"
#include "Paddle.h"
#include "Simulator.h"
#include "NetManager.h"
#include <OgreHardwarePixelBuffer.h>
#include <string>
#include <CEGUI/CEGUI.h>
#include <CEGUI/RendererModules/Ogre/Renderer.h>

//---------------------------------------------------------------------------

class TutorialApplication : public BaseApplication
{
public: 
  TutorialApplication(void);
  virtual ~TutorialApplication(void);

protected:
	Ball* ball;
	Paddle* lPaddle;
  Paddle* rPaddle;
	Simulator* sim;
	PlayingField* bCourt;
  NetManager* net;
  //Server* server;
  CEGUI::OgreRenderer* mRenderer;

	bool zPressed;
  bool cPressed;
  int zoom;
  bool sound;

  virtual void createScene(void);
  virtual void createCamera(void);
	virtual void createViewports(void);
	
	bool keyPressed(const OIS::KeyEvent& ke);
	bool keyReleased(const OIS::KeyEvent& ke);

	virtual bool frameRenderingQueued(const Ogre::FrameEvent& fe);
 
private:
  bool processUnbufferedInput(const Ogre::FrameEvent& fe);
 
};

//---------------------------------------------------------------------------

#endif // #ifndef __TutorialApplication_h_

//---------------------------------------------------------------------------

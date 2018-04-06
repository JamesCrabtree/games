/*
-----------------------------------------------------------------------------
Filename:    TutorialApplication.cpp
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

#include "TutorialApplication.h"
#include <OgreMeshManager.h>
#include "SDL/SDL.h"
#include "SDL/SDL_mixer.h"

using namespace Ogre;

Mix_Chunk* bounce = NULL;
CEGUI::Window *button = NULL;

//---------------------------------------------------------------------------
TutorialApplication::TutorialApplication(void) : 
    ball(0), 
    lPaddle(0),
    sim(0),
    zPressed(false),
    cPressed(false), 
    lives(3),
    sound(true),
    mRenderer(0)
{}
//---------------------------------------------------------------------------
TutorialApplication::~TutorialApplication(void)
{
    if (sim) delete sim;
    if (ball) delete ball;
    if (lPaddle) delete lPaddle;
    CEGUI::OgreRenderer::destroySystem();
}

//---------------------------------------------------------------------------
void TutorialApplication::createScene(void)
{
    //Initialize CEGUI
    mRenderer = &CEGUI::OgreRenderer::bootstrapSystem();
 
    CEGUI::ImageManager::setImagesetDefaultResourceGroup("Imagesets");
    CEGUI::Font::setDefaultResourceGroup("Fonts");
    CEGUI::Scheme::setDefaultResourceGroup("Schemes");
    CEGUI::WidgetLookManager::setDefaultResourceGroup("LookNFeel");
    CEGUI::WindowManager::setDefaultResourceGroup("Layouts");
 
    CEGUI::SchemeManager::getSingleton().createFromFile("TaharezLook.scheme");
 
    CEGUI::WindowManager &wmgr = CEGUI::WindowManager::getSingleton();
    CEGUI::Window *sheet = wmgr.createWindow("DefaultWindow", "CEGUIDemo/Sheet");
 
    button = wmgr.createWindow("TaharezLook/Button", "CEGUIDemo/QuitButton");
    
    mSceneMgr->setAmbientLight(Ogre::ColourValue(0.4, 0.3, 0.5 ));
    mSceneMgr->setShadowTechnique(Ogre::SHADOWTYPE_STENCIL_ADDITIVE);


    sim = new Simulator();

    ball = new Ball(mSceneMgr, sim);
    bCourt = new PlayingField(mSceneMgr, sim);
    lPaddle = new Paddle("left", mSceneMgr, sim, Ogre::Vector3(0,50,1200), 0);

    ball->addToSimulator();
    bCourt->addToSimulator();
    lPaddle->addToSimulator();

    Ogre::Light* light = mSceneMgr->createLight("MainLight");
    light->setPosition(500, 500, 500);
    light->setDiffuseColour(0.1, 0.6, 0.6);
    light->setSpecularColour(0.91, 0.9, 0.9);

    Ogre::Light* pointLight = mSceneMgr->createLight("PointLight");
    pointLight->setType(Ogre::Light::LT_POINT);
    pointLight->setDiffuseColour(0.9, 0.5, 0.8);
    pointLight->setSpecularColour(0.2, 0.0, 0.1);
    pointLight->setPosition(Ogre::Vector3(0, 0, 0));

    mCamera->setPosition(-300, 2200, 950);
    mCamera->lookAt(Ogre::Vector3(-300,50,750));

    //Starting score display   
    std::stringstream ss;
    ss << ball->score;
    std::string display = "Score: " + ss.str();
    button->setText(display);
    button->setSize(CEGUI::USize(CEGUI::UDim(0.15, 0), CEGUI::UDim(0.05, 0)));

    sheet->addChild(button);
    CEGUI::System::getSingleton().getDefaultGUIContext().setRootWindow(sheet);

    SDL_Init( SDL_INIT_EVERYTHING );
    Mix_OpenAudio( 22050, MIX_DEFAULT_FORMAT, 2, 4096 );

    bounce = Mix_LoadWAV( "aww.wav" );

}
//---------------------------------------------------------------------------
void TutorialApplication::createCamera(void)
{
    mCamera = mSceneMgr->createCamera("PlayerCam");
    mCamera->setPosition(Ogre::Vector3(0, 300, 500));
    mCamera->setNearClipDistance(5);
    mCameraMan = new OgreBites::SdkCameraMan(mCamera);
}
 
void TutorialApplication::createViewports(void)
{
    Ogre::Viewport* vp = mWindow->addViewport(mCamera);
    vp->setBackgroundColour(Ogre::ColourValue(0, 0, 0));
    mCamera->setAspectRatio(
    Ogre::Real(vp->getActualWidth()) /
    Ogre::Real(vp->getActualHeight()));
}
bool TutorialApplication::frameRenderingQueued(const Ogre::FrameEvent& fe)
{
  bool ret = BaseApplication::frameRenderingQueued(fe);

  if(zPressed && cPressed){

  }
  else if(cPressed){
        lPaddle->yaw -= 25;
        lPaddle->getNode()->translate(100*fe.timeSinceLastFrame*Ogre::Vector3(6,0,0));
        lPaddle->getBody()->translate(100*fe.timeSinceLastFrame*btVector3(6,0,0));

    }
  else if(zPressed){
        lPaddle->yaw += 25;
        lPaddle->getNode()->translate(100*fe.timeSinceLastFrame*Ogre::Vector3(-6,0,0));
        lPaddle->getBody()->translate(100*fe.timeSinceLastFrame*btVector3(-6,0,0));
    }

  if(!ball->firstHit){
    ball->getBody()->applyCentralForce(btVector3(100, 0, 300));
  }
  if(ball->getNode()->getPosition().z >= 1300){
    if(sound)
        Mix_PlayChannel( -1, bounce, 0 );
    if(--lives <= 0){
        //show end screen
    }
    mSceneMgr->destroyEntity(ball->getEntity());
    mSceneMgr->destroySceneNode(ball->getNode());
    sim->removeObject(ball);
    ball = new Ball(mSceneMgr, sim);
    ball->addToSimulator();
  }

  sim->stepSimulation(fe.timeSinceLastFrame);
  Ogre::Vector3 ballpos = ball->getNode()->getPosition();
  Ogre::Vector3 original = mCamera->getPosition(); 
  Ogre::Vector3 newpos(ballpos.x, original.y, ballpos.z + 200);
  newpos = newpos + mCamera->getDirection() * zoom;
  mCamera->setPosition(newpos);

  //Update score display
  std::stringstream ss;
  ss << ball->score;
  std::string display = "Score: " + ss.str();
  button->setText(display);

  return ret;
}

 
bool TutorialApplication::keyPressed(const OIS::KeyEvent& ke){
    BaseApplication::keyPressed(ke);
    switch(ke.key)
    {
        case OIS::KC_ESCAPE:
            mShutDown = true;
            break;

        case OIS::KC_Z:
            zPressed = true;
            break;

        case OIS::KC_C:
            cPressed = true;
            break;
        case OIS::KC_S:
            zoom = -1;
            break;
        case OIS::KC_X:
            zoom = 1;
            break;    

        case OIS::KC_M:
            if(sound){
                sound = false;
                lPaddle->turnOffSound();
                ball->turnOffSound();
                bCourt->turnOffSound();
            }
            else{
                sound = true;
                lPaddle->turnOnSound();
                ball->turnOnSound();
                bCourt->turnOnSound();
            }
            break;        
 
        default:
            break;
    }
    return true;
}

bool TutorialApplication::keyReleased(const OIS::KeyEvent& ke){
    BaseApplication::keyReleased(ke);
    switch(ke.key)
    {

        case OIS::KC_Z:
            zPressed = false;
            break;

        case OIS::KC_C:
            cPressed = false;
            break;

        case OIS::KC_S:
            zoom = 0;
            break;
        case OIS::KC_X:
            zoom = 0;
            break;
 
        default:
            break;
    }
    return true;
}

#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32
#define WIN32_LEAN_AND_MEAN
#include "windows.h"
#endif

#ifdef __cplusplus
extern "C" {
#endif

#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32
    INT WINAPI WinMain(HINSTANCE hInst, HINSTANCE, LPSTR strCmdLine, INT)
#else
    int main(int argc, char *argv[])
#endif
    {
        // Create application object
        TutorialApplication app;

        try {
            app.go();
        } catch(Ogre::Exception& e)  {
#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32
            MessageBox(NULL, e.getFullDescription().c_str(),
             "An exception has occurred!", 
             MB_OK | MB_ICONERROR | MB_TASKMODAL);
#else
            std::cerr << "An exception has occurred: " <<
                e.getFullDescription().c_str() << std::endl;
#endif
        }

        return 0;
    }

#ifdef __cplusplus
}
#endif

//---------------------------------------------------------------------------

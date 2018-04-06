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
#include "Client.h"
#include "Server.h"

using namespace Ogre;

Mix_Chunk* bounce = NULL;
Mix_Chunk* score = NULL;

CEGUI::Window *sheet;
CEGUI::Window *button1 = NULL;
CEGUI::Window *button2 = NULL;
CEGUI::Window *singlePlayerBut = NULL;
CEGUI::Window *serverBut = NULL;
CEGUI::Window *clientBut = NULL;

int player1;
int player2;
bool isServer;
bool menu;
bool multiplayer;
bool even;

Ogre::String serverName;
Server* server;
Client* client;

//---------------------------------------------------------------------------
TutorialApplication::TutorialApplication() : 
    ball(0), 
    lPaddle(0),
    rPaddle(0),
    sim(0),
    net(0),
    zPressed(false),
    cPressed(false), 
    sound(true),
    mRenderer(0)
{
    player1 = 0;
    player2 = 0;
    isServer = true;
    multiplayer = false;
    menu = true;
    even = true;
}
//---------------------------------------------------------------------------
TutorialApplication::~TutorialApplication(void)
{
    if (sim) delete sim;
    if (ball) delete ball;
    if(server) delete server;
    if(client) delete client;
    if(net) delete net;
    if (lPaddle) delete lPaddle;
    if (rPaddle) delete rPaddle;
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
    sheet = wmgr.createWindow("DefaultWindow", "CEGUIDemo/Sheet");
 
    button1 = wmgr.createWindow("TaharezLook/Button", "CEGUIDemo/QuitButton");
    button2 = wmgr.createWindow("TaharezLook/Button", "CEGUIDemo/QuitButton");
    singlePlayerBut = wmgr.createWindow("TaharezLook/Button", "CEGUIDemo/QuitButton");
    serverBut = wmgr.createWindow("TaharezLook/Button", "CEGUIDemo/QuitButton");
    clientBut = wmgr.createWindow("TaharezLook/Button", "CEGUIDemo/QuitButton");
    
    mSceneMgr->setAmbientLight(Ogre::ColourValue(0.4, 0.3, 0.5 ));
    mSceneMgr->setShadowTechnique(Ogre::SHADOWTYPE_STENCIL_ADDITIVE);

    sim = new Simulator();

    net = new NetManager();
    net->initNetManager();

    ball = new Ball(mSceneMgr, sim, isServer);
    bCourt = new PlayingField(mSceneMgr, sim);
    lPaddle = new Paddle("player1", mSceneMgr, sim, Ogre::Vector3(0,5,-120), 0);
    rPaddle = new Paddle("player2", mSceneMgr, sim, Ogre::Vector3(0,5,120), 0);

    ball->addToSimulator();
    bCourt->addToSimulator();
    lPaddle->addToSimulator();
    rPaddle->addToSimulator();

    Ogre::Light* light = mSceneMgr->createLight("MainLight");
    light->setPosition(50, 50, 50);
    light->setDiffuseColour(0.1, 0.6, 0.6);
    light->setSpecularColour(0.91, 0.9, 0.9);

    Ogre::Light* pointLight = mSceneMgr->createLight("PointLight");
    pointLight->setType(Ogre::Light::LT_POINT);
    pointLight->setDiffuseColour(0.9, 0.5, 0.8);
    pointLight->setSpecularColour(0.2, 0.0, 0.1);
    pointLight->setPosition(Ogre::Vector3(0, 0, 0));
   
    mCamera->setPosition(0, 75, -250);
    mCamera->lookAt(Ogre::Vector3(0,0,150));

    //Starting score display   
    std::stringstream ss1;
    ss1 << player1;
    std::string display = "Player 1: " + ss1.str();
    button1->setText(display);
    button1->setSize(CEGUI::USize(CEGUI::UDim(0.15, 0), CEGUI::UDim(0.05, 0)));

    std::stringstream ss2;
    ss2 << player2;
    display = "Player 2: " + ss2.str();
    button2->setText(display);
    button2->setSize(CEGUI::USize(CEGUI::UDim(0.15, 0), CEGUI::UDim(0.05, 0)));
    button2->setPosition(CEGUI::UVector2(CEGUI::UDim(0.85f, 0),
                                CEGUI::UDim(0.0f, 0)));
    singlePlayerBut->setText("1: Single Player");
    singlePlayerBut->setSize(CEGUI::USize(CEGUI::UDim(0.3, 0), CEGUI::UDim(0.05, 0)));
    singlePlayerBut->setPosition(CEGUI::UVector2(CEGUI::UDim(0.35f, 0),
                                CEGUI::UDim(0.2f, 0)));

    serverBut->setText("2: Start Server");
    serverBut->setSize(CEGUI::USize(CEGUI::UDim(0.3, 0), CEGUI::UDim(0.05, 0)));
    serverBut->setPosition(CEGUI::UVector2(CEGUI::UDim(0.35f, 0),
                                CEGUI::UDim(0.3f, 0)));

    clientBut->setText("3: Start Client");
    clientBut->setSize(CEGUI::USize(CEGUI::UDim(0.3, 0), CEGUI::UDim(0.05, 0)));
    clientBut->setPosition(CEGUI::UVector2(CEGUI::UDim(0.35f, 0),
                                CEGUI::UDim(0.4f, 0)));

    sheet->addChild(button1);
    sheet->addChild(button2);
    sheet->addChild(singlePlayerBut);
    sheet->addChild(serverBut);
    sheet->addChild(clientBut);
    CEGUI::System::getSingleton().getDefaultGUIContext().setRootWindow(sheet);

    SDL_Init( SDL_INIT_EVERYTHING );
    Mix_OpenAudio( 22050, MIX_DEFAULT_FORMAT, 2, 4096 );

    bounce = Mix_LoadWAV( "aww.wav" );
    score = Mix_LoadWAV( "cheer.wav" );

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
  if(!menu)
  {
    if(zPressed && cPressed){}
    else if(cPressed){
        if(isServer)
            lPaddle->getNode()->translate(10*fe.timeSinceLastFrame*Ogre::Vector3(-6,0,0));
        else
            rPaddle->getNode()->translate(10*fe.timeSinceLastFrame*Ogre::Vector3(6,0,0));
        //lPaddle->getBody()->translate(10*fe.timeSinceLastFrame*btVector3(6,0,0));

    }
    else if(zPressed){
        if(isServer)
            lPaddle->getNode()->translate(10*fe.timeSinceLastFrame*Ogre::Vector3(6,0,0));
        else
            rPaddle->getNode()->translate(10*fe.timeSinceLastFrame*Ogre::Vector3(-6,0,0));
        //lPaddle->getBody()->translate(10*fe.timeSinceLastFrame*btVector3(-6,0,0));
    }

    if(!ball->firstHit && isServer){
        if(even)
            ball->getBody()->applyCentralForce(btVector3(rand() % 10 + 80, 0, 150));
        else
            ball->getBody()->applyCentralForce(btVector3(rand() % 10 - 90, 0, 150));
        even = !even;
    }
    if(ball->getNode()->getPosition().z <= -140 && isServer){
        if(sound)
            Mix_PlayChannel( -1, bounce, 0 );
        player2++;
        delete ball;
        ball = new Ball(mSceneMgr, sim, isServer);
        ball->addToSimulator();
        //bCourt->rebuildObstacles(mSceneMgr, sim);
    }
    if(ball->getNode()->getPosition().z >= 140){
        if(sound)
            Mix_PlayChannel( -1, score, 0 ); 
        player1++;
        delete ball;
        ball = new Ball(mSceneMgr, sim, isServer);
        ball->addToSimulator();
        //bCourt->rebuildObstacles(mSceneMgr, sim);
    }
    if(ball->getNode()->getPosition().z <= -130 && !isServer){
        if(sound)
            Mix_PlayChannel( -1, score, 0 );
    }
    if(ball->getNode()->getPosition().z >= 130 && !isServer){
        if(sound)
            Mix_PlayChannel( -1, bounce, 0 );
    }
    if(!multiplayer){
        if(rPaddle->lastTime > 1.2){
            if(ball->getPosition().x > rPaddle->getPosition().x)
                rPaddle->getNode()->translate(10*fe.timeSinceLastFrame*Ogre::Vector3(4,0,0));
            else 
                rPaddle->getNode()->translate(10*fe.timeSinceLastFrame*Ogre::Vector3(-4,0,0));
        }
        else{
            if(rPaddle->getPosition().x > 0)
                rPaddle->getNode()->translate(10*fe.timeSinceLastFrame*Ogre::Vector3(-4,0,0));
            else
                rPaddle->getNode()->translate(10*fe.timeSinceLastFrame*Ogre::Vector3(4,0,0));
        }
    }
    else if(!isServer){
        client->update(Ogre::StringConverter::toString(rPaddle->getPosition().x));
        if(net->pollForActivity(1)){
            Ogre::String message;
            std::istringstream data(Ogre::String(net->udpServerData[0].output));

            std::getline(data, message, ' ');
            lPaddle->getNode()->setPosition(Ogre::Vector3(Ogre::StringConverter::parseReal(message),5,-120)); 

            Ogre::Vector3 ballPos = Ogre::Vector3(0,0,0); 
            std::getline(data, message, ' ');
            //std::cout << message << " ";
            ballPos.x = Ogre::StringConverter::parseReal(message);

            std::getline(data, message, ' ');
            //std::cout << message << " ";
            ballPos.y = Ogre::StringConverter::parseReal(message);

            std::getline(data, message, ' ');
            //std::cout << message << "\n";
            ballPos.z = Ogre::StringConverter::parseReal(message);

            ball->getNode()->setPosition(ballPos);

            std::getline(data, message, ' ');
            player1 = Ogre::StringConverter::parseInt(message);

            std::getline(data, message, ' ');
            player2 = Ogre::StringConverter::parseInt(message);

        }
    }
    else{
        server->update(Ogre::StringConverter::toString(lPaddle->getPosition().x) + " " + Ogre::StringConverter::toString(ball->getPosition()) + " " 
            + Ogre::StringConverter::toString(player1) + " " + Ogre::StringConverter::toString(player2) + " ");
        if(net->pollForActivity(1)){
            Ogre::String data(net->udpClientData[0]->output);
            rPaddle->getNode()->setPosition(Ogre::Vector3(Ogre::StringConverter::parseReal(data),5,120)); 
        }
    }


    sim->stepSimulation(fe.timeSinceLastFrame);
    // Ogre::Vector3 ballpos = ball->getNode()->getPosition();
    // Ogre::Vector3 original = mCamera->getPosition(); 
    // Ogre::Vector3 newpos(ballpos.x, original.y, ballpos.z + 200);
    // newpos = newpos + mCamera->getDirection() * zoom;
    // mCamera->setPosition(newpos);

    //Update score display
    std::stringstream ss1;
    ss1 << player1;
    std::string display1 = "Player 1: " + ss1.str();
    button1->setText(display1);
    std::stringstream ss2;
    ss2 << player2;
    std:: string display2 = "Player 2: " + ss2.str();
    button2->setText(display2);
  }
  else if(isServer && multiplayer){
    if(net->pollForActivity(1)){
        menu = false;
    }
  }
return ret;
}

 
bool TutorialApplication::keyPressed(const OIS::KeyEvent& ke){
    BaseApplication::keyPressed(ke);
    if(menu)
    {
        switch(ke.key)
        {
            case OIS::KC_1:
                menu = false;
                sheet->destroyChild(serverBut);
                sheet->destroyChild(singlePlayerBut);
                sheet->destroyChild(clientBut);
                break;
            case OIS::KC_2:
                server = new Server(net);
                server->startLobby();
                multiplayer = true;
                sheet->destroyChild(serverBut);
                sheet->destroyChild(singlePlayerBut);
                sheet->destroyChild(clientBut);
                break;
            case OIS::KC_3:
                client = new Client(net, serverName);
                isServer = false;
                multiplayer = true;
                menu = false;
                sheet->destroyChild(serverBut);
                sheet->destroyChild(singlePlayerBut);
                sheet->destroyChild(clientBut);
                mCamera->setPosition(0, 75, 250);
                mCamera->lookAt(Ogre::Vector3(0,0,-150));
                break;
            case OIS::KC_ESCAPE:
                mShutDown = true;
                break;
            default:
                break;
        }
    }

    else{
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

        if(argc > 1){
            serverName = argv[1];
        }

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

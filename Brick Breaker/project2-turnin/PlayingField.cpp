#include <OgreEntity.h> 
#include <OgreSceneManager.h>
#include "PlayingField.h"

PlayingField::PlayingField(Ogre::SceneManager* scnMgr, Simulator* sim){
	walls[0] = new Wall(scnMgr, sim, Ogre::Vector3::UNIT_Y, 
	Ogre::Vector3::UNIT_Z, Ogre::Vector3(0,0,0), "floor", 1200, 3000);

	walls[1] = new Wall(scnMgr, sim, Ogre::Vector3::NEGATIVE_UNIT_Y, 
	Ogre::Vector3::UNIT_Z, Ogre::Vector3(0,500,0), "roof", 1200, 3000);

	walls[2] = new Wall(scnMgr, sim, Ogre::Vector3::UNIT_Z, 
	Ogre::Vector3::UNIT_Y, Ogre::Vector3(0,250,-1500), "wall1", 1200, 500);

	walls[3] = new Wall(scnMgr, sim, Ogre::Vector3::NEGATIVE_UNIT_Z, 
	Ogre::Vector3::UNIT_Y, Ogre::Vector3(0,250,1500), "wall2", 1200, 500);

	walls[4] = new Wall(scnMgr, sim, Ogre::Vector3::UNIT_X, 
	Ogre::Vector3::UNIT_Y, Ogre::Vector3(-600,250,0), "wall3", 3000, 500);

	walls[5] = new Wall(scnMgr, sim, Ogre::Vector3::NEGATIVE_UNIT_X, 
	Ogre::Vector3::UNIT_Y, Ogre::Vector3(600,250,0), "wall4", 3000, 500);
		        
	x_length = 1200.0f;
    z_length = 3000.0f;

    makeObstacles(scnMgr, sim);
}

void PlayingField::addToSimulator(){
	for(int i = 0; i < 6; ++i){
		walls[i]->addToSimulator();
	}
	for(int i = 0; i < 32; ++i){
		obs[i]->addToSimulator();
	}
}

void PlayingField::turnOffSound(){
    for(int i = 0; i < 6; ++i){
        walls[i]->turnOffSound();
    }
    for(int i = 0; i < 32; ++i){
        obs[i]->turnOffSound();
    }
}

void PlayingField::turnOnSound(){
    for(int i = 0; i < 6; ++i){
        walls[i]->turnOnSound();
    }
    for(int i = 0; i < 32; ++i){
        obs[i]->turnOnSound();
    }
}

void PlayingField::makeObstacles(Ogre::SceneManager* scnMgr, Simulator* sim){
	obs[0] = new Obstacle("ob11", scnMgr, sim, Ogre::Vector3(-500, 50, -1450), Ogre::Vector3(2,1,1),1);
    obs[1] = new Obstacle("ob12", scnMgr, sim, Ogre::Vector3(-500, 50, -1350), Ogre::Vector3(2,1,1),2);
    obs[2] = new Obstacle("ob13", scnMgr, sim, Ogre::Vector3(-500, 50, -1250), Ogre::Vector3(2,1,1),1);
    obs[3] = new Obstacle("ob14", scnMgr, sim, Ogre::Vector3(-500, 50, -1150), Ogre::Vector3(2,1,1),2);
    obs[4] = new Obstacle("ob15", scnMgr, sim, Ogre::Vector3(-500, 50, -1050), Ogre::Vector3(2,1,1),1);

    obs[5] = new Obstacle("ob21", scnMgr, sim, Ogre::Vector3(-300, 50, -1450), Ogre::Vector3(2,1,1),2);
    obs[6] = new Obstacle("ob22", scnMgr, sim, Ogre::Vector3(-300, 50, -1350), Ogre::Vector3(2,1,1),1);
    obs[7] = new Obstacle("ob23", scnMgr, sim, Ogre::Vector3(-300, 50, -1250), Ogre::Vector3(2,1,1),2);
    obs[8] = new Obstacle("ob24", scnMgr, sim, Ogre::Vector3(-300, 50, -1150), Ogre::Vector3(2,1,1),1);
    obs[9] = new Obstacle("ob25", scnMgr, sim, Ogre::Vector3(-300, 50, -1050), Ogre::Vector3(2,1,1),2);

    obs[10] = new Obstacle("ob31", scnMgr, sim, Ogre::Vector3(-100, 50, -1450), Ogre::Vector3(2,1,1),1);
    obs[11] = new Obstacle("ob32", scnMgr, sim, Ogre::Vector3(-100, 50, -1350), Ogre::Vector3(2,1,1),2);
    obs[12] = new Obstacle("ob33", scnMgr, sim, Ogre::Vector3(-100, 50, -1250), Ogre::Vector3(2,1,1),1);
    obs[13] = new Obstacle("ob34", scnMgr, sim, Ogre::Vector3(-100, 50, -1150), Ogre::Vector3(2,1,1),2);
    obs[14] = new Obstacle("ob35", scnMgr, sim, Ogre::Vector3(-100, 50, -1050), Ogre::Vector3(2,1,1),1);
    obs[15] = new Obstacle("ob36", scnMgr, sim, Ogre::Vector3(-100, 50, -950), Ogre::Vector3(2,1,1),2);

    obs[16] = new Obstacle("ob41", scnMgr, sim, Ogre::Vector3(100, 50, -1450), Ogre::Vector3(2,1,1),2);
    obs[17] = new Obstacle("ob42", scnMgr, sim, Ogre::Vector3(100, 50, -1350), Ogre::Vector3(2,1,1),1);
    obs[18] = new Obstacle("ob43", scnMgr, sim, Ogre::Vector3(100, 50, -1250), Ogre::Vector3(2,1,1),2);
    obs[19] = new Obstacle("ob44", scnMgr, sim, Ogre::Vector3(100, 50, -1150), Ogre::Vector3(2,1,1),1);
    obs[20] = new Obstacle("ob45", scnMgr, sim, Ogre::Vector3(100, 50, -1050), Ogre::Vector3(2,1,1),2);
    obs[21] = new Obstacle("ob46", scnMgr, sim, Ogre::Vector3(100, 50, -950), Ogre::Vector3(2,1,1),1);

    obs[22] = new Obstacle("ob51", scnMgr, sim, Ogre::Vector3(300, 50, -1450), Ogre::Vector3(2,1,1),1);
    obs[23] = new Obstacle("ob52", scnMgr, sim, Ogre::Vector3(300, 50, -1350), Ogre::Vector3(2,1,1),2);
    obs[24] = new Obstacle("ob53", scnMgr, sim, Ogre::Vector3(300, 50, -1250), Ogre::Vector3(2,1,1),1);
    obs[25] = new Obstacle("ob54", scnMgr, sim, Ogre::Vector3(300, 50, -1150), Ogre::Vector3(2,1,1),2);
    obs[26] = new Obstacle("ob55", scnMgr, sim, Ogre::Vector3(300, 50, -1050), Ogre::Vector3(2,1,1),1);

    obs[27] = new Obstacle("ob61", scnMgr, sim, Ogre::Vector3(500, 50, -1450), Ogre::Vector3(2,1,1),2);
    obs[28] = new Obstacle("ob62", scnMgr, sim, Ogre::Vector3(500, 50, -1350), Ogre::Vector3(2,1,1),1);
    obs[29] = new Obstacle("ob63", scnMgr, sim, Ogre::Vector3(500, 50, -1250), Ogre::Vector3(2,1,1),2);
    obs[30] = new Obstacle("ob64", scnMgr, sim, Ogre::Vector3(500, 50, -1150), Ogre::Vector3(2,1,1),1);
    obs[31] = new Obstacle("ob65", scnMgr, sim, Ogre::Vector3(500, 50, -1050), Ogre::Vector3(2,1,1),2);
}

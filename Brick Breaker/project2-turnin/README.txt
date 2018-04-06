This is a Brick Breaker game created using the Ogre3D graphics engine and the bullet physics library.  
Unfortunately, the executable for this project cannot be run without extensive setup, so you probably wont be 
able to play it without fiddling with makefiles and rpaths for a few hours, but my source code is 
here if you want to take a look. I created the project along with a group of 2 other students 
for my Game Technology class.  We employed an object oriented approach, in which all physical objects 
inherit from GameObject, which handles most of the physics components.  We wrote almost all of the 
program from scratch, other than the OgreMotionState and BulletContactCallback files that were wrappers 
for physics provided to us. The main function for the program is located in TutorialApplication.cpp.
noinst_HEADERS = BaseApplication.h TutorialApplication.h Ball.h PlayingField.h Paddle.h Simulator.h BulletContactCallback.h GameObject.h OgreMotionState.h Wall.h Obstacle.h NetManager.h Server.h Client.h

bin_PROGRAMS = assignment2
assignment2_CPPFLAGS = -I$(top_srcdir)
assignment2_SOURCES = BaseApplication.cpp TutorialApplication.cpp Ball.cpp PlayingField.cpp Paddle.cpp Simulator.cpp  GameObject.cpp OgreMotionState.cpp Wall.cpp Obstacle.cpp NetManager.cpp Server.cpp Client.cpp
assignment2_CXXFLAGS = $(OGRE_CFLAGS) $(OIS_CFLAGS) $(bullet_CFLAGS) $(SDL_CFLAGS) -lSDL -lSDL_mixer -lSDL_net $(SDL_CFLAGS) $(CEGUI_CFLAGS) $(CEGUI_OGRE_CFLAGS) $(SDLNET_CFLAGS)
assignment2_LDADD = $(OGRE_LIBS) $(OIS_LIBS) $(bullet_LIBS) $(SDL_LIBS) $(CEGUI_LIBS) $(CEGUI_OGRE_LIBS) 
assignment2_LDFLAGS = -lOgreOverlay -lboost_system -lSDL -lSDL_mixer -lSDL_net
CEGUI_OGRE_LIBS = -L/lusr/opt/cegui-0.8.4/lib -lCEGUIOgreRenderer-0 -lOgreMain -lpthread -lCEGUIBase-0

EXTRA_DIST = buildit makeit
AUTOMAKE_OPTIONS = foreign

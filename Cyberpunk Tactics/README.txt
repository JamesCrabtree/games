This is a RTS demo that I implemented using an A* algorithm for pathfinding. To play the demo 
run the Pathfinder executable.  The C# program I wrote to implement the algorithm is located in
the Assets folder as the files: 
Grid.cs: Methods for creating the grid system that the pathfinding algorithm uses 
Node.cs: A class containing information kept about each node of the grid
Pathfinding.cs: contains main method of the program
PlayerUnit.cs: Class containing methods for controlling units

Controls
--------
Left Click: Select a Unit
Left Click+Drag = Box Selection
Shift+Left Click: Add a unit to your currently selected units
Right Click: Set a rally point for your currently selected units or target an enemy unit
NumKey 1 = activate throw grenade ability for all selected units, Then left click to choose where
	you want the grenade to land
Esc: exit
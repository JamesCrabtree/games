This is a RTS demo that I implemented using an A* algorithm for pathfinding along with box-selection
and a real-time projectile trajectory calculation algorithm. To play the demo 
run the Pathfinder executable.  The C# programs I wrote to implement the various aspects of the game are located in
the Assets folder as the files: 
Cotrols/Grid.cs: Methods for creating the grid system that the pathfinding algorithm uses 
Controls/Node.cs: A class containing information kept about each node of the grid
Controls/Pathfinding.cs: contains main method of the program
Controls/Controller.cs: Inheritable base class for unit control
Controls/Heap.cs: An Implementation of a Binary Heap Data Structure
Controls/UnitSelect.cs: Component that handles player input

Marine/PlayerController.cs, PlayerHead.cs: inhereted from Controller to control units
Marine/PlayerUnit.cs: Class containing methods and data for controllable units

Projectiles/grenade.cs, bullet.cs: Instructs projectile on how to react to collisions
Projectiles/LaunchArcRenderer.cs: Creates Line showing projected trajectory of grenade

Controls
--------
Left Click: Select a Unit
Left Click+Drag = Box Selection
Shift+Left Click: Add a unit to your currently selected units
Right Click: Set a rally point for your currently selected units or target an enemy unit
NumKey 1 = activate throw grenade ability for all selected units, Then left click to choose where
	you want the grenade to land
Esc: exit

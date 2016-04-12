# unity-tetris
Simple Tetris clone game made with Unity

This is a very brief introduction at the overall project , since the source code is commented.
The architecture is structured in a way similar to a MVC: there is the logic part where the game is computed
and a UI part which takes care of the visualization. The two communicate thanks to a controller script (GameManager) which listens events generating from the logic part and propagates them to the UI. The logic part has a Facade class Tetris which implements the
interface ITetris.

This setup allows a great flexibility because the state is synchronized only through events. And since the controller script 
uses the interface, the actual implementation could also be a remote game executing on a server.
It's also easily extendable to a full 3D Tetris with perspective camera (I'll do it, soon or later) 

The game has only one scene, a script switches between panels when needed.
The game area is in world space and the bricks are 3D cubes. It's possible to play with any grid size.
The game also supports unlimited shapes of pieces, but for this exercise there are only the canonical 7.
For the best visualization, set the platform to Android and use 16:10 Portrait aspect ratio.

The project also comes with some basic tests done with Unity testing tools.

Developed with Unity 5.3.2f1, tested with:
 - Nexus 4 running Android Lollipop 5.1.1
 - Nexus 5X running Android Marshmallow 6.0.1

# ForgePositionInterpolation

This is a demo project for https://github.com/BeardedManStudios/ForgeNetworkingRemastered made in Unity 2018.1.1f1  
It was made to display a [bug with its debug tools, specifically, that the bytes received/sent, increased alongside ping increase (which shouldn't happen)](https://github.com/BeardedManStudios/ForgeNetworkingRemastered/issues/403)  
For the bug to be tested/displayed/reproduced, I had to make a constant RPC sending, and the classic is sending a position constantly.  
And I used the [classic snapshot system](https://developer.valvesoftware.com/wiki/Source_Multiplayer_Networking), which I think everyone who is to use Forge seriously, will make more or less.  
You can use this to share anything, not just a mere 2D position.

**tl;dr: It has a fully finished and working position snapshot system, as expected from all FPS + it displays a minor bug if you emulate high ping**

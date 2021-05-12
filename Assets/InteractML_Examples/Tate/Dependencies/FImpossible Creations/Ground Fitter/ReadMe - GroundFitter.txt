__________________________________________________________________________________________

Package "Ground Fitter"
Version 1.2.0

Made by FImpossible Creations - Filip Moeglich
https://www.FilipMoeglich.pl
FImpossibleGames@Gmail.com or Filip.Moeglich@Gmail.com

__________________________________________________________________________________________

Unity Connect: https://connect.unity.com/u/5b2e9407880c6425c117fab1
Youtube: https://www.youtube.com/channel/UCDvDWSr6MAu1Qy9vX4w8jkw
Facebook: https://www.facebook.com/FImpossibleCreations
Twitter (@FimpossibleC): https://twitter.com/FImpossibleC
Google+: https://plus.google.com/u/3/115325467674876785237

__________________________________________________________________________________________
Description:

Ground Fitter is component which is dynamically fitting object to ground.
You can choose if you want fit object in X and Z rotation axis (good for spiders) or limited X.
Thanks to smart calculations you have clear control over Y axis rotation.
Add component to gameObject and CHOOSE RIGHT COLLISION LAYERS for your game.

__________________________________________________________________________________________
Changelog:

V1.2.0
- Reworked some scripts for cleaner view in code (parenting etc.) WARNING: Some variables can be resetted when you import this version
- Upgraded precision of root motion rotation
- Added new simplified component
- Added scene which is using simplified component in use with CharacterController component

* Changed Variable Names Like:
RotationYAxis to UpAxisRotation
ChangePosition to GlueToGround
YOffset to UpOffset
delta to deltaTime

V1.1.9
- Added NavMesh use with GroundFitter example scene and script
- Added possibility to use root motion just on FGroundFitter component without need of FGroundFitterMovement
- Added possibility to move in any direction (sideways/diagonal), not just forward / backward + rotation

V1.1.7
- Added support for root motion
- Supporting animation clip names no matter they're written in upper / lowercase
- Ignoring self collider

V1.1.6
- Added option "Change Position" so you can untoggle this if you want to use ground fitter just to change object's rotation
- Some cleaning in code
- Added "Zone Cast Offset" variable to change position of raycast zone plane
 
V1.1.5
- Added gizmo raycasts for component so you can preview how raycasts will go
- Added some new features to ground fitter, main one - ZoneCast - casting additional raycasts in X shape and making average rotation from them
- Upgraded fitter movement component, now it falls correctly when object lost ground under feet (with correct checkRange) and upgraded some other stuff
- Some modifications in component's variables and methods order (in case if someone was using or changing this components in custom coding)

V1.1.0
- FGroundFitter is not creating additional GameObject anymore, it's repaced by few math functions
- FGroundFitter_Demo_Movement renamed to FGroundFitter_Demo_Patrolling
- You can now limit side and forward rotation limit to value you want to
- Added FGroundFitter_Movement component which is example of using ground fitter as movement controller, you can enchance it to your needs
- Added gizmos for scene window and some new options in inspector
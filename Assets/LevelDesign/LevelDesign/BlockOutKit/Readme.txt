SHADER SETUP
- Change all materials` shader to the one corresponding to your project`s Render Pipeline in the category "BlockOut"
- Change Material "1 - Glass" to your Render Pipeline standard shader
- Change Material "2 - Leaf" to your Render Pipeline Standard shader with CutOut mode

Overview of the asset:

CHARACTER
- Set to humanoid rig
- Character controller is set up
- You can disable Body and Arms for First Person Projects and VR

TRANSPORT
- Wheels, Steering Wheels and Doors are transformable
- You can disable parts of the transport that you do not need to build required vehicle
- Ground Vehicle can be used for Cars and Tanks
- Aircraft has Propellers and Jet Engines that you can hide

PROCEDURAL MESHES
- In prefab folder there is Procedural Primitive Prefab, just drag and drop and change settings shown in script
- You can just drag and drop PrimitiveGenerator script from Scripts folder on any desired object, make sure to disable previously existed collisions

CAMERA SCRIPTS
- Just drag and drop from Script folder on your Main Camera
- Free Fly camera
- Orbit camera
- Top-Down camera

ENVIRONMENT MESHES
- Trees can be found in Prefabs/Vegetation folder
- Building parts can be found in Prefabs/Environment folder

SHADERS
- Shaders located in BlockOutKit category
- Standard shader uses Mask Map
- Standard Triplanar shader uses World Coordinates to map the texture on the objects
- Simple Triplanar shader uses one texture without anything else

POSTPROCESSING PROFILE
- Default profile in case you are in project without Preview Package

TEXTURES
- Go to the Textures folder to see available textures
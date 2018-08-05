Load Doom wads directly into Unity.
(currently Doom1 shareware wad)

Builds meshes for linedefs and sectors.
Constructs textures directly from WAD.
Creates Unity gameobjects for things.
Has option to override materials and textures.

How to use:
Download Doom1 shareware wad from the internet and place it into Assets/StreamingAssets

You can download shareware wad from:
https://doomwiki.org/wiki/DOOM1.WAD

Open up the test_scene.scene and hit play to load the first map
Change the "Autoload Map" parameter in the WadLoader component to load different map "E1M1" "E1M2" "E1M3"...
You can walk around using arrow keys and open doors with spacebar.
(currently only doortype 1 and it's keycard variants are done)
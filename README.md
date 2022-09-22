# ULTRASKINS
ULTRASKINS is an addon for UMM that allows the user to replace the textures of the weapons


HOW TO USE:
drop the ultraskins folder into the plugins folder in BepinEx, shrimple as that

you can replace textures in the "Custom" file to swap indivisual textures, or create your own texture set in a new folder with your own textures as well
in order to load a custom texture set, go to the file properties of TexturesToLoad.Txt, uncheck the "Read only" box, open the file with your text editor of choice
the text file should look like this \n
///////////////////

Custom							//Texture set/ Folder that the textures will be loaded from

T_PistolNew						//Default Revolver Diffuse						
T_PistolNew_ID					//Default Revolver Diffuse ID
T_PistolNew_Emissive			//Default Revolver Emission
T_MinosRevolver_128				//Slab Revolver Diffuse
T_MinosRevolver_ID				//Slab Revolver Diffuse ID
T_MinosRevolver_128_Emissive	//Slab Revolver Emission
T_ShotgunNew					//Shotgun Diffuse
T_ShotgunNew_ID					//Shotgun Diffuse ID
T_NailgunNew_NoGlow				//Default Nailgun Diffuse
T_NailgunNew_ID					//Default Nailgun Diffuse ID
T_Sawblade						//Alt Nailgun's Saw Diffuse
T_Sawblade_ID					//Alt Nailgun's Saw Diffuse ID
T_SawbladeLauncher				//Alt Nailgun's Body Diffuse
T_SawbladeLauncher_ID			//Alt Nailgun's Body Diffuse ID
Railgun_Main_AlphaGlow			//RailCannon Diffuse, IMPORTANT NOTE: the ingame shader for this weapon uses the alpha channel for the glow, however, ULTRASKINS bypasses that and has a custom texture for the glow called "Railgun_Main_Emission"
T_Railgun_ID					//RailCannon Diffuse ID
T_RocketLauncher				//RocketLauncher Diffuse
T_RocketLauncher_ID				//RocketLauncher Diffuse ID
ArmV2_Diffuse					//Feedbacker
v2_armtex						//KnuckleBlaster
T_GreenArm						//Whiplash
skull2
skull2 compressed

///////////////////

Diffuse: The color texture used by the weapon.
ID: an ID texture is an rgb texture where each color channel/color is used as a mask, the R, G, B color channels represent Colors 1, 2, and 3 respectively in the terminal color editor, the alpha channel is used as a base for the colors to be overlayed on.
Emission : Glow texture used by the weapons, this texture should idealy be white and black as the color of the glow is determined by the colors you choose in the hud options.
as stated above, the railcannon's glow has been moved to its own seperate texture for ease of use.

the first line in the file is the name of the folder of the texture set it will load, Case sensitive
every other line under it are the textures names, you can replace the texture of a custom modded weapon simply by finding its texture name, and adding it to the list
its important that the texture's name in the list matches exactly with the actual file name it self, otherwise it will not load

once you're done editing the TexturesToLoad.txt file, make sure to save and **check the "Read only" box**, this step is important, otherwise the mod will just die,
then press k ingame to reload all the textures, if that doesnt work properly, then restart the level or restart the game.

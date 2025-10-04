# MonkeCosmetics
This mod is a sort of rewrite of the original cosmetics mod known as Gorilla Cosmetics.

This is not intergrated with the wardrobe currently but in the future that may be a possiblity.

### How to add assets?
Putting The `.MCmat` file in the plguins folder will load all materials that are in the MCmat file.

Eventually we will switch to a whole different schema but for now this works.

## Credits

* Ngbatz - Head dev
* Chloye - Made base of the mod
* All the testers!

## For Developers

### How to add materials
> You should already have a slight understanding of how to use unity and building assetbundles doing this.
* First download this https://github.com/NgbatzYT/MonkeCosmeticsUnityProj/archive/refs/heads/main.zip extract and open in unity.
* Second create a material and make it how you want, I recommend changing the tiling to about 5 if you have a texture or to your liking as it will appear different in-game.
* Third add it to an assetbundle by clicking on the material looking at the bottom and selecting assetbundle and creating a new assetbundle name.
* Fourth go to the tools tab at the top and select build all materials.
* Fifth Grab the created `.mcmat` file and test in game!

If you still can't figure out how to do it you can go to `#material-creation-help` in https://discord.gg/5E2WaaRx5u to get help!

### Disclaimer
Try not to name materials the same as others so like add your username or something to it, it just needs to be unique to not cause problems!

### Operator Functions
All valid special operators are below, you can append these to the end of the material name and it will change what the material does (You can only have one at a time).

`_FollowPlayerColor`: This makes the materials colour follow the players colour.

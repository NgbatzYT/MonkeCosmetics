# MonkeCosmetics
This mod is a sort of rewrite of the original cosmetics mod known as Gorilla Cosmetics.

This is not intergrated with the wardrobe currently but in the future that may be a possiblity.

### How to add assets?
Putting The `.MCmat` file in the plguins folder will load all materials that are in the MCmat file.

Eventually we will switch to a whole different schema but for now this works.

## Credits

* Ngbatz - Head dev
* Chloye - Made base of the mod
* EmeryKills - Helped Test a LOT
* And to all the testers!

## For Developers

### How to add materials
> You should already have a slight understanding of how to use unity and building assetbundles doing this.
* First create a material and make it how you want.
* Second add it to an assetbundle by clicking on the material looking at the bottom and selecting assetbundle and creating a new assetbundle name.
* Third build all the assetbundles.
* Fourth change the extension to `.mcmat` capitilisation doesn't matter and then test and then you can do what you want with it.

### Disclaimer
Try not to name materials the same as others so like add your username or something to it, it just needs to be unique to not cause problems!

### Operator Functions
All valid special operators are below, you can append these to the end of the material name and it will change what the material does (You can only have one at a time).

`_FollowPlayerColor` or `_FollowPlayerColour`: This makes the materials colour follow the players colour.

# MonkeCosmetics
This mod is a sort of rewrite of the original cosmetics mod known as Gorilla Cosmetics.

This works with the original GorillaCosmetics `.material`/`.gmat` files.
They will be loaded in but compatibility can't be guaranteed.

This is not intergrated with the wardrobe currently but in the future that may be a possiblity.

### How to add assets?
Putting The `.MCmat` file in the plguins folder will load all materials that are in the MCmat file.

Eventually we will switch to a whole different schema but for now this works.

For GorillaCosmetics Materials just put the `.material`/`.gmat` files into the plugins folder and they will load.

## Credits

* Ngbatz - Head dev
* Chloye - Made base of the mod
* EmeryKills - Helped Test a LOT
* And to all the testers!

## For Developers

### Disclaimer
Try not to name materials the same as others so like add your username or something to it, it just needs to be unique to not cause problems!

### Operator Functions
All valid special operators are below, you can append these to the end of the material name and it will change what the material does (You can only have one at a time).

`_FollowPlayerColor` or `_FollowPlayerColour`: This makes the materials colour follow the players colour.

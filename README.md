# MonkeCosmetics
A cosmetic mod!
Supports materials and cosmetics.

### How to add assets?
Putting The `.MCcosmetic` file in the materials folder in the BepInEx folder, will load all materials and cosmetics in the file.

## Credits

* Ngbatz - Head dev
* Chloye - Made base of the mod
* All the testers!

# For Developers

## How to make materials and cosmetics
> You should already have a slight understanding of how to use Unity before doing this.

### Setup
* First, download the project from https://github.com/NgbatzYT/MonkeCosmeticsUnityProj/archive/refs/heads/main.zip, extract it and open it in Unity 6000.2.9f1.
* Then go to the **Tools** tab at the top and select **MC Creator** to open the creator window.

### How to make materials
* First, create a material in Unity and make it how you want.
* In the MC Creator window, set **Creator Type** to `Material`.
* Click **Add Material** for each material you want in the bundle, then fill in each entry:
  * **Material**: the material you made.
  * **Thumbnail**: a `Texture2D` used as the preview icon.
  * **Material ID**: a unique ID in the format `creator.matname`.
  * **Custom Colours**: tick this if you want the players colour apply to it.
* Click **Create MC Material**.
* Grab the created `.MCcosmetic` file from `Assets/MCcosmetics` and test it in game!

### How to make cosmetics
* First, make your cosmetic as a GameObject (prefab) in Unity.
* In the MC Creator window, set **Creator Type** to `Cosmetic`.
* Click **Add Cosmetic** for each cosmetic you want in the bundle, then fill in each entry:
  * **Cosmetic Object**: the prefab/GameObject you made.
  * **Thumbnail**: a `Texture2D` used as the preview icon.
  * **Cosmetic Name**: the name shown in game.
  * **Cosmetic ID**: a unique ID in the format `creator.cosmeticname`.
  * **Position**: where the cosmetic attaches (head, etc.).
* Click **Create MC Cosmetic**.
* Grab the created `.MCcosmetic` file from `Assets/MCcosmetics` and test it in game!

> **Notes:**
> * Every entry needs its object/material and a thumbnail, otherwise the build will error in the Console.
> * The bundle is named after the ID of the **first** entry in the list, so keep that in mind if you add multiple items.
> * Each build creates a single `.MCcosmetic` file containing everything in the list.

If you still can't figure out how to do it you can go to `#cosmetic-creation-help` in https://discord.gg/5E2WaaRx5u to get help!

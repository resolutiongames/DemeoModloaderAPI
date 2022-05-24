Demeo Modloader API
===

First off, some notes about what the Demeo Modloader API is:

* It is **NOT a full "modding API"** - it's merely a small technical tool for mod programmers to make it easier to load DLLs into the game.
* It is a prototype, severely lacking in features right now, and is being released to try new things out and see if they're on the right track.
* It does **NOT** function on the version of the game that runs natively on the Quest headset. If you own a Quest headset
  and would like to run mods, use the Oculus Link version of the game that runs on your PC (this is for performance
  reasons, unfortunately) - you own this version of the game for free when you bought the game on Quest, same as the PC version.

With that out of the way, this repo is an example mod, showing how to use the modloader. Here's how it works, in chronological order:

### Write the Mod

A new mod can be created by building a regular ol' .netstandard2.1 DLL that references the Demeo `Assembly-CSharp.dll`.
If you have the Steam version of Demeo VR, it is located
in `"C:\Program Files (x86)\Steam\steamapps\common\Demeo\demeo_Data\Managed\Assembly-CSharp.dll"`. Important: Never
distribute this file. You must have Demeo installed on your local computer to develop mods for it.

You can see an example of this reference in this project's .csproj file. You must set the `DemeoDir` environment
variable for your local user to point at the installation directory of Demeo for this example mod to work (otherwise, it
defaults to Steam's default install location)

Once you have set up your project, create a new type that inherits from `DemeoMod`:

```csharp
using Boardgame.Modding;
public class MyCustomMod : DemeoMod {
    public override ModdingAPI.ModInformation ModInformation => new() {
        // TODO: fill out
    };
    public override void Load() {
        // TODO: fill out
    }
}
```

### Install the mod

Build the project, and copy the resulting DLL file to the `DemeoMods` folder in your Demeo installation directory. It's
probably good to nest the dll in a folder, in case you have other resources and the like that you'd like to bundle with
your mod. For example, it could be located here:

`"C:\Program Files (x86)\Steam\steamapps\common\Demeo\DemeoMods\MyCustomMod\MyCustomMod.dll"`

(The .csproj in this example project has a build step to automatically copy the build result to Demeo - you may want a slightly different setup)

### When the game boots up...

When the game boots up, it will recursively scan the DemeoMods folder for any DLL it finds. It opens up each DLL to
check if any types within inherit from the DemeoMod type (using Mono.Cecil to not load it into the AppDomain if it's not
a mod DLL). If it does, it loads the assembly into the game (via Assembly.Load), and loops through each type that
inherits from the DemeoMod type (usually only one per DLL) and instantiates them, calling the default constructor with
no parameters. Once constructed, it will invoke various abstract and virtual methods defined on the DemeoMod type at the
appropriate time.

### The DemeoMod type

Currently, the DemeoMod type is pretty bare. It has a few things in it:

* The Load abstract method - this is invoked after the game has fully started up.
* The gameContext public variable - this is assigned right before Load is called, and contains a whole bunch of useful
  types to interact with the game.
* The OnEarlyInit virtual method - this is invoked extremely early in the startup process, right after the mod is
  loaded. The gameContext variable is not initialized. This is intended for e.g. mods that disable the intro cutscene or
  whatever.
* The ModInformation abstract property - this is called to pull out information about the mod to display in the mod UI ingame.

### The ModInformation type

There are a few fields in the ModInformation type returned by your mod.

    public string name; // The displayed name of your mod in the UI
    public string version; // The version displayed in the UI
    public string author; // You!
    public string description; // A description of the mod (currently unused?)
    public bool isNetworkCompatible;

The final bool, `isNetworkCompatible`, will take some explanation.

We use a matchmaking/networking system called Photon, and it has different matchmaking "pools" of players, identified by
a string common across all players in the same pool. We currently set this to the version of the game - so players
playing one version of the game don't connect to a just-released new version of the game someone jumped on as soon as it
released.

If your mod only works if everyone in the game has your mod installed (for e.g. mods that affect behaviour),
set `isNetworkCompatible` to false. This will append the name and version of your mod to this "matchmaking pool
string" (called ConnectionString internally), causing you to only matchmake with people who have the same name and
version of your mod installed.

If your mod works just fine on its own, and does not require other players to have the same mod installed - for example,
a cosmetics-only mod - set `isNetworkCompatible` to true.

If you think you know better and you have a mod that changes behaviour, and you're *really* careful around all your code
to make sure that everything stays compatible with a client who doesn't have the mod installed (lookin' at you,
HouseRules, you're great), you can set `isNetworkCompatible` to true. Please be careful, though, as you could easily
ruin the experience of other players who are playing with a vanilla Demeo! (Maybe chat with Demeo devs before doing this)

### Contact

The Demeo Modding Group discord is an unofficial discord filled with some super super great people who can hopefully
help you out. In that discord, you'll find me, @khyperia - feel free to ping me with whatever questions you have!
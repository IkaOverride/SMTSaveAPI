# SMTSaveAPI
### Save and load custom variables independently from the game’s save file

**SMTSaveAPI** allows mod developers to easily save and load custom variables alongside game saves, ensuring persistent data storage without modifying the game’s original save file and preventing compatibility issues

## Features
- Save and load custom variables with ease
- Events for saving and loading operations
- Supports persistent and temporary saved values: delete your values when your mod is deleted
- Prevents users from renaming their modded save file to use it with another game save

## Getting Started (User)
### Installation
- It is recommended to use a mod manager (like [r2modman](https://thunderstore.io/c/supermarket-together/p/ebkr/r2modman)) to download mods
- If you don't wish to use a mod manager, go to *Manual Installation*

### Manual Installation
- Download the appropriate version of [BepInEx (likely Windows x64)](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2)
- Extract the downloaded files to your Supermarket Together folder ([how to find a game folder](https://youtube.com/watch?v=jL8eB21q01s))
- Download the [latest release of the mod](https://github.com/IkaOverride/SMTSaveAPI/releases/latest)
- Place the mod's DLL file into the `Supermarket Together/BepInEx/plugins` folder

## Getting Started (Developer)
### Installation
- Add **SMTSaveAPI** as a dependency to your mod
- Use `SavedValue<T>` to store custom variables
- Hook into the API's saving/loading events if needed

### Example Usage
Below is an example mod called `ExampleMod`, a mod that saves and loads store time:

```csharp
[BepInPlugin("ExampleModId", "ExampleMod", "1.0.0")]
[BepInDependency("SMTSaveAPI", BepInDependency.DependencyFlags.HardDependency)] // Ensure SMTSaveAPI loads first
public class ExampleMod : BaseUnityPlugin
{
    internal static SavedValue<float> StoreTime = new SavedValue<float>("ExampleModId.StoreTime", 8f, false);
    /*
        "ExampleModId.StoreTime" -> unique ID for the variable (REQUIRED)
                                    I recommend "ModId.ValueName"

        8f                       -> default value (OPTIONAL)
                                    here we use 8f because 8:00 AM is the default time when loading a save, and time is saved as a float in the assembly

        false                    -> should the value persist even if unregistered (OPTIONAL, default is false)
                                    value unregistered = either you delete your value, or the user deletes your mod
    */

    private void Awake()
    {        
        // Subscribe to API events (OPTIONAL, depends on mod needs)
        SaveEventHandler.Saving += OnSaving;
        SaveEventHandler.Loaded += OnLoaded;
    }

    // Called before saving custom variables
    private void OnSaving()
    {
        StoreTime.Value = GameData.Instance.NetworktimeOfDay;
    }

    // Called after custom variables have been loaded
    private void OnLoaded()
    {
        GameData.Instance.NetworktimeOfDay = StoreTime.Value;
        if (StoreTime.Value > 8f && StoreTime.Value < 22.5f)
        {
            // Code that opens the store here
        }
    }
}
```

### Events
SMTSaveAPI provides events to hook into the saving/loading process:

#### Saving Event  
Triggered **before** the save file is written:
```csharp
SaveEventHandler.Saving += () => { /* Custom save logic here */ };
```

#### Loaded Event
Triggered **after** the save file is loaded:
```csharp
SaveEventHandler.Loaded += () => { /* Custom load logic here */ };
```

## **Contributing**
Contributions are welcome! If you find a bug or have suggestions, feel free to open an issue or submit a pull request
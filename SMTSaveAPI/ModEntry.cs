using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace SMTSaveAPI
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class ModEntry : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        internal static new ConfigFile Config;

        private void Awake()
        {
            Logger = base.Logger;
            Config = base.Config;
            new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll();
        }
    }
}

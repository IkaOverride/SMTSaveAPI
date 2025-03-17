using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using SMTSaveAPI.API.Managers;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;

namespace SMTSaveAPI.Patches
{
    [HarmonyPatch]
    internal static class SavePatches
    {
        [HarmonyPatch(typeof(NetworkSpawner), nameof(NetworkSpawner.SavePropsCoroutine), MethodType.Enumerator), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnSaving(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .Start()

                .MatchForward(false, [
                    new CodeMatch(OpCodes.Ldstr, "SavingContainer"),
                    new CodeMatch(),
                    new CodeMatch(),
                    new CodeMatch(OpCodes.Ldc_I4_0)
                ])
                .Advance(-2)
                .Insert([
                    new(OpCodes.Call, AccessTools.Method(typeof(CustomSaveManager), nameof(CustomSaveManager.SaveValues)))
                ])

                .Instructions();
        }
        [HarmonyPatch(typeof(NetworkSpawner), nameof(NetworkSpawner.OnStartServer)), HarmonyPostfix]
        private static void OnLoading()
            => CustomSaveManager.LoadValues();

        [HarmonyPatch(typeof(GameData), nameof(GameData.DoDaySaveBackup)), HarmonyPostfix]
        private static void OnSavingBackup()
        {
            string savePath = SavePathManager.CustomSaveFilePath;
            if (File.Exists(savePath))
                File.Copy(savePath, SavePathManager.ToBackupPath(savePath, GameData.Instance.NetworkgameDay), true);
        }

        [HarmonyPatch(typeof(SetFsmString), nameof(SetFsmString.DoSetFsmString)), HarmonyPostfix]
        private static void OnLoadingBackup(SetFsmString __instance)
        {
            if (__instance.variableName.Value == "FilenameToCopy")
            {
                string customBackupPath = SavePathManager.ToCustomPath(SavePathManager.GetSavePath(__instance.setValue.Value));
                if (File.Exists(customBackupPath))
                    File.Copy(customBackupPath, SavePathManager.RemoveBackupSuffix(customBackupPath), true);
            }
        }
    }
}

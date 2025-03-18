using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using SMTSaveAPI.API.Managers;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace SMTSaveAPI.Patches
{
    [HarmonyPatch]
    internal static class SavePatches
    {
        private static readonly SemaphoreSlim SaveLock = new(1, 1);

        private static readonly SynchronizationContext unityContext = SynchronizationContext.Current;

        [HarmonyPatch(typeof(NetworkSpawner), nameof(NetworkSpawner.SavePropsCoroutine), MethodType.Enumerator), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnSaving(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .Start()

                .MatchForward(false, [
                    new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(GameCanvas), nameof(GameCanvas.Instance))),
                    new CodeMatch(),
                    new CodeMatch(OpCodes.Ldstr, "SavingContainer"),
                    new CodeMatch(),
                    new CodeMatch(),
                    new CodeMatch(OpCodes.Ldc_I4_0)
                ])
                .RemoveInstructions(10)
                .Insert([
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, AccessTools.Method(typeof(SavePatches), nameof(RunSaveTask)))
                ])

                .Instructions();
        }
        
        private static void RunSaveTask(NetworkSpawner __instance)
        {
            Task.Run(async () => {
                await SaveLock.WaitAsync();

                try
                {
                    CustomSaveManager.SaveValues();
                }
                finally
                {
                    SaveLock.Release();
                }

                unityContext.Post(_ =>
                {
                    GameCanvas.Instance.transform.Find("SavingContainer").gameObject.SetActive(false);
                    __instance.isSaving = false;
                }, null);
            });
        }

        [HarmonyPatch(typeof(NetworkSpawner), nameof(NetworkSpawner.OnStartServer)), HarmonyPrefix]
        private static void OnLoading()
            => Task.Run(CustomSaveManager.LoadValues);

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

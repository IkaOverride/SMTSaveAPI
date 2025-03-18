using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SMTSaveAPI.API.Events;
using SMTSaveAPI.API.SavedValue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SMTSaveAPI.API.Managers
{
    /// <summary>
    /// Manages the saving and loading of custom variables in a separate save file.
    /// Provides functionality to store and retrieve custom data independently from the game's main save system.
    /// </summary>
    public static class CustomSaveManager
    {
        /// <summary>
        /// Dictionary storing all registered saved values.
        /// </summary>
        public static readonly Dictionary<string, ISavedValue> SavedValues = [];

        /// <summary>
        /// Retrieves a saved value by key.
        /// </summary>
        /// <typeparam name="T">The expected type of the saved value.</typeparam>
        /// <param name="key">The unique key identifying the saved value.</param>
        /// <returns>The saved value if found, otherwise returns null.</returns>
        public static SavedValue<T> GetSavedValue<T>(string key)
            => SavedValues.TryGetValue(key, out ISavedValue iValue) && iValue is SavedValue<T> value ? value : null;

        /// <summary>
        /// Retrieves the raw value of a saved variable by key.
        /// </summary>
        /// <typeparam name="T">The expected type of the value.</typeparam>
        /// <param name="key">The unique key identifying the saved value.</param>
        /// <returns>The value if found, otherwise the default value of type <typeparamref name="T"/>.</returns>
        public static T GetValue<T>(string key)
            => SavedValues.TryGetValue(key, out ISavedValue iValue) && iValue is SavedValue<T> value ? value.Value : default;

        /// <summary>
        /// Represents the MD5 hash of the base save file used for save identification.
        /// </summary>
        public static SavedValue<string> BaseSaveHash { get; } = new SavedValue<string>("SMTSaveAPI.BaseSaveHash");

        /// <summary>
        /// Saves all registered values to the custom save file.
        /// </summary>
        internal static void SaveValues()
        {
            SaveEventHandler.OnSaving();

            Stopwatch sw = Stopwatch.StartNew();

            BaseSaveHash.Value = GetBaseSaveMD5(SavePathManager.BaseSaveFilePath);

            Dictionary<string, JObject> json = File.Exists(SavePathManager.CustomSaveFilePath)
                ? JsonConvert.DeserializeObject<Dictionary<string, JObject>>(XORCipher(File.ReadAllText(SavePathManager.CustomSaveFilePath)))
                : [];

            foreach (KeyValuePair<string, ISavedValue> kvp in SavedValues)
            {
                if (kvp.Value.Value == null)
                {
                    json.Remove(kvp.Key);
                    continue;
                }

                json[kvp.Key] = new()
                {
                    ["persistent"] = kvp.Value.Persistent,
                    ["value"] = JToken.FromObject(kvp.Value.Value)
                };
            }

            File.WriteAllText(SavePathManager.CustomSaveFilePath, XORCipher(JsonConvert.SerializeObject(json)));

            ModEntry.Logger.LogInfo($"Saved custom values in {sw.Elapsed.TotalMilliseconds:0.00}ms");
        }

        /// <summary>
        /// Loads all saved values from the custom save file.
        /// </summary>
        internal static void LoadValues()
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (!File.Exists(SavePathManager.CustomSaveFilePath))
            {
                SaveEventHandler.OnLoaded();
                return;
            }

            Dictionary<string, JObject> deserializedValues = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(XORCipher(File.ReadAllText(SavePathManager.CustomSaveFilePath)));

            if (!deserializedValues.TryGetValue("SMTSaveAPI.BaseSaveHash", out JObject obj) || obj["value"]?.Value<string>() != GetBaseSaveMD5(SavePathManager.BaseSaveFilePath))
            {
                File.Delete(SavePathManager.CustomSaveFilePath);
                SaveEventHandler.OnLoaded();
                return;
            }

            foreach (KeyValuePair<string, JObject> entry in deserializedValues.ToList())
            {
                if (!SavedValues.TryGetValue(entry.Key, out ISavedValue savedValue))
                {
                    if (!entry.Value.Value<bool>("persistent"))
                        deserializedValues.Remove(entry.Key);

                    continue;
                }

                savedValue.Value = entry.Value["value"].ToObject(savedValue.ValueType);
            }

            File.WriteAllText(SavePathManager.CustomSaveFilePath, XORCipher(JsonConvert.SerializeObject(deserializedValues)));

            ModEntry.Logger.LogInfo($"Loaded custom values in {sw.Elapsed.TotalMilliseconds:0.00}ms");

            SaveEventHandler.OnLoaded();
        }

        /// <summary>
        /// Performs a simple XOR cipher encryption/decryption on a string with SMTSaveAPI's key.
        /// </summary>
        /// <param name="data">The input string to encrypt or decrypt.</param>
        /// <returns>The transformed (encrypted or decrypted) string.</returns>
        private static string XORCipher(string data)
        {
            string key = "Ff2hiu0ofQ456ftoFM";
            char[] result = data.ToCharArray();
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (char)(data[i] ^ key[i % key.Length]);
            }
            return new string(result);
        }

        /// <summary>
        /// Computes the MD5 hash of a base save file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>The MD5 hash as a hexadecimal string.</returns>
        private static string GetBaseSaveMD5(string filePath)
        {
            using MD5 md5 = MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(ES3.DecryptBytes(File.ReadAllBytes(filePath), "g#asojrtg@omos)^yq"))).Replace("-", "").ToLower();
        }
    }
}

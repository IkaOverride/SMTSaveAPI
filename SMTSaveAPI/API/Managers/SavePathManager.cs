using HutongGames.PlayMaker;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SMTSaveAPI.API.Managers
{
    /// <summary>
    /// Helper class for save file paths, including standard, modded, and backup save locations.
    /// Provides utilities to retrieve and modify save file paths for different purposes.
    /// </summary>
    public static class SavePathManager
    {
        /// <summary>
        /// Gets the base filename of the current save file.
        /// </summary>
        public static string BaseSaveFileName
        {
            get => FsmVariables.GlobalVariables.GetFsmString("CurrentFilename").Value;
        }

        /// <summary>
        /// Gets the base file path of the current save file.
        /// </summary>
        public static string BaseSaveFilePath
        {
            get => GetSavePath(BaseSaveFileName);
        }

        /// <summary>
        /// Gets the file path for the custom save file.
        /// </summary>
        public static string CustomSaveFilePath
        {
            get => ToCustomPath(BaseSaveFilePath);
        }

        /// <summary>
        /// Constructs a full file path by combining the application's persistent data path with the provided filename.
        /// </summary>
        /// <param name="fileName">The filename of the save file.</param>
        /// <returns>The full path to the save file.</returns>
        public static string GetSavePath(string fileName)
            => string.Concat(Application.persistentDataPath, "/", fileName);

        /// <summary>
        /// Converts a base file path for a modded save.
        /// </summary>
        /// <param name="basePath">The base path of the original save file.</param>
        /// <returns>The modified file path with the updated directory name and file extension.</returns>
        public static string ToCustomPath(string basePath)
            => basePath.Replace("StoreFile", "StoreModdedFile").Replace(".es3", ".smtsave");

        /// <summary>
        /// Removes the day-specific backup suffix (e.g., "Day1", "Day2") from the given file path.
        /// This method is useful for reverting a backup file path to its original, non-day-specific version.
        /// </summary>
        /// <param name="path">The file path containing a day-specific backup suffix.</param>
        /// <returns>The file path without the day-specific suffix.</returns>
        public static string RemoveBackupSuffix(string path)
            => Regex.Replace(path, @"Day\d+", "");

        /// <summary>
        /// Generates a backup file path by appending the given day number to the base path.
        /// </summary>
        /// <param name="basePath">The original file path to use as the base for the backup.</param>
        /// <param name="day">The day number that will be appended to the base path to form the backup filename.</param>
        /// <returns>A new file path with the day number appended before the file extension.</returns>
        public static string ToBackupPath(string basePath, int day)
            => ToBackupPath(basePath, day.ToString());

        /// <summary>
        /// Generates a backup file path by appending a specified day identifier to the base path.
        /// </summary>
        /// <param name="basePath">The original file path to use as the base for the backup.</param>
        /// <param name="day">A string representing the day identifier that will be inserted into the base path.</param>
        /// <returns>A new file path with the day identifier inserted before the file extension.</returns>
        public static string ToBackupPath(string basePath, string day)
            => basePath.Insert(basePath.Length - Path.GetExtension(basePath).Length, $"Day{day}");
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WaddleRush.Persistence
{
    [Serializable] public sealed class ProfileData
    {
        public int version = 1, level = 1, xp, coins;
        public string selectedSkin = "Aurora";
        public List<string> unlockedSkins = new List<string> { "Aurora" };
        public int highScore, matches, fishCollected, croppedMass, fusions;
    }

    public interface ILocalSaveService { ProfileData Load(); void Save(ProfileData data); }

    public sealed class LocalSaveService : ILocalSaveService
    {
        readonly string main, backup, temp;
        public LocalSaveService(string directory)
        { main = Path.Combine(directory, "profile.json"); backup = Path.Combine(directory, "profile_backup.json"); temp = main + ".tmp"; }

        public ProfileData Load()
        {
            var data = Read(main) ?? Read(backup) ?? new ProfileData();
            if (data.version < 1) data.version = 1;
            if (data.level < 1) data.level = 1;
            if (data.unlockedSkins == null) data.unlockedSkins = new List<string> { "Aurora" };
            return data;
        }

        ProfileData Read(string path)
        {
            try { return File.Exists(path) ? JsonUtility.FromJson<ProfileData>(File.ReadAllText(path)) : null; }
            catch (Exception e) { Debug.LogWarning("Local profile recovery: " + e.Message); return null; }
        }

        public void Save(ProfileData data)
        {
            var directory = Path.GetDirectoryName(main);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(temp, JsonUtility.ToJson(data, true));
            if (JsonUtility.FromJson<ProfileData>(File.ReadAllText(temp)) == null) throw new IOException("Save verification failed");
            if (File.Exists(main)) File.Copy(main, backup, true);
            if (File.Exists(main)) File.Delete(main);
            File.Move(temp, main);
        }
    }
}

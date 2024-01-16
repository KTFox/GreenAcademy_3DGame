using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace RPG.Saving {
    public class SavingSystem : MonoBehaviour {

        /// <summary>
        /// Save all SaveableEntity states into saveFile
        /// </summary>
        /// <param name="saveFile"></param>
        public void Save(string saveFile) {
            Dictionary<string, object> state = LoadFile(saveFile);

            CaptureAllSaveableEntityState(state);
            SaveFile(saveFile, state);
        }

        /// <summary>
        /// Load all SaveableEntity states from saveFile
        /// </summary>
        /// <param name="saveFile"></param>
        public void Load(string saveFile) {
            RestoreAllSaveableEntityState(LoadFile(saveFile));
        }

        private void SaveFile(string saveFile, Dictionary<string, object> state) {
            string path = GetPathFromSaveFile(saveFile);

            using (FileStream stream = File.Open(path, FileMode.Create)) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }

            Debug.Log($"Saving to {path}");
        }

        /// <summary>
        /// Return an empty dictionary if saveFile doesn't exist
        /// </summary>
        /// <param name="saveFile"></param>
        /// <returns></returns>
        private Dictionary<string, object> LoadFile(string saveFile) {
            string path = GetPathFromSaveFile(saveFile);

            if (!File.Exists(path)) {
                return new Dictionary<string, object>();
            }

            using (FileStream stream = File.Open(path, FileMode.Open)) {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void CaptureAllSaveableEntityState(Dictionary<string, object> state) {
            SaveableEntity[] saveableEntities = FindObjectsOfType<SaveableEntity>();

            foreach (SaveableEntity saveableEntity in saveableEntities) {
                state[saveableEntity.GetUniqueIdentifier()] = saveableEntity.CaptureState();
            }
        }

        private void RestoreAllSaveableEntityState(Dictionary<string, object> state) {
            SaveableEntity[] saveableEntities = FindObjectsOfType<SaveableEntity>();

            foreach (SaveableEntity saveableEntity in saveableEntities) {
                string identifier = saveableEntity.GetUniqueIdentifier();

                if (state.ContainsKey(identifier)) {
                    saveableEntity.RestoreState(state[identifier]);
                }
            }
        }

        /// <summary>
        /// Get Persistent Data Path from saveFile
        /// </summary>
        /// <param name="saveFile"></param>
        /// <returns></returns>
        private string GetPathFromSaveFile(string saveFile) {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}

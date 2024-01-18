using System;
using UnityEngine;

namespace RPG.Stats {
    public class BaseStats : MonoBehaviour {

        [Range(1, 3)]
        [SerializeField]
        private int startLevel = 1;
        [SerializeField]
        private CharacterClass characterClass;
        [SerializeField]
        private ProgressionSO progressionSO;

        private int currentLevel;

        private void Start() {
            currentLevel = CalculateLevel();

            Experience experience = GetComponent<Experience>();
            if (experience != null) {
                experience.onExperienceGained += UpdateExperience;
            }

            Debug.Log("First calculate current level");
        }

        private void UpdateExperience() {
            if (currentLevel < CalculateLevel()) {
                currentLevel = CalculateLevel();
                Debug.Log("Level up");
            }
        }

        public float GetStat(Stat stat) {
            return progressionSO.GetStat(characterClass, stat, CalculateLevel());
        }

        public int GetCurrentLevel() {
            if (currentLevel < 1) {
                currentLevel = CalculateLevel();
            }

            return currentLevel;
        }

        public int CalculateLevel() {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startLevel;

            float currentXP = experience.GetExperiencePoint();
            int penultimateLevel = progressionSO.GetPenultimateLevel(Stat.ExperienceToLevelUp, characterClass);

            for (int level = 1; level <= penultimateLevel; level++) {
                float XPToLevelUp = progressionSO.GetStat(characterClass, Stat.ExperienceToLevelUp, level);
                if (XPToLevelUp > currentXP) {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }
    }
}

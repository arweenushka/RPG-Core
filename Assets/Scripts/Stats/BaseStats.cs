using System;
using Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Stats
{
    public class BaseStats : MonoBehaviour
    {
        //level could be only in this range
        [Range(1, 80)]
        [SerializeField] private int startingLevel;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression = null;
        [SerializeField] private GameObject levelUpParticleEffect = null;
        //enemies should not use some stat modifiers for damage for example. for player will be true
        [SerializeField] private bool shouldUseModifiers = false;
        [SerializeField] private UnityEvent onLevelUpEvent;
        
        private int currentLevel = 0;
        public event Action onLevelUp;
        private Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
        }

        private void Start()
        {
            currentLevel = CalculateLevel();
        }

        private void OnEnable() {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable() {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        } 

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                LevelUpEffect();
                //event added to the list
                onLevelUp();
                onLevelUpEvent.Invoke();
            }
        }
        
        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat)/100);
        }
        
        //pure stat without modifiers
        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }
        
        //used for getting additional value for different stat attributes. ex. in case of damage it takes damage from
        //picked up weapon and summarize it with current level damage
        private float GetAdditiveModifier(Stat stat)
        {
            //player true , enemies false
            if (!shouldUseModifiers) return 0;
            
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
        
        //used for getting additional bonus for different stat attributes. ex. in case of damage it takes damage from
        //picked up weapon and add bonus of this weapon it it has it
        private float GetPercentageModifier(Stat stat)
        {
            //player true , enemies false
            if (!shouldUseModifiers) return 0;
            
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
        
        public int GetLevel()
        {
            if (currentLevel < 1)
            {
                currentLevel = CalculateLevel();
            }
            return currentLevel;
        } 
        
        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            float currentXP = experience.GetExperience();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (XPToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }
    }
}

using Stats;
using TMPro;
using UnityEngine;

namespace Attributes
{
    public class PlayerLevelDisplay : MonoBehaviour
    {

        private BaseStats baseStats;
        
        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            DisplayLevel();
        }
        
        private void DisplayLevel()
        {
            //show health with 1 decimal
            GetComponent<TextMeshProUGUI>().SetText("{0}", baseStats.GetLevel());
        }
    }
}
using TMPro;
using UnityEngine;

namespace UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damageText = null;

        public void SetValue(float damageAmount)
        {
            damageText.SetText("{0}", damageAmount);
        }
    }
}
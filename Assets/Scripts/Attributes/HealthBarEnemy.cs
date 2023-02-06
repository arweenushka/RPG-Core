
using UnityEngine;

namespace Attributes
{
    //one of the ways to do health bar with foreground scaling. used for enemies in the project
    public class HealthBarEnemy : MonoBehaviour
    {
        [SerializeField] private Health healthComponent;
        [SerializeField] private RectTransform foreground;
        [SerializeField] private Canvas rootCanvas;
        private void Update()
        {
            //do not show health bar if ene,y has full health or is dead
            if (Mathf.Approximately(healthComponent.GetHealthFraction(), 0) 
                || Mathf.Approximately(healthComponent.GetHealthFraction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(healthComponent.GetHealthFraction(), 1, 1);
        }
    }
}
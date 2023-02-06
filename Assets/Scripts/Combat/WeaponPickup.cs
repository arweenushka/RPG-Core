using Attributes;
using UnityEngine;
using Controllers;
using UnityEngine.Serialization;

namespace Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        private Fighter fighter;
        private Health health;
        [FormerlySerializedAs("weapon")] [SerializeField] private WeaponConfig weaponConfig = null;
        [SerializeField] private float healthToRestore = 0;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Pickup(other.gameObject);
            }
        }
        
        //pickupObject is player
        private void Pickup(GameObject pickupObject)
        {
            //right now Unarmed config is used for healing pick up object. when healing dont change weapon in arms to unarmed.
            //still use previous one
            if (weaponConfig != null && !weaponConfig.name.Equals("Unarmed"))
            {
                pickupObject.GetComponent<Fighter>().EquipWeapon(weaponConfig);
            }
            
            if (healthToRestore > 0)
            {
                pickupObject.GetComponent<Health>().Heal(healthToRestore);
            }
            Destroy(gameObject);
        }

        public CursorType GetCursorType()
        {
            return CursorType.PickUp;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //player can pick up only if stay close to pick up object
                if (Vector3.Distance(transform.position, callingController.transform.position) < 2.0f)
                {
                    Pickup(callingController.gameObject);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}

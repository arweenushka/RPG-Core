using Attributes;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make new Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private Weapon equipedPrefab = null;
        [SerializeField] private AnimatorOverrideController animatorOverrideController = null;
        //closest position to stop at for attack with weapon
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float weaponDamage = 5f;
        [SerializeField] private float percentageBonus = 0;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;
        
        const string weaponName = "Weapon";

        //getters
        public float WeaponRange => weaponRange;
        public float WeaponDamage => weaponDamage;

        //spawn weapon in player hand
        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            //destroy currently handed weapon before take new one
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;
            if (equipedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                //get weapon name of new weapon to be able to destroy it if needed 
                weapon = Instantiate(equipedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }
            //overrideController and else if section is added to fix animation defect where after picking up sword and
            //then fireball character animation is still shown for the sword weapon
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverrideController != null)
            {
                //change weapon animation in real time depends of type of weapon
                animator.runtimeAnimatorController = animatorOverrideController;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }
        
        private static void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;
            //change weapon name to avoid possible problems with destroying old weapons
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }
        
        //get place of the weapon depends of the hand setupped 
        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        //to check if we should shoot or we should hit with hand weapon
        public bool HasProjectile()
        {
            return projectile != null;
        }
        
        //shoot arrow
        //public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject attackInitiator)
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject attackInitiator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            //projectileInstance.SetTarget(target, attackInitiator, weaponDamage);
            projectileInstance.SetTarget(target, attackInitiator, calculatedDamage);
        }
        
        public float GetPercentageBonus()
        {
            return percentageBonus;
        }
    }
}

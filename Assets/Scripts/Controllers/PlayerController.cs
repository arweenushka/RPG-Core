using System;
using Attributes;
using Combat;
using Core;
using Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        private Mover mover;
        private Fighter fighter;
        private Health health;

        [Serializable]
        private struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        
        [SerializeField] float raycastRadius = 1f;

        // Start is called before the first frame update
        private void Awake()
        {
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (InteractWithUI()) return;
            //if player is dead then do nothing from next steps in update func
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            
            //if we have interacted with combat and have attacked, then we are doing only attack and skip another action. (return false to do it)
            if (InteractWithComponent()) return;
            if(InteractWithMovement()) return;
            
            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            //check if we are over some UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        /*check if object has Combat target script. if yes then we can attack it.
        useful in case if in our view we see some obstacle between camera and object to attack.
        in case when direction is correct but object for attack is not visible because of obstacle, then do not attack it.
        attack only when user click exactly on visible "object to attack"*/
        
        /*
         * after checking if we are able to attack anything, stop another actions with this object for attack.
         * do attack and then send false to update method to show that we should not continue call other methods
         */
        [Obsolete("Method1 is deprecated, please use InteractWithComponent() instead.")]
        private bool InteractWithCombat()
        {
            //get all objects on current ray way and check into them
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                
                //check if object to ineract with has combat target component and obly if yes continue with next steps
                if (target == null) continue;
                
                //if it is not possible to attack the object then loop and check next one
                if(!fighter.CanAttack(target.gameObject)) continue;

                if (Input.GetMouseButton(0))
                {
                    fighter.Attack(target.gameObject);
                }

                SetCursor(CursorType.Combat);
                //yes, i am interacted with combat(even hovering that combat)
                return true;
            }
            //if we  didn´t find the target to interact with then we return false and continue with next action()ineract with movement
            return false;
            
        }
        
        //used to avoid defect when one raycastable object placed near another and when player hover cursor incorrect cursor appears. 
        //ex. in case if pick up object placed near enemy and we want to hit enemy but instead we see pickup option
        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }
        
        private bool InteractWithComponent()
        {
            //get all objects on current ray way and check into them
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        //set cursor depend of the action. combat or pickup
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            //if we  didn´t find the target to interact with then we return false and continue with next action()
            return false;
            
        }

        //if we hover mouse on the object ouside the scene, then do nothing.
        //move only to the place where player could go
        private bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;
                
                //when left mouse button is clicked or held -> move to the point and stop or move continuosly
                if (Input.GetMouseButton(0))
                {
                    //player is moving with normal speed. so speed for the player multiply on 1f
                    mover.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                //yes, i am interacted with movement(even hovering that place where want to move)
                return true;
            }
            //if we  didn´t find the target to interact with then we return false and continue with next action().(do nothing in this case)
            return false;
        }
        
        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(
                hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            return true;
        }

        
        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            } return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            Ray ray;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return ray;
        }
    }
}

using UnityEngine;

namespace Core
{
   /*use this class to avoid looping dependencies(movement of combat and in the same time combat from movement)
    * For this use StartAction func and send to it action
    * in this case attack action from fighter script
    * and StartMove action from Mover script
    **/
    public class ActionScheduler : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private IAction currentAction;

        //cancel previous action before start next action
        public void StartAction(IAction action)
        {
            //if action previos action already cancelled and we continue to do same new action then do nothig
            if(currentAction == action) return;
            //cancel previous action only if it is not the first action
            if (currentAction != null)
            {
                currentAction.CancelAction();
            }
            currentAction = action;
        }
        
        //cancel any action
        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}

using UnityEngine;

namespace Core
{
    public class FollowCamera : MonoBehaviour
    {
        //target=player position
        [SerializeField] private Transform playerPosition;
        

        // Start is called before the first frame update
        void Start()
        {
        }

        /*late update is executing in unity after "normal" update. to exclude problems when camera is going before player,
    update camera position in LateUpdate method*/
        private void LateUpdate()
        {
            //position of the object in which this script is attached = target position(in this case player position)
            transform.position = playerPosition.position;
        }
    }
}
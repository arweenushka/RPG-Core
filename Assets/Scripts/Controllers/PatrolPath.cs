using UnityEngine;

namespace Controllers
{
    public class PatrolPath : MonoBehaviour
    {
        private const float WAYPOINT_RADIUS = 0.3f;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        //draw patrol path of the enemy on the scene
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), WAYPOINT_RADIUS);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
                
            }
        }

        //get next waypoint
        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        //get position of waypoint 
        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}

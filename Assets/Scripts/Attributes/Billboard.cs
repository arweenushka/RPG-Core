using UnityEngine;

namespace Attributes
{
    //used to follow enemy health bar for the enemy itself and looking at player always (World Space)
    public class Billboard : MonoBehaviour
    {
        public Transform cam;


        // Update is called once per frame
        void LateUpdate()
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}

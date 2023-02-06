using Combat;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Controllers
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController callingController);
    }
}
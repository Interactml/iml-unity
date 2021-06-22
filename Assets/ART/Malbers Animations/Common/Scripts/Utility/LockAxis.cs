using UnityEngine;

public class LockAxis : MonoBehaviour
{
    public bool LockX = true;
    public bool LockY = false;
    public bool LockZ = false;

    void Update()
    {
        Vector3 pos = transform.position;

        if (LockX) pos.x = 0;
        if (LockY) pos.y = 0;
        if (LockZ) pos.z = 0; 

        transform.position = pos;
    }
}

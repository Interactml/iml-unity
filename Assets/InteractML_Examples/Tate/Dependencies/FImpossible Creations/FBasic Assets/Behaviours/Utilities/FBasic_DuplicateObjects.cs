using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    /// FM: Class to duplicate game objects with offsetted positions
    /// </summary>
    public class FBasic_DuplicateObjects : MonoBehaviour
    {
        public enum FEDuplicateDirection
        {
            GoIterative,
            GoFromCenter
        }

        public enum FEDuplicateOrigin
        {
            FromToDuplicate,
            FromComponent
        }

        [Tooltip("Put here object which you want duplicate")]
        public GameObject ToDuplicate;

        [Tooltip("How many copies in which axis")]
        public Vector3 DuplicatesCount = new Vector3(3, 1, 3);
        [Tooltip("How far from each other should be created copies")]
        public Vector3 Offsets = new Vector3(3f, 0f, 3f);
        public Vector3 Randomize = new Vector3(0f, 0f, 0f);
        public Vector3 RandomRotate = new Vector3(0f, 0f, 0f);
        public Vector3 RandomScale= new Vector3(0f, 0f, 0f);
        public int Seed = 0;

        [Tooltip("If you want raycast from up and put objects for example on terrain")]
        public bool PlaceOnGround = false;
        [Tooltip("Duplicates will be created when entered playmode")]
        public bool DuplicateAtStart = false;

        public float GizmosSize = 1f;

        public FEDuplicateDirection DuplicationType = FEDuplicateDirection.GoIterative;
        public FEDuplicateOrigin DuplicationOrigin = FEDuplicateOrigin.FromToDuplicate;

        private void Start()
        {
            if (DuplicateAtStart) Duplicate();
        }

        void Reset()
        {
            Seed = Random.Range(-int.MaxValue + 1, int.MaxValue - 1);
        }

        public void Duplicate()
        {
            if (ToDuplicate == null) return;
            Random.InitState(Seed);

            Vector3 originPosition;
            if (DuplicationOrigin == FEDuplicateOrigin.FromComponent) originPosition = transform.position; else originPosition = ToDuplicate.transform.position;


            if (DuplicationType == FEDuplicateDirection.GoIterative)
            {
                // Going through right number of elements for copying
                for (int x = 0; x < DuplicatesCount.x; x++)
                    for (int y = 0; y < DuplicatesCount.y; y++)
                        for (int z = 0; z < DuplicatesCount.z; z++)
                        {
                            if (DuplicationOrigin == FEDuplicateOrigin.FromToDuplicate)
                                if (x == 0 && y == 0 && z == 0) continue;

                            // Calculating target position for object and creating new instance in world
                            Vector3 offset = originPosition;
                            offset.x += x * Offsets.x;
                            offset.y += y * Offsets.y;
                            offset.z += z * Offsets.z;

                            GameObject newObject = GameObject.Instantiate(ToDuplicate);
                            newObject.transform.position = offset + GetRandomVector();
                            newObject.transform.rotation *= Quaternion.Euler(Random.Range(-RandomRotate.x, RandomRotate.x), Random.Range(-RandomRotate.y, RandomRotate.y), Random.Range(-RandomRotate.z, RandomRotate.z));

                            Vector3 scale = newObject.transform.localScale + new Vector3(Random.Range(-RandomScale.x, RandomScale.x), Random.Range(-RandomScale.y, RandomScale.y), Random.Range(-RandomScale.z, RandomScale.z));
                            newObject.transform.localScale = scale;

                            // Raycasting for putting objects on detected ground (also can place on top of roof if there are)
                            if (PlaceOnGround)
                            {
                                RaycastHit hit;
                                Physics.Raycast(newObject.transform.position + Vector3.up * 100f, Vector3.down, out hit, 200f);
                                if (hit.transform)
                                {
                                    newObject.transform.position = hit.point;
                                }
                            }
                        }
            }
            else if (DuplicationType == FEDuplicateDirection.GoFromCenter)
            {
                for (int x = 0; x < DuplicatesCount.x; x++)
                    for (int y = 0; y < DuplicatesCount.y; y++)
                        for (int z = 0; z < DuplicatesCount.z; z++)
                        {
                            float xSign = 1f;
                            float ySign = 1f;
                            float zSign = 1f;

                            if (x % 2 == 1) xSign = -1f;
                            if (y % 2 == 1) ySign = -1f;
                            if (z % 2 == 1) zSign = -1f;

                            Vector3 val = new Vector3(x, y, z);

                            if (x == 0) val.x = 0.5f;
                            if (y == 0) val.y = 0.5f;
                            if (z == 0) val.z = 0.5f;

                            Vector3 offset = originPosition;
                            offset.x += val.x * Offsets.x * xSign;
                            offset.y += val.y * Offsets.y * ySign;
                            offset.z += val.z * Offsets.z * zSign;

                            GameObject newObject = GameObject.Instantiate(ToDuplicate);
                            newObject.transform.position = offset + GetRandomVector();

                            newObject.transform.rotation *= Quaternion.Euler(Random.Range(-RandomRotate.x, RandomRotate.x), Random.Range(-RandomRotate.y, RandomRotate.y), Random.Range(-RandomRotate.z, RandomRotate.z));

                            Vector3 scale = newObject.transform.localScale + new Vector3(Random.Range(-RandomScale.x, RandomScale.x), Random.Range(-RandomScale.y, RandomScale.y), Random.Range(-RandomScale.z, RandomScale.z));
                            newObject.transform.localScale = scale;

                            if (PlaceOnGround)
                            {
                                RaycastHit hit;
                                Physics.Raycast(newObject.transform.position + Vector3.up * 100f, Vector3.down, out hit, 200f);
                                if (hit.transform)
                                {
                                    newObject.transform.position = hit.point;
                                }
                            }
                        }
            }
        }

        private void OnDrawGizmos()
        {
            if (ToDuplicate == null) return;
            Random.InitState(Seed);

            Vector3 originPosition;

            if (DuplicationOrigin == FEDuplicateOrigin.FromComponent) originPosition = transform.position; else originPosition = ToDuplicate.transform.position;

            Gizmos.color = new Color(0.2f, 0.7f, 0.2f, 0.6f);

            if (DuplicationType == FEDuplicateDirection.GoIterative)
            {
                // Drawing cubes to visualize how many and where will be created copies of target object
                for (int x = 0; x < DuplicatesCount.x; x++)
                    for (int y = 0; y < DuplicatesCount.y; y++)
                        for (int z = 0; z < DuplicatesCount.z; z++)
                        {
                            Vector3 offset = originPosition;
                            offset.x += x * Offsets.x;
                            offset.y += y * Offsets.y;
                            offset.z += z * Offsets.z;

                            Gizmos.DrawCube(offset + GetRandomVector(), Vector3.one * 0.25f * GizmosSize);
                        }
            }
            else if (DuplicationType == FEDuplicateDirection.GoFromCenter)
            {
                for (int x = 0; x < DuplicatesCount.x; x++)
                    for (int y = 0; y < DuplicatesCount.y; y++)
                        for (int z = 0; z < DuplicatesCount.z; z++)
                        {
                            float xSign = 1f;
                            float ySign = 1f;
                            float zSign = 1f;

                            if (x % 2 == 1) xSign = -1f;
                            if (y % 2 == 1) ySign = -1f;
                            if (z % 2 == 1) zSign = -1f;

                            Vector3 val = new Vector3(x, y, z);

                            if (x == 0) val.x = 0.5f;
                            if (y == 0) val.y = 0.5f;
                            if (z == 0) val.z = 0.5f;

                            Vector3 offset = originPosition;
                            offset.x += val.x * Offsets.x * xSign;
                            offset.y += val.y * Offsets.y * ySign;
                            offset.z += val.z * Offsets.z * zSign;

                            Gizmos.DrawCube(offset + GetRandomVector(), Vector3.one * 0.25f * GizmosSize);
                        }
            }

        }

        private Vector3 GetRandomVector()
        {
            if (Randomize == Vector3.zero) return Randomize;
            return new Vector3(Random.Range(-Randomize.x, Randomize.x), Random.Range(-Randomize.y, Randomize.y), Random.Range(-Randomize.z, Randomize.z));
        }

#if UNITY_EDITOR

        /// <summary>
        /// FM: Editor class for duplicator to put here button for easy access to duplicating from editor level
        /// </summary>
        [UnityEditor.CanEditMultipleObjects]
        [UnityEditor.CustomEditor(typeof(FBasic_DuplicateObjects))]
        public class FBasic_DuplicateObjectsEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                FBasic_DuplicateObjects targetScript = (FBasic_DuplicateObjects)target;
                DrawDefaultInspector();

                GUILayout.Space(10f);

                if (GUILayout.Button("Duplicate")) targetScript.Duplicate();
            }
        }
#endif

    }
}

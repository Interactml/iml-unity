using UnityEngine;
using System.Collections.Generic;
using System;

namespace MalbersAnimations.Utilities
{

    public class ActiveMeshes : MonoBehaviour
    {
        [SerializeField]
        public List<ActiveSMesh> Meshes = new List<ActiveSMesh>();
        [HideInInspector]
        [SerializeField]
        public bool showMeshesList = true;


        /// <summary>
        /// All Active Meshes Index stored on a string separated by a space ' '
        /// </summary>
        public string AllIndex
        {
            set
            {
                string[] getIndex = value.Split(' ');

                for (int i = 0; i < Meshes.Count; i++)
                {
                    if (getIndex.Length > i)
                    {
                        int index;

                        if (int.TryParse(getIndex[i], out index))
                        {
                            if (index == -1) continue;

                            Meshes[i].ChangeMesh(index);
                        }
                    }
                }
            }

            get
            {
                string AllIndex = "";

                for (int i = 0; i < Meshes.Count; i++)
                {
                    AllIndex += Meshes[i].Current.ToString() + " ";
                }

                AllIndex.Remove(AllIndex.Length - 1);   //Remove the last space }
                return AllIndex;
            }
        }


        public bool random;
        private void Awake()
        {
            if (random)
            {
                foreach (var mat in Meshes)
                {
                    mat.ChangeMesh(UnityEngine.Random.Range(0, mat.meshes.Length));
                }
            }
        }


        /// <summary>
        /// Set All ActiveMeshes Index from the Meshes list.
        /// </summary>
        public void SetActiveMeshesIndex(int[] MeshesIndex)
        {
            if (MeshesIndex.Length != Meshes.Count)
            {
                Debug.LogError("Meshes Index array Lenghts don't match");
                return;
            }

            for (int i = 0; i < MeshesIndex.Length; i++)
            {
                Meshes[i].ChangeMesh(MeshesIndex[i]);
            }
        }


        public virtual void ChangeMesh(int index)
        {
            if (Meshes.Count > index)
            {
                Meshes[index].ChangeMesh();
            }
        }

        public virtual void ChangeMesh(int indexList, int IndexMesh)
        {
            if (indexList < 0) indexList = 0;
            indexList = indexList % Meshes.Count;

            if (Meshes[indexList] != null) Meshes[indexList].ChangeMesh(IndexMesh);
        }


        /// <summary>
        /// Change to next mesh using the name
        /// </summary>
        public virtual void ChangeMesh(string name, bool next)
        {
            ActiveSMesh mesh = Meshes.Find(item => item.Name == name);

            if (mesh != null)
            {
                mesh.ChangeMesh(next);
            }
        }

        public virtual void ChangeMesh(string name)
        {
            ChangeMesh(name, true);
        }

        public virtual void ChangeMesh(string name, int CurrentIndex)
        {
            ActiveSMesh mesh = Meshes.Find(item => item.Name == name);

            if (mesh != null)
            {
                mesh.ChangeMesh(CurrentIndex);
            }
        }

        public virtual void ChangeMesh(int index, bool next)
        {
            Meshes[index].ChangeMesh(next);
        }

        /// <summary>
        /// Toggle all meshes on the list
        /// </summary>
        public virtual void ChangeMesh(bool next = true)
        {
            foreach (var mesh in Meshes)
            {
                mesh.ChangeMesh(next);
            }
        }

        /// <summary>
        /// Get the Active mesh by their name
        /// </summary>
        public virtual ActiveSMesh GetActiveMesh(string name)
        {
            if (Meshes.Count == 0) return null;

            return Meshes.Find(item => item.Name == name);
        }

        /// <summary>
        /// Get the Active Mesh by their Index
        /// </summary>
        public virtual ActiveSMesh GetActiveMesh(int index)
        {
            if (Meshes.Count == 0) return null;

            if (index >= Meshes.Count) index = 0;
            if (index < 0) index = Meshes.Count - 1;

            return Meshes[index];
        }
    }


    [Serializable]
    public class ActiveSMesh
    {
        [HideInInspector]
        public string Name = "NameHere";
        public Transform[] meshes;
        [HideInInspector]

        [SerializeField]
        public int Current;

      

        [Space]
        [Header("Invoked when the Active mesh is changed")]
        public Events.TransformEvent OnActiveMesh = new Events.TransformEvent();

 


        /// <summary>
        /// Change mesh to the Next/Before
        /// </summary>
        public virtual void ChangeMesh(bool next = true)
        {
            if (next)
                Current++;
            else
                Current--;

            if (Current >= meshes.Length) Current = 0;
            if (Current < 0) Current = meshes.Length - 1;

            foreach (var item in meshes)
            {
                if (item) item.gameObject.SetActive(false);
            }

            if (meshes[Current])
            {
                meshes[Current].gameObject.SetActive(true);
                OnActiveMesh.Invoke(meshes[Current]);
            }
        }

        /// <summary>
        /// Returns the Active Transform
        /// </summary>
        public virtual Transform GetCurrentActiveMesh()
        {
            return meshes[Current];
        }

        /// <summary>
        /// Set a mesh by Index
        /// </summary>
        public virtual void ChangeMesh(int Index)
        {
            Current = Index - 1;
            ChangeMesh();
        }

        public void Set_by_BinaryIndex(int binaryCurrent)
        {
            int current = 0; 

            for (int i = 0; i < meshes.Length; i++)
            {
                if (MalbersTools.IsBitActive(binaryCurrent, i))
                {
                    current = i;   //Find the first active bit and use it as current
                    break;
                }
            }
            ChangeMesh(current);
        }
    }
}
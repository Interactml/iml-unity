using UnityEngine;
using System.Collections.Generic;
using MalbersAnimations.Events;
using System;

namespace MalbersAnimations.Utilities
{
    /// <summary>
    /// This Mono is used to change Materials on any Mesh Renderer using a list of Materials Items
    /// </summary>
    public class MaterialChanger : MonoBehaviour
    {
        [SerializeField]
        public List<MaterialItem> materialList = new List<MaterialItem>();
        [HideInInspector]
        [SerializeField]
        public bool showMeshesList = true;

        public bool random;

        MaterialItem Active;

        /// <summary>
        /// All Material Changer Index Stored on a string separated by a space ' '
        /// </summary>
        public string AllIndex
        {
            set
            {
                string[] getIndex = value.Split(' ');

                for (int i = 0; i < materialList.Count; i++)
                {
                    if (getIndex.Length > i)
                    {
                        int index;

                        if (int.TryParse(getIndex[i], out index))
                        {
                            if (index == -1) continue;

                            materialList[i].ChangeMaterial(index);
                        }
                    }
                }
            }

            get
            {
                string AllIndex = "";

                for (int i = 0; i < materialList.Count; i++)
                {
                    AllIndex += materialList[i].current.ToString() + " ";
                }

                AllIndex.Remove(AllIndex.Length - 1);   //Remove the last space }
                return AllIndex;
            }
        }



        private void Awake()
        {
            foreach (var mat in materialList)
            {
                if (mat.Linked && mat.Master >= 0 && mat.Master < materialList.Count)   //If the master material item is in range
                {
                    materialList[mat.Master].OnMaterialChanged.AddListener(mat.ChangeMaterial);         //Used for linked materials
                }
            }

            if (random) Randomize();
        }

        public virtual void Randomize()
        {
            foreach (var mat in materialList)
            {
                mat.ChangeMaterial(UnityEngine.Random.Range(0, mat.materials.Length));
            }
        }


        /// <summary>
        /// Swap to the next or previous material on each Material Item;
        /// </summary>
        public virtual void SetAllMaterials(bool Next = true)
        {
            foreach (var materialItem in materialList)
            {
                materialItem.ChangeMaterial(Next);
            }
        }

        /// <summary>
        /// Set all the MaterialItems on the List a specific Material using an Index
        /// </summary>
        /// <param name="index">the index on the Materials[], for each Material Item</param>
        public virtual void SetAllMaterials(int index)
        {
            foreach (var mat in materialList)
            {
                mat.ChangeMaterial(index);
            }
        }

        /// <summary>
        /// Set a Material from the List of material inside the materialList... 
        /// </summary>
        /// <param name="indexList">index of the Material List</param>
        /// <param name="indexCurrent">index a material on the MaterialList</param>
        public virtual void SetMaterial(int indexList, int indexCurrent)
        {
            if (indexList < 0) indexList = 0;
            indexList = indexList % materialList.Count;

            if (materialList[indexList] != null)
                materialList[indexList].ChangeMaterial(indexCurrent);
        }

        /// <summary>
        ///  Set a Material from the List of material inside the materialList... 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="next"></param>
        public virtual void SetMaterial(int index, bool next = true)
        {
            if (index < 0) index = 0;
            index = index % materialList.Count;

            if (materialList[index] != null)
            {
                materialList[index].ChangeMaterial(next);
            }
        }

        public virtual void SetMaterial(string name, int Index)
        {
            MaterialItem materialItem = materialList.Find(item => item.Name == name);

            if (materialItem != null)
            {
                materialItem.ChangeMaterial(Index);
            }
            else
            {
                Debug.LogWarning("No material Item Found with the name: " + name);
            }
        }

        public virtual void SetMaterial(string name, bool next = true)
        {
            MaterialItem materialItem = materialList.Find(item => item.Name == name);

            if (materialItem != null)
            {
                materialItem.ChangeMaterial(next);
            }
            else
            {
                Debug.LogWarning("No material Item Found with the name: " + name);
            }
        }

        /// <summary>
        /// Set all the MaterialItems on the List an External Material
        /// </summary>
        /// <param name="mat"></param>
        public virtual void SetAllMaterials(Material mat)
        {
            foreach (var MaterialItem in materialList)
            {
                MaterialItem.ChangeMaterial(mat);
            }
        }


        /// <summary>
        /// Swap to the Next material on a specific Material Item on the List using index
        /// </summary>
        /// <param name="index">index on the Material Item on the material list</param>
        public virtual void NextMaterialItem(int index)
        {
            if (index < 0) index = 0;
            index = index % materialList.Count;

            materialList[index].NextMaterial();
        }

        /// <summary>
        /// Swap to the Next material on a specific Material Item on the List using the Name
        /// </summary>
        /// <param name="name">the Name used for the MaterialItem</param>
        public virtual void NextMaterialItem(string name)
        {
            MaterialItem mat = materialList.Find(item => item.Name.ToUpper() == name.ToUpper());

            if (mat != null) mat.NextMaterial();
        }

        /// <summary>
        /// Returns the Current Index on the material list using the index of the slot
        /// </summary>
        public virtual int CurrentMaterialIndex(int index)
        {
            return materialList[index].current;
        }

        /// <summary>
        /// Returns the Current index of the material list slot using the index of the slot
        /// </summary>
        public virtual int CurrentMaterialIndex(string name)
        {
            int index = materialList.FindIndex(item => item.Name == name);
            return materialList[index].current;
        }

    }

    [System.Serializable]
    public class MaterialItem
    {
        [SerializeField]
        [HideInInspector]
        public string Name;                 //The name for the Material to change
        public Renderer mesh;               //The mesh renderer to use for the materials
        public Material[] materials;        //The list of the Materials
        public bool Linked = false;
        [Range(0,100)]
        public int Master = 0;

        /// <summary>
        /// Current Index Material Item
        /// </summary>
        [HideInInspector]  [SerializeField] public int current = 0;
      
       
        public bool HasLODs;
        public Renderer[] LODs;

        /// <summary>
        /// Material ID
        /// </summary>
        [Tooltip("Material ID")]
        public int indexM = 0;

        public IntEvent OnMaterialChanged = new IntEvent();
        
        #region Constructors
        public MaterialItem()
        {
            Name = "NameHere";
            mesh = null;
            materials = new Material[0];
        }

        public MaterialItem(MeshRenderer MR)
        {
            Name = "NameHere";
            mesh = MR;
            materials = new Material[0];
        }

        public MaterialItem(string name, MeshRenderer MR, Material[] mats)
        {
            Name = name;
            mesh = MR;
            materials = mats;
        }

        public MaterialItem(string name, MeshRenderer MR)
        {
            Name = name;
            mesh = MR;
            materials = new Material[0];
        }
        #endregion

        /// <summary>
        /// Changes to the next material on the list..(Same as NextMaterial)
        /// </summary>
        public virtual void ChangeMaterial()
        {
            current++;
            if (current < 0) current = 0;
            current = current % materials.Length;

            Material[] currentMaterial = mesh.sharedMaterials;

            if (materials[current] != null)
            {
                currentMaterial[indexM] = materials[current];
                mesh.sharedMaterials = currentMaterial;
                ChangeLOD(current);
                OnMaterialChanged.Invoke(current);
            }
            else
            {
                Debug.LogWarning("The Material on the Slot: " + current + " is empty");
            }
        }


        public virtual void Set_by_BinaryIndex(int binaryCurrent)
        {
            int current = 0;

            for (int i = 0; i < materials.Length; i++)
            {
                if (MalbersTools.IsBitActive(binaryCurrent, i))
                {
                    current = i;        //find the first active bit and get the current
                    break;
                }
            }
            ChangeMaterial(current);
        }


        internal void ChangeLOD(int index)
        {
            if (!HasLODs) return;

            foreach (var mesh in LODs)
            {
                if (mesh == null) break;

                Material[] currentMaterial = mesh.sharedMaterials;
                currentMaterial[indexM] = materials[current];
                if (materials[current] != null)
                    mesh.sharedMaterials = currentMaterial;
            }
        }

        internal void ChangeLOD(Material mat)
        {
            if (!HasLODs) return;

            Material[] currentMaterial = mesh.sharedMaterials;
            currentMaterial[indexM] = mat;

            foreach (var mesh in LODs)
            {
                mesh.sharedMaterials = currentMaterial;
            }
        }

        /// <summary>
        /// Changes to the Next material on the list.(Same as ChangeMaterial)
        /// </summary>
        public virtual void NextMaterial()
        {
            ChangeMaterial();
        }

        /// <summary>
        /// Used for Change a specific material on the list using and Index.
        /// </summary>
        /// <param name="index">Index for the Material Array</param>
        public virtual void ChangeMaterial(int index)
        {
            if (index < 0) index = 0;
            index = index % materials.Length;

            if (materials[index] != null)
            {
                Material[] currentMaterial = mesh.sharedMaterials;

                if (currentMaterial.Length - 1 < indexM)
                {
                    Debug.LogWarning("The Meshes on the "+Name+" Material Item, does not have " + (indexM+1) + " Materials, please change the ID parameter to value lower than "+ currentMaterial.Length);
                    return;
                }

                currentMaterial[indexM] = materials[index];

                mesh.sharedMaterials = currentMaterial;
                current = index;
                ChangeLOD(index);
                OnMaterialChanged.Invoke(current);
            }
            else
            {
                Debug.LogWarning("The material on the Slot: " + index + "  is empty");
            }
        }

        /// <summary>
        /// Changes to the previous material on the list.
        /// </summary>
        public virtual void PreviousMaterial()
        {
            current--;
            if (current < 0) current = materials.Length - 1;

            if (materials[current] != null)
            {
                Material[] currentMaterial = mesh.sharedMaterials;
                currentMaterial[indexM] = materials[current];

                mesh.sharedMaterials = currentMaterial;
                ChangeLOD(current);
                OnMaterialChanged.Invoke(current);
            }
            else
            {
                Debug.LogWarning("The Material on the Slot: " + current + " is empty");
            }
        }

        /// <summary>
        /// Changes to a specific External material
        /// </summary>
        public virtual void ChangeMaterial(Material mat)
        {
            Material[] currentMaterial = mesh.sharedMaterials;
            currentMaterial[indexM] = mat;

            mesh.sharedMaterials = currentMaterial;
            ChangeLOD(mat);
        }

        /// <summary>
        /// Changes to the Next or Previous material on the list
        /// </summary>
        /// <param name="Next">true: Next, false: Previous</param>
        public virtual void ChangeMaterial(bool Next = true)
        {
            if (Next)
                NextMaterial();
            else
                PreviousMaterial();
        }

    }
}


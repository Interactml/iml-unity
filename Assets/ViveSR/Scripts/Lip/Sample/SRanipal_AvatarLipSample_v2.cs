//========= Copyright 2019, HTC Corporation. All rights reserved. ===========
using System.Collections.Generic;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Lip
        {
            public class SRanipal_AvatarLipSample_v2 : MonoBehaviour
            {
                [SerializeField] private List<LipShapeTable_v2> LipShapeTables;

                public bool NeededToGetData = true;
                private Dictionary<LipShape_v2, float> LipWeightings;

                private void Start()
                {
                    if (!SRanipal_Lip_Framework.Instance.EnableLip)
                    {
                        enabled = false;
                        return;
                    }
                    SetLipShapeTables(LipShapeTables);
                }

                private void Update()
                {
                    if (SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING) return;

                    if (NeededToGetData)
                    {
                        SRanipal_Lip_v2.GetLipWeightings(out LipWeightings);
                        UpdateLipShapes(LipWeightings);
                    }
                }

                public void SetLipShapeTables(List<LipShapeTable_v2> lipShapeTables)
                {
                    bool valid = true;
                    if (lipShapeTables == null)
                    {
                        valid = false;
                    }
                    else
                    {
                        for (int table = 0; table < lipShapeTables.Count; ++table)
                        {
                            if (lipShapeTables[table].skinnedMeshRenderer == null)
                            {
                                valid = false;
                                break;
                            }
                            for (int shape = 0; shape < lipShapeTables[table].lipShapes.Length; ++shape)
                            {
                                LipShape_v2 lipShape = lipShapeTables[table].lipShapes[shape];
                                if (lipShape > LipShape_v2.Max || lipShape < 0)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (valid)
                        LipShapeTables = lipShapeTables;
                }

                public void UpdateLipShapes(Dictionary<LipShape_v2, float> lipWeightings)
                {
                    foreach (var table in LipShapeTables)
                        RenderModelLipShape(table, lipWeightings);
                }

                private void RenderModelLipShape(LipShapeTable_v2 lipShapeTable, Dictionary<LipShape_v2, float> weighting)
                {
                    for (int i = 0; i < lipShapeTable.lipShapes.Length; i++)
                    {
                        int targetIndex = (int)lipShapeTable.lipShapes[i];
                        if (targetIndex > (int)LipShape_v2.Max || targetIndex < 0) continue;
                        lipShapeTable.skinnedMeshRenderer.SetBlendShapeWeight(i, weighting[(LipShape_v2)targetIndex] * 100);
                    }
                }
            }
        }
    }
}
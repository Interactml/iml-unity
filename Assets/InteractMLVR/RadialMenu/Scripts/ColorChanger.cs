using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Material one;
    public Material two;

    public void SetPink()
    {
        SetMaterial(one);
    }

    public void SetGrey()
    {
        SetMaterial(two);
    }

    private void SetMaterial(Material newMaterial)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = newMaterial;
    }
}

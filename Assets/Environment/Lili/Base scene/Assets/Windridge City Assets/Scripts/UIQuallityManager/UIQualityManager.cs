using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class UIQualityManager : MonoBehaviour
{
    public Canvas canvas;

    public PostProcessVolume postProcessVolume;
    public PostProcessProfile[] volumes;


    public Dropdown dpPostProcessing;
    public Dropdown dpQuality;

    public Terrain terrain;

    public List<NameValue> detailDensities;
    public Dropdown dpDetailDensity;

    public List<NameValue> shapeQualities;
    public Dropdown dpShapeQuality;

    private void Start()
    {
        List<string> qualityOptions = new List<string>();
        qualityOptions.AddRange(QualitySettings.names);
        dpQuality.AddOptions(qualityOptions);
        dpQuality.value = dpQuality.options.Count - 1;

        List<string> postProcessing = new List<string>();
        foreach (var item in volumes)
        {
            postProcessing.Add(item.name);
        }
        dpPostProcessing.AddOptions(postProcessing);



        List<string> detailDensitiesList = new List<string>();
        foreach (var item in detailDensities)
        {
            detailDensitiesList.Add(item.name);
        }
        dpDetailDensity.AddOptions(detailDensitiesList);


        List<string> shapeQualitiesList = new List<string>();
        foreach (var item in shapeQualities)
        {
            shapeQualitiesList.Add(item.name);
        }
        dpShapeQuality.AddOptions(shapeQualitiesList);


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            canvas.enabled = !canvas.enabled;
        }
    }

    public void SetPostProcessing(int id)
    {
        postProcessVolume.profile = volumes[id];

    }

    public void SetQuality(int id)
    {
        QualitySettings.SetQualityLevel(id);

    }

    public void SetDetailDensity(int id)
    {
        terrain.detailObjectDensity = detailDensities[id].value;
        terrain.Flush();
    }

    public void SetShapeQuality(int id)
    {
        terrain.heightmapPixelError = shapeQualities[id].value;
        terrain.Flush();
    }

}

[Serializable]
public class NameValue
{
    public float value;
    public string name;

}



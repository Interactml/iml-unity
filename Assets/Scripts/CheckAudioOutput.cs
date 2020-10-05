using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(AudioSource))]
public class CheckAudioOutput : MonoBehaviour
{
    public AudioListener ActiveAudioListener;
    public int SpectrumSize;
    public float CurrentSpectrumAvg;

    private void Awake()
    {
        ActiveAudioListener = GameObject.FindObjectOfType<AudioListener>();
        SpectrumSize = 1024;
    }

    void Update()
    {
        float[] spectrum = new float[SpectrumSize];

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        CurrentSpectrumAvg = (float)Math.Round(spectrum.Sum() / spectrum.Length, 4);
        float[] spectrum_ = new float[SpectrumSize];
    }
}
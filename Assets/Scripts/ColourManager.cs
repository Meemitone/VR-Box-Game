using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourManager : MonoBehaviour
{
    public Material[] CurrentMaterials;
    public Material[] StoredMaterials;

    public float PlayerHue = 270;
    public float InteractHue = 86;
    public float BackgroundHue = 0;
    public float valueChange;

    public bool doColour = false;
    public bool blind = false;
    void Start()
    {
        for (int I = 0; I < CurrentMaterials.Length; I++)
        {
            CurrentMaterials[I].color = StoredMaterials[I].color;
        }
    }
    
    private void Update()
    {
        if(doColour)
        {
            colourBlind();
            doColour = false;
        }
    }

    public void colourBlind()
    {

        if (!blind)
        {
            blind = true;
            for (int I = 0; I < CurrentMaterials.Length; I++)
            {
                float h, s, v;
                Color.RGBToHSV(CurrentMaterials[I].color, out h, out s, out v); // sets the colours to blind

                h = BackgroundHue;
                if (I == 0)
                {
                    h = PlayerHue / 360f;
                    v += valueChange / 360f;
                }

                if (I == 1)
                {
                    h = InteractHue / 360f;
                    v += valueChange / 360f;
                }

                if (I > 1)
                {
                    s = 0;
                    v -= valueChange / 360f;
                }
                
                CurrentMaterials[I].color = Color.HSVToRGB(h,s,v);
            }
        }
        else
        {
            blind = false;
            for (int I = 0; I < CurrentMaterials.Length; I++)
            {
                CurrentMaterials[I].color = StoredMaterials[I].color; // resets the colours to normal
            }
        }
    }
}

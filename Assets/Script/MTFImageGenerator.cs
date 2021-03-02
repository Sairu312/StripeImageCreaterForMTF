using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class MTFImageGenerator : MonoBehaviour {

    public int resorution;
    public float wavelength;
    public float imageSize = 7168f;
    public float cpd = 1f;
    public float angle = 0;
    public Texture2D image;

    public bool save = false;

    public bool reload = false;
    public bool sin;
    public bool tilt;
    public bool toSize;
    public bool toCpd;
    public bool toWaveLength;
    public bool gray;

    public GameObject plane;

    // Use this for initialization
    void Start()
    {
        image = new Texture2D(resorution, resorution, TextureFormat.RGB24, false);
        if (!sin)SquareWave();
        else SinWave();
    }
	
	// Update is called once per frame
	void Update ()
    {

        if (save )
        {
            if (wavelength > 2)
            {
                MakePNG();
            }
            else Debug.Log("Dont Save");
            save = false;
        }
        if (reload)
        {
            if (!sin) SquareWave();
            else SinWave();
            reload = false;
            if (gray) Gray();
        }
        if (toSize)
        {
            imageSize =(float)resorution / (float)wavelength * 2000f * Mathf.Tan(Mathf.PI / 360)/cpd;
            toSize = false;
        }
        if (toCpd)
        {
            cpd = (float)resorution / (float)wavelength * 2000f * Mathf.Tan(Mathf.PI / 360) / imageSize;
            toCpd = false;
        }
        if(toWaveLength)
        {
            wavelength = (float)resorution / cpd * 2000f * Mathf.Tan(Mathf.PI / 360) / imageSize;
            toWaveLength = false;
        }
    }

    void SinWave()
    {
        if (!tilt)
        {
            float colorX = 0f;
            for (int y = 0; y < resorution; y++)
            {
                for (int x = 0; x < resorution; x++)
                {
                    colorX = Mathf.Sin(x * Mathf.PI * 2 /  wavelength)/2f+0.5f;
                    image.SetPixel(x, y, new Color(colorX, colorX, colorX));
                }
            }
        }
        else
        {
            for (int y = 0; y < resorution; y++)
            {
                for (int x = 0; x < resorution; x++)
                {
                    float color = 0.5f + Mathf.Sin((x - y * Mathf.Tan(angle * Mathf.PI / 180)) * Mathf.PI * 2 * Mathf.Cos(angle * Mathf.PI / 180) / wavelength) / 2f;
                    image.SetPixel(x, y, new Color(color, color, color));
                }
            }

        }
        image.Apply();
        plane.GetComponent<Renderer>().material.mainTexture = image;
        image.name = cpd.ToString() + "cpd" + imageSize.ToString() + "mmsin";

    }

    void SquareWave()
    {
        if (!tilt)
        {
            int colorX = 0;
            int colorY = 0;
            for (int y = 0; y < resorution; y++)
            {
                for (int x = 0; x < resorution; x++)
                {
                    if (x % wavelength > wavelength / 2) colorX = -1;
                    else colorX = 1;
                    if (y % wavelength > wavelength / 2) colorY = -1;
                    else colorY = 1;
                    if (colorX * colorY == 1) image.SetPixel(x, y, Color.black);
                    else image.SetPixel(x, y, Color.white);
                }
            }
        }
        else
        {
            for (int y = 0; y < resorution; y++)
            {
                for (int x = 0; x < resorution; x++)
                {
                    if ((x+y) % wavelength > wavelength / 2) image.SetPixel(x, y, Color.black);
                    else image.SetPixel(x, y, Color.white);
                }
            }
        }
        image.Apply();
        plane.GetComponent<Renderer>().material.mainTexture = image;
        image.name = cpd.ToString() + "cpd" + imageSize.ToString() + "mmsquare";
    }

    void Gray()
    {
        for(int y = 0; y < resorution; y++)
        {
            for(int x =0; x < resorution; x++)
            {
                image.SetPixel(x, y, Color.gray);
            }
        }
        image.Apply();
        plane.GetComponent<Renderer>().material.mainTexture = image;
        image.name = cpd.ToString() + "cpd" + imageSize.ToString() + "gray";
    }


    void MakePNG() {
        byte[] pngData = image.EncodeToPNG();
        string filePath = EditorUtility.SaveFilePanel("Save Image", "/Users/inukaisatoru/Desktop/LFDImage/空間周波数チャート/", image.name, "png");
        if(filePath.Length > 0)
        {
            File.WriteAllBytes(filePath, pngData);
        }
    }
}

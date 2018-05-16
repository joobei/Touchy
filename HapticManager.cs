/*
 * This file is part of Touchy (https://github.com/uhhhci/Touchy).
 * Copyright (c) 2015 Nicholas Katzakis
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, version 3.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class HapticManager : MonoBehaviour {

    /* Import Functions from DLL
     * There are other functions in the DLL
     * that are not yet imported here */

    [DllImport("Touchy")]
    public static extern bool init();

    [DllImport("Touchy")]
    public static extern void shutdown();

    [DllImport("Touchy")]
    public static extern void addSphere(double radius, double x, double y, double z);

    [DllImport("Touchy")]
    public static extern void getEEPosition(double [] position);

    [DllImport("Touchy")]
    public static extern void setSphereRadius(double radius);

    [DllImport("Touchy")]
    public static extern double getSphereRadius();

    [DllImport("Touchy")]
    public static extern int getButtonState();

    private GameObject hapticCursor;
    public GameObject targetSphere;

    // Use this for initialization
    void Start () {

        bool worked = init();
        if (worked) { print("Haptic Device Initialized."); }
        else { print("Haptic Initialization Error!"); }

        //Add a sphere to the world upon starting
        //at some point this should be refactored
        //to Find all game objects with sphere collider
        //and add them to the haptic space
        //but this also requires to changes to the Touchy plugin.
        addSphere(30f,0,0,0);

        //spawn a haptic cursor
        hapticCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hapticCursor.name = "EndEffector";
        hapticCursor.transform.localScale = Vector3.one * 0.003f;
    }

    private void Update()
    {
        //update end effector position
        double[] tempPosition = new double[3];
        getEEPosition(tempPosition);
        Vector3 tempVector;
        tempVector.x = (float)tempPosition[0]/1000;
        tempVector.y = (float)tempPosition[1]/1000;
        tempVector.z = (float)tempPosition[2]/1000;
        hapticCursor.transform.position = tempVector;

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            double currentRadius = getSphereRadius();
            setSphereRadius((double)(currentRadius - 10));
            Vector3 previousScale = targetSphere.transform.localScale;
            Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f);
            targetSphere.transform.localScale = previousScale - scaleChange;
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            double currentRadius = getSphereRadius();
            setSphereRadius((double)(currentRadius + 10));
            Vector3 previousScale = targetSphere.transform.localScale;
            Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f);
            targetSphere.transform.localScale = previousScale + scaleChange;
        }

        int buttons = getButtonState();
        if (buttons != 0)
        { 
            //Handle Button Presses
            //1 - 2 or both buttons 
            print(buttons);
        }
    }

    void OnApplicationQuit()
    {
        shutdown();   
    }


}

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
    public static extern void disableSphere();

    [DllImport("Touchy")]
    public static extern void enableSphere();

    [DllImport("Touchy")]
    public static extern void getEEPosition(double [] position);

    [DllImport("Touchy")]
    public static extern void setSphereRadius(double radius);

    [DllImport("Touchy")]
    public static extern double getSphereRadius();

    [DllImport("Touchy")]
    public static extern int getButtonState();

    public GameObject hapticCursor;
    public GameObject targetSphere;

    //Event for stylus button
    public delegate void StylusAction();
    public static event StylusAction OnStylusButton;

    [Tooltip("The desired sphere radius in mm")]
    public float radius;
    public float stylusButtonInterval;

    private float lastTime;

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
        Vector3 spherePos = targetSphere.transform.position;
        addSphere(radius, spherePos.x, spherePos.y, spherePos.z);

        //Apply the radius to the sphere in Unity
        float radiusMeters = radius * 0.001f; //convert form mm to m
        targetSphere.transform.localScale = Vector3.one * radiusMeters * 2;
    }

    private void Update()
    {
        //update end effector position
        double[] tempPosition = new double[3];
        getEEPosition(tempPosition);
        Vector3 tempVector;
        tempVector.x = (float)tempPosition[0]/1000; //convert from mm to m
        tempVector.y = (float)tempPosition[1]/1000;
        tempVector.z = (float)-tempPosition[2]/1000;
        hapticCursor.transform.position = tempVector;

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            double currentRadius = getSphereRadius();
            setSphereRadius((double)(currentRadius - 10));

            targetSphere.transform.localScale -= new Vector3(0.02f, 0.02f, 0.02f);
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            double currentRadius = getSphereRadius();
            setSphereRadius((double)(currentRadius + 10));

            targetSphere.transform.localScale += new Vector3(0.02f, 0.02f, 0.02f);
        }

        int stylusButtons = getButtonState();
        //Handle button presses (1, 2 or both buttons)
        if (stylusButtons != 0 && Time.time - lastTime > stylusButtonInterval)
        {
            if (OnStylusButton != null)
            {
                OnStylusButton();
            }
            lastTime = Time.time;
        }
    }

    void OnApplicationQuit()
    {
        shutdown();   
    }
}

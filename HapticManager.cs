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

    public GameObject hapticCursor;
    public GameObject targetSphere;

    //Event for stylus button
    public delegate void StylusAction(object sender, StylusButtonEventType stylusButtonEventType);
    public static event StylusAction OnStylusButton;

    public delegate void MouseAction();
    public static event MouseAction OnMouseButton;

    [Tooltip("The desired sphere radius in mm")]
    public float radius;
    public float stylusButtonInterval;


    public enum StylusButtonEventType
    {
        Grey_Up,
        Grey_Down,
        White_Up,
        White_Down,
        Both_Down,
        Both_to_Grey,
        Both_to_White,
        Both_to_None
    };

    private bool worked;
    private bool running = false;
    private bool[] stylusButtonStates = new bool[3];

    // Use this for initialization
    void Start () {

        worked = init();
        if (worked) { print("DLL Phantom Initialized."); running = true; }
        else { print("DLL Phantom Initialization Error!"); running = false; }

        //Add a sphere to the world upon starting
        //at some point this should be refactored
        //to Find all game objects with sphere collider
        //and add them to the haptic space
        //but this also requires to changes to the Touchy plugin.
        //Vector3 spherePos = targetSphere.transform.position;

        //startCenterCallback(radius, spherePos.x, spherePos.y, spherePos.z);
        //addSphere(radius, spherePos.x, spherePos.y, spherePos.z);

        //Apply the radius to the sphere in Unity
        float radiusMeters = radius * 0.001f; //convert form mm to m
        targetSphere.transform.localScale = Vector3.one * radiusMeters * 2;
    }

    private void Update()
    {
        if (running)
        {
            //update end effector position
            double[] tempPosition = new double[3];
            getEEPosition(tempPosition);
            Vector3 tempVector;
            tempVector.x = (float)-tempPosition[2] / 1000; //convert from mm to m
            tempVector.y = (float)tempPosition[1] / 1000;
            tempVector.z = (float)-tempPosition[0] / 1000;
            hapticCursor.transform.localPosition = tempVector;

            //Handle button presses (1, 2 or both buttons)
            print(getButtonState());
            handleStylusButtons(getButtonState());
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (OnMouseButton != null)
                OnMouseButton();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            stopCenterCallback();
            print("DLL stopCenterCallback");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            startCenterCallback(40, 0, 0, 0);
            print("DLL startCenterCallback");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            OnStylusButton(this, StylusButtonEventType.Grey_Down);
        }
    }

    void handleStylusButtons(int stylusCode)
    {
        //if (OnStylusButton != null)
        //{

            if (stylusButtonStates[0] /*Grey*/ == false && stylusCode == 1)
            {
                OnStylusButton(this,StylusButtonEventType.Grey_Down);
                stylusButtonStates[0] = true;
            }

            if (stylusButtonStates[0] /*Grey*/ == true && stylusCode == 0)
            {
                OnStylusButton(this, StylusButtonEventType.Grey_Up);
                stylusButtonStates[0] = false;
            }

            if (stylusButtonStates[1] /*White*/ == false && stylusCode == 2)
            {
                OnStylusButton(this, StylusButtonEventType.White_Down);
                stylusButtonStates[1] = true;
            }

            if (stylusButtonStates[1] /*White*/ == true && stylusCode == 0)
            {
                OnStylusButton(this, StylusButtonEventType.White_Up);
                stylusButtonStates[1] = false;
            }
        //}
    }

    void OnApplicationQuit()
    {
        if (worked)
        {
            running = false;
            shutdown();
            print("DLL Phantom Shutdown called");
        }
    }

    public void enableSphere()
    {
        startSphereCallback(radius, 0, 0, 0);
        print("DLL startSphereCallback");
    }

    public static void disableSphere()
    {
        stopSphereCallback();
        print("DLL stopSphereCallback");
    }

    /* Import Functions from DLL
    * There are other functions in the DLL
    * that are not yet imported here */

    [DllImport("Touchy")]
    public static extern bool init();
    [DllImport("Touchy")]
    public static extern void startCenterCallback(double radius, double x, double y, double z);
    [DllImport("Touchy")]
    public static extern void stopCenterCallback();
    [DllImport("Touchy")]
    public static extern void startSphereCallback(double radius, double x, double y, double z);
    [DllImport("Touchy")]
    public static extern void stopSphereCallback();
    [DllImport("Touchy")]
    public static extern void setSphereRadius(double radius);
    [DllImport("Touchy")]
    public static extern double getSphereRadius();
    [DllImport("Touchy")]
    public static extern void setSpherePosition(double x, double y, double z);
    [DllImport("Touchy")]
    public static extern void getEEPosition(double[] position);
    [DllImport("Touchy")]
    public static extern int getButtonState();
    [DllImport("Touchy")]
    public static extern void shutdown();
}

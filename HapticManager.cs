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

    private GameObject hapticCursor;

    // Use this for initialization
    void Start () {

        bool worked = init();
        if (worked) { print("It worked!"); }
        else { print("Haptic Initialization Error!"); }

        //Add a sphere to the world upon starting
        //at some point this should be refactored
        //to Find all game objects with sphere collider
        //and add them to the haptic space
        //but this also requires to changes to the Touchy plugin.
        addSphere(30f,0,0,0);

        //spawn a haptic cursor
        hapticCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hapticCursor.transform.localScale.Set(0.03f, 0.03f, 0.03f);
    }

    private void Update()
    {
        double[] tempPosition = new double[3];
        getEEPosition(tempPosition);
        Vector3 tempVector;
        tempVector.x = (float)tempPosition[0];
        tempVector.y = (float)tempPosition[1];
        tempVector.z = (float)tempPosition[2];

        hapticCursor.transform.position = tempVector;
    }

    void OnApplicationQuit()
    {
        shutdown();
    }

    //set sphere size in mm
    void scaleSphere(int size)
    {

    }
}

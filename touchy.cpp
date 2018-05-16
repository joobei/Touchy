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

#define dllexport __declspec(dllexport)

//#include <cstdio>
//#include <cassert>
//#include <conio.h>
#include "vector_math.h"
using namespace vmath;

#include "HD/hd.h" //part of 3DS OpenHaptics

//Global variable and handle declarations
HDSchedulerHandle hSphereCallback;
HHD hHD;

//Sphere globals (only one sphere for now)
double sphereRadius;
vec3<double> spherePosition;

/*******************************************************************************
 Generates an opposing force if the device attempts to penetrate the sphere.
 *******************************************************************************/
HDCallbackCode HDCALLBACK FrictionlessSphereCallback(void *data)
{

    
    // Stiffness, i.e. k value, of the sphere.  Higher stiffness results
    // in a harder surface.
    const double sphereStiffness = 1.f;
    
    hdBeginFrame(hdGetCurrentDevice());
    
    // Get the position of the device.
    vec3<double> position;
    hdGetDoublev(HD_CURRENT_POSITION, position);
    
    // Find the distance between the device and the center of the
    // sphere.
    vec3<double> newvec = (position - spherePosition);
    double distance = length(newvec);
    
    // If the user is within the sphere -- i.e. if the distance from the user to
    // the center of the sphere is less than the sphere radius -- then the user
    // is penetrating the sphere and a force should be commanded to repel him
    // towards the surface.
    if (distance < sphereRadius)
    {
        // Calculate the penetration distance.
        double penetrationDistance = sphereRadius - distance;
        
        // Create a unit vector in the direction of the force, this will always
        // be outward from the center of the sphere through the user's
        // position.
        vec3<double> forceDirection = (position - spherePosition) / distance;
        
        // Use F=kx to create a force vector that is away from the center of
        // the sphere and proportional to the penetration distance, and scsaled
        // by the object stiffness.
        // Hooke's law explicitly:
        double k = sphereStiffness;
        vec3<double> x = penetrationDistance*forceDirection;
        vec3<double> f = k*x;
        hdSetDoublev(HD_CURRENT_FORCE, f);
    }
    
    hdEndFrame(hdGetCurrentDevice());
    
    HDErrorInfo error;
    if (HD_DEVICE_ERROR(error = hdGetError()))
    {     
       if (error.errorCode == HD_SCHEDULER_FULL)
        {
         return HD_CALLBACK_DONE;
        }
    }
    
    return HD_CALLBACK_CONTINUE;
}

extern "C" dllexport bool init() {
    
    HDErrorInfo error;
    // Initialize the default haptic device.
    hHD = hdInitDevice(HD_DEFAULT_DEVICE);
    if (HD_DEVICE_ERROR(error = hdGetError()))
    {
        //printf("Failed to initialize haptic device");
        return(false);
    }

	//initialize default sphere attributes so that variables are not empty
	const double sphereRadius = 40.0;  //40 millimeters
	const vec3<double> spherePosition = vec3<double>(0, 0, 0);
    
    // Start the servo scheduler and enable forces.
    hdEnable(HD_FORCE_OUTPUT);
    hdStartScheduler();
    if (HD_DEVICE_ERROR(error = hdGetError()))
    {
        //printf("Failed to start scheduler");
        return(false);
    }
    
    return(true);
}

extern "C" dllexport void addSphere(double radius, double x, double y, double z) {
	sphereRadius = radius;
	spherePosition.x = x;
	spherePosition.y = y;
	spherePosition.z = z;

	// Application loop - schedule our call to the main callback.
    hSphereCallback = hdScheduleAsynchronous(FrictionlessSphereCallback, 0, HD_DEFAULT_SCHEDULER_PRIORITY);
}

extern "C" dllexport void setSphereRadius(double radius) 
{
	sphereRadius = radius;
}

extern "C" dllexport double getSphereRadius()
{
	return sphereRadius;
}


extern "C" dllexport void setSpherePosition(double x, double y, double z)
{
	spherePosition.x = x;
	spherePosition.y = y;
	spherePosition.z = z;
}

extern "C" dllexport void getEEPosition(double position[3])
{

    hdGetDoublev(HD_CURRENT_POSITION, position);
}
extern "C" dllexport int getButtonState()
{
	HDint buttonState;
	hdGetIntegerv(HD_CURRENT_BUTTONS, &buttonState);
	return buttonState;
}

extern "C" dllexport void shutdown() {
    // For cleanup, unschedule our callbacks and stop the servo loop.
    hdWaitForCompletion(hSphereCallback, HD_WAIT_CHECK_STATUS);
    hdStopScheduler();
    hdUnschedule(hSphereCallback);
    hdDisableDevice(hHD);
}

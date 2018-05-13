#define dllexport __declspec(dllexport)

#include <cstdio>
#include <cassert>
#include <conio.h>
#include "vector_math.h"
using namespace vmath;

#include "HD/hd.h" 

//Global variable and handle declarations
HDSchedulerHandle hSphereCallback;
HHD hHD;
int sphereRadius;



/*******************************************************************************
Haptic sphere callback.
The sphere is oriented at 0,0,0 with radius 40, and provides a repelling force
if the device attempts to penetrate through it.
*******************************************************************************/
HDCallbackCode HDCALLBACK FrictionlessSphereCallback(void *data)
{
	const double sphereRadius = 40.0;
	const vec3<double> spherePosition = vec3<double>(0, 0, 0);

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
		//hduPrintError(stderr, &error, "Error during main scheduler callback\n");
		printf("Error during main scheduler callback\n");

		/*if (hduIsSchedulerError(&error))
		{
			return HD_CALLBACK_DONE;
		}*/
	}

	return HD_CALLBACK_CONTINUE;
}

 extern "C" dllexport bool init() {

	HDErrorInfo error;
	// Initialize the default haptic device.
	hHD = hdInitDevice(HD_DEFAULT_DEVICE);
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		printf("Failed to initialize haptic device");
		return(false);
	}

	// Start the servo scheduler and enable forces.
	hdEnable(HD_FORCE_OUTPUT);
	hdStartScheduler();
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		printf("Failed to start scheduler");
		return(false);
	}
	
	return(true);
}

 extern "C" dllexport void addSphere() {
	// Application loop - schedule our call to the main callback.
	hSphereCallback = hdScheduleAsynchronous(FrictionlessSphereCallback, 0, HD_DEFAULT_SCHEDULER_PRIORITY);
}

 extern "C" dllexport void getPosition(double position[3])
{
	hdGetDoublev(HD_CURRENT_POSITION, position);
}

 extern "C" dllexport void shutdown() {
	// For cleanup, unschedule our callbacks and stop the servo loop.
	hdWaitForCompletion(hSphereCallback, HD_WAIT_CHECK_STATUS);
	hdStopScheduler();
	hdUnschedule(hSphereCallback);
	hdDisableDevice(hHD);
}
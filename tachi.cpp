#include <cstdio>
#include <cassert>
#include <conio.h>
#include "vector_math.h"
using namespace vmath;

#include "HD/hd.h" 


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

int main() {

	HDErrorInfo error;
	// Initialize the default haptic device.
	HHD hHD = hdInitDevice(HD_DEFAULT_DEVICE);
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		printf("Failed to initialize haptic device");
		system("pause");
		return -1;
	}

	// Start the servo scheduler and enable forces.
	hdEnable(HD_FORCE_OUTPUT);
	hdStartScheduler();
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		printf("Failed to start scheduler");
		system("pause");
		return -1;
	}

	// Application loop - schedule our call to the main callback.
	HDSchedulerHandle hSphereCallback = hdScheduleAsynchronous(
		FrictionlessSphereCallback, 0, HD_DEFAULT_SCHEDULER_PRIORITY);

	vec3<double> eePosition = vec3<double>(-1, -1, -1);

	//system("pause");
	while (!_kbhit())
	{
		hdGetDoublev(HD_CURRENT_POSITION, eePosition);
		printf("position x: %lf \n", eePosition.x);
		if (!hdWaitForCompletion(hSphereCallback, HD_WAIT_CHECK_STATUS))
		{
			fprintf(stderr, "\nThe main scheduler callback has exited\n");
			system("pause");
			break;
		}
	}

	// For cleanup, unschedule our callbacks and stop the servo loop.
	hdStopScheduler();
	hdUnschedule(hSphereCallback);
	hdDisableDevice(hHD);

	return 0;
}
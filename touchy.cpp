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

#include "HD/hd.h" //part of 3DS OpenHaptics
#include <HDU/hduVector.h>
#include <HDU/hduError.h>

//Global variable and handle declarations
HDSchedulerHandle hCallback;
HHD hHD;

/* Holds data retrieved from HDAPI. */
typedef struct
{
	HDboolean m_buttonState;       /* Has the device button has been pressed. */
	hduVector3Dd m_devicePosition; /* Current device coordinates. */
	HDErrorInfo m_error;
} DeviceData;

static DeviceData gServoDeviceData;

HDCallbackCode HDCALLBACK CallbackIdle(void *data)
{
	int nButtons = 0;
	// Get the position of the device.
	/*vec3<double> cursorPosition;*/
	
	hdBeginFrame(hHD);

	/* Retrieve the current button(s). */
	hdGetIntegerv(HD_CURRENT_BUTTONS, &nButtons);
	
	/* In order to get the specific button 1 state, we use a bitmask to
	test for the HD_DEVICE_BUTTON_1 bit. */
	gServoDeviceData.m_buttonState =
		(nButtons & HD_DEVICE_BUTTON_1) ? HD_TRUE : HD_FALSE;

	/* Get the current location of the device (HD_GET_CURRENT_POSITION)
	We declare a vector of three doubles since hdGetDoublev returns
	the information in a vector of size 3. */
	hdGetDoublev(HD_CURRENT_POSITION, gServoDeviceData.m_devicePosition);

	/* Also check the error state of HDAPI. */
	gServoDeviceData.m_error = hdGetError();

	hdEndFrame(hHD);

	return HD_CALLBACK_CONTINUE;
}

extern "C" dllexport int init() {

    HDErrorInfo error;
    // Initialize the default haptic device.
    hHD = hdInitDevice(HD_DEFAULT_DEVICE);
    if (HD_DEVICE_ERROR(error = hdGetError()))
    {
        return(error.errorCode);
    }

    // Start the servo scheduler and enable forces.
    hdEnable(HD_FORCE_OUTPUT);
    hdStartScheduler();
    if (HD_DEVICE_ERROR(error = hdGetError()))
    {
		return(error.errorCode);
    }

    return(HD_SUCCESS);
}

extern "C" dllexport int startIdleCallback() {
	HDErrorInfo error;
	hCallback = hdScheduleAsynchronous(CallbackIdle, 0, HD_DEFAULT_SCHEDULER_PRIORITY);
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		return(error.errorCode);
	}

	return HD_SUCCESS;
}



extern "C" dllexport int stopCallback() {
	HDErrorInfo error;
    
	// For cleanup, unschedule our callbacks and stop the servo loop.
    hdWaitForCompletion(hCallback, HD_WAIT_CHECK_STATUS);
	
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		return(error.errorCode);
	}
    
	//unschedule the callback
	hdUnschedule(hCallback);
	
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		return(error.errorCode);
	}

	return HD_SUCCESS;
}


extern "C" dllexport void getEEPosition(double positions[3])
{
	positions[0] = gServoDeviceData.m_devicePosition[0];
	positions[1] = gServoDeviceData.m_devicePosition[1];
	positions[2] = gServoDeviceData.m_devicePosition[2];
}

extern "C" dllexport bool getButtonState()
{
	return gServoDeviceData.m_buttonState;
}

extern "C" dllexport int shutdown() {
	HDErrorInfo error;

    // For cleanup, unschedule our callbacks and stop the servo loop.
    hdWaitForCompletion(hCallback, HD_WAIT_CHECK_STATUS);
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		return(error.errorCode+1000);
	}
    hdStopScheduler();
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		return(error.errorCode+2000);
	}
   /* hdUnschedule(hSphereCallback);
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		return(error.errorCode+3000);
	}*/
    hdDisableDevice(hHD);
	if (HD_DEVICE_ERROR(error = hdGetError()))
	{
		return(error.errorCode+4000);
	}

	return HD_SUCCESS;
}

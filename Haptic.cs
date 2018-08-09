using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public abstract class Haptic : MonoBehaviour
{

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

    //cursor that will move when the stylus position changes
    public GameObject cursor;

    //array that holds the button states read from the c++ DLL
    private bool[] stylusButtonStates = new bool[3];

    //For error checking
    private int haptic_error_code;

    //Event for stylus button
    //Other classes can subscribe to these events
    //at the moment this class only fires OnSylusButtonDown and up
    //but you're welcome to extend it to other event types (see "StylusButtonEventType" enum on line #9 above)
    public delegate void StylusAction(object sender, StylusButtonEventType stylusButtonEventType);
    public static event StylusAction OnStylusButtonDown;
    public static event StylusAction OnStylusButtonUp;


    public override void OnEnable()
    {
        //initialize haptic device driver
        haptic_error_code = init();
        if (haptic_error_code != 0) { print(hapticError_to_String(haptic_error_code)); }
        else
        {
            //If no error, print a message on the console to let you konw the device has been initialized
            print(this.GetType().Name + " : Haptic Device Initialized");
        }

        //start idle callback (for buttons and position - no force)
        haptic_error_code = startIdleCallback();
        if (haptic_error_code != 0) { print(hapticError_to_String(haptic_error_code)); }
    }

    // Update is called once per frame
    protected virtual void Update () {
        
        //update end effector position
        double[] tempPosition = new double[3];
        getEEPosition(tempPosition);
        Vector3 tempVector;

        //convert from mm to m because Unity3D units are in meters when using HMD/VR etc.
        tempVector.x = (float)tempPosition[0] / 1000;
        tempVector.y = (float)tempPosition[1] / 1000;
        tempVector.z = (float)-tempPosition[2] / 1000;  //reverse Z axis (phantom omni facing user)

        //apply position to our cursor object
        cursor.transform.localPosition = tempVector*cursorPositionScalingFactor;

        //Handle button presses (1, 2 or both buttons)
        handleStylusButtons(getButtonState());
    }


    //looks at button states and launches appropriate events
    //only works for Grey button. Feel free to extend to include other buttons
    //also fires OnStylusButtonDown method of child classes.
    private void handleStylusButtons(bool stylusCode)
    {
        if (stylusButtonStates[0] /*Grey*/ == false && stylusCode == true)
        {
            StylusButtonEvent(StylusButtonEventType.Grey_Down);
            if (OnStylusButtonDown != null)
            {
                OnStylusButtonDown(this,StylusButtonEventType.Grey_Down);
            }
            stylusButtonStates[0] = true;
        }

        if (stylusButtonStates[0] /*Grey*/ == true && stylusCode == false)
        {
            StylusButtonEvent(StylusButtonEventType.Grey_Up);
            stylusButtonStates[0] = false;
        }
    }

    //Your class should implement this
    protected abstract void StylusButtonEvent(StylusButtonEventType e);


    //Ran when the gameobject is disabled or when the child class/component is disabled.
    //also ran when application quits (i.e. when stopping play from the unity editor).
    public virtual void OnDisable()
    {
        stopCallback(); 
        haptic_error_code = shutdown();
        if (haptic_error_code != 0)
        {
            print("DLL Phantom Shutdown error: ");
            print(hapticError_to_String(haptic_error_code));
        }
        else
        {
            print(this.GetType().Name + " haptic device shutdown");
        }
    }


    // Import Functions from DLL

    [DllImport("Touchy")]
    public static extern int init();

    [DllImport("Touchy")]
    public static extern int stopCallback(); //<--- This is important to call always before starting another callback

    [DllImport("Touchy")]
    public static extern int startIdleCallback(); //<-- no forces yet updates end effector position and button states

    //basic sphere located at x,y,z
    [DllImport("Touchy")]
    public static extern int startSphereCallback(double radius, double position_x, double position_y, double position_z);

    //starts a spherical gravity well
    //located at x,y,z (opposite force of sphere callback)
    [DllImport("Touchy")]
    public static extern int startCenterCallback(double radius, double position_x, double position_y, double position_z);

    //important to run when this script is finished with the haptic device
    [DllImport("Touchy")]
    public static extern int shutdown();
    

    //self explanatory
    [DllImport("Touchy")]
    public static extern void setSphereRadius(double radius);
    [DllImport("Touchy")]
    public static extern double getSphereRadius();
    [DllImport("Touchy")]
    public static extern void setSpherePosition(double x, double y, double z);

    //get end effector position
    [DllImport("Touchy")]
    public static extern int getEEPosition(double[] position);

    [DllImport("Touchy")]
    public static extern bool getButtonState();

    [DllImport("Touchy")]
    public static extern int getLastError();


    public const int HD_SUCCESS = 0x0000;
    /* Function errors */
    public const int HD_INVALID_ENUM = 0x0100;
    public const int HD_INVALID_VALUE = 0x0101;
    public const int HD_INVALID_OPERATION = 0x0102;
    public const int HD_INVALID_INPUT_TYPE = 0x0103;
    public const int HD_BAD_HANDLE = 0x0104
    /* Force errors */;
    public const int HD_WARM_MOTORS = 0x0200;
    public const int HD_EXCEEDED_MAX_FORCE = 0x0201;
    public const int HD_EXCEEDED_MAX_FORCE_IMPULSE = 0x0202;
    public const int HD_EXCEEDED_MAX_VELOCITY = 0x0203;
    public const int HD_FORCE_ERROR = 0x0204;
    /* Device errors */
    public const int HD_DEVICE_FAULT = 0x0300;
    public const int HD_DEVICE_ALREADY_INITIATED = 0x0301;
    public const int HD_COMM_ERROR = 0x0302;
    public const int HD_COMM_CONFIG_ERROR = 0x0303;
    public const int HD_TIMER_ERROR = 0x0304;
    /* Haptic rendering context */
    public const int HD_ILLEGAL_BEGIN = 0x0400;
    public const int HD_ILLEGAL_END = 0x0401;
    public const int HD_FRAME_ERROR = 0x0402;
    /* Scheduler errors */
    public const int HD_INVALID_PRIORITY = 0x0500;
    public const int HD_SCHEDULER_FULL = 0x0501;
    /* Licensing erros */
    public const int HD_INVALID_LICENSE = 0x0600;

    public static string hapticError_to_String(int errorCode)
    {
        if (errorCode != 0)
        {
            /* Function errors */
            if (errorCode == HD_INVALID_ENUM) return errorCode + " HD_INVALID_ENUM";
            if (errorCode == HD_INVALID_VALUE) return errorCode + " HD_INVALID_VALUE";
            if (errorCode == HD_INVALID_OPERATION) return errorCode + " HD_INVALID_OPERATION";
            if (errorCode == HD_INVALID_INPUT_TYPE) return errorCode + " HD_INVALID_INPUT_TYPE";
            if (errorCode == HD_BAD_HANDLE) return errorCode + " HD_BAD_HANDLE";
            /* Force errors */
            ;
            if (errorCode == HD_WARM_MOTORS) return errorCode + " HD_WARM_MOTORS";
            if (errorCode == HD_EXCEEDED_MAX_FORCE) return errorCode + " HD_EXCEEDED_MAX_FORCE";
            if (errorCode == HD_EXCEEDED_MAX_FORCE_IMPULSE) return errorCode + " HD_EXCEEDED_MAX_FORCE_IMPULSE";
            if (errorCode == HD_EXCEEDED_MAX_VELOCITY) return errorCode + " HD_EXCEEDED_MAX_VELOCITY";
            if (errorCode == HD_FORCE_ERROR) return errorCode + " HD_FORCE_ERROR";
            /* Device errors */
            if (errorCode == HD_DEVICE_FAULT) return errorCode + " HD_DEVICE_FAULT";
            if (errorCode == HD_DEVICE_ALREADY_INITIATED) return errorCode + " HD_DEVICE_ALREADY_INITIATED";
            if (errorCode == HD_COMM_ERROR) return errorCode + " HD_COMM_ERROR";
            if (errorCode == HD_COMM_CONFIG_ERROR) return errorCode + " HD_COMM_CONFIG_ERROR";
            if (errorCode == HD_TIMER_ERROR) return errorCode + " HD_TIMER_ERROR";
            /* Haptic rendering context */
            if (errorCode == HD_ILLEGAL_BEGIN) return errorCode + " HD_ILLEGAL_BEGIN";
            if (errorCode == HD_ILLEGAL_END) return errorCode + " HD_ILLEGAL_END";
            if (errorCode == HD_FRAME_ERROR) return errorCode + " HD_FRAME_ERROR";
            /* Scheduler errors */
            if (errorCode == HD_INVALID_PRIORITY) return errorCode + " HD_INVALID_PRIORITY";
            if (errorCode == HD_SCHEDULER_FULL) return errorCode + " HD_SCHEDULER_FULL";
            /* Licensing erros */
            if (errorCode == HD_INVALID_LICENSE) return errorCode + " HD_INVALID_LICENSE";

            if (errorCode > 1600 && errorCode < 1999) { return "first line"; }
            if (errorCode > 1999 && errorCode < 2999) { return "second line"; }
            if (errorCode > 2999 && errorCode < 3999) { return "third line"; }
            if (errorCode > 3999 && errorCode < 4999) { return "fourth line"; }
            //Debug.Break();
        }
        return (errorCode + " SUCCESS");
    }
}

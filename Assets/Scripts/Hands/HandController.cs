using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandController : MonoBehaviour
{
    private InputDevice left, right;
    public GameObject menuLeft, menuRight;//the empty gameobjects that hold the menu
    public GameObject menu;
    private int state = 0; //0 is left hand, 1 is right, 2 is air
    private int gripStateLeft = 0, gripStateRight = 0; //0 is released, 1 is pressed to summon menu, 2 is pressed to release menu
    private Vector3 leftTrans, rightTrans;
    private Quaternion lQ, rQ;
    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        var inputDevices = new List<InputDevice>();
        var characteristics = InputDeviceCharacteristics.HeldInHand;
        InputDevices.GetDevicesWithCharacteristics(characteristics, inputDevices); // get all handed controllers
        if (inputDevices.Count > 2 || inputDevices.Count < 2)
        {
            //error text (removed for build)
            //Debug.Log("got " + inputDevices.Count);
        }
        else
        {
            //Debug.Log("got " + inputDevices.Count);
            foreach (InputDevice dev in inputDevices)
            {
                if ((dev.characteristics & InputDeviceCharacteristics.Left) == InputDeviceCharacteristics.Left)
                {
                    left = dev;
                }
                if ((dev.characteristics & InputDeviceCharacteristics.Right) == InputDeviceCharacteristics.Right)
                {
                    right = dev;
                }
            }
        }

        leftTrans = menu.transform.localPosition;
        lQ = menu.transform.localRotation;
        rightTrans = leftTrans;
        rightTrans.y *= -1;
        rQ = lQ;
    }

    // Update is called once per frame
    void Update()
    {
        if(!left.isValid)
        {
            Start();
        }
        float leftInVal, rightInVal;
        if(left.TryGetFeatureValue(CommonUsages.grip, out leftInVal))
        {
            if(gripStateLeft == 0 && leftInVal > 0.8)
            {
                if(state == 0)
                {
                    gripStateLeft = 2;
                    Release();
                    return;
                }
                gripStateLeft = 1;
                SummonLeft();
                return;
            }
            if(leftInVal < 0.4)
            {
                gripStateLeft = 0;
            }
        }
        else
        {
            //Debug.Log("Grip not found, " + left.characteristics);
        }
        if (right.TryGetFeatureValue(CommonUsages.grip, out rightInVal))
        {
            if (gripStateRight == 0 && rightInVal > 0.8)
            {
                if (state == 1)
                {
                    gripStateRight = 2;
                    Release();
                    return;
                }
                gripStateRight = 1;
                SummonRight();
                return;
            }
            if (rightInVal < 0.4)
            {
                gripStateRight = 0;
            }
        }
        else
        {
            //Debug.Log("Grip not found, "+right.characteristics);
        }
    }

    private void SummonRight()
    {
        menu.transform.SetParent(menuRight.transform);
        menu.transform.localPosition = rightTrans;
        menu.transform.localRotation = rQ;
        state = 1;
    }

    private void SummonLeft()
    {
        menu.transform.SetParent(menuLeft.transform);
        menu.transform.localPosition = leftTrans;
        menu.transform.localRotation = lQ;
        state = 0;
    }

    private void Release()
    {
        menu.transform.parent.DetachChildren();
        state = 2;
    }
}

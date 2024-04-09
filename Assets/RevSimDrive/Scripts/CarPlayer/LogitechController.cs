using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogitechController : MonoBehaviour
{
    LogitechGSDK.LogiControllerPropertiesData properties;

    public float xAxis, GasInput, BreakInput, ClutchInput;

    public int CurrentGear;

    // Start is called before the first frame update
    void Start()
    {
        print(LogitechGSDK.LogiSteeringInitialize(false));
    }

    // Update is called once per frame
    void Update()
    {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);

            xAxis = rec.lX / 32768f;

            if(rec.lY > 0)
            {
                GasInput = 0;
            }
            else if(rec.lY < 0)
            {
                GasInput = rec.lY / -32768f;
            }

            if (rec.lRz > 0)
            {
                BreakInput = 0;
            }
            else if (rec.lRz < 0)
            {
                GasInput = rec.lRz / -32768f;
            }

            if (rec.rglSlider[0] > 0)
            {
                ClutchInput = 0;
            }
            else if (rec.lY < 0)
            {
                ClutchInput = rec.rglSlider[0] / -32768f;
            }
        }
        else
        {
            print("No steering wheel connected");
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Utils : MonoBehaviour
{
    public static int GetUnixTime()
    {
        int unitxTime;
        TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
        TimeSpan unixTicks = new TimeSpan(DateTime.Now.Ticks) - epochTicks;
        unitxTime = (int)unixTicks.TotalSeconds;

        return unitxTime;
    }

    //int offEarnTime = Util.GetUnixTime() - UserData._outUnixTime;
    //Debug.Log(string.Format("{0}", unitxTime));
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    // check empty string
    public static bool IsEmptyString(Object o, string fieldName, string str)
    {
        if (str == "")
        {
            Debug.Log(fieldName + " is empty and must contain a value in object " + o.name.ToString());
            return true;
        }

        return false;
    }

    //check null values or empty list
    public static bool IsEnumerableValues(Object o, string fieldName, IEnumerable values)
    {
        bool error = false;
        int count = 0;

        foreach (var item in values)
        {
            if(item == null)
            {
                Debug.Log(fieldName + " has null values in object " + o.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }

            if(count == 0)
            {
                Debug.Log(fieldName + " hasn't any values in object " + o.name.ToString());
                error = true;
            }
        }

        return error;
    }
}

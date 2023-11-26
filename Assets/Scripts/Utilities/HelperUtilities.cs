using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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


    // null value debug check
    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, UnityEngine.Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fieldName + " is null and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    // positive value debug check- if zero is allowed set isZeroAllowed to true. Returns true if there is an error
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    //check null values or empty list
    public static bool IsEnumerableValues(Object o, string fieldName, IEnumerable values)
    {
        bool error = false;
        int count = 0;

        //Debug.Log(values);
        


        if (values == null)
        {
            Debug.Log(fieldName + " is null in object " + o.name.ToString());
            //Debug.Log("oke");
            return true;
        }

        foreach (var item in values)
        {
            if (item == null)
            {
                Debug.Log(fieldName + " has null values in object " + o.name.ToString());
                //Debug.Log("oke");
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " hasn't any values in object " + o.name.ToString());
            //Debug.Log("oke");
            error = true;
        }
        //Debug.Log("oke1");
        return error;
    }

    // Get the nearest spawn position to the player
    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(10000f, 10000f, 0f);

        // Loop through room spawn positions
        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            // convert the spawn grid positions to world positions
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if (Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                nearestSpawnPosition = spawnPositionWorld;
            }
        }

        return nearestSpawnPosition;
    }
}

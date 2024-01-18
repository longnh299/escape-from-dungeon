using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DontDestroy : MonoBehaviour
{
    [ClientCallback]
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}

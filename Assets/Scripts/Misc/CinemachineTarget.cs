using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    private void Awake()
    {
        // Load components
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCinemachineTargetGroup();
    }

    // Set the cinemachine camera target group.
    private void SetCinemachineTargetGroup()
    {
        // Create target group for cinemachine for the cinemachine camera to follow  - group will include the player and screen cursor
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target { weight = 1f, radius = 2.5f, target = GameManager.Instance.GetPlayer().transform };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTarget_player};

        cinemachineTargetGroup.m_Targets = cinemachineTargetArray; // save cinemachine target into Targrt list of CinemachineTargetGroup in unity editor

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portals : MonoBehaviour
{
    [SerializeField] private Transform endPos;

    public static Portals Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Transform EndPos => endPos;
}

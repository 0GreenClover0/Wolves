using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    public static ExitPoint instance;

    public void Awake()
    {
        instance = this;
    }
}

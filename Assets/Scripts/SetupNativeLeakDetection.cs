using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class SetupNativeLeakDetection : MonoBehaviour
{
    void Start() {
        NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
    }
}

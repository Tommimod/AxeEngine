#if AXE_ENGINE_ENABLE_STATIC
using System;
using UnityEngine;

namespace AxeEngine
{
    [AxeProperty, Serializable]
    public struct UnityObject
    {
        public GameObject Value;
    }
}
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainJam
{
    public class Misc : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}
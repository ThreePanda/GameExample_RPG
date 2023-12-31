using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    //我的门号标记
    public enum DestinationTag
    {
        Enter,A,B,C
    }
    [Header("My Door Number")]
    public DestinationTag destinationTag;
}

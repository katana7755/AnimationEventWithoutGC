using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{
    public void OnAnimationEvent_0(AnimationEvent aniEvt)
    {
        Debug.LogWarning($"Animation Event 0 : {aniEvt.intParameter}, {aniEvt.floatParameter}, {aniEvt.stringParameter}, {aniEvt.objectReferenceParameter}");
    }

    public void OnAnimationEvent_1(AnimationEvent aniEvt)
    {
        Debug.LogWarning($"Animation Event 1 : {aniEvt.intParameter}, {aniEvt.floatParameter}, {aniEvt.stringParameter}, {aniEvt.objectReferenceParameter}");
    }

    public void OnAnimationEvent_2(AnimationEvent aniEvt)
    {
        Debug.LogWarning($"Animation Event 2 : {aniEvt.intParameter}, {aniEvt.floatParameter}, {aniEvt.stringParameter}, {aniEvt.objectReferenceParameter}");
    }
}

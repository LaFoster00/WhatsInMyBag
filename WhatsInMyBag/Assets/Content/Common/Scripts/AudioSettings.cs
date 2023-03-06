using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New AudioSettings", menuName = "Audio/AudioSettings")]
public class AudioSettings : ScriptableObject
{
    [Range(0, 100)] public float volume = 100;

    public void SetVolume(float value)
    {
        volume = value;
        UpdateRTCP();
    }

    public void UpdateRTCP()
    {
        AkSoundEngine.SetRTPCValue("Game_Volume", volume);
    }
}

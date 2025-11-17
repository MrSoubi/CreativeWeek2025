using System;
using System.Collections;
using FMODUnity;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter m_MusicEmitter;
    [SerializeField] private RSE_OnMusicStarted m_OnMusicStarted;

    private void Start()
    {
        StartCoroutine(WaitForMusicStart());
    }

    IEnumerator WaitForMusicStart()
    {
        yield return new WaitForSeconds(3f);
        m_MusicEmitter.Play();
        m_OnMusicStarted.onMusicStarted.Invoke();
    }
}

using System;
using System.Collections;
using FMODUnity;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter m_MusicEmitter;
    [SerializeField] private RSE_OnMusicStarted m_OnMusicStarted;
    [SerializeField] private RSO_MusicTime m_MusicTime;

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

    private void Update()
    {
        if (m_MusicEmitter.IsPlaying())
        {
            int milliseconds;
            m_MusicEmitter.EventInstance.getTimelinePosition(out milliseconds); 
            m_MusicTime.Time = milliseconds / 1000f;
        }
    }
}

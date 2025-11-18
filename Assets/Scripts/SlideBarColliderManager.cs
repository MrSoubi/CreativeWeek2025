using System;
using UnityEngine;

public class SlideBarColliderManager : MonoBehaviour
{
    [SerializeField] PlatformEffector2D platformEffector2D;
    [SerializeField] RSE_EnableSliding m_EnableSliding;
    [SerializeField] RSE_DisableSliding m_DisableSliding;

    private void OnEnable()
    {
        m_EnableSliding.Triggered.AddListener(EnableColliders);
        m_DisableSliding.Triggered.AddListener(DisableColliders);
    }
    
    private void OnDisable()
    {
        m_EnableSliding.Triggered.RemoveListener(EnableColliders);
        m_DisableSliding.Triggered.RemoveListener(DisableColliders);
    }
    
    private void EnableColliders()
    {
        platformEffector2D.colliderMask = LayerMask.GetMask("Player");
    }
    
    private void DisableColliders()
    {
        platformEffector2D.colliderMask = 0;
    }
}

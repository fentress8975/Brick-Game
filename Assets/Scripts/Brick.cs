using System;
using Unity.Netcode;
using UnityEngine;

public class Brick : NetworkBehaviour
{
    public const int PLAYER1 = 1;
    public const int PLAYER2 = 2;
    public Action<Brick> OnDestruction;
    [SerializeField] private BrickFX m_BrickFX;
    [SerializeField] private Renderer m_ModelRenderer;
    [SerializeField] private Material m_Player1Color;
    [SerializeField] private Material m_Player2Color;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerServerBall _))
        {
            OnDestruction?.Invoke(this);
            OnNetworkDespawn();
            Destroy(gameObject);
        }
    }

    public void ChangeColor(int number)
    {
        var _materials = m_ModelRenderer.materials;
        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i].name == "Color (Instance)")
            {
                _materials[i] = number == PLAYER1 ? m_Player1Color : m_Player2Color;
                break;
            }
        }
        m_ModelRenderer.materials = _materials;
    }

    [ClientRpc]
    public void ChangeColorClientRpc(int number)
    {
        ChangeColor(number);
    }

#if UNITY_EDITOR
    public void Kill()
    {
        OnDestruction?.Invoke(this);
        OnNetworkDespawn();
        Destroy(gameObject);
    }

    [ContextMenu("Destroy")]
    public void Destruction()
    {
        m_BrickFX.Play();
    }
#endif
}

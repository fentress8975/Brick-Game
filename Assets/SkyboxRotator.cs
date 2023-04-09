using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [SerializeField] private Material m_Skybox;
    private int m_ShaderId;
    private float m_Rotation = 0;

    private void Start()
    {
        m_ShaderId = Shader.PropertyToID("_Rotation");
        StartCoroutine(SkyboxRotation());
    }

    private IEnumerator SkyboxRotation()
    {
        while (true)
        {
            m_Skybox.SetFloat(m_ShaderId, m_Rotation);
            m_Rotation += 0.01f;
            yield return new WaitForSeconds(.1f);
        }
    }
}

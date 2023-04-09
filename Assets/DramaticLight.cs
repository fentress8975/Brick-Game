using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DramaticLight : MonoBehaviour
{
    [SerializeField] private Material m_Material;
    private int m_EmissionID;


    private void Start()
    {
        StartCoroutine(DramaticLightFX());
        m_EmissionID = Shader.PropertyToID("_EmissionColor");
        
    }

    private IEnumerator DramaticLightFX()
    {
        while (true)
        {
            m_Material.SetColor(m_EmissionID, Color.black);
            m_Material.SetColor(3, Color.black);

            yield return new WaitForSeconds(Random.Range(0.05f,0.1f));
            m_Material.SetColor(m_EmissionID, Color.grey);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            m_Material.SetColor(m_EmissionID, Color.white);

            yield return new WaitForSeconds(Random.Range(3, 5));
        }
    }
}

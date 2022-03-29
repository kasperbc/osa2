using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    void Start()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();

        float duration = particleSystem.main.duration;

        particleSystem.Play();

        Invoke("KillGameObject", duration);
    }

    void KillGameObject()
    {
        Destroy(gameObject);
    }
}

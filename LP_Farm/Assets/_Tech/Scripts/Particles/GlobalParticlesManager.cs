using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalParticlesManager : Singleton<GlobalParticlesManager>
{
    [SerializeField] private ParticleSystem _particle;

    public void PlayParticle(Vector3 position) {
        _particle.transform.position = position;
        _particle.Play();
    }
}

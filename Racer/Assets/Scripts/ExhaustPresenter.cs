using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustPresenter : Base
{
    private ParticleSystem nitros = null;
    private ParticleSystem shifting = null;

    private void Start()
    {
        nitros = GlobalFactory.CreateRacerNitrosParticle(transform);
        shifting = GlobalFactory.CreateRacerShiftingParticle(transform);
        nitros.Stop(true);
        shifting.Stop(true);
    }

    private void StartNitors()
    {
        nitros.Play();
    }

    private void StopNitors()
    {
        nitros.Stop();
    }

    private void Shifting()
    {
        shifting.Play();
    }
}

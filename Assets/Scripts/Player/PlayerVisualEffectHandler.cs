using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualEffectHandler : MonoBehaviour
{

    [SerializeField] PlayerParticles particles;

    Dictionary<ParticleType, ParticleSystem> particlesDictionary;

    void Start()
    {
        if (particles)
        {
            initializeParticles();
            
        }
    }
    

    private void initializeParticles()
    {

        particlesDictionary = new Dictionary<ParticleType, ParticleSystem>{
            {ParticleType.jump, Instantiate(particles.jumpParticle,transform)},
            {ParticleType.run, Instantiate(particles.runParticles,transform)},
            {ParticleType.walk, Instantiate(particles.walkParticles,transform)},
            {ParticleType.dash, Instantiate(particles.dashParticles,transform)},
            // {ParticleType.land, Instantiate(particles.landParticles,transform)},
        };
    }

    public void playParticle(ParticleType type)
    {


        if (particles && particlesDictionary.TryGetValue(type, out ParticleSystem particle))
        {
            particle.Play();
        }

    }
    public void playParticle(ParticleType type,Transform transform)
    {


        if (particles && particlesDictionary.TryGetValue(type, out ParticleSystem particle))
        {
            particle.gameObject.transform.position = transform.position;
            particle.gameObject.transform.rotation = transform.rotation;
            particle.Play();
        }

    }
    public void stopParticle(ParticleType type)
    {
        if (particles && particlesDictionary.TryGetValue(type, out ParticleSystem particle))
        {
            particle.Stop();
        }
    }

    
    public void stopAllParticles()
    {
        foreach (var ele in particlesDictionary)
        {
            if (ele.Value != null)
            {
                ele.Value.Stop();
            }
        }

    }
    public enum ParticleType
    {
        jump,
        run,
        walk,
        land,
        dash,
    }



    





}
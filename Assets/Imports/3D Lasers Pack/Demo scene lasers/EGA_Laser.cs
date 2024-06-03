using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class EGA_Laser : MonoBehaviour
{
    private static readonly int _noise = Shader.PropertyToID("_Noise");
    private static readonly int _mainTex = Shader.PropertyToID("_MainTex");

    public GameObject ShootEffect;
    public GameObject HitEffect;
    public float HitOffset = 0;

    public float MaxLength;
    private LineRenderer _laser;

    public float MainTextureLength = 1f;
    public float NoiseTextureLength = 1f;

    private Vector4 _length = new Vector4(1, 1, 1, 1);

    //One activation per shoot
    private bool _laserSaver = false;
    private bool _updateSaver = false;

    private ParticleSystem[] _effects;
    private ParticleSystem[] _hit;

    void OnEnable()
    {
        //Get LineRender and ParticleSystem components from current prefab;  
        _laser = GetComponent<LineRenderer>();
        _effects = GetComponentsInChildren<ParticleSystem>();
        _hit = HitEffect.GetComponentsInChildren<ParticleSystem>();
        //if (Laser.material.HasProperty("_SpeedMainTexUVNoiseZW")) LaserStartSpeed = Laser.material.GetVector("_SpeedMainTexUVNoiseZW");
        //Save [1] and [3] textures speed
        //{ DISABLED AFTER UPDATE}
        //LaserSpeed = LaserStartSpeed;
    }

    public void SetPoints(Vector3 startPoint, Vector3 endPoint)
    {
        _laser.SetPosition(0, startPoint);
        _laser.SetPosition(1, endPoint);

        HitEffect.transform.position = endPoint + Vector3.down * HitOffset;
        // HitEffect.transform.rotation = Quaternion.identity;
        
        foreach (var allPs in _effects)
        {
            if (!allPs.isPlaying)
                allPs.Play();
        }

        //Texture tiling
        _length[0] = MainTextureLength * Vector3.Distance(transform.position, endPoint);
        _length[2] = NoiseTextureLength * Vector3.Distance(transform.position, endPoint);

        _laser.material.SetTextureScale(_mainTex, new Vector2(_length[0], _length[1]));
        _laser.material.SetTextureScale(_noise, new Vector2(_length[2], _length[3]));
    }

    // void Update()
    // {
    //     //if (Laser.material.HasProperty("_SpeedMainTexUVNoiseZW")) Laser.material.SetVector("_SpeedMainTexUVNoiseZW", LaserSpeed);
    //     //SetVector("_TilingMainTexUVNoiseZW", Length); - old code, _TilingMainTexUVNoiseZW no more exist
    //     _laser.material.SetTextureScale("_MainTex", new Vector2(_length[0], _length[1]));                    
    //     _laser.material.SetTextureScale("_Noise", new Vector2(_length[2], _length[3]));
    //     //To set LineRender position
    //     // if (Laser == null || UpdateSaver) return;
    //     
    //     _laser.SetPosition(0, transform.position);
    //     
    //     
    //     RaycastHit hit; //DELATE THIS IF YOU WANT USE LASERS IN 2D
    //     //ADD THIS IF YOU WANNT TO USE LASERS IN 2D: RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, MaxLength);       
    //     if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, MaxLength))//CHANGE THIS IF YOU WANT TO USE LASERRS IN 2D: if (hit.collider != null)
    //     {
    //         //End laser position if collides with object
    //         _laser.SetPosition(1, hit.point);
    //         HitEffect.transform.position = hit.point + hit.normal * HitOffset;
    //         //Hit effect zero rotation
    //         HitEffect.transform.rotation = Quaternion.identity;
    //         foreach (var AllPs in _effects)
    //         {
    //             if (!AllPs.isPlaying) AllPs.Play();
    //         }
    //         //Texture tiling
    //         _length[0] = MainTextureLength * Vector3.Distance(transform.position, hit.point);
    //         _length[2] = NoiseTextureLength * Vector3.Distance(transform.position, hit.point);
    //         //Texture speed balancer {DISABLED AFTER UPDATE}
    //         //LaserSpeed[0] = (LaserStartSpeed[0] * 4) / (Vector3.Distance(transform.position, hit.point));
    //         //LaserSpeed[2] = (LaserStartSpeed[2] * 4) / (Vector3.Distance(transform.position, hit.point));
    //     }
    //     else
    //     {
    //         //End laser position if doesn't collide with object
    //         var EndPos = transform.position + transform.forward * MaxLength;
    //         _laser.SetPosition(1, EndPos);
    //         HitEffect.transform.position = EndPos;
    //         foreach (var AllPs in _hit)
    //         {
    //             if (AllPs.isPlaying) AllPs.Stop();
    //         }
    //         //Texture tiling
    //         _length[0] = MainTextureLength * Vector3.Distance(transform.position, EndPos);
    //         _length[2] = NoiseTextureLength * Vector3.Distance(transform.position, EndPos);
    //         //LaserSpeed[0] = (LaserStartSpeed[0] * 4) / (Vector3.Distance(transform.position, EndPos)); {DISABLED AFTER UPDATE}
    //         //LaserSpeed[2] = (LaserStartSpeed[2] * 4) / (Vector3.Distance(transform.position, EndPos)); {DISABLED AFTER UPDATE}
    //     }
    //     //Insurance against the appearance of a laser in the center of coordinates!
    //     if (_laser.enabled == false && _laserSaver == false)
    //     {
    //         _laserSaver = true;
    //         _laser.enabled = true;
    //     }
    // }

    public void DisablePrepare()
    {
        if (_laser != null)
        {
            _laser.enabled = false;
        }

        _updateSaver = true;
        //Effects can = null in multiply shooting
        if (_effects != null)
        {
            foreach (var AllPs in _effects)
            {
                if (AllPs.isPlaying) AllPs.Stop();
            }
        }
    }
}
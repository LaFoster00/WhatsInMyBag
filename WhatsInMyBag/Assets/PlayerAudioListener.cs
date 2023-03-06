using MiscUtil.Collections.Extensions;
using Player.Controller;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(AkAudioListener))]
[RequireComponent(typeof(AkGameObj))]
public class PlayerAudioListener : MonoBehaviour
{
    private Transform _camera;
    private Transform _player;
    [SerializeField] private float distanceTowardsCamera = 7.5f;
    [SerializeField] private bool offsetTowardsCamera = true;
    
    // Start is called before the first frame update
    void Awake()
    {
        _camera = Camera.main.transform;
        _player = FindObjectOfType<PlayerController>().transform;

        foreach (var listener in FindObjectsOfType<AkAudioListener>())
        {
            if (listener.gameObject != gameObject)
                Destroy(listener);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _player.position + (offsetTowardsCamera ? (_camera.position - _player.position).normalized * distanceTowardsCamera : Vector3.zero);
        transform.rotation = Quaternion.Euler(new float3(0, _camera.rotation.eulerAngles.y, 0));
    }
}
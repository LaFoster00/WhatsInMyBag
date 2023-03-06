using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class OrbitCameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    public float distance = 20;
    [Range(0, 89.9f)] public float angle = 40;
    public float cameraSize = 8;
    public float targetRotation = -40;

    private float _currentRotation;
    private Camera _camera;
    
    // Update is called once per frame
    void Update()
    {
        if (!_camera)
        {
            _camera = Camera.main;
        }

        _camera.orthographicSize = cameraSize;
        _camera.fieldOfView = cameraSize;
        float3 orbitOffset = new float3(0, 0, -distance);

        _currentRotation =
            Mathf.LerpAngle(_currentRotation, targetRotation, math.clamp(Time.smoothDeltaTime * 10f, 0, 1));

        float3 orbitRotation = new float3(angle, _currentRotation ,0);
        orbitOffset = math.mul(quaternion.Euler(math.radians(orbitRotation)), orbitOffset);
        _camera.transform.position = target.position + (Vector3)orbitOffset;
        
        _camera.transform.rotation = quaternion.Euler(math.radians(angle), math.radians(_currentRotation), 0);
    }
}

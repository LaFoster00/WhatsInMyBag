using UnityEngine;

public class PlayerAnimationAudioController : MonoBehaviour
{
    [Header("Walking")] [SerializeField] private AK.Wwise.Event stepEvent;
    [SerializeField] private AK.Wwise.Switch carpetSwitch;
    [SerializeField] private AK.Wwise.Switch concreteSwitch;
    [SerializeField] private AK.Wwise.Switch liquidSwitch;
    [SerializeField] private AK.Wwise.Switch tileSwitch;
    [SerializeField] private AK.Wwise.Switch woodSwitch;

    private string lastStep;
    private string currentUndergroundType;

    public void Step(string s)
    {
        if (lastStep != s)
        {
            stepEvent.Post(gameObject);
        }

        lastStep = s;
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10, Color.cyan);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, LayerMask.GetMask("Floor")))
        {
            if (currentUndergroundType == null || !hit.collider.CompareTag(currentUndergroundType))
            {
                switch (hit.collider.tag)
                {
                    case "Carpet":
                        carpetSwitch.SetValue(gameObject);
                        break;
                    case "Concrete":
                        concreteSwitch.SetValue(gameObject);
                        break;
                    case "Liquid":
                        liquidSwitch.SetValue(gameObject);
                        break;
                    case "Tile":
                        tileSwitch.SetValue(gameObject);
                        break;
                    case "Wood":
                        woodSwitch.SetValue(gameObject);
                        break;
                }

                print(hit.collider.tag);
                currentUndergroundType = hit.collider.tag;
            }

        }
    }
}

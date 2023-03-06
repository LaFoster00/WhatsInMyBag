using UnityEngine;
using UnityEngine.EventSystems;

public class UiSound : MonoBehaviour
{
    [SerializeField] public AK.Wwise.Event selectEvent;
    [SerializeField] private AK.Wwise.Event submitEvent;
    [SerializeField] private AK.Wwise.Event dragEvent;

    public void OnSelect(BaseEventData eventData)
    {
        print("Select");
        selectEvent.Post(gameObject);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        print("Submit");
        submitEvent.Post(gameObject);
    }

    public void OnDrag(BaseEventData data)
    {
        print("Drag");
        dragEvent.Post(gameObject);
    }
}

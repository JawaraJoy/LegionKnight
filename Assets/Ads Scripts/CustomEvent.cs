using UnityEngine;
using UnityEngine.Events;

public class CustomEvent : MonoBehaviour
{
    // UnityEvent to hold the function to call on button click
    public UnityEvent onClick;

    // This method can be called to invoke the event
    public void Click()
    {
        if (onClick != null)
        {
            onClick.Invoke();
        }
    }
}

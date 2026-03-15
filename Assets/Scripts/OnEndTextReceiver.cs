using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class OnEndTextReceiver : MonoBehaviour
{
    [SerializeField] UnityEvent onEndText;

    public void OnEndText()
    {
        onEndText?.Invoke();
    }
}

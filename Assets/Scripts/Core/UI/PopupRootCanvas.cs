using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class PopupRootCanvas : MonoBehaviour
{
    private void Start()
    {
        if (Managers.Instance != null)
        {

            StartCoroutine(RegisterRootCanvasCoroutine());
        }
    }

    private IEnumerator RegisterRootCanvasCoroutine()
    {
        yield return new WaitUntil(() => Managers.Instance != null && Managers.UI != null);
        Managers.UI.RegisterRootCanvas(this.transform);
    }
}

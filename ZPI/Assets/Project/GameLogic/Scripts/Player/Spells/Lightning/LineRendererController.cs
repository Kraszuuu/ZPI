using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    [SerializeField]
    List<LineRenderer> lineRenderers = new List<LineRenderer>();

    public void SetPosition(Transform startPos, Transform endPos)
    {
        if  (lineRenderers.Count > 0)
        {
            for (int i = 0; i < lineRenderers.Count; i++)
            {
                if (lineRenderers[i].positionCount >= 2)
                {
                    lineRenderers[i].SetPosition(0, startPos.position);
                    lineRenderers[i].SetPosition(1, endPos.position);
                }
                else
                {
                    Debug.Log("The line renderer should have at least 2 positions");
                }
            }
        }
        else
        {
            Debug.Log("Line Renderers list is empyt");
        }
    }
}

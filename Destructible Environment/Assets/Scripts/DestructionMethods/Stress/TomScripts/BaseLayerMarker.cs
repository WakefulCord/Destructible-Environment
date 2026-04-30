// Place in a new file: BaseLayerMarker.cs
using UnityEngine;

public class BaseLayerMarker : MonoBehaviour
{
    void OnDestroy()
    {
        var structure = GetComponentInParent<Structurestress>();
        if (structure != null)
        {
            structure.OnBaseLayerDestroyed(this);
        }
    }
}

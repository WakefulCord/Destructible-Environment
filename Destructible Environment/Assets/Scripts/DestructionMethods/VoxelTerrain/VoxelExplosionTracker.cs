using System.Collections.Generic;
using UnityEngine;

public class VoxelExplosionTracker : MonoBehaviour
{
    [SerializeField] WorldManager worldManager;

    private List<ExplosiveBehaviour> tracked = new List<ExplosiveBehaviour>();

    private void Update()
    {
        ExplosiveBehaviour[] all = FindObjectsByType<ExplosiveBehaviour>(FindObjectsSortMode.None);

        foreach (var explosive in all)
        {
            if (!tracked.Contains(explosive))
            {
                tracked.Add(explosive);
                explosive.Exploded += OnExploded;
            }
        }
    }

    private void OnExploded(ExplosiveBehaviour explosive)
    {
        Debug.Log("Explosion detected at: " + explosive.transform.position);
        worldManager.DestroyMultiple(explosive.transform.position, 3);
        explosive.Exploded -= OnExploded;
        tracked.Remove(explosive);
    }
}

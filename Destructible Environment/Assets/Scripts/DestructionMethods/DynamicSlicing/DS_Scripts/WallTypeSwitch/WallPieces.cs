using System.Collections;
using UnityEngine;

public class WallPieces : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine("DestroyAfterSeconds");
    }

    IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(Random.Range(3,8));
        Destroy(gameObject);
    }
}
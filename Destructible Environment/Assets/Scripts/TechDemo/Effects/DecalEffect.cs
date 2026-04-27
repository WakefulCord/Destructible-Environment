using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalEffect : MonoBehaviour
{
    DecalProjector decal;

    public void OnInit(float decalScale, float fadeTimer)
    {
        decal = GetComponent<DecalProjector>();

        if (decal == null )
        {
            Debug.LogWarning($"DecalFadeOut: No DecalProjector found on {gameObject.name}.");
            return;
        }
        //scale
        decal.size = new Vector3(decal.size.x * decalScale, decal.size.y * decalScale, decal.size.z);

        //rotation - random z rotation
        float randomZRot = Random.Range(0.0f, 360f);
        Vector3 currentEuler = transform.rotation.eulerAngles;
        Vector3 newEuler = new Vector3(currentEuler.x, currentEuler.y, currentEuler.z + randomZRot);

        transform.rotation = Quaternion.Euler(newEuler);
        


        StartCoroutine(FadeOut(fadeTimer));
    }

    private IEnumerator FadeOut(float timer)
    {
        float elapsed = 0f;

        while (elapsed < timer)
        {
            elapsed += Time.deltaTime;
            decal.fadeFactor = Mathf.Lerp(1f, 0f, elapsed / timer);
            yield return null;
        }

        Destroy(gameObject);
    }

}

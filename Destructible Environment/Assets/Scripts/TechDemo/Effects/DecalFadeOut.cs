using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalFadeOut : MonoBehaviour
{
    DecalProjector decal;

    public void OnInit(float fadeTimer)
    {
        decal = GetComponent<DecalProjector>();

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

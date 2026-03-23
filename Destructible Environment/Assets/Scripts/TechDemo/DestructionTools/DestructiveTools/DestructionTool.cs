using UnityEngine;

public class DestructionTool : ScriptableObject
{
    [Header("---Destruction Tool---")]
    [Space(2)]
    [Header("Tool Data")]
    [SerializeField] private string toolName;
    [Tooltip("Prefab of weapon set up correctly, including behaviour script")][SerializeField] private GameObject toolPrefab;
    [SerializeField] private Sprite toolIcon;
    [Space(4)]

    [Header("Destruction Data")]
    [SerializeField] private DestructionFeedback destructionData;
    [SerializeField] private DestructionLayer compatibleLayers;
    [Space(4)]

    [Header("Tool Stats")]
    [SerializeField] private float useCooldown;
    [SerializeField] private float altUseCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float radius;
    [Space(4)]

    [Header("Tool Effects")]
    [SerializeField] private AudioClip useAudio;

    [Range(0f, 1f)][SerializeField] private float useAudioVolume;
    [SerializeField] private GameObject useEffect;
    [Space(4)]

    [Header("Animation Fields")]
    [SerializeField] private bool doAimAtCursor;
    [Space(4)]

    [Header("Tool Modes")]
    [SerializeField] private bool isAutomatic = false;
    public GameObject GetUseEffect => useEffect;

    public bool DoAimAtCursor => doAimAtCursor;
    public DestructionFeedback GetDestructionFeedback => destructionData;
    public float GetUseVolume => useAudioVolume;
    public AudioClip GetUseAudio => useAudio;
    public DestructionLayer GetCompatibleLayers => compatibleLayers;
    public float UseCooldown => useCooldown;
    public float AltUseCooldown => altUseCooldown;
    public float Damage => damage;
    public float Radius => radius;

    public string GetName => toolName;
    public GameObject GetPrefab => toolPrefab;
    public bool IsAutomatic => isAutomatic;
}

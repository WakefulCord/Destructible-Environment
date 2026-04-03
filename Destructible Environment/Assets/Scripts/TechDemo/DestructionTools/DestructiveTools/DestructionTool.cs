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
    [SerializeField] private float damage;
    [SerializeField] private float radius;

    [Header("---Primary Action Settings---")]
    [Header("Action")]
    [SerializeField] private ToolFireMode primaryFireMode;
    [SerializeField] private bool primaryUsesCooldown = true;
    [SerializeField] private float primaryCooldown;

    [Header("Fire Feedback")]
    [SerializeField] private AudioClip primaryAudio;
    [Range(0f, 1f)][SerializeField] private float primaryAudioVolume;
    [SerializeField] private GameObject primaryEffect;
    [Space(4)]

    [Header("---Secondary Action Settings---")]
    [Header("Action")]
    [SerializeField] private ToolFireMode secondaryFireMode;
    [SerializeField] private bool secondaryUsesCooldown = true;
    [SerializeField] private float secondaryCooldown;


    [Space(4)]

    [Header("Animation Fields")]
    [SerializeField] private bool doPointAtCursor;
   

   
    public GameObject PrimaryEffect => primaryEffect;

    public bool DoPointAtCursor => doPointAtCursor;
    public DestructionFeedback GetDestructionFeedback => destructionData;
    public float GetUseVolume => primaryAudioVolume;
    public AudioClip PrimaryAudio => primaryAudio;
    public DestructionLayer GetCompatibleLayers => compatibleLayers;
    public float UseCooldown => primaryCooldown;
    public float AltUseCooldown => secondaryCooldown;
    public float Damage => damage;
    public float Radius => radius;

    public string GetName => toolName;
    public GameObject GetPrefab => toolPrefab;
    public ToolFireMode PrimaryFireMode => primaryFireMode;
    public ToolFireMode SecondaryFireMode => secondaryFireMode;
    public bool PrimaryUsesCooldown => primaryUsesCooldown;
    public bool SecondaryUsesCooldown => secondaryUsesCooldown;


}
public enum ToolFireMode
{
    SingleClick,
    Hold
}

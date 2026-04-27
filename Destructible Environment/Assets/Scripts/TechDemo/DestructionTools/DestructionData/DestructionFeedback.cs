using UnityEngine;

public class DestructionFeedback : ScriptableObject // effects/ sound of destruction like explosion or digging particles
{
    [Header("---Destruction Feedback---")]
    [Header("Audio Feedback")]
     [SerializeField] private AudioClip destructionAudioClip; // audio of destruction, explosion, bullet impact, dig noise

    [Range(0f,1f)][SerializeField] private float destructionAudioVolume;

    //visual feedback
    [SerializeField]private GameObject destructionEffect;
    
    [Header("Destructive Feeback")]
    [SerializeField] private float destructiveForce;

    //public 
    public GameObject DestructionEffect => destructionEffect;
    public AudioClip GetDestructionAudio => destructionAudioClip;
    public float GetDestructionVolume  => destructionAudioVolume;

    public float DestructiveForce => destructiveForce;

}

using UnityEngine;

public class DestructionTool : ScriptableObject
{
    [Header("Tool Fields")]
    [SerializeField] private string toolName;
    [SerializeField] private GameObject toolPrefab;
    [SerializeField] private Sprite toolIcon;
    [SerializeField] private DestructiveData data;

    [SerializeField] private DestructionLayer compatibleLayers;

    public DestructionLayer GetCompatibleLayers => compatibleLayers;

    //public
    public string GetName => toolName;
    public GameObject GetPrefab => toolPrefab;
    public DestructiveData GetData => data;
  
    
}

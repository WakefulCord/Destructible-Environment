using UnityEngine;

public class PlayerLoadout : MonoBehaviour // holds data for tools being used
{
    #region Class References

    #endregion

    #region Private Fields
    [Header("Loadout Fields")]
    [SerializeField] private DestructionTool[] avaiableTools;

    #endregion

    #region Properties
    public DestructionTool[] GetTools => avaiableTools;

    public int ToolCount => avaiableTools.Length;

    #endregion

}

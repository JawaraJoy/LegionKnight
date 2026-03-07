using UnityEngine;

[CreateAssetMenu(fileName = "CameraPostSet", menuName = "Rush/Camera/CameraPostSet", order = 1)]
public partial class CameraPostSetConfig : ScriptableObject
{
    [SerializeField]
    private string m_PostName;
    [SerializeField]
    private float m_TransitionDuration = 0.1f;
    [SerializeField]
    private Vector3 m_Post;
    public string PostName => m_PostName;
    public Vector3 Post => m_Post;
    public float TransitionDuration => m_TransitionDuration;
    
}
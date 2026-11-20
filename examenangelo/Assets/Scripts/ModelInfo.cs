using UnityEngine;

public class ModelInfo : MonoBehaviour
{
    [SerializeField] private ElementData data;
    public ElementData ElementData { get => data; set => data = value; }
}

using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementReaction", menuName = "Scriptable Objects/ElementReaction")]
public class ElementReaction : ScriptableObject
{
    [Header("Required Elements")]
    public ElementData elementA;
    public ElementData elementB;

    [Header("Result")]
    public ElementData result;
    public AudioClip reactionSound;

    [Header("Interaction")]
    public float interactionDistance = 0.15f;
    public AudioClip reactionsound;

    public bool CanReact(ElementData a, ElementData b)
    {
        return (a == elementA && b == elementB) || (a == elementB && b == elementA);
    }
    //arfneiurneiurneirun

}

using UnityEngine;

public enum TipoInteraccion
{
    Amistosa,
    DueloFisico,
    BatallaMagica,
    Gymbros
}

[CreateAssetMenu(fileName = "Interaccion", menuName = "Bestiario/Interacción entre criaturas")]
public class ElementReaction : ScriptableObject
{
    [Header("Criaturas y animaciones")]
    public ElementData elementA;
    public AccionInteraccion accionA;
    public ElementData elementB;
    public AccionInteraccion accionB;

    [Header("Interacción AR")]
    public TipoInteraccion tipo;
    [Min(0.01f)] public float interactionDistance = 0.3f;

    public bool EsBatalla => tipo == TipoInteraccion.DueloFisico || tipo == TipoInteraccion.BatallaMagica;

    public bool CanReact(ElementData a, ElementData b)
    {
        return a != null && b != null && ((a == elementA && b == elementB) || (a == elementB && b == elementA));
    }

    public AccionInteraccion AccionPara(ElementData criatura) => criatura == elementA ? accionA : accionB;
}

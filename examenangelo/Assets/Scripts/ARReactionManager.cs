using System;
using System.Collections.Generic;
using LolSuperman.Tarea08;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ARReactionManager : MonoBehaviour
{
    [Serializable]
    private sealed class GritoDeBatalla
    {
        public ElementData criatura;
        public AudioSource fuente;
    }

    [SerializeField] private ImageModelTrackingManager imageTracking;
    [SerializeField] private List<ElementReaction> reacciones = new List<ElementReaction>();

    [Header("Arrastra los AudioSource de los gritos de batalla")]
    [SerializeField] private List<GritoDeBatalla> gritosDeBatalla = new List<GritoDeBatalla>();

    private readonly List<TrackedModel> visibles = new List<TrackedModel>();
    private readonly List<(TrackedModel a, TrackedModel b, ElementReaction regla)> activos =
        new List<(TrackedModel, TrackedModel, ElementReaction)>();

    private void Update()
    {
        for (int i = activos.Count - 1; i >= 0; i--)
        {
            var par = activos[i];
            if (EnRango(par.a, par.b, par.regla)) continue;
            Terminar(par.a, par.b);
            activos.RemoveAt(i);
        }

        if (imageTracking == null || !imageTracking.isActiveAndEnabled) return;
        visibles.Clear();
        foreach (var modelo in imageTracking.ActiveModels)
            if (modelo.Elemento != null && modelo.Presentacion != null)
                visibles.Add(modelo);

        for (int i = 0; i < visibles.Count; i++)
            for (int j = i + 1; j < visibles.Count; j++)
                IntentarInteraccion(visibles[i], visibles[j]);
    }

    private void IntentarInteraccion(TrackedModel a, TrackedModel b)
    {
        if (!a.Presentacion.PuedeInteractuar || !b.Presentacion.PuedeInteractuar) return;
        foreach (var regla in reacciones)
        {
            if (regla == null || !regla.CanReact(a.Elemento, b.Elemento) || !EnRango(a, b, regla)) continue;

            a.Presentacion.IniciarInteraccion(regla.AccionPara(a.Elemento), b.transform);
            b.Presentacion.IniciarInteraccion(regla.AccionPara(b.Elemento), a.transform);
            activos.Add((a, b, regla));

            if (regla.EsBatalla)
            {
                ReproducirGrito(a.Elemento);
                ReproducirGrito(b.Elemento);
            }
            return;
        }
    }

    private static bool EnRango(TrackedModel a, TrackedModel b, ElementReaction regla)
    {
        if (a == null || b == null || !a.isActiveAndEnabled || !b.isActiveAndEnabled ||
            !a.Presentacion.isActiveAndEnabled || !b.Presentacion.isActiveAndEnabled) return false;

        Vector3 diferencia = a.PosicionDeTarjeta - b.PosicionDeTarjeta;
        return diferencia.sqrMagnitude <= regla.interactionDistance * regla.interactionDistance;
    }

    private void ReproducirGrito(ElementData elemento)
    {
        foreach (var grito in gritosDeBatalla)
        {
            if (grito.criatura != elemento || grito.fuente == null || grito.fuente.clip == null) continue;
            grito.fuente.Play();
            return;
        }
    }

    private static void Terminar(TrackedModel a, TrackedModel b)
    {
        if (a != null && a.Presentacion != null) a.Presentacion.TerminarInteraccion();
        if (b != null && b.Presentacion != null) b.Presentacion.TerminarInteraccion();
    }

    private void OnDisable() => ResetReaction();

    public void ResetReaction()
    {
        foreach (var par in activos) Terminar(par.a, par.b);
        activos.Clear();
    }
}

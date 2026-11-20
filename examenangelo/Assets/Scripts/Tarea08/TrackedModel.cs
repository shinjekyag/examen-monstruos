using System;
using UnityEngine;

namespace LolSuperman.Tarea08
{
    [DisallowMultipleComponent]
    public sealed class TrackedModel : MonoBehaviour
    {
        [SerializeField] private ModelInfo information;
        public ModelInfo Information => information;
        public ElementData Elemento { get; private set; }
        public PresentacionCriatura Presentacion { get; private set; }
        public Vector3 PosicionDeTarjeta => transform.parent != null ? transform.parent.position : transform.position;
        public event Action<TrackedModel> BecameUnavailable;

        public void SetInformation(ModelInfo value) => information = value;

        public void VincularCriatura(ElementData elemento)
        {
            Elemento = elemento;
            Presentacion = GetComponent<PresentacionCriatura>();
        }

        private void OnDisable() => BecameUnavailable?.Invoke(this);
    }
}

using UnityEngine;

public enum AccionInteraccion
{
    Baile,
    AtaqueFisico,
    AtaqueMagico,
    Gymbros
}

[DisallowMultipleComponent, RequireComponent(typeof(Animator))]
public sealed class PresentacionCriatura : MonoBehaviour
{
    [Header("Sonido al tocar")]
    [SerializeField] private AudioSource sonidoCaracteristico;

    [Header("Ataque mágico")]
    [SerializeField] private ParticleSystem particulasAtaqueMagico;
    [SerializeField] private Transform manoDelAtaque;
    [SerializeField, Range(0f, 0.95f)] private float momentoDelHechizo = 0.42f;
    [SerializeField, Min(0.05f)] private float duracionDelEfecto = 0.5f;
    [SerializeField, Min(0.01f)] private float escalaDelEfecto = 0.07f;

    private const float DuracionEntrada = 0.7f;
    private const float DuracionAtencion = 0.85f;
    private static readonly int Reposo = Animator.StringToHash("Reposo");
    private static readonly int Baile = Animator.StringToHash("Baile");
    private static readonly int AtaqueFisico = Animator.StringToHash("Ataque físico");
    private static readonly int AtaqueMagico = Animator.StringToHash("Ataque mágico");
    private static readonly int Gymbros = Animator.StringToHash("Gymbros");

    private Animator animador;
    private ParticleSystem efecto;
    private Vector3 posicionBase;
    private Quaternion rotacionBase;
    private Vector3 escalaBase;
    private Transform contrincante;
    private bool enInteraccion;
    private bool animandoAtencion;
    private bool usaMagia;
    private float horaDeEntrada;
    private float horaDeAtencion;
    private float apagarEfectoEn;
    private int ultimoCicloMagico;

    public bool PuedeInteractuar => isActiveAndEnabled && !enInteraccion && Time.time >= horaDeEntrada + DuracionEntrada;
    public bool EnInteraccion => enInteraccion;

    private void Awake() => animador = GetComponent<Animator>();

    private void OnEnable()
    {
        posicionBase = transform.localPosition;
        rotacionBase = transform.localRotation;
        escalaBase = transform.localScale;
        horaDeEntrada = Time.time;
        horaDeAtencion = float.NegativeInfinity;
        animandoAtencion = false;
        transform.localScale = escalaBase * 0.08f;
        animador.Play(Reposo, 0, 0);
    }

    private void OnDisable()
    {
        enInteraccion = false;
        usaMagia = false;
        contrincante = null;
        DetenerEfecto();
        transform.SetLocalPositionAndRotation(posicionBase, rotacionBase);
        transform.localScale = escalaBase;
    }

    public void MostrarAtencion()
    {
        horaDeAtencion = Time.time;
        if (!enInteraccion)
        {
            animandoAtencion = true;
            animador.Play(Baile, 0, 0);
        }
        if (sonidoCaracteristico != null && sonidoCaracteristico.clip != null)
            sonidoCaracteristico.Play();
    }

    public void IniciarInteraccion(AccionInteraccion accion, Transform oponente)
    {
        int estado = accion switch
        {
            AccionInteraccion.Baile => Baile,
            AccionInteraccion.AtaqueFisico => AtaqueFisico,
            AccionInteraccion.AtaqueMagico => AtaqueMagico,
            AccionInteraccion.Gymbros => Gymbros,
            _ => Reposo
        };
        enInteraccion = true;
        animandoAtencion = false;
        usaMagia = accion == AccionInteraccion.AtaqueMagico;
        contrincante = accion == AccionInteraccion.Gymbros ? null : oponente;
        ultimoCicloMagico = -1;
        animador.Play(estado, 0, 0);
    }

    public void TerminarInteraccion()
    {
        enInteraccion = false;
        usaMagia = false;
        contrincante = null;
        DetenerEfecto();
        if (isActiveAndEnabled) animador.CrossFadeInFixedTime(Reposo, 0.12f, 0);
    }

    private void LateUpdate()
    {
        float entrada = Mathf.Clamp01((Time.time - horaDeEntrada) / DuracionEntrada);
        float atencion = Mathf.Clamp01((Time.time - horaDeAtencion) / DuracionAtencion);
        if (animandoAtencion && atencion >= 1f)
        {
            animandoAtencion = false;
            animador.CrossFadeInFixedTime(Reposo, 0.12f, 0);
        }
        float salto = atencion < 1f ? Mathf.Sin(atencion * Mathf.PI) : 0f;
        Quaternion rotacion = rotacionBase;
        if (contrincante != null)
        {
            Vector3 objetivo = transform.parent != null
                ? transform.parent.InverseTransformPoint(contrincante.position)
                : contrincante.position;
            Vector3 direccion = Vector3.ProjectOnPlane(objetivo - posicionBase, Vector3.up);
            if (direccion.sqrMagnitude > 0.0001f)
                rotacion = Quaternion.LookRotation(direccion, Vector3.up) * rotacionBase;
        }

        transform.localScale = escalaBase * (Mathf.SmoothStep(0.08f, 1f, entrada) + salto * 0.045f);
        transform.localPosition = posicionBase + Vector3.up * (Mathf.Sin(entrada * Mathf.PI) * 0.025f + salto * 0.035f);
        transform.localRotation = rotacion * Quaternion.Euler(salto * 12f, (1f - entrada) * -35f, 0);

        if (efecto != null && Time.time >= apagarEfectoEn && efecto.isPlaying) DetenerEfecto();
        if (usaMagia) ActualizarHechizo();
    }

    private void ActualizarHechizo()
    {
        var estado = animador.GetCurrentAnimatorStateInfo(0);
        if (estado.shortNameHash != AtaqueMagico) return;
        int ciclo = Mathf.FloorToInt(estado.normalizedTime);
        if (ciclo == ultimoCicloMagico || estado.normalizedTime - ciclo < momentoDelHechizo) return;
        ultimoCicloMagico = ciclo;
        if (particulasAtaqueMagico == null || manoDelAtaque == null) return;

        if (efecto == null)
        {
            efecto = Instantiate(particulasAtaqueMagico, manoDelAtaque, false);
            efecto.name = particulasAtaqueMagico.name + " (mano)";
            efecto.transform.localPosition = Vector3.zero;
            efecto.transform.localScale *= escalaDelEfecto;
            foreach (var sistema in efecto.GetComponentsInChildren<ParticleSystem>(true))
            {
                var main = sistema.main;
                main.simulationSpace = ParticleSystemSimulationSpace.Local;
                main.scalingMode = ParticleSystemScalingMode.Local;
            }
        }

        efecto.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        efecto.Play(true);
        apagarEfectoEn = Time.time + duracionDelEfecto;
    }

    private void DetenerEfecto()
    {
        if (efecto != null) efecto.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}

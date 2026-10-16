using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 2f;
    [Tooltip("Velocidad máxima de giro en radianes por segundo")]
    public float velocidadGiro = 3f;

    [Header("Percepción")]
    public float radio = 3f;
    public float radioSeparacion = 1f;
    [Tooltip("Ángulo a cada lado del frente (80 = 160° totales)")]
    public float anguloVista = 120f;
    public LayerMask capaObjetos;
    public float distanciaObstaculo = 2f;

    [Header("Pesos")]
    public float pesoCohesion = 0.8f;
    public float pesoAlineacion = 1.2f;
    public float pesoSeparacion = 2f;
    public float pesoLimite = 2f;
    public float pesoObstaculo = 4f;
    public float pesoObjetivo = 0f;

    [Header("Objetivo opcional (déjalo vacío si no lo usas)")]
    public Transform objetivo;

    BoidManager manager;
    Vector3 direction;

    private void Awake()
    {
        direction = transform.forward;
    }

    private void Start()
    {
        manager = BoidManager.Instance;
    }

    private void Update()
    {
        if (manager == null) return;

        CalcularVecinos(out Vector3 cohesion, out Vector3 alineacion, out Vector3 separacion);
        Vector3 limite = CalcularLimite();
        Vector3 obstaculo = CalcularObstaculo();
        Vector3 haciaObjetivo = CalcularObjetivo();

        Vector3 dirObjetivo =
            cohesion * pesoCohesion +
            alineacion * pesoAlineacion +
            separacion * pesoSeparacion +
            limite * pesoLimite +
            obstaculo * pesoObstaculo +
            haciaObjetivo * pesoObjetivo;

        if (dirObjetivo.sqrMagnitude > 0.0001f)
        {
            dirObjetivo.Normalize();
            // Gira como máximo velocidadGiro rad/s y mantiene direction normalizada
            direction = Vector3.RotateTowards(direction, dirObjetivo, velocidadGiro * Time.deltaTime, 0f);
        }

        transform.forward = direction;
        transform.position += direction * velocidad * Time.deltaTime;
    }

    // Cohesión, alineación y separación en un solo recorrido
    private void CalcularVecinos(out Vector3 cohesion, out Vector3 alineacion, out Vector3 separacion)
    {
        Vector3 centro = Vector3.zero;
        Vector3 dirMedia = Vector3.zero;
        Vector3 sep = Vector3.zero;
        int numVecinos = 0;

        float sqrRadio = radio * radio;
        float sqrSep = radioSeparacion * radioSeparacion;
        float sqrMax = Mathf.Max(sqrRadio, sqrSep);

        foreach (Boid otro in manager.boids)
        {
            if (otro == null || otro == this) continue;

            Vector3 diff = transform.position - otro.transform.position;
            float sqrDist = diff.sqrMagnitude;

            if (sqrDist > sqrMax || sqrDist < 0.0001f) continue;

            // Separación: más fuerte cuanto más cerca
            if (sqrDist < sqrSep)
            {
                sep += diff / sqrDist;
            }

            // Cohesión y alineación: solo vecinos dentro del radio y del campo de visión
            if (sqrDist < sqrRadio && EnVista(-diff))
            {
                centro += otro.transform.position;
                dirMedia += otro.direction;
                numVecinos++;
            }
        }

        if (numVecinos > 0)
        {
            centro /= numVecinos;
            cohesion = (centro - transform.position).normalized;
            alineacion = dirMedia.normalized;
        }
        else
        {
            cohesion = Vector3.zero;
            alineacion = Vector3.zero;
        }

        separacion = sep.sqrMagnitude > 0.0001f ? sep.normalized : Vector3.zero;
    }

    // Vuelve hacia el centro de forma gradual al salir de la zona
    private Vector3 CalcularLimite()
    {
        Vector3 haciaCentro = manager.transform.position - transform.position;
        float dist = haciaCentro.magnitude;

        if (dist < manager.radioZona) return Vector3.zero;

        float exceso = (dist - manager.radioZona) / manager.radioZona;
        return haciaCentro.normalized * Mathf.Clamp01(exceso + 0.2f);
    }

    // Esquiva obstáculos usando la normal de la superficie
    private Vector3 CalcularObstaculo()
    {
        if (Physics.SphereCast(transform.position, 0.2f, direction, out RaycastHit hit, distanciaObstaculo, capaObjetos))
        {
            // Desliza a lo largo de la superficie y se aleja de ella
            Vector3 deslizar = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
            Vector3 escape = (hit.normal + deslizar).normalized;

            // Más fuerte cuanto más cerca está el obstáculo
            float urgencia = 1f - Mathf.Clamp01(hit.distance / distanciaObstaculo);
            return escape * Mathf.Max(urgencia, 0.3f);
        }
        return Vector3.zero;
    }

    // Objetivo opcional; si no hay Transform asignado no hace nada
    private Vector3 CalcularObjetivo()
    {
        if (objetivo == null) return Vector3.zero;
        return (objetivo.position - transform.position).normalized;
    }

    // Recibe el vector hacia el otro boid
    private bool EnVista(Vector3 haciaOtro)
    {
        return Vector3.Angle(direction, haciaOtro) < anguloVista;
    }
}
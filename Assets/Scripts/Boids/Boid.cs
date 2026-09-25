using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Movement")]
    public float velocidad = 2f;
    public float velocidadGiro = 2f;

    [Header("Perception")]
    public float radio = 4f;
    public float radioSeparacion = 0.5f;
    public float anguloVista = 80f;

    [Header("Pesos")]
    public float pesoCoesion = 1f;
    public float pesoAlineacion = 1f;
    public float pesoSeparacion = 1.5f;
    public float pesoLimite = 4f;
    public float pesoObstaculo = 4f;

    [Header("Capas")]
    public LayerMask capaObjetos;

    BoidManager manager;

    private void Start()
    {
        manager = BoidManager.instance;
    }

    private void Update()
    {
        Vector3 coesion = CalcularCoesion();
        Vector3 alineacion = CalcularAlineacion();
        Vector3 separacion = CalcularSeparacio();
        Vector3 limite = CalcularLimite();
        Vector3 obtaculo = CalcularObstaculo();

        Vector3 direccionObjetivo = coesion*pesoCoesion + alineacion*pesoAlineacion + separacion*pesoSeparacion + limite*pesoLimite + obtaculo*pesoObstaculo;
        direccionObjetivo.Normalize();

        transform.forward = Vector3.Lerp(transform.forward, direccionObjetivo, velocidadGiro * Time.deltaTime);

        transform.position += transform.forward * velocidad * Time.deltaTime;
    }

    public Vector3 CalcularCoesion()
    {
        Vector3 centro = Vector3.zero;
        int newVecino = 0;

        foreach (Boid boidActual in manager.boidsList)
        {
            if (boidActual == this) continue;
            if (Vector3.SqrMagnitude(transform.position - boidActual.transform.position) < radio * radio && EnVista(boidActual))
            {
                centro += boidActual.transform.position;
                newVecino++;
            }
        }
        centro /= newVecino;
        return (centro - transform.position).normalized;
    }
    public Vector3 CalcularAlineacion()
    {
        Vector3 alineacion = Vector3.zero;
        int newVecino = 0;

        foreach (Boid boidActual in manager.boidsList)
        {
            if (boidActual == this) continue;
            if (Vector3.SqrMagnitude(transform.position - boidActual.transform.position) < radio * radio && EnVista(boidActual))
            {
                alineacion += boidActual.transform.forward;
                newVecino++;
            }
        }
        alineacion /= newVecino;
        return alineacion.normalized;
    }
    public Vector3 CalcularSeparacio()
    {
        Vector3 separacion = Vector3.zero;

        foreach (Boid boidActual in manager.boidsList)
        {
            if (boidActual == this) continue;
            if (Vector3.SqrMagnitude(transform.position - boidActual.transform.position) < radioSeparacion * radioSeparacion)
            {
                separacion += (transform.position - boidActual.transform.position).normalized;
            }
        }
        return separacion;
    }
    public Vector3 CalcularLimite()
    {
        if (Vector3.Distance(manager.transform.position, transform.position) < manager.radioZona) return Vector3.zero;

        return (manager.transform.position - transform.position).normalized;
    }
    public Vector3 CalcularObstaculo()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, radioSeparacion, capaObjetos))
        {
            Vector3 direccionEscape = Vector3.zero;

            direccionEscape -= transform.forward;

            if (Physics.Raycast(transform.position, -transform.up, out hit, radioSeparacion, capaObjetos))
            {
                direccionEscape += transform.up;
            }
            if (Physics.Raycast(transform.position, transform.up, out hit, radioSeparacion, capaObjetos))
            {
                direccionEscape -= transform.up;
            }
            if (Physics.Raycast(transform.position, -transform.right, out hit, radioSeparacion, capaObjetos))
            {
                direccionEscape += transform.right;
            }
            if (Physics.Raycast(transform.position, transform.right, out hit, radioSeparacion, capaObjetos))
            {
                direccionEscape -= transform.right;
            }

            return hit.point;
        }
        return Vector3.zero;
    }

    bool EnVista(Boid otro)
    {
        Vector3 direccionOtro = otro.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, direccionOtro);

        if (angle < anguloVista) return true;
        return false;
    }
}

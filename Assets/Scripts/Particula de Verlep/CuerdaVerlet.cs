using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]

public class CuerdaVerlet : MonoBehaviour
{
    [Min(2)]
    [SerializeField] uint numPuntos = 10;
    [SerializeField] float distanciaPunstos = 0.2f;

    [SerializeField] Vector3 gravedad = new Vector3(0f, -9.8f, 0f);

    [Range(1f, 100f)]
    [SerializeField]
    int numIteraciones = 10;

    List<VerletParticle> listaParticulas;

    LineRenderer lineRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numIteraciones;

        listaParticulas = new List<VerletParticle>();
        for (int i = 0; i < numPuntos; i++)
        {
            Vector3 distanciaInicio = new Vector3(i * distanciaPunstos, 0f, 0f);
            listaParticulas.Add(new VerletParticle(transform.position + distanciaInicio, i == 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        DibujarLista();
    }

    private void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;

        listaParticulas[0].posicionActual = transform.position;
        // if (esquina)

        for (int i = 0;i < numPuntos;i++)
        {
            listaParticulas[i].Simulacion(gravedad, fdt);
        }

        for (int iter = 0; iter < numIteraciones; iter++)
        {
            for (int i = 0; i < numPuntos; i++)
            {
                ResolucionConstrains(listaParticulas[i - 1], listaParticulas[i]);
            }
        }
    }

    private void ResolucionConstrains(VerletParticle particleA, VerletParticle particleB)
    {
        Vector3 vAB = particleB.posicionActual - particleA.posicionActual;
        float distancia = vAB.magnitude;
        if (distancia == distanciaPunstos) return;
        float error = distancia - distanciaPunstos;
        Vector3 direccionCorreccion = vAB.normalized * error;

        if (particleA.puntoFijo)
        {
            particleB.posicionActual -= direccionCorreccion;
        }
        else if (particleB.puntoFijo)
        {
            particleA.posicionActual += direccionCorreccion;
        }
        else
        {
            particleB.posicionActual = direccionCorreccion * 0.5f;
            particleA.posicionActual = direccionCorreccion * 0.5f;
        }
    }

    void DibujarLista()
    {
        for (int i = 0; i < listaParticulas.Count; i++)
        {
            lineRenderer.SetPosition(i, listaParticulas[i].posicionActual);
        }
    }
}

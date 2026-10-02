using UnityEngine;

public class DosParticulasVerlet : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    VerletParticle particleA;
    VerletParticle particleB;
    [SerializeField] Vector3 gravedad = new Vector3 (0f, -9.8f, 0f);
    [SerializeField] float distanciaPunstos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particleA = new VerletParticle(pointA.position, false);
        particleB = new VerletParticle(pointB.position, true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;
        particleA.Simulacion(gravedad, fdt);
        particleB.Simulacion(gravedad, fdt);
        ResolucionConstrains(particleA, particleB);
        pointA.position = particleA.posicionActual;
        pointB.position = particleB.posicionActual;
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
}

using System;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class VerletParticle
{
    public Vector3 posicionActual;
    public Vector3 posicionPasada;
    public bool puntoFijo;
    public bool esquina;
    
    public VerletParticle(Vector3 pos, bool pFijo)
    {
        posicionActual = pos;
        posicionPasada = pos;
        puntoFijo = pFijo;
    }

    public void Simulacion(Vector3 aceleracion, float fdt)
    {
        if (puntoFijo) return;
        Vector3 movimiento = posicionActual - posicionPasada;
        Vector3 posicionNueva = posicionActual + movimiento + aceleracion * fdt * fdt;
        posicionPasada = posicionActual;
        posicionActual = posicionNueva;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class BoidManager : MonoBehaviour
{
    public static BoidManager instance;
    [SerializeField] private GameObject boidPrefab;
    public float radioZona;

    [Header("Spawn inicio")]
    public int cantidad = 10;
    public int radio = 10;

    public List<Boid> boidsList = new List<Boid>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < cantidad; i++)
        {
            GameObject go = Instantiate(boidPrefab, this.transform.position + Random.insideUnitSphere * radio, Random.rotation);
            boidsList.Add(go.GetComponent<Boid>());
        }
    }
}

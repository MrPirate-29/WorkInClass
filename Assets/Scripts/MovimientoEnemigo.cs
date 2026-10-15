using UnityEngine;

public class MovimientoEnemigo : MonoBehaviour
{
    public float velocidad = 1.5f;
    public float distancia = 2f;

    private Vector3 puntoInicial;
    private int direccion = 1;

    void Start()
    {
        puntoInicial = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector2.right * direccion * velocidad * Time.deltaTime);

        if (transform.position.x >= puntoInicial.x + distancia)
            direccion = -1;
        else if (transform.position.x <= puntoInicial.x - distancia)
            direccion = 1;

        transform.localScale = new Vector3(Mathf.Sign(-direccion), transform.localScale.y, 1);
    }
}

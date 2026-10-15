using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Jugador : MonoBehaviour
{
    public float velocidad = 3f;
    private Rigidbody2D rb;
    private float movimiento;
    public float alturaSalto = 4f;
    private bool esPiso;
    public Transform comprobadorPiso;
    public float radioComprobadorPiso = 0.1f;
    public LayerMask layerPiso;
    private Animator animator;
    private int cantAbejas = 0;
    public TMP_Text textoAbejas;
    private bool enRetroceso = false;
    public AudioSource audioSource;
    public AudioClip audioPuerquito;
    public AudioClip audioCaracol;
    public AudioClip audioAbeja;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (textoAbejas == null)
        {
            textoAbejas = FindAnyObjectByType<TMP_Text>();
        }
        if (textoAbejas == null)
        {
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            var textoGO = new GameObject("TxTAbejas");
            textoGO.transform.SetParent(canvasGO.transform, false);
            textoAbejas = textoGO.AddComponent<TextMeshProUGUI>();
            textoAbejas.text = "0";
            textoAbejas.fontSize = 48;
            textoAbejas.color = Color.white;
            var rt = textoGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(20, -20);
            rt.sizeDelta = new Vector2(200, 60);
        }
    }

    void Update()
    {
        if (transform.position.y < -6f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (!enRetroceso)
        {
            movimiento = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(movimiento * velocidad, rb.linearVelocity.y);
            if (movimiento != 0) transform.localScale = new Vector3(Mathf.Sign(movimiento), 1, 1);
        }
        if (Input.GetButtonDown("Jump") && esPiso)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, alturaSalto);
        animator.SetFloat("Velocidad", Mathf.Abs(movimiento));
        animator.SetFloat("VelocidadVertical", rb.linearVelocity.y);
        animator.SetBool("estarEnPiso", esPiso);
    }

    public void FixedUpdate()
    {
        esPiso = Physics2D.OverlapCircle(comprobadorPiso.position, radioComprobadorPiso, layerPiso);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("abejita"))
        {
            if (audioSource != null && audioAbeja != null)
                audioSource.PlayOneShot(audioAbeja);
            Destroy(collision.gameObject);
            cantAbejas++;
            if (textoAbejas != null)
                textoAbejas.text = "" + cantAbejas;
        }
        if (collision.transform.CompareTag("puerquito"))
        {
            if (audioSource != null && audioPuerquito != null)
                audioSource.PlayOneShot(audioPuerquito);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (collision.transform.CompareTag("caracol"))
        {
            if (audioSource != null && audioCaracol != null)
                audioSource.PlayOneShot(audioCaracol);
            enRetroceso = true;
            Vector2 arrastre = (rb.position - (Vector2)collision.transform.position).normalized * 3;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(arrastre, ForceMode2D.Impulse);
            Collider2D[] colliders = collision.GetComponents<Collider2D>();
            foreach (Collider2D col in colliders)
                col.enabled = false;
            collision.GetComponent<Animator>().enabled = true;
            Destroy(collision.gameObject, 0.4f);
            Invoke(nameof(QuitarRetroceso), 0.2f);
        }
    }

    void QuitarRetroceso()
    {
        enRetroceso = false;
    }
}

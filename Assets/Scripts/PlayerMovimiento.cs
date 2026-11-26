using UnityEngine;

// Mueve al jugador con WASD (o flechas) según hacia dónde mira la cámara,
// usando un CharacterController, con gravedad y salto con la barra espaciadora.
[RequireComponent(typeof(CharacterController))]
public class PlayerMovimiento : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform camara;

    private CharacterController controlador;

    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;

    [Header("Gravedad")]
    [SerializeField] private float GravedadDelJugador = -9.81f;

    [Header("Salto")]
    [SerializeField] private float alturaSalto = 1.2f; // Altura máxima del salto, en metros

    private Vector3 velocidadVertical;

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();

        // Si no se asignó la cámara en el Inspector, se usa la cámara principal
        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;
    }

    private void Update()
    {
        Vector3 desplazamientoXZ = MoverJugadorEnPlano();
        AplicarGravedadYSalto();

        // Un solo Move por frame: así el CharacterController detecta bien si está tocando el suelo
        controlador.Move(desplazamientoXZ + velocidadVertical * Time.deltaTime);
    }

    private Vector3 MoverJugadorEnPlano()
    {
        if (camara == null) return Vector3.zero;

        // Capturamos las teclas (WASD y flechas): valores -1, 0 o 1
        float valorHorizontal = Entrada.Horizontal();
        float valorVertical = Entrada.Vertical();

        // Hacia dónde mira la cámara: adelante/atrás y derecha/izquierda
        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;

        // Eliminamos el eje Y para que el jugador se mueva solo sobre el suelo
        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;

        // Normalizamos para que ambas direcciones tengan el mismo largo
        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        // Combinamos ambas direcciones (así funcionan también las diagonales)
        Vector3 direccionPlano = derechaCamara * valorHorizontal + adelanteCamara * valorVertical;

        // Evita que al presionar dos teclas en diagonal (por ejemplo W + D) el jugador vaya más rápido
        if (direccionPlano.sqrMagnitude > 0.0001f)
            direccionPlano.Normalize();

        // Multiplicamos por Time.deltaTime para que la velocidad dependa del tiempo y no de los FPS
        return direccionPlano * (velocidadMovimiento * Time.deltaTime);
    }

    private void AplicarGravedadYSalto()
    {
        if (controlador.isGrounded)
        {
            // En el suelo dejamos una pequeña fuerza hacia abajo para que no se acumule la caída
            if (velocidadVertical.y < 0f)
                velocidadVertical.y = -2f;

            // Salto: velocidad inicial necesaria para llegar a alturaSalto (v = raíz de 2 * altura * gravedad)
            if (Entrada.Saltar())
                velocidadVertical.y = Mathf.Sqrt(alturaSalto * -2f * GravedadDelJugador);
        }

        // Si choca con algo por arriba mientras sube, empieza a caer
        if ((controlador.collisionFlags & CollisionFlags.Above) != 0 && velocidadVertical.y > 0f)
            velocidadVertical.y = 0f;

        // La gravedad actúa siempre
        velocidadVertical.y += GravedadDelJugador * Time.deltaTime;
    }
}

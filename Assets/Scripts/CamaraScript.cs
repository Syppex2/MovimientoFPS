using UnityEngine;

// Cámara en primera persona: el mouse en X gira al jugador (izquierda/derecha)
// y el mouse en Y inclina la cámara (arriba/abajo).
public class CamaraScript : MonoBehaviour
{
    [SerializeField] public float Sensibilidad = 150f; // Sensibilidad del mouse, se ajusta en el Inspector
    [SerializeField] public Transform Player;          // Referencia al jugador que gira junto a la cámara
    [SerializeField] public float LimiteVertical = 80f; // Cuántos grados puede mirar hacia arriba o hacia abajo
    [SerializeField] public bool MostrarValoresEnConsola = false; // Activar para revisar los valores del mouse

    private float xRotation = 0f; // Giro horizontal acumulado
    private float yRotation = 0f; // Inclinación vertical acumulada

    private void Start()
    {
        BloquearCursor(true); // El cursor queda centrado y oculto mientras se juega
    }

    private void Update()
    {
        // Escape libera el cursor (por ejemplo, para salir del modo Play); un clic lo vuelve a bloquear
        if (Entrada.Escape()) BloquearCursor(false);
        if (Entrada.Clic() && Cursor.lockState != CursorLockMode.Locked) BloquearCursor(true);

        // Con el cursor libre la cámara no se mueve
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Capturamos los valores del mouse. Se multiplican por la sensibilidad
        // y por Time.deltaTime para que el giro dependa del tiempo y no de los FPS
        float valorX = Entrada.MouseX() * Sensibilidad * Time.deltaTime;
        float valorY = Entrada.MouseY() * Sensibilidad * Time.deltaTime;

        // Acumulamos los valores en las variables de rotación
        xRotation += valorX;
        yRotation -= valorY; // Se resta para que mover el mouse hacia arriba mire hacia arriba

        // Limitamos la rotación vertical para que la cámara no dé la vuelta completa
        yRotation = Mathf.Clamp(yRotation, -LimiteVertical, LimiteVertical);

        // La cámara solo se inclina en vertical; el giro horizontal lo hace el jugador
        transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);

        if (Player != null)
        {
            Player.Rotate(Vector3.up * valorX, Space.Self);
        }
        else
        {
            Debug.LogWarning("Cámara: asigna el Player en el Inspector para que la cámara pueda girar al jugador");
        }

        if (MostrarValoresEnConsola)
            Debug.Log($"MouseEnX={valorX:F1}, MouseEnY={valorY:F1}");
    }

    private void BloquearCursor(bool bloquear)
    {
        Cursor.lockState = bloquear ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !bloquear;
    }
}

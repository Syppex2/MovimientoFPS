using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

// Lee el teclado y el mouse en un solo lugar.
// Si el proyecto usa el Input Manager clásico (el de las clases), se usa Input.GetAxis.
// Si el proyecto solo tiene activo el Input System nuevo (pasa en Unity 6), se usa ese,
// así los scripts funcionan sin tener que cambiar la configuración del proyecto.
public static class Entrada
{
    // Eje horizontal: A/D o flechas izquierda/derecha. Devuelve -1, 0 o 1.
    public static float Horizontal()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetAxisRaw("Horizontal");
#elif ENABLE_INPUT_SYSTEM
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return 0f;
        float valor = 0f;
        if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) valor += 1f;
        if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) valor -= 1f;
        return valor;
#else
        return 0f;
#endif
    }

    // Eje vertical: W/S o flechas arriba/abajo. Devuelve -1, 0 o 1.
    public static float Vertical()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetAxisRaw("Vertical");
#elif ENABLE_INPUT_SYSTEM
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return 0f;
        float valor = 0f;
        if (teclado.wKey.isPressed || teclado.upArrowKey.isPressed) valor += 1f;
        if (teclado.sKey.isPressed || teclado.downArrowKey.isPressed) valor -= 1f;
        return valor;
#else
        return 0f;
#endif
    }

    // Movimiento del mouse en X (izquierda/derecha).
    public static float MouseX()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetAxis("Mouse X");
#elif ENABLE_INPUT_SYSTEM
        // El Input System entrega píxeles; se escala para que se parezca al Input Manager.
        return Mouse.current != null ? Mouse.current.delta.ReadValue().x * 0.1f : 0f;
#else
        return 0f;
#endif
    }

    // Movimiento del mouse en Y (arriba/abajo).
    public static float MouseY()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetAxis("Mouse Y");
#elif ENABLE_INPUT_SYSTEM
        return Mouse.current != null ? Mouse.current.delta.ReadValue().y * 0.1f : 0f;
#else
        return 0f;
#endif
    }

    // Barra espaciadora (botón "Jump" del Input Manager) presionada en este frame.
    public static bool Saltar()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetButtonDown("Jump");
#elif ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        return false;
#endif
    }

    // Tecla Escape presionada en este frame.
    public static bool Escape()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Escape);
#elif ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return false;
#endif
    }

    // Clic izquierdo presionado en este frame.
    public static bool Clic()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetMouseButtonDown(0);
#elif ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return false;
#endif
    }
}

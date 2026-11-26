# Movimiento FPS en Unity

Proyecto de prueba con un personaje en primera persona: se mueve con WASD según hacia dónde mira la cámara, mira con el mouse, tiene gravedad y choca con los obstáculos. Está basado en las clases de Movimiento de personaje y Manejo de cámara.

## Contenido

| Archivo | Qué hace |
|---|---|
| `Assets/Scripts/PlayerMovimiento.cs` | Mueve al jugador con WASD o flechas usando un CharacterController, relativo a la cámara y con gravedad. |
| `Assets/Scripts/CamaraScript.cs` | Cámara en primera persona: el mouse gira al jugador y la cámara, con límite vertical y cursor bloqueado. |
| `Assets/Scripts/Entrada.cs` | Lee teclado y mouse. Usa `Input.GetAxis` (Input Manager clásico) y, si el proyecto solo tiene el Input System nuevo, usa ese. |
| `Assets/Editor/ConstructorEscena.cs` | Arma la escena completa con un clic desde el menú **Proyecto > Crear escena de prueba**. |
| `.gitignore` | Excluye de GitHub las carpetas que Unity regenera (Library, Temp, Logs, etc.). |
| `ignore.conf` | Lo mismo, pero para Unity Version Control. |

## Cómo armar el proyecto

Los scripts ya están en este repositorio (https://github.com/Syppex2/MovimientoFPS). Lo único que falta es el proyecto de Unity, que se crea en tu computador.

1. En **Unity Hub**, crea un proyecto nuevo con la plantilla **3D (Built-In Render Pipeline)** o **Universal 3D**. Cualquiera de las dos funciona. Después cierra Unity.
2. Abre una terminal en la carpeta raíz del proyecto (la que contiene `Assets`, `Packages` y `ProjectSettings`) y ejecuta, una línea a la vez:

```
git init
git branch -M main
git remote add origin https://github.com/Syppex2/MovimientoFPS.git
git pull origin main --allow-unrelated-histories
```

   El `git pull` descarga los scripts, este README, el `.gitignore` y el `ignore.conf` dentro de tu proyecto.
3. Abre el proyecto en Unity y espera que termine de compilar.
4. En el menú superior, elige **Proyecto > Crear escena de prueba**. Se crea y guarda `Assets/Scenes/EscenaPrincipal.unity`.
5. Presiona **Play**.

Si `git pull` dice que algún archivo local sería sobrescrito (por ejemplo `.gitignore` o `ignore.conf`), borra ese archivo de tu carpeta y vuelve a ejecutar el `git pull`.

## Controles

| Acción | Tecla |
|---|---|
| Moverse | W A S D o flechas |
| Mirar | Mouse |
| Liberar el cursor (por ejemplo, para salir de Play) | Escape |
| Volver a bloquear el cursor | Clic izquierdo |

## Ajustes en el Inspector

- **Player > PlayerMovimiento**: `Velocidad Movimiento` (5 por defecto) y `Gravedad Del Jugador` (-9.81).
- **Main Camera > CamaraScript**: `Sensibilidad` (150 por defecto; súbela o bájala si el giro se siente lento o rápido), `Limite Vertical` (80 grados) y `Mostrar Valores En Consola` para ver los valores del mouse como en la clase.

## Si algo no funciona

- **No aparece el menú "Proyecto"**: la carpeta `Editor` debe quedar dentro de `Assets` (`Assets/Editor/ConstructorEscena.cs`). Revisa también que la consola no tenga errores en rojo.
- **Los objetos se ven rosados**: el proyecto usa un render pipeline distinto (por ejemplo HDRP). Usa la plantilla 3D o Universal 3D.
- **El personaje no se mueve**: revisa que el Player tenga el componente `CharacterController` y que en `PlayerMovimiento` esté asignada la cámara.
- **La cámara no gira al jugador**: en `CamaraScript`, el campo `Player` debe tener asignado el objeto Player.

## Subirlo a GitHub (como en la clase)

El repositorio usa la plantilla **.gitignore: Unity**. Con los pasos de "Cómo armar el proyecto" ya quedó enlazado (`git init`, `git remote add` y `git pull`). Cuando la escena funcione, sube el proyecto completo desde la misma terminal:

```
git add .
git status
git commit -m "Primer respaldo del proyecto Unity"
git push -u origin main
git log --oneline -5
```

Antes del commit, revisa con `git status` que no aparezcan `Library` ni `Temp`. En GitHub deberían quedar `Assets`, `Packages` y `ProjectSettings`. La primera vez que hagas `push`, Git te pedirá iniciar sesión en GitHub.

Para recuperarlo en otro equipo: `git clone https://github.com/Syppex2/MovimientoFPS.git` y abre la carpeta desde Unity Hub con **Add project from disk**. La primera apertura tarda más porque Unity vuelve a generar la carpeta `Library`.

## Subirlo a Unity Version Control

En Unity Cloud ya existe el repositorio **MovimientoFPS**, dentro del proyecto **eva3**. Para conectarlo:

1. Abre el proyecto en Unity y vincúlalo al proyecto de Unity Cloud: **Edit > Project Settings > Services**, inicia sesión con tu cuenta y elige el proyecto **eva3**.
2. Entra en el menú **Window** y busca **Unity Version Control**.
3. Elige usar un repositorio existente: **MovimientoFPS** (aparece como `eva3/MovimientoFPS`).
4. Haz el primer **Check in** de los cambios con un comentario, por ejemplo "Primer respaldo del proyecto Unity".

# WorkInClass — Juego 2D (Unity)

Proyecto de juego 2D desarrollado en **Unity 6.3 LTS** con arte *Legacy Fantasy*, construido siguiendo un flujo de trabajo **Git Flow** con avances registrados por commits.

## Descripción

Plataformero 2D con:
- Personaje con movimiento horizontal, salto y colisión contra el piso.
- Animaciones (idle, correr, saltar).
- Escenario con background escalado a cámara y piso basado en Tilemap con colisión.
- Cámara configurada para seguir al jugador.
- Enemigos (abejas) con colisión.

## Estructura del proyecto

```
Assets/
├── Animaciones/     # Clips y controllers de animación
├── Prefab/          # Prefabs (Jugador, Abeja)
├── Scenes/          # SampleScene
├── Scripts/         # Jugador.cs, Camara.cs
├── Settings/        # Configuración de URP 2D
└── Sprites/         # Sprites Legacy Fantasy
```

## Flujo de trabajo (Git Flow)

- `main` — rama estable con los hitos integrados.
- Ramas de desarrollo por funcionalidad, integradas a `main` mediante merge:
  - `Idle_and_Scene` — escena, físicas, piso, movimiento, salto y animaciones.
  - `Camara_y_Abeja` — configuración de la cámara y enemigos (abejas).

## Requisitos

- Unity 6.3 LTS (6000.3.16f1) o superior.
- Universal Render Pipeline (URP) 2D.

## Cómo abrir

1. Clonar el repositorio.
2. Abrir la carpeta del proyecto desde **Unity Hub**.
3. Abrir la escena `Assets/Scenes/SampleScene.unity`.

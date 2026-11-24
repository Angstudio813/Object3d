# Object3D — Planta de Reciclaje (Unity)

Escena 3D interactiva de una **planta de reciclaje** con controlador en primera persona,
generación procedural del escenario y contenedores de reciclaje. Proyecto de Unity (URP/HDRP
según configuración del editor) pensado para visualización 3D / recorrido virtual.

## Requisitos

| Elemento | Versión |
|---|---|
| Unity Editor | **6000.6.10f1** (Unity 6) |
| Render Pipeline | El definido en `ProjectSettings` |
| Sistema de entrada | Input System (`com.unity.inputsystem`) |

> Instala la versión exacta del editor indicada en `ProjectSettings/ProjectVersion.txt`.
> Si usas otra versión de Unity 6, el editor pedirá actualizar el proyecto (es normal).

## Cómo abrir el proyecto

1. Clona el repositorio:
   ```bash
   git clone https://github.com/Angstudio813/Object3d.git
   ```
2. Abre **Unity Hub → Add → Add project from disk** y selecciona la carpeta clonada.
3. Unity regenerará automáticamente `Library/`, `Temp/`, `Logs/` y los archivos `.csproj`
   (no se versionan, por eso el repo es ligero).
4. Abre la escena principal: `Assets/Scenes/EscenarioReciclaje.unity`
5. Pulsa **Play**.

## Estructura del repositorio

```
Assets/
├── Editor/                      # Herramientas de editor (menú "Herramientas")
│   ├── CrearControladorFPSEditor.cs   # Crea el jugador FPS en la escena
│   └── GeneradorPlantaReciclaje.cs    # Genera toda la planta automáticamente
├── Generated/                   # Terreno y layers generados por la herramienta
├── materials/                   # Materiales base (colores)
├── Models/                      # Modelos 3D (Car, Home) en FBX + texturas
├── Prefabs/                     # Contenedores de reciclaje (Metal, Papel, Plásticos, Vidrio)
├── Scenes/                      # EscenarioReciclaje (principal) y SampleScene
├── Scripts/
│   └── ControladorFPS.cs        # Movimiento y cámara en primera persona
└── Textures/
Packages/                        # manifest.json + packages-lock.json (dependencias)
ProjectSettings/                 # Configuración del proyecto Unity
```

## Scripts principales

- **`ControladorFPS.cs`** — Movimiento del jugador con `CharacterController`:
  `WASD` para moverse, ratón para mirar, `Esc` libera el cursor, clic izquierdo lo vuelve a capturar.
- **`GeneradorPlantaReciclaje.cs`** — Menú **`Herramientas → Generar Planta de Reciclaje (automatico)`**.
  Crea carreteras, fábrica, cinta transportadora, contenedores, almacén, oficina, decoración,
  terreno, materiales y prefabs.
- **`CrearControladorFPSEditor.cs`** — Utilidad de editor para instanciar el jugador FPS.

## Qué NO se sube (y por qué)

`Library/`, `Temp/`, `Logs/`, `Obj/`, `UserSettings/`, `Build/`, los `.csproj`, `.sln`/`.slnx`
y los archivos de IDE se generan solos al abrir el proyecto. Súbelos y el repo pasaría de
~10 MB a más de **1.9 GB** sin ningún beneficio. Todo eso ya está cubierto por `.gitignore`.

## Notas para el siguiente desarrollador

- Los archivos `.meta` **sí** se versionan: son obligatorios para que las referencias internas
  de Unity no se rompan entre máquinas.
- Tras clonar, la primera apertura tarda un poco mientras Unity importa assets y paquetes.
- Si el editor pide reimportar, acepta: no hay pérdida de datos.

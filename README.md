# Object3D — Planta de Reciclaje (Unity)

Repositorio con **dos proyectos de Unity** sobre la misma temática: una planta de reciclaje
en 3D con controlador en primera persona, generación procedural del escenario y contenedores
de reciclaje.

| Carpeta | Proyecto | Descripción |
|---|---|---|
| `/` (raíz) | **Planta de Reciclaje** (escritorio) | Recorrido FPS por la planta, generada por herramienta de editor |
| `Objeto3D_VR/` | **Objeto3D_VR** | Misma planta adaptada a **realidad virtual** (Oculus / XR) |

## Requisitos

| Elemento | Versión |
|---|---|
| Unity Editor | **6000.6.10f1** (Unity 6) |
| Sistema de entrada | Input System (`com.unity.inputsystem`) |
| VR (solo `Objeto3D_VR`) | Paquetes XR / Oculus incluidos en `Packages/manifest.json` |

> Instala la versión exacta del editor indicada en cada `ProjectSettings/ProjectVersion.txt`.
> Si usas otra versión de Unity 6, el editor pedirá actualizar el proyecto (es normal).

## Cómo abrir un proyecto

1. Clona el repositorio:
   ```bash
   git clone https://github.com/Angstudio813/Object3d.git
   ```
2. Abre **Unity Hub → Add → Add project from disk** y selecciona **la carpeta del proyecto**:
   - Proyecto escritorio → la raíz del repo (donde está `ProjectSettings/`)
   - Proyecto VR → la carpeta `Objeto3D_VR/`
3. Unity regenerará automáticamente `Library/`, `Temp/`, `Logs/` y los `.csproj`
   (no se versionan, por eso el repo es ligero: ~20 MB en total).
4. Escena principal: `Assets/Scenes/EscenarioReciclaje.unity`
5. Pulsa **Play**.

## Estructura del repositorio

```
.                                  # Proyecto Unity "Planta de Reciclaje"
├── Assets/
│   ├── Editor/                    # Herramientas de editor (menú "Herramientas")
│   │   ├── CrearControladorFPSEditor.cs   # Crea el jugador FPS en la escena
│   │   └── GeneradorPlantaReciclaje.cs    # Genera toda la planta automáticamente
│   ├── Generated/                 # Terreno y layers generados por la herramienta
│   ├── materials/                 # Materiales base (colores)
│   ├── Models/                    # Modelos 3D (Car, Home) en FBX + texturas
│   ├── Prefabs/                   # Contenedores (Metal, Papel, Plásticos, Vidrio)
│   ├── Scenes/                    # EscenarioReciclaje (principal) y SampleScene
│   ├── Scripts/ControladorFPS.cs  # Movimiento y cámara en primera persona
│   └── Textures/
├── Packages/                      # manifest.json + packages-lock.json
├── ProjectSettings/               # Configuración del proyecto Unity
├── .vscode/
├── .gitignore
└── Objeto3D_VR/                   # Proyecto Unity VR (independiente)
    ├── Assets/
    │   ├── Editor/  Generated/  materials/  Models/
    │   ├── Oculus/  Plugins/  XR/  Resources/  CompositionLayers/
    │   ├── Prefabs/  Scenes/  Scripts/  Textures/  StreamingAssets/
    │   └── InputSystem_Actions.inputactions, New Terrain.asset
    ├── Packages/
    ├── ProjectSettings/
    ├── .vscode/
    └── .gitignore
```

> Cada carpeta es un **proyecto Unity independiente**. No compartas entre ellos la carpeta
> `Library/`: si copias un proyecto, copia solo `Assets/`, `Packages/` y `ProjectSettings/`.

## Scripts principales

- **`ControladorFPS.cs`** — Movimiento del jugador con `CharacterController`:
  `WASD` para moverse, ratón para mirar, `Esc` libera el cursor, clic izquierdo lo vuelve a capturar.
- **`GeneradorPlantaReciclaje.cs`** — Menú **`Herramientas → Generar Planta de Reciclaje (automatico)`**.
  Crea carreteras, fábrica, cinta transportadora, contenedores, almacén, oficina, decoración,
  terreno, materiales y prefabs.
- **`CrearControladorFPSEditor.cs`** — Utilidad de editor para instanciar el jugador FPS.

## Qué NO se sube (y por qué)

`Library/`, `Temp/`, `Logs/`, `Obj/`, `UserSettings/`, `Build/`, `Assets/_Recovery/`,
los `.csproj`, `.sln`/`.slnx` y los archivos de IDE se generan solos al abrir el proyecto.

Sin excluirlos, este repo pasaría de **~20 MB a más de 5.6 GB** (1.9 GB del proyecto de
escritorio + 3.7 GB del VR) sin ningún beneficio. Todo eso ya está cubierto por los
`.gitignore` (hay uno en la raíz y otro dentro de `Objeto3D_VR/`).

## Notas para el siguiente desarrollador

- Los archivos `.meta` **sí** se versionan: son obligatorios para que las referencias internas
  de Unity no se rompan entre máquinas.
- Tras clonar, la primera apertura tarda un poco mientras Unity importa assets y paquetes.
- Si el editor pide reimportar, acepta: no hay pérdida de datos.
- Algunos assets están **duplicados** entre ambos proyectos (mismos FBX/texturas). Git los
  guarda una sola vez internamente, así que no inflan el repo.

# Configuración del Minijuego

## Scripts Creados

1. **MinigameSpawner.cs** - Spawnea el ataúd y el vampiro cuando se activa MinigameScene
2. **VampireChase.cs** - Hace que el vampiro persiga al coche (jugador)

## Pasos para Configurar

### 1. Crear Prefab del Vampiro
- En Unity, arrastra `vampire.fbx` desde `Assets/_ARScenes/ModelViewerScene/Models/` a la escena
- Añade el componente `VampireChase` al GameObject del vampiro
- Añade un `Rigidbody` si no tiene uno
- Guarda como prefab en `Assets/_ARScenes/MinigameScene/Prefabs/Vampire.prefab`

### 2. Crear Prefab del Ataúd
- Si tienes un modelo de ataúd, arrástralo a la escena
- Si no, crea un GameObject vacío o usa un cubo como placeholder
- Guarda como prefab en `Assets/_ARScenes/MinigameScene/Prefabs/Coffin.prefab`

### 3. Configurar MinigameScene
- Selecciona el GameObject `MinigameScene` en la escena Main
- Añade el componente `MinigameSpawner`
- Asigna los prefabs creados en los campos:
  - `Coffin Prefab`: El prefab del ataúd
  - `Vampire Prefab`: El prefab del vampiro
- Ajusta `Spawn Position` según donde quieras que aparezcan

### 4. Configurar el Coche
- Asegúrate de que el coche tenga el tag "Car"
- Si no lo tiene, créalo en Edit > Project Settings > Tags and Layers

### 5. Ajustar Parámetros del Vampiro
- En el prefab del vampiro, ajusta los parámetros de `VampireChase`:
  - `Chase Speed`: Velocidad de persecución (default: 2)
  - `Rotation Speed`: Velocidad de rotación (default: 5)
  - `Min Distance To Player`: Distancia mínima al jugador (default: 1)

## Notas
- El ataúd aparecerá primero y se quedará fijo
- El vampiro aparecerá 1 segundo después dentro del ataúd
- El vampiro perseguirá automáticamente al coche cuando esté activo
- Los objetos se destruyen automáticamente cuando se desactiva MinigameScene


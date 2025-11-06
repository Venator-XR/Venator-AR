# Configuración del Sistema de Game Over

## Scripts Creados

1. **GameOverManager.cs** - Maneja el estado de game over, pausa el juego y muestra la UI
2. **VampireChase.cs** - Modificado para detectar colisión con el coche y activar game over

## Pasos para Configurar en Unity

### 1. Crear el Canvas de Game Over

1. En la jerarquía, haz clic derecho > UI > Canvas
2. Renombra el Canvas a "GameOverCanvas"
3. Configura el Canvas:
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler: Scale With Screen Size
   - Reference Resolution: 1920 x 1080

### 2. Crear el Panel de Fondo

1. Clic derecho en GameOverCanvas > UI > Panel
2. Renombra a "BackgroundPanel"
3. Configura:
   - Color: Negro con Alpha 200 (semi-transparente)
   - Anchors: Stretch (todos los lados)

### 3. Crear el Texto "Has perdido"

1. Clic derecho en GameOverCanvas > UI > Text - TextMeshPro (o Text si no tienes TMP)
2. Renombra a "GameOverText"
3. Configura:
   - Text: "Has perdido"
   - Font Size: 72
   - Alignment: Center
   - Color: Rojo o Blanco
   - Posición: Centro de la pantalla (Y: 100)

### 4. Crear el Botón de Aceptar

1. Clic derecho en GameOverCanvas > UI > Button - TextMeshPro (o Button)
2. Renombra a "AcceptButton"
3. Configura:
   - Text del hijo: "Aceptar"
   - Font Size: 36
   - Posición: Centro de la pantalla (Y: -100)
   - Size: 200 x 60

### 5. Configurar GameOverManager

1. Crea un GameObject vacío en MinigameScene
2. Renombra a "GameOverManager"
3. Añade el componente `GameOverManager`
4. En el Inspector, asigna:
   - **Game Over Canvas**: Arrastra el GameOverCanvas creado
   - **Accept Button**: Arrastra el botón AcceptButton
   - **Scene Manager**: Arrastra el GameObject que tiene el componente SceneManager (probablemente en la raíz de la escena Main)

### 6. Configurar Colliders en el Vampiro y Ataúd

Para que la detección de colisión funcione, tanto el prefab del vampiro como el del ataúd necesitan Colliders:

#### Vampiro:
1. Selecciona el prefab del vampiro
2. Añade un componente **Box Collider** o **Sphere Collider**
3. Configura:
   - **Is Trigger**: Marcado (para usar OnTriggerEnter)
   - Ajusta el tamaño para que cubra el modelo del vampiro

#### Ataúd:
1. Selecciona el prefab del ataúd
2. Añade un componente **Box Collider** o **Sphere Collider**
3. Configura:
   - **Is Trigger**: Marcado (para usar OnTriggerEnter)
   - Ajusta el tamaño para que cubra el modelo del ataúd

**Nota**: 
- El script `CoffinCollision` se añade automáticamente a los ataúdes cuando se spawnean, pero el prefab debe tener un Collider.
- Si prefieres usar colisiones físicas en lugar de triggers, desmarca "Is Trigger" y asegúrate de que tanto el vampiro/ataúd como el coche tengan Rigidbody.

### 7. Verificar el Tag del Coche

Asegúrate de que el coche tenga el tag "Car":
1. Selecciona el prefab del coche
2. En el Inspector, Tag > Car (si no existe, créalo en Edit > Project Settings > Tags and Layers)

## Funcionamiento

- Cuando un vampiro **o un ataúd** toca el coche, se activa `GameOver()`
- Se eliminan automáticamente todos los vampiros y ataúdes activos
- El juego se pausa (`Time.timeScale = 0`)
- Se muestra el canvas de Game Over
- Al hacer clic en "Aceptar":
  - Se reanuda el tiempo (`Time.timeScale = 1`)
  - Se llama a `SceneManager.BackToMenu()`
  - Se desactiva MinigameScene y se vuelve al menú principal

## Notas

- El GameOverManager usa un patrón Singleton para ser accesible desde cualquier script
- Solo se puede activar el game over una vez (evita múltiples activaciones)
- El tiempo se reanuda automáticamente al volver al menú


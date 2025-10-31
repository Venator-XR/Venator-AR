# Quick Start Guide - Video Player System

## ⚡ 5-Minute Setup

### Step 1: Setup the Video Plane (1 min)

1. Select your video plane GameObject
2. **Add Component** → Search "Video Player" (if not already there)
3. **Add Component** → Search "VideoPlayerManager"
4. Assign your video file to the VideoPlayer component
5. ✅ Done! The manager auto-detects the VideoPlayer

### Step 2: Create UI Canvas (30 seconds)

1. **Right-click in Hierarchy** → UI → Canvas
2. Set Canvas to **World Space** in Inspector
3. Set **Render Mode**: World Space
4. Position the Canvas in front of your video plane
5. Adjust **Width**: 1920, **Height**: 200 (or to taste)
6. Scale: (0.001, 0.001, 0.001) to make it appropriately sized

### Step 3: Create Play/Pause Button (1 min)

1. **Right-click Canvas** → UI → Button
2. Rename to "PlayPauseButton"
3. **Add Component** → "PlayPauseButtonUI"
4. Drag your video plane to the "Video Manager" field
5. Optional: Add play/pause sprites:
   - Import play icon and pause icon images
   - Assign to "Play Sprite" and "Pause Sprite" fields
6. ✅ Test: Click the button to play/pause!

### Step 4: Create Timeline Slider (1 min)

1. **Right-click Canvas** → UI → Slider
2. Rename to "TimelineSlider"
3. Position it in the canvas (use Rect Transform)
4. **Add Component** → "VideoSliderUI"
5. Drag your video plane to the "Video Manager" field
6. The Slider component auto-assigns
7. ✅ Test: Drag to scrub through video!

### Step 5: Create Volume Slider (1 min)

1. **Right-click Canvas** → UI → Slider
2. Rename to "VolumeSlider"
3. Position to the right or below timeline slider
4. **Add Component** → "VolumeSliderUI"
5. Drag your video plane to the "Video Manager" field
6. Optional: Enable color feedback:
   - Use Dynamic Fill Color: ✓
   - Low/Medium/High Volume Colors
7. ✅ Test: Drag to change volume!

## 🎨 Visual Customization

### Make Play Button Change Appearance

1. Select PlayPauseButton
2. In PlayPauseButtonUI component:
   - Assign Play Sprite (▶ icon)
   - Assign Pause Sprite (⏸ icon)
   - Optional: Use Color Change: ✓
     - Play Color: Green
     - Pause Color: Red

### Make Volume Slider Colorful

1. Select VolumeSlider
2. In VolumeSliderUI component:
   - Use Dynamic Fill Color: ✓
   - Low Volume Color: Red
   - Medium Volume Color: Yellow
   - High Volume Color: Green

### Customize Slider Appearance

1. Select a Slider GameObject
2. Expand it in hierarchy to see:
   - Background: Change color/sprite
   - Fill Area → Fill: Change color (or use VolumeSliderUI for dynamic)
   - Handle Slide Area → Handle: Change size/color/sprite
3. Use Unity's built-in UI customization tools

## 🎯 Recommended Layout

```
        [Video Plane]
             |
          [Canvas (World Space)]
        ┌────────────────────┐
        │ [PlayPauseButton]  │
        │ [Timeline Slider]  │
        │ [Volume Slider]    │
        └────────────────────┘
```

## 🔧 Troubleshooting

### "Button doesn't respond to clicks"

- ✅ Check: Canvas has GraphicRaycaster component
- ✅ Check: Scene has EventSystem (auto-created with Canvas)
- ✅ Check: Canvas Render Mode is set correctly
- ✅ Check: Button component is present

### "Slider doesn't work"

- ✅ Check: Slider component is present
- ✅ Check: VideoSliderUI or VolumeSliderUI component is attached
- ✅ Check: Video Manager reference is assigned
- ✅ Check: EventSystem exists in scene

### "Can't see UI in Scene"

- ✅ Check: Canvas is in World Space mode
- ✅ Check: Canvas position is visible to camera
- ✅ Check: Canvas scale is appropriate (try 0.001, 0.001, 0.001)
- ✅ Check: UI elements are inside Canvas bounds

### "Video doesn't play"

- ✅ Check: Video file is in StreamingAssets or Resources
- ✅ Check: VideoPlayer component has video clip assigned
- ✅ Check: VideoPlayerManager is on the same GameObject

## 📱 Controls Summary

| Control             | Action                | Visual Feedback                   |
| ------------------- | --------------------- | --------------------------------- |
| **PlayPauseButton** | Click to play/pause   | Sprite changes (▶/⏸)              |
| **TimelineSlider**  | Drag to scrub video   | Handle moves, fill grows          |
| **VolumeSlider**    | Drag to adjust volume | Color gradient (red→yellow→green) |

## 🚀 Advanced Features

### Add Time Display (Optional)

1. Create Canvas UI
2. Add TextMeshPro text elements
3. Add **VideoPlayerUI** component to Canvas
4. Assign VideoPlayerManager reference
5. Assign text elements to show time/duration

### Add More Buttons

Want skip forward/backward? Create new cubes and add this script:

```csharp
public class SkipButton : MonoBehaviour
{
    public VideoPlayerManager videoManager;
    public float skipSeconds = 10f;

    void OnMouseDown()
    {
        videoManager.SkipForward(skipSeconds);
    }
}
```

## 📋 Inspector Checklist

Before testing, verify these are set:

**VideoPlayerManager** (on video plane):

- [x] VideoPlayer reference (should auto-fill)
- [x] Default Volume: 0.5

**Canvas**:

- [x] Render Mode: World Space
- [x] Event Camera: Assigned (if needed)
- [x] Has GraphicRaycaster component

**EventSystem** (auto-created):

- [x] Exists in scene hierarchy
- [x] Has StandaloneInputModule

**PlayPauseButtonUI**:

- [x] Video Manager: Assigned
- [x] Button Image: Assigned
- [x] Play Sprite: Set (optional)
- [x] Pause Sprite: Set (optional)

**VideoSliderUI**:

- [x] Video Manager: Assigned
- [x] Slider component: Present (auto-detected)

**VolumeSliderUI**:

- [x] Video Manager: Assigned
- [x] Slider component: Present (auto-detected)
- [x] Fill Image: Assigned (for color feedback)

## 🎓 How It Works (SOLID Principles)

**Single Responsibility**:

- VideoPlayerManager: Only manages video playback
- PlayPauseButtonUI: Only handles button interaction
- VideoSliderUI: Only handles timeline control
- VolumeSliderUI: Only handles volume control

**Open/Closed**:

- Add new features by creating new components
- Don't modify existing classes

**Dependency Inversion**:

- UI components depend on VideoPlayerManager
- Not directly on VideoPlayer

**Observer Pattern**:

- Manager fires events
- UI components listen and update
- Decoupled communication

This makes the code:

- ✅ Easy to test
- ✅ Easy to extend
- ✅ Easy to maintain
- ✅ Reusable across projects

## 🌟 Tips

1. **Start Simple**: Get one control working, then add others
2. **Use Canvas Preview**: Select Canvas in Scene view to edit UI layout
3. **Test in Play Mode**: Click play and interact with UI controls
4. **Adjust Canvas Scale**: If UI is too big/small, adjust Canvas scale
5. **Use Anchors**: Set UI element anchors for responsive layout
6. **Import Icons**: Use play/pause icons from free icon packs for better visuals

## 🎨 UI vs 3D Controls

**✅ Benefits of UI Sliders:**

- Built-in Unity components (less custom code)
- Easier to style and customize
- Better touch/click handling
- More familiar for most developers
- Can use Unity's UI system features (animations, transitions, etc.)

**Legacy 3D versions available:**

- `VideoSlider3D.cs` - For 3D object-based timeline control
- `VolumeSlider3D.cs` - For 3D object-based volume control
- `PlayPauseButton3D.cs` - For 3D object-based button

Use 3D versions if you need controls in 3D world space without a Canvas.

## 📚 Full Documentation

See `README_VideoPlayerSystem.md` for complete API reference and advanced usage.

---

**Need Help?** Check that:

1. All references are assigned in Inspector
2. Colliders are present on interactive objects
3. VideoPlayerManager is initialized (check Console for "Video prepared" message)

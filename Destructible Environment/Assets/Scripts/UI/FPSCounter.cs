using UnityEngine;

namespace UI
{
    /// <summary>
    /// (This is a temporary FPS counter, can be moved to Canvas from OnGUI in the future if needed)

    /// A self-initialising FPS counter that spawns itself at runtime and persists across all scenes.
    /// Displays average, minimum, maximum, and current FPS with a colour-coded overlay using Unity's OnGUI system.
    /// No scene setup required — it initialises automatically via RuntimeInitializeOnLoadMethod.
    /// </summary>

    public class FPSCounter : MonoBehaviour
    {
        // ============= Appearance Settings =============
        private int frameRange = 60;                                                                    // Number of frames to average FPS over
        private float panelWidth = 240f;                                                                // Width of the overlay panel (Reducing this will condense the text)
        private float panelHeight = 60f;                                                                // Height of the overlay panel
        private float paddingX = 10f;                                                                   // Horizontal padding inside the overlay panel
        private float paddingY = 8f;                                                                    // Vertical padding inside the overlay panel
        private float lineHeight = 24f;                                                                 // Height of each text row in the overlay panel

        // Background & Text 
        private Color backgroundColour = new Color(0.05f, 0.1f, 0.1f, 0.15f);                           // Background colour and transparency of the FPS overlay panel
        private Color textColour = new Color(0.85f, 0.85f, 0.85f, 1f);                                  // Main text colour for the FPS labels
        private string colourHigh = "#73C34F";                                                          // Green  — 60+ FPS
        private string colourMid = "#D3B53D";                                                           // Yellow — 30-59 FPS
        private string colourLow = "#B83B3B";                                                           // Red    — below 30 FPS

        // Frame time
        private int graphHeight = 40;                                                                   // Height of the frame time graph in pixels
        private float graphMaxMs = 33.3f;                                                               // Maximum frame time in milliseconds for the graph 

        // ============= Internal State =============
        private float _deltaTime = 0.0f;                                                                // Float   - Smoothed delta time for FPS calculation
        private int[] _fpsBuffer;                                                                       // Int[]   - Circular buffer storing FPS values for the last 'frameRange' frames
        private int _fpsBufferIndex;                                                                    // Int[]   - Current write index in the circular FPS buffer
        private bool _isVisible = true;                                                                 // Bool    - Whether the FPS overlay is currently visible


        // ============= GUI State =============
        private GUIStyle _guiStyle_Left;                                                                // GUIStyle  - Left-aligned style for FPS labels
        private GUIStyle _guiStyle_Right;                                                               // GUIStyle  - Right-aligned style for FPS values
        private Texture2D _guiTexture_Background;                                                       // Texture2D - Background texture for the FPS overlay panel
        private Texture2D _guiTexture_GraphBar;                                                         // Texture2D - Texture used to draw the frame time graph bars


        // ============= Auto Initialisation =============

        // Automatically spawns the FPS counter when the game starts — no scene setup required
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoInitialise()
        {
            // 1. Create a new GameObject to hold the FPS counter
            GameObject fpsObj = new GameObject("System_FPSCounter");
            // 2. Add the FPSCounter component to the GameObject
            fpsObj.AddComponent<FPSCounter>();
            // 3. Persist across all scenes
            DontDestroyOnLoad(fpsObj);
        }


        // ============= Unity Methods =============
        private void Awake()
        {
            Initialise_FPSBuffer();
        }

        private void Update()
        {
            Update_FrameTime();
            Update_FPSBuffer();
            Update_ToggleHotkey();
        }

        private void OnGUI()
        {
            Initialise_GUIStyles();

            // 1. Only draw the overlay if it is visible
            if (_isVisible) Draw_FPSOverlay();
        }


        // ============= Initialisation Methods =============
        private void Initialise_FPSBuffer()
        {
            // 1. Create an array to store the last 'frameRange' FPS values
            _fpsBuffer = new int[frameRange];
        }

        private void Initialise_GUIStyles()
        {
            // 1. Only run once — if already initialised, skip
            if (_guiStyle_Left != null) return;

            // 2. Create a small background texture using the background colour setting
            _guiTexture_Background = new Texture2D(1, 1);
            _guiTexture_Background.SetPixel(0, 0, backgroundColour);
            _guiTexture_Background.Apply();

            // 3. Set up the left-aligned text style using the text colour setting
            _guiStyle_Left = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                richText = true,
            };
            _guiStyle_Left.normal.textColor = textColour;

            // 4. Set up the right-aligned text style, copying from the left style
            _guiStyle_Right = new GUIStyle(_guiStyle_Left)
            {
                alignment = TextAnchor.UpperRight
            };

            // 5. Create a small texture for the graph bars using the text colour setting
            _guiTexture_GraphBar = new Texture2D(1, 1);
            _guiTexture_GraphBar.SetPixel(0, 0, Color.white);
            _guiTexture_GraphBar.Apply();
        }


        // ============= Update Methods =============
        private void Update_FrameTime()
        {
            // 1. Smooth the delta time over time to avoid single-frame spikes
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }

        private void Update_FPSBuffer()
        {
            // 1. Re-create the buffer if it is missing or the frame range has changed
            if (_fpsBuffer == null || _fpsBuffer.Length != frameRange)
                _fpsBuffer = new int[frameRange];

            // 2. Store the current FPS in the buffer and advance the index
            _fpsBuffer[_fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);

            // 3. Loop the index back to 0 when it reaches the end (circular buffer)
            if (_fpsBufferIndex >= frameRange) _fpsBufferIndex = 0;
        }

        private void Update_ToggleHotkey()
        {
            // 1. Check if either CTRL key is held
            bool ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            // 2. Check if F is pressed this frame
            bool fPressed = Input.GetKeyDown(KeyCode.F);

            // 3. Toggle visibility if both keys are pressed together (CTRL + F)
            if (ctrlHeld && fPressed)
                _isVisible = !_isVisible;
        }


        // ============= Draw Methods =============
        private void Draw_FPSOverlay()
        {
            // 1. Calculate scale based on screen height so the overlay looks consistent across resolutions
            float scale = Mathf.Max(1f, Screen.height / 1080f);
            Initialise_FontSize(scale);

            // 2. Calculate FPS stats and get the current smoothed FPS
            Calculate_FPSStats(out int avg, out int min, out int max);
            int currentFps = (int)(1f / _deltaTime);

            // 3. Draw the background panel then the text labels on top
            Draw_Background(scale);
            Draw_Labels(scale, avg, min, max, currentFps);

            // 4. Draw the frame time graph below the main panel
            Draw_FrameGraph(scale);
        }

        private void Initialise_FontSize(float scale)
        {
            // 1. Scale the font size relative to the screen resolution
            int fontSize = Mathf.Max(16, (int)(22 * scale));
            _guiStyle_Left.fontSize = fontSize;
            _guiStyle_Right.fontSize = fontSize;
        }

        private void Draw_Background(float scale)
        {
            // 1. Get the background rect and draw the texture inside it
            Rect bgRect = Get_BackgroundRect(scale);
            GUI.DrawTexture(bgRect, _guiTexture_Background);
        }

        private void Draw_Labels(float scale, int avg, int min, int max, int currentFps)
        {
            // 1. Define the two text rows inside the background rect using the padding and line height settings
            Rect bgRect = Get_BackgroundRect(scale);
            Rect topRect = new Rect(bgRect.x + paddingX * scale, bgRect.y + paddingY * scale, bgRect.width - paddingX * scale * 2, lineHeight * scale);
            Rect botRect = new Rect(bgRect.x + paddingX * scale, bgRect.y + paddingY * scale + lineHeight * scale, bgRect.width - paddingX * scale * 2, lineHeight * scale);

            // 2. Top row — column headers on the left, current FPS on the right
            GUI.Label(topRect, "avg  min  max", _guiStyle_Left);
            GUI.Label(topRect, $"<color=white>{currentFps}</color> fps", _guiStyle_Right);

            // 3. Bottom row — colour-coded FPS values on the left, frame time on the right
            string fpsValues = $"<color={Get_FPSColour(avg)}>{avg.ToString().PadLeft(3)}</color>  " +
                               $"<color={Get_FPSColour(min)}>{min.ToString().PadLeft(3)}</color>  " +
                               $"<color={Get_FPSColour(max)}>{max.ToString().PadLeft(3)}</color>";

            GUI.Label(botRect, fpsValues, _guiStyle_Left);
            GUI.Label(botRect, $"{(_deltaTime * 1000f):0.00} ms", _guiStyle_Right);
        }

        private void Draw_FrameGraph(float scale)
        {
            // 1. Position the graph directly below the main panel
            Rect bgRect = Get_BackgroundRect(scale);
            float graphY = bgRect.y + bgRect.height + 4 * scale;
            float barWidth = bgRect.width / frameRange;

            // 2. Draw one bar per frame in the buffer
            for (int i = 0; i < frameRange; i++)
            {
                int index = (_fpsBufferIndex + i) % frameRange;
                float frameMs = _fpsBuffer[index] > 0 ? 1000f / _fpsBuffer[index] : 0f;
                float barH = Mathf.Clamp(frameMs / graphMaxMs, 0f, 1f) * graphHeight * scale;

                Rect barRect = new Rect(bgRect.x + i * barWidth, graphY + (graphHeight * scale - barH), barWidth - 1f, barH);

                // 3. Colour the bar based on frame time — green, yellow or red
                GUI.color = frameMs <= 16.6f ? new Color(0.45f, 0.76f, 0.31f) :
                            frameMs <= 33.3f ? new Color(0.83f, 0.71f, 0.24f) :
                                               new Color(0.72f, 0.23f, 0.23f);
                GUI.DrawTexture(barRect, _guiTexture_GraphBar);
            }

            // 4. Reset GUI colour to white so nothing else is tinted
            GUI.color = Color.white;
        }


        // ============= Helper Methods =============
        private void Calculate_FPSStats(out int avg, out int min, out int max)
        {
            // 1. Loop through the buffer and accumulate sum, min and max
            int sum = 0;
            min = int.MaxValue;
            max = 0;

            for (int i = 0; i < frameRange; i++)
            {
                int fps = _fpsBuffer[i];
                sum += fps;
                if (fps < min) min = fps;
                if (fps > max) max = fps;
            }

            // 2. Calculate the average and guard against an uninitialised min
            avg = frameRange > 0 ? sum / frameRange : 0;
            if (min == int.MaxValue) min = 0;
        }

        private Rect Get_BackgroundRect(float scale)
        {
            // 1. Return a fixed rect anchored to the top-left corner of the screen using the panel size settings
            return new Rect(10, 10, panelWidth * scale, panelHeight * scale);
        }

        private string Get_FPSColour(int fps)
        {
            // 1. Return the appropriate colour based on FPS thresholds
            if (fps >= 60) return colourHigh;
            if (fps >= 30) return colourMid;
            return colourLow;
        }
    }
}
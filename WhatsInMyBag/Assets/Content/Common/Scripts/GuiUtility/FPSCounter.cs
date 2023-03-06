using UnityEngine;
using GameEvents;

namespace Common.Gui.Utility
{
    public class FPSCounter : MonoBehaviour
    {
        float deltaTime = 0.0f;

        private bool _enabled = false;
        
        private void OnEnable()
        {
            GameEventManager.AddListener<GuiEvent>(OnGuiEvent);
            _enabled = false;
        }

        private void OnDisable()
        {
            GameEventManager.AddListener<GuiEvent>(OnGuiEvent);
        }

        public void SetCounterActive(bool value)
        {
            _enabled = value;
        }
        
        private void OnGuiEvent(GuiEvent @event)
        {
            switch (@event.Type)
            {
                case GuiEventType.Enable_DebugInfo:
                    _enabled = true;
                    break;
                case GuiEventType.Disable_DebugInfo:
                    _enabled = false;
                    break;
            }
        }
        
        void Update()
        {
            if (_enabled) deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            if (!_enabled) return;
            
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = Color.magenta;
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}
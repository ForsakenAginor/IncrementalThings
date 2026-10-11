using UnityEngine;
using UnityEngine.UI;

namespace Source.Scripts.Utility.UI
{
    public class MultiGraphicButton : Button
    {
        [SerializeField] private Graphic[] _allGraphics;
        private ColorBlock _currentColors;
    
        public Graphic[] AdditionalGraphics => _allGraphics;
    
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
        
            if (_allGraphics == null || _allGraphics.Length == 0)
                return;
        
            Color targetColor;
        
            switch (state)
            {
                case SelectionState.Normal:
                    targetColor = colors.normalColor;
                    break;
                case SelectionState.Highlighted:
                    targetColor = colors.highlightedColor;
                    break;
                case SelectionState.Pressed:
                    targetColor = colors.pressedColor;
                    break;
                case SelectionState.Selected:
                    targetColor = colors.selectedColor;
                    break;
                case SelectionState.Disabled:
                    targetColor = colors.disabledColor;
                    break;
                default:
                    targetColor = colors.normalColor;
                    break;
            }
        
            foreach (var graphic in _allGraphics)
            {
                if (graphic == null) continue;
            
                graphic.CrossFadeColor(targetColor, 
                    instant ? 0f : colors.fadeDuration, 
                    true, 
                    true);
            }
        }
    }
}
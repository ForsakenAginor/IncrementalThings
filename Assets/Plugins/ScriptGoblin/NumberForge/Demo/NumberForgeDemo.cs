using UnityEngine;
using ScriptGoblin.NumberForge.Formatting;

namespace ScriptGoblin.NumberForge.Demo
{
    /// <summary>
    /// Fullscreen animated demo scene for NumberForge.
    /// Attach to any GameObject in a scene with a solid-color camera background (#0F0F0E).
    /// </summary>
    public class NumberForgeDemo : MonoBehaviour
    {
        private static readonly BigNumber[] _milestones =
        {
            new(1.234,  2),
            new(9.876,  4),
            new(4.200,  6),
            new(7.777,  9),
            new(1.337,  12),
            new(9.999,  15),
            new(3.141,  30),
            new(2.718,  50),
            new(1.000,  100),
        };

        // #0F0F0E
        private static readonly Color _bg       = new(0.059f, 0.059f, 0.055f);
        // slightly lighter panel surface
        private static readonly Color _panelBg  = new(0.094f, 0.094f, 0.090f);
        // #FF5252
        private static readonly Color _accent   = new(1.000f, 0.322f, 0.322f);
        private static readonly Color _white    = Color.white;
        private static readonly Color _dim      = new(0.420f, 0.420f, 0.400f);
        private static readonly Color _border   = new(0.173f, 0.173f, 0.165f);

        private double _animTime;

        private GUIStyle _styleTitle;
        private GUIStyle _styleTagline;
        private GUIStyle _styleCounter;
        private GUIStyle _styleSuffix;
        private GUIStyle _styleLabel;
        private GUIStyle _styleLabelBold;
        private GUIStyle _styleCode;
        private bool     _stylesReady;

        // computed once in BuildStyles, used everywhere for consistent row sizing
        private float _lineH;
        private float _headerH;
        private float _codeH;

        private void Update() => _animTime += Time.deltaTime * 0.18;

        private void OnGUI()
        {
            BuildStyles();

            var sw = Screen.width;
            var sh = Screen.height;
            DrawRect(new Rect(0, 0, sw, sh), _bg);

            var pad    = sw * 0.026f;
            var innerW = sw - pad * 2f;
            var y      = pad * 0.55f;

            y = DrawHeader(pad, y, innerW);
            y += pad * 0.9f;

            var rowH  = sh - y - pad * 0.7f;
            var col1W = innerW * 0.37f;
            var col2W = innerW * 0.32f;
            var col3W = innerW - col1W - col2W - pad * 2f;

            DrawCounterPanel(new Rect(pad,                             y, col1W, rowH));
            DrawFormatsPanel(new Rect(pad + col1W + pad,               y, col2W, rowH));
            DrawMathPanel   (new Rect(pad + col1W + pad + col2W + pad, y, col3W, rowH));
        }

        // ── Header ───────────────────────────────────────────────────────────

        private float DrawHeader(float x, float y, float w)
        {
            var titleH   = _lineH * 1.5f;
            var taglineH = _lineH;
            var h        = _lineH * 0.4f + titleH + _lineH * 0.25f + taglineH + _lineH * 0.4f;

            DrawRect(new Rect(x, y, w, h), _panelBg);
            DrawBorder(new Rect(x, y, w, h));

            var titleY   = y + _lineH * 0.4f;
            var taglineY = titleY + titleH + _lineH * 0.25f;

            var titleStyle = new GUIStyle(_styleTitle) { richText = true };
            titleStyle.normal.textColor = _white;
            var accentHex = ColorUtility.ToHtmlStringRGB(_accent);
            GUI.Label(new Rect(x + 20, titleY, w * 0.5f, titleH),
                $"Number <color=#{accentHex}>Forge</color>", titleStyle);

            GUI.color = _dim;
            GUI.Label(new Rect(x + 20, taglineY, w * 0.6f, taglineH),
                "Arbitrarily large numbers  ·  Four display formats  ·  Full math API", _styleTagline);
            GUI.color = _white;

            return y + h;
        }

        // ── Left: animated resource counter ──────────────────────────────────

        private void DrawCounterPanel(Rect r)
        {
            DrawPanel(r, "Resource Counter");
            var inner = Inset(r);

            var value     = CurrentValue();
            var opts      = new FormatOptions { notation = NumberNotation.Short, decimals = 3 };
            var split     = SplitSuffix(value.ToFormattedString(opts));

            // big number display — top 44% of the inner area
            var displayH = inner.height * 0.44f;
            DrawRect(new Rect(inner.x, inner.y, inner.width, displayH), Darken(_panelBg, 0.5f));
            DrawRect(new Rect(inner.x, inner.y, 3f, displayH), _accent);

            var numH = displayH * 0.50f;
            var numY = inner.y + (displayH - numH) / 2f - displayH * 0.04f;
            GUI.color = _accent;
            GUI.Label(new Rect(inner.x, numY, inner.width, numH), split.number, _styleCounter);

            if (split.suffix.Length > 0)
            {
                GUI.color = new Color(_accent.r, _accent.g, _accent.b, 0.6f);
                GUI.Label(new Rect(inner.x, numY + numH * 0.80f, inner.width, displayH * 0.22f),
                    split.suffix, _styleSuffix);
            }
            GUI.color = _white;

            // progress bars
            var barAreaY = inner.y + displayH + _lineH * 0.55f;
            DrawProgressRow(inner.x, barAreaY,               inner.width, "Gold", value, _accent);
            DrawProgressRow(inner.x, barAreaY + _lineH * 1.5f, inner.width, "XP",
                BigNumber.Lerp(_milestones[0], _milestones[^1], LerpT(6.0)), _dim);

            DrawCodeSnippet(inner.x, inner.yMax - _codeH, inner.width, "gold += damage * multiplier;");
        }

        // ── Centre: format comparison ─────────────────────────────────────────

        private void DrawFormatsPanel(Rect r)
        {
            DrawPanel(r, "Four Display Formats");
            var inner  = Inset(r);
            var usable = inner.yMax - _codeH - _lineH * 0.3f - inner.y;
            var rowH   = usable / 4f;
            var y      = inner.y + _lineH * 0.2f;

            DrawFormatRow(inner.x, y,              inner.width, rowH, "Short",       NumberNotation.Short);
            DrawFormatRow(inner.x, y + rowH,       inner.width, rowH, "Long",        NumberNotation.Long);
            DrawFormatRow(inner.x, y + rowH * 2f,  inner.width, rowH, "Scientific",  NumberNotation.Scientific);
            DrawFormatRow(inner.x, y + rowH * 3f,  inner.width, rowH, "Engineering", NumberNotation.Engineering);

            DrawCodeSnippet(inner.x, inner.yMax - _codeH, inner.width, "value.ToFormattedString(opts)");
        }

        // ── Right: math API ───────────────────────────────────────────────────

        private void DrawMathPanel(Rect r)
        {
            DrawPanel(r, "Math API");
            var inner = Inset(r);
            var value = CurrentValue();
            var opts  = new FormatOptions { notation = NumberNotation.Short, decimals = 2 };
            var gap   = _lineH * 1.15f;
            var half  = _lineH * 0.55f;
            var curY  = inner.y + _lineH * 0.15f;

            DrawMathRow(inner.x, curY, inner.width, "Abs",   value.Abs().ToFormattedString(opts));  curY += gap;
            DrawMathRow(inner.x, curY, inner.width, "Sqrt",  value.Sqrt().ToFormattedString(opts)); curY += gap;
            DrawMathRow(inner.x, curY, inner.width, "Pow²",  value.Pow(2).ToFormattedString(opts)); curY += gap;
            DrawMathRow(inner.x, curY, inner.width, "Log10", $"{value.Log10():F2}");                curY += gap;

            DrawDivider(inner.x, curY, inner.width); curY += half;

            DrawDimLabel(inner.x, curY, inner.width, "Lerp"); curY += _lineH;

            var lerpV = BigNumber.Lerp(new BigNumber(1, 0), new BigNumber(1, 9), LerpT(6.0));
            GUI.color = _white;
            GUI.Label(new Rect(inner.x, curY, inner.width, _lineH), lerpV.ToFormattedString(opts), _styleLabelBold);
            GUI.color = _white;
            curY += _lineH;

            var barH = _lineH * 0.32f;
            DrawRect(new Rect(inner.x, curY, inner.width, barH), Darken(_panelBg, 0.5f));
            DrawRect(new Rect(inner.x, curY, inner.width * (float)LerpT(6.0), barH), _accent);
            curY += barH + half;

            DrawDivider(inner.x, curY, inner.width); curY += half;

            DrawDimLabel(inner.x, curY, inner.width, "Clamp  [1M … 1T]"); curY += _lineH;

            var clamped = BigNumber.Clamp(value, new BigNumber(1, 6), new BigNumber(1, 12));
            GUI.color = _white;
            GUI.Label(new Rect(inner.x, curY, inner.width, _lineH), clamped.ToFormattedString(opts), _styleLabelBold);
            GUI.color = _white;

            DrawCodeSnippet(inner.x, inner.yMax - _codeH, inner.width, "BigNumber.Lerp(a, b, t)");
        }

        // ── Row helpers ───────────────────────────────────────────────────────

        private void DrawFormatRow(float x, float y, float w, float h, string label, NumberNotation notation)
        {
            DrawRect(new Rect(x, y + 3, w, h - 6), Darken(_panelBg, 0.45f));
            var text = CurrentValue().ToFormattedString(new FormatOptions { notation = notation, decimals = 2 });
            GUI.color = _dim;
            GUI.Label(new Rect(x + 10, y + (h - _lineH) / 2f, w * 0.44f, _lineH), label, _styleLabel);
            GUI.color = _white;
            GUI.Label(new Rect(x + w * 0.46f, y + (h - _lineH) / 2f, w * 0.52f, _lineH), text, _styleLabel);
            GUI.color = _white;
        }

        private void DrawMathRow(float x, float y, float w, string label, string result)
        {
            GUI.color = _dim;
            GUI.Label(new Rect(x, y, w * 0.40f, _lineH), label, _styleLabel);
            GUI.color = _white;
            GUI.Label(new Rect(x + w * 0.42f, y, w * 0.58f, _lineH), result, _styleLabel);
            GUI.color = _white;
        }

        private void DrawProgressRow(float x, float y, float w, string label, BigNumber value, Color col)
        {
            var opts       = new FormatOptions { notation = NumberNotation.Short, decimals = 2 };
            var barH       = _lineH * 0.35f;
            var normalized = (float)System.Math.Min(1.0, value.Log10() / 100.0);

            GUI.color = _dim;
            GUI.Label(new Rect(x, y, w * 0.18f, _lineH), label, _styleLabel);

            var barR = new Rect(x + w * 0.20f, y + (_lineH - barH) / 2f, w * 0.50f, barH);
            DrawRect(barR, Darken(_panelBg, 0.5f));
            DrawRect(new Rect(barR.x, barR.y, barR.width * normalized, barR.height), col);

            GUI.color = _white;
            GUI.Label(new Rect(x + w * 0.73f, y, w * 0.27f, _lineH), value.ToFormattedString(opts), _styleLabel);
            GUI.color = _white;
        }

        private void DrawDimLabel(float x, float y, float w, string text)
        {
            GUI.color = _dim;
            GUI.Label(new Rect(x, y, w, _lineH), text, _styleLabel);
            GUI.color = _white;
        }

        // ── Primitives ────────────────────────────────────────────────────────

        private void DrawPanel(Rect r, string title)
        {
            DrawRect(r, _panelBg);
            DrawBorder(r);
            // accent left stripe
            DrawRect(new Rect(r.x, r.y, 3f, r.height), _accent);
            // header bar
            DrawRect(new Rect(r.x, r.y, r.width, _headerH), Darken(_panelBg, 0.4f));
            GUI.color = _dim;
            GUI.Label(new Rect(r.x + 12, r.y + (_headerH - _lineH) / 2f, r.width - 16, _lineH), title, _styleLabel);
            GUI.color = _white;
        }

        private void DrawCodeSnippet(float x, float y, float w, string code)
        {
            DrawRect(new Rect(x, y, w, _codeH), new Color(0.035f, 0.035f, 0.033f));
            DrawRect(new Rect(x, y, 3f, _codeH), _accent);
            GUI.color = _accent;
            GUI.Label(new Rect(x + 10, y + (_codeH - _lineH) / 2f, w - 14, _lineH), code, _styleCode);
            GUI.color = _white;
        }

        private static void DrawDivider(float x, float y, float w)
            => DrawRect(new Rect(x, y, w, 1), new Color(0.173f, 0.173f, 0.165f));

        private static void DrawRect(Rect r, Color col)
        {
            var prev = GUI.color;
            GUI.color = col;
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = prev;
        }

        private static void DrawBorder(Rect r)
        {
            var col = new Color(0.22f, 0.22f, 0.20f);
            DrawRect(new Rect(r.x,        r.y,        r.width, 1),        col);
            DrawRect(new Rect(r.x,        r.yMax - 1, r.width, 1),        col);
            DrawRect(new Rect(r.x,        r.y,        1,        r.height), col);
            DrawRect(new Rect(r.xMax - 1, r.y,        1,        r.height), col);
        }

        // ── Layout ────────────────────────────────────────────────────────────

        private Rect Inset(Rect r)
        {
            const float side = 12f;
            return new Rect(r.x + side, r.y + _headerH, r.width - side * 2f, r.height - _headerH - side);
        }

        // ── Value helpers ─────────────────────────────────────────────────────

        private BigNumber CurrentValue()
        {
            var t    = _animTime % (_milestones.Length - 1);
            var from = (int)t;
            return BigNumber.Lerp(_milestones[from], _milestones[from + 1], (float)(t - from));
        }

        private float LerpT(double period) => (float)((_animTime % period) / period);

        private static (string number, string suffix) SplitSuffix(string s)
        {
            var i = s.Length - 1;
            while (i >= 0 && !char.IsDigit(s[i])) i--;
            return i == s.Length - 1 ? (s, "") : (s[..(i + 1)], "  " + s[(i + 1)..]);
        }

        private static Color Darken(Color c, float t)
            => new(c.r * (1 - t), c.g * (1 - t), c.b * (1 - t), c.a);

        // ── Styles ────────────────────────────────────────────────────────────

        private void BuildStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            _lineH   = Screen.height * 0.036f;
            _headerH = _lineH * 1.5f;
            _codeH   = _lineH * 1.55f;

            var labelSize = Mathf.RoundToInt(Screen.height * 0.019f);

            _styleTitle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.044f),
                fontStyle = FontStyle.Bold,
                normal    = { textColor = _white }
            };

            _styleTagline = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.height * 0.016f),
                normal   = { textColor = _dim }
            };

            _styleCounter = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.065f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = _accent }
            };

            _styleSuffix = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.026f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = _accent }
            };

            _styleLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize = labelSize,
                normal   = { textColor = _white }
            };

            _styleLabelBold = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.024f),
                fontStyle = FontStyle.Bold,
                normal    = { textColor = _white }
            };

            _styleCode = new GUIStyle(GUI.skin.label)
            {
                fontSize  = labelSize,
                fontStyle = FontStyle.Italic,
                normal    = { textColor = _accent }
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Text.RegularExpressions;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Graphics.Color;


namespace DGui
{
    public delegate void DTextBoxEventHandler();

    /// <summary>
    /// Base class for text input controls.
    /// Contains text append and insert logic and a cursor texture.
    /// </summary>
    public abstract class DTextBoxBase : DPanel
    {

        #region Events
        public event DTextBoxEventHandler EnterPressed;
        #endregion


        #region Private Properties
        private Keys[] _acceptedKeys = { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L,
            Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
            Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.Decimal, Keys.Add, Keys.Subtract,
        Keys.Multiply, Keys.Divide};

        private string _text = string.Empty;
        private SpriteFont _spriteFont;
        private string _fontName = "Miramonte";

        // Cursor
        private Texture2D _cursor;
        private int _cursorIndex = 0;
        private Vector2 _cursorPosition;
        private int _cursorFlashCounter = 0;
        private int _cursorFlashTime = 35;
        private bool _cursorBlinking = false;

        // Keyboard input
        private Dictionary<Keys, TimeSpan> _lastPressedKeys = new Dictionary<Keys, TimeSpan>();
        private List<Keys> _fastRepeatKeys = new List<Keys>();
        private TimeSpan _keyRepeatTime = new TimeSpan(0, 0, 0, 0, 500); // ms
        private TimeSpan _fastKeyRepeatTime = new TimeSpan(0, 0, 0, 0, 25);

        // Other properties
        private bool _readOnly = false;
        private int _maxLength = Int32.MaxValue;

        // Selection
        private int _selectionIndex = 0;
        private bool _selection = false;
        #endregion


        #region Public Properties
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetText(_text, value);
            }
        }
        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }
        public string FontName
        {
            get
            {
                return _fontName;
            }
            set
            {
                _fontName = value;
            }
        }
        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                _readOnly = value;
            }
        }
        public SpriteFont SpriteFont
        {
            get
            {
                return _spriteFont;
            }
            set
            {
                _spriteFont = value;
            }
        }
        #endregion


        #region Protected Properties
        protected bool Selection
        {
            get { return _selection; }
            set { _selection = value; }
        }
        protected int SelectionIndex
        {
            get { return _selectionIndex; }
            set { _selectionIndex = value; }
        }
        protected Vector2 CursorPosition
        {
            get { return _cursorPosition; }
            set { _cursorPosition = value; }
        }
        protected int CursorIndex
        {
            get { return _cursorIndex; }
            set { _cursorIndex = value; }
        }
        protected Texture2D Cursor
        {
            get { return _cursor; }
            set { _cursor = value; }
        }
        #endregion



        public DTextBoxBase(DGuiManager guiManager)
            : base(guiManager)
        {
        }


        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteFont = Game.Content.Load<SpriteFont>(_fontName);

            // Make a cursor texture
            int width = 1;
            int height = (int)_spriteFont.MeasureString("M").Y - 1;
            _cursor = new Texture2D(Game.GraphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Color);
            int count = width * height;
            Color[] colorArray = new Color[count];
            for (int i = 0; i < count; i++)
            {
                colorArray[i] = ColorTheme.FontColor;
            }
            _cursor.SetData<Color>(colorArray);
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Enabled)
            {
                UpdateKeyboardInput(gameTime);
            }
        }





        /// <summary>
        /// Perform keyboard input.
        /// Includes insertion and deletion logic.
        /// </summary>
        /// <param name="gameTime"></param>
        protected void UpdateKeyboardInput(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();

            // Remove any keys from last pressed keys if time has expired, or if they're up
            // Use a fast repeat keys list after the initial delay to input keys faster after being held down for a short time (windows textbox-like behavior)
            List<Keys> culledKeys = new List<Keys>(); // list of keys that can be repeated again because they have timed out or key is lifted
            foreach (KeyValuePair<Keys, TimeSpan> keyPair in _lastPressedKeys)
            {
                if (ks.IsKeyUp(keyPair.Key))
                {
                    if (_fastRepeatKeys.Contains(keyPair.Key))
                        _fastRepeatKeys.Remove(keyPair.Key);

                    culledKeys.Add(keyPair.Key);
                }
                else
                {
                    TimeSpan keySpan = gameTime.TotalGameTime - keyPair.Value;

                    if (keySpan > _keyRepeatTime && !_fastRepeatKeys.Contains(keyPair.Key))
                    {
                        // Input key and then add to the fast list
                        culledKeys.Add(keyPair.Key);
                        _fastRepeatKeys.Add(keyPair.Key);
                    }
                    else if (keySpan > _fastKeyRepeatTime && _fastRepeatKeys.Contains(keyPair.Key))
                    {
                        // Fast repeat
                        culledKeys.Add(keyPair.Key);
                    }
                }
            }
            foreach (Keys key in culledKeys)
            {
                _lastPressedKeys.Remove(key);
            }



            // Do key input.
            // Here be dragons
            if (IsFocused && Enabled && !ReadOnly)
            {
                // Select All (Ctrl+A)
                if (ks.IsKeyDown(Keys.LeftControl) && ks.IsKeyDown(Keys.A))
                {
                    SelectAll();
                }
                if (!ks.IsKeyDown(Keys.LeftControl) && !ks.IsKeyDown(Keys.LeftAlt))
                {
                    // Backspace
                    if (ks.IsKeyDown(Keys.Back) && _text.Length > 0 && !_lastPressedKeys.ContainsKey(Keys.Back))
                    {
                        if (!_selection)
                        {
                            if (_cursorIndex > 0)
                            {
                                string originalText = _text;
                                _text = _text.Substring(0, _cursorIndex - 1) + _text.Substring(_cursorIndex, _text.Length - _cursorIndex);
                                _cursorIndex--;
                                SetText(originalText, _text);
                            }
                        }
                        else
                            DeleteSelection();

                        _lastPressedKeys.Add(Keys.Back, gameTime.TotalGameTime);
                    }
                    else if (ks.IsKeyDown(Keys.Delete) && _text.Length > 0 && !_lastPressedKeys.ContainsKey(Keys.Delete))
                    {
                        if (!_selection)
                        {
                            if (_cursorIndex < _text.Length)
                            {
                                string originalText = _text;
                                _text = _text.Substring(0, _cursorIndex) + _text.Substring(_cursorIndex + 1, _text.Length - (_cursorIndex + 1));
                                SetText(originalText, _text);
                            }
                        }
                        else
                            DeleteSelection();

                        _lastPressedKeys.Add(Keys.Delete, gameTime.TotalGameTime);
                    }
                    // Enter
                    else if (ks.IsKeyDown(Keys.Enter) && _text.Length > 0 && !_lastPressedKeys.ContainsKey(Keys.Enter))
                    {
                        _lastPressedKeys.Add(Keys.Enter, gameTime.TotalGameTime);
                        if (EnterPressed != null)
                            EnterPressed();
                    }
                    else
                    {
                        // Add keystrokes to the textbox
                        string str = null;
                        Keys[] keys = ks.GetPressedKeys();

                        #region Input keys (Alphanumeric, grammatical, shifted numeral keys)
                        foreach (Keys k in keys)
                        {
                            if (!_lastPressedKeys.ContainsKey(k))
                            {
                                if (k == Keys.Left)
                                {
                                    // Move cursor left
                                    if (_cursorIndex > 0)
                                        _cursorIndex--;

                                    _selection = false;
                                    SetText(_text, _text);
                                }
                                else if (k == Keys.Right)
                                {
                                    if (_cursorIndex < _text.Length)
                                        _cursorIndex++;

                                    _selection = false;
                                    SetText(_text, _text);
                                }


                                // Numeric
                                if ((k >= Keys.D0 && k <= Keys.D9)) //||
                                //(k >= Keys.NumPad0 && k <= Keys.NumPad9))
                                {
                                    if (ks.IsKeyUp(Keys.LeftShift))
                                    {
                                        str += k.ToString().Replace("D", string.Empty);
                                    }
                                    else
                                    // Number row symbols
                                    {
                                        // Shifted keys
                                        switch (k)
                                        {
                                            case Keys.D1:
                                                str += "!";
                                                break;
                                            case Keys.D2:
                                                str += "@";
                                                break;
                                            case Keys.D3:
                                                str += "#";
                                                break;
                                            case Keys.D4:
                                                str += "$";
                                                break;
                                            case Keys.D5:
                                                str += "%";
                                                break;
                                            case Keys.D6:
                                                str += "^";
                                                break;
                                            case Keys.D7:
                                                str += "&";
                                                break;
                                            case Keys.D8:
                                                str += "*";
                                                break;
                                            case Keys.D9:
                                                str += "(";
                                                break;
                                            case Keys.D0:
                                                str += ")";
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }

                                else if (k >= Keys.A && k <= Keys.Z)
                                {
                                    if (ks.IsKeyUp(Keys.LeftShift) && ks.IsKeyUp(Keys.CapsLock))
                                        str += k.ToString().ToLower();
                                    else
                                        str += k;
                                }
                                else
                                {
                                    if (ks.IsKeyUp(Keys.LeftShift))
                                    {
                                        // Non-shifted keys
                                        switch (k)
                                        {
                                            case Keys.OemMinus:
                                                str += "-";
                                                break;
                                            case Keys.OemPlus:
                                                str += "=";
                                                break;
                                            case Keys.OemOpenBrackets:
                                                str += "[";
                                                break;
                                            case Keys.OemCloseBrackets:
                                                str += "]";
                                                break;
                                            case Keys.OemSemicolon:
                                                str += ";";
                                                break;
                                            case Keys.OemQuotes:
                                                str += "'";
                                                break;
                                            case Keys.OemComma:
                                                str += ",";
                                                break;
                                            case Keys.OemPeriod:
                                                str += ".";
                                                break;
                                            case Keys.OemBackslash:
                                                str += "/";
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        // Shifted keys
                                        switch (k)
                                        {
                                            case Keys.OemMinus:
                                                str += "_";
                                                break;
                                            case Keys.OemPlus:
                                                str += "+";
                                                break;
                                            case Keys.OemOpenBrackets:
                                                str += "{";
                                                break;
                                            case Keys.OemCloseBrackets:
                                                str += "}";
                                                break;
                                            case Keys.OemSemicolon:
                                                str += ":";
                                                break;
                                            case Keys.OemQuotes:
                                                str += "\"";
                                                break;
                                            case Keys.OemComma:
                                                str += "<";
                                                break;
                                            case Keys.OemPeriod:
                                                str += ">";
                                                break;
                                            case Keys.OemBackslash:
                                                str += "?";
                                                break;
                                            default:
                                                break;
                                        }
                                    }

                                    // Keys that dont depend on shift
                                    switch (k)
                                    {
                                        case Keys.Space:
                                            str += " ";
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                // Grammatical
                                if (k >= Keys.OemBackslash && k <= Keys.OemSemicolon)
                                {
                                    str += k;
                                }

                                // If we have input a valid string
                                if (str != null)
                                {
                                    // Text selection replace
                                    if (_selection)
                                        DeleteSelection();

                                    // Handle text scroll, cursor position
                                    string originalText = _text;

                                    if (_text.Length >= 0)
                                        _text = _text.Insert(_cursorIndex, str);
                                    else
                                        _text += str;

                                    SetText(originalText, _text);

                                    if (str.Length > 0)
                                        _cursorIndex += str.Length;
                                }


                                _lastPressedKeys.Add(k, gameTime.TotalGameTime);
                            }
                        }
                        #endregion
                    }
                }
            }
        }



        /// <summary>
        /// Select everything in the textbox.
        /// Override to perform text selection
        /// </summary>
        public virtual void SelectAll()
        {
            if (_text.Length > 0)
            {
                _cursorIndex = 0;
                _selectionIndex = _text.Length;
                _selection = true;
            }
        }


        /// <summary>
        /// Delete only the selection.
        /// </summary>
        public virtual void DeleteSelection()
        {
            // Delete selection
            string originalText = _text;
            // Discover whether cursor position or selection position is leftmost
            if (_selectionIndex < _cursorIndex)
            {
                _text = _text.Substring(0, _selectionIndex) + _text.Substring(_cursorIndex, _text.Length - _cursorIndex);
                _cursorIndex -= (_cursorIndex - _selectionIndex);
            }
            else
                _text = _text.Substring(0, _cursorIndex) + _text.Substring(_selectionIndex, _text.Length - _selectionIndex);
            SetText(originalText, _text);
            _selection = false;
            ResetCursorBlink();
        }


        /// <summary>
        /// Set text to this value.
        /// </summary>
        /// <param name="original">Original text</param>
        /// <param name="changed">Changed text</param>
        public virtual void SetText(string original, string changed)
        {
            if (changed.Length < _maxLength)
            {
                _text = changed;

                ResetCursorBlink();
                //UpdateCursorPosition();
            }
        }


        protected void ResetCursorBlink()
        {
            _cursorBlinking = false;
            _cursorFlashCounter = -(_cursorFlashTime / 4);
        }



        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Draw cursor
            if (IsFocused)
            {
                if (_cursorFlashCounter >= _cursorFlashTime)
                {
                    _cursorBlinking = !_cursorBlinking;
                    _cursorFlashCounter = 0;
                }
                _cursorFlashCounter++;


                if (!_cursorBlinking)
                {
                    _guiManager.SpriteBatch.Draw(_cursor, _cursorPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            }
        }
    }
}

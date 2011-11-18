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

using DGui.DrawingSystem;


namespace DGui
{

    /// <summary>
    /// Single line textbox.
    /// </summary>
    public class DTextBox : DTextBoxBase
    {
        const float LEFTPAD = 4;
        const int WIDTH = 180;
        const int HEIGHT = 28;

        // Text
        DText _textItem = null;


        // Text selection
        Texture2D _selectionBox;
        RectangleF _selectionBoxRectangle;
        Vector2 _selectionPosition = Vector2.Zero;
        Color _selectionBoxColor = new Color(100, 100, 100, 100);

        // Mouse input
        bool _mouseCursorPlaceDown = false;
        TimeSpan _doubleClickTime = new TimeSpan(0, 0, 0, 0, 300);
        TimeSpan _lastClickTime = TimeSpan.Zero;



        bool _allSelected = false;


        protected int _visibleTextCursorIndex = 0;




        #region Public properties
        //public Color FontColor
        //{
        //    get
        //    {
        //        return fontColor;
        //    }
        //    set
        //    {
        //        fontColor = value;
        //    }
        //}
        #endregion



        public DTextBox(DGuiManager guiManager, float x, float y, int width, int height, string text, 
            Color fillColor, Color borderColor)
            : this(guiManager, x, y, width, height, text)
        {
            this.ColorTheme.FillColor = fillColor;
            this.ColorTheme.BorderColor = borderColor;
        }

        public DTextBox(DGuiManager guiManager, float x, float y, int width, int height, string text)
            : this(guiManager, x, y, width, height)
        {
            if (text != null)
                Text = text;
        }

        public DTextBox(DGuiManager guiManager, float x, float y, int width, int height)
            : this(guiManager, x, y)
        {
            this.Size = new Vector2(width, height);
        }

        public DTextBox(DGuiManager guiManager, float x, float y, string text)
            : this(guiManager, x, y)
        {
            if (text != null)
                Text = text;
        }

        public DTextBox(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            this.Position = new Vector2(x, y);
        }

        public DTextBox(DGuiManager guiManager)
            : base(guiManager)
        {
            FillColor = ColorTheme.InputFillColor;
            BorderColor = ColorTheme.BorderColor;
            Size = new Vector2(WIDTH, HEIGHT);
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // graphical object containing the typed text
            _textItem = new DText(_guiManager);
            _textItem.FontName = FontName;
            _textItem.FontColor = ColorTheme.InputFontColor;
            _textItem.HorizontalAlignment = DText.DHorizontalAlignment.Left;
            _textItem.VerticalAlignment = DText.DVerticalAlignment.Center;
            _textItem.Initialize();
            _textItem.Text = Text;
            CursorIndex = Text.Length;
            _textItem.Position = new Vector2(LEFTPAD, LEFTPAD);
            this.AddPanel(_textItem);


            UpdateCursorPosition();


            // Create selection box texture
            _selectionBox = new Texture2D(Game.GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            Color[] selectionBoxColorArray = new Color[] { _selectionBoxColor };
            _selectionBox.SetData<Color>(selectionBoxColorArray);




            base.LoadContent();
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
                UpdateCursorSelection(gameTime);
            }

            if (IsFocused && Enabled && !ReadOnly)
            {
                UpdateCursorPosition();
            }
        }



        




        /// <summary>
        /// Perform mouse cursor select and drag-selection.
        /// Also includes double-click for select all.
        /// </summary>
        /// <param name="gameTime"></param>
        protected void UpdateCursorSelection(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            // Hover-dependent updates
            if (IsMouseHoveringOver)
            {
                // Mouse click?
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    // Check for cursor click
                    if (!_mouseCursorPlaceDown)
                    {
                        _mouseCursorPlaceDown = true;
                        if (_lastClickTime == TimeSpan.Zero)
                            _lastClickTime = gameTime.TotalGameTime;

                        if (gameTime.TotalGameTime - _lastClickTime != TimeSpan.Zero &&
                            gameTime.TotalGameTime - _lastClickTime < _doubleClickTime)
                        {
                            SelectAll();
                        }
                        else
                        {
                            Selection = false;
                            ResetCursorBlink();

                            // Cursor mouse positioning
                            int closestLetterIndex = GetClosestIndexToMouse();
                            if (closestLetterIndex != -1 && closestLetterIndex != CursorIndex)
                            {
                                CursorIndex = closestLetterIndex;
                                UpdateCursorPosition();
                            }
                        }
                        _lastClickTime = gameTime.TotalGameTime;
                    }
                }
            }
            else if (ms.LeftButton == ButtonState.Pressed && !_mouseCursorPlaceDown)
            {
                // Click outside bounds. Unfocus
                //_cursorIndex = 0;
                //_selection = false;
                UpdateCursorPosition();
            }





            // Updates whether hovering or not
            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (IsFocused)
                {
                    int closestLetterIndex = GetClosestIndexToMouse();

                    // Only update selection box if we got a valid index and we changed selection range
                    if (closestLetterIndex != -1 && closestLetterIndex != CursorIndex)
                    {
                        if (!_allSelected)
                        {
                            // Render selection box only if we have a range
                            if (closestLetterIndex != SelectionIndex || !Selection)
                            {
                                // Set an anchor if we're starting a drag
                                if (!Selection)
                                {
                                    SelectionIndex = CursorIndex;
                                    _selectionPosition = CursorPosition;
                                }
                                CursorIndex = closestLetterIndex;

                                // Get the position of the drag, use with cursor position to construct selection box
                                Vector2 dragLocation = GetClosestCursorPositionToMouse();
                                Vector2 leftmostSelectionPoint = Vector2.Zero;
                                if (CursorIndex < SelectionIndex)
                                {
                                    SetSelectionRectangle(new RectangleF(dragLocation.X, dragLocation.Y,
                                        _selectionPosition.X - dragLocation.X,
                                        Cursor.Height));
                                }
                                else
                                {
                                    SetSelectionRectangle(new RectangleF(_selectionPosition.X, dragLocation.Y,
                                        dragLocation.X - _selectionPosition.X,
                                        Cursor.Height));
                                }
                            }
                            else
                            {
                                // We're at the cursor, remove selection
                                Selection = false;
                                CursorIndex = closestLetterIndex;
                                //ResetCursorBlink();
                            }
                        }
                    }
                }
            }
            else
            {
                _mouseCursorPlaceDown = false;
                //AllSelected = false;
            }
        }




        /// <summary>
        /// Select all text in the text box.
        /// </summary>
        public override void SelectAll()
        {
            base.SelectAll();

            if (Text.Length > 0)
            {
                _allSelected = true;
                CursorPosition = new Vector2(_textItem.AbsoluteTransform.X, _textItem.AbsoluteTransform.Y);
                _selectionPosition = new Vector2(CursorPosition.X + _textItem.SpriteFont.MeasureString(_textItem.Text).X, CursorPosition.Y);
                SetSelectionRectangle(new RectangleF(CursorPosition.X, CursorPosition.Y, _selectionPosition.X - CursorPosition.X, Cursor.Height));
            }
        }






        /// <summary>
        /// Recreate the selection texture rectangle to fit the current selection size.
        /// </summary>
        /// <param name="rectangle">Rectangle size. Position is unused.</param>
        private void SetSelectionRectangle(RectangleF rectangle)
        {
            if (rectangle.Width >= 1 && rectangle.Height >= 1)
            {
                _selectionBox = new Texture2D(Game.GraphicsDevice, (int)(rectangle.Width), (int)rectangle.Height, 1, TextureUsage.None, SurfaceFormat.Color);
                int count = _selectionBox.Width * _selectionBox.Height;
                Color[] colorArray = new Color[count];
                for (int i = 0; i < count; i++)
                    colorArray[i] = _selectionBoxColor;
                _selectionBox.SetData<Color>(colorArray);
                _selectionBoxRectangle = rectangle;
                Selection = true;
            }
        }




        private int GetClosestIndexToMouse()
        {
            MouseState ms = Mouse.GetState();

            // Iterate through substrings of the text to see if we can find the closest point where the cursor lies
            string substring = Text.Substring(_visibleTextCursorIndex, Text.Length - _visibleTextCursorIndex);
            float closestXValue = 0f;
            int closestLetterIndex = -1;
            for (int i = 0; i <= substring.Length; i++)
            {
                Vector2 lettersSize = _textItem.SpriteFont.MeasureString(substring.Substring(0, i));
                float lettersX = _textItem.AbsoluteTransform.X + lettersSize.X;
                float distance = Math.Abs(ms.X - lettersX);
                if (distance < closestXValue || closestLetterIndex == -1)
                {
                    closestXValue = distance;
                    closestLetterIndex = i;
                }
            }
            return closestLetterIndex;
        }



        private Vector2 GetClosestCursorPositionToMouse()
        {
            int closestIndex = GetClosestIndexToMouse();
            Vector2 lettersSize = _textItem.SpriteFont.MeasureString(Text.Substring(_visibleTextCursorIndex, closestIndex));
            Vector2 result = new Vector2(_textItem.AbsoluteTransform.X + lettersSize.X, _textItem.AbsoluteTransform.Y);
            return result;
        }








        public override void SetText(string original, string changed)
        {
            base.SetText(original, changed);
            UpdateCursorPosition();
        }



        private void UpdateCursorPosition()
        {
            // Clip off the left character until we fit all, including the cursor
            if (Text.Length >= 0)
            {
                int startIndex = -1;
                bool foundShortestString = false;
                string substring = string.Empty;

                // Find the first visible character index that allows the cursor to be seen
                do
                {
                    startIndex++;
                    substring = Text.Substring(startIndex, CursorIndex - startIndex);
                    float length = _textItem.SpriteFont.MeasureString(substring).X;
                    if (length + 1 + LEFTPAD < (Size.X - LEFTPAD))
                        foundShortestString = true;
                } while (!foundShortestString);
                _visibleTextCursorIndex = startIndex;

                // Next, find the maximum substring we can display
                int subStringCount = 0;
                bool foundWholeString = false;
                string wholeString = string.Empty;
                do
                {

                    if (_visibleTextCursorIndex + subStringCount >= Text.Length)
                        break;

                    wholeString = Text.Substring(_visibleTextCursorIndex, subStringCount);
                    float length = _textItem.SpriteFont.MeasureString(wholeString).X;
                    if (length + 1 + LEFTPAD >= Size.X - LEFTPAD)
                    {
                        foundWholeString = true;
                        subStringCount--;
                    }
                    else
                        subStringCount++;
                } while (!foundWholeString);


                _textItem.Text = Text.Substring(_visibleTextCursorIndex, subStringCount);




                // Our cursor index changed
                Vector2 textLeftOfCursorSize;

                int zoomAmount = 8;
                if (CursorIndex < _visibleTextCursorIndex)
                {
                    if (_visibleTextCursorIndex > zoomAmount)
                        _visibleTextCursorIndex -= zoomAmount;
                    else
                        _visibleTextCursorIndex = 0;
                }


                textLeftOfCursorSize = _textItem.SpriteFont.MeasureString(Text.Substring(_visibleTextCursorIndex, CursorIndex - _visibleTextCursorIndex));
                CursorPosition = new Vector2(_textItem.AbsoluteTransform.X + textLeftOfCursorSize.X, _textItem.AbsoluteTransform.Y);
            }
        }




        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Draw cursor
            if (IsFocused)
            {
                if (Selection)
                {
                    _guiManager.SpriteBatch.Draw(_selectionBox,
                        new Vector2(_selectionBoxRectangle.X, _selectionBoxRectangle.Y),
                        null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            }
        }
    }
}

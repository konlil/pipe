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
    /// Multi-line textbox
    /// </summary>
    public class DMultiLineTextBox : DTextBoxBase
    {
        const float LEFTPAD = 4;
        const int WIDTH = 220;
        const int HEIGHT = 200;

        // Text
        List<DText> _textItems = new List<DText>();


        // Text selection
        List<Texture2D> _selectionBoxes = new List<Texture2D>(); // scaling up looks bad so use different textures for each
        List<RectangleF> _selectionBoxRectangles = new List<RectangleF>();

        // Multi-line specific
        List<int> _lineStartIndexes = new List<int>();
        int _lineIndex = 0;
        //int _selectionLineIndex = 0;



        Vector2 _selectionPosition = Vector2.Zero;
        Color _selectionBoxColor = new Color(100, 100, 100, 100);

        // Mouse input
        bool _mouseCursorPlaceDown = false;
        TimeSpan _doubleClickTime = new TimeSpan(0, 0, 0, 0, 300);
        TimeSpan _lastClickTime = TimeSpan.Zero;



        bool _allSelected = false;



        private int CurrentLineStartIndex
        {
            get
            {
                if (_lineIndex < _lineStartIndexes.Count)
                    return _lineStartIndexes[_lineIndex];
                else
                    return 0;
            }
        }




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



        public DMultiLineTextBox(DGuiManager guiManager, float x, float y, int width, int height, string text, 
            Color fillColor, Color borderColor)
            : this(guiManager, x, y, width, height, text)
        {
            this.ColorTheme.FillColor = fillColor;
            this.ColorTheme.BorderColor = borderColor;
        }

        public DMultiLineTextBox(DGuiManager guiManager, float x, float y, int width, int height, string text)
            : this(guiManager, x, y, width, height)
        {
            if (text != null)
                Text = text;
        }

        public DMultiLineTextBox(DGuiManager guiManager, float x, float y, int width, int height)
            : this(guiManager, x, y)
        {
            this.Size = new Vector2(width, height);
        }

        public DMultiLineTextBox(DGuiManager guiManager, float x, float y, string text)
            : this(guiManager, x, y)
        {
            if (text != null)
                Text = text;
        }

        public DMultiLineTextBox(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            this.Position = new Vector2(x, y);
        }

        public DMultiLineTextBox(DGuiManager guiManager)
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
            base.LoadContent();

            // graphical object containing the typed text
            DText defaultTextItem = CreateNewLineText();
            defaultTextItem.Text = Text;
            CursorIndex = Text.Length;
            defaultTextItem.Position = new Vector2(LEFTPAD, LEFTPAD);
            this.AddPanel(defaultTextItem);
            _textItems.Add(defaultTextItem);


            _lineStartIndexes.Add(0);


            UpdateCursorPosition();




            
        }


        private DText CreateNewLineText()
        {
            DText defaultTextItem = new DText(_guiManager);
            defaultTextItem.FontName = FontName;
            defaultTextItem.FontColor = ColorTheme.InputFontColor;
            defaultTextItem.HorizontalAlignment = DText.DHorizontalAlignment.Left;
            defaultTextItem.VerticalAlignment = DText.DVerticalAlignment.Center;
            defaultTextItem.Initialize();
            return defaultTextItem;
        }



        private Texture2D CreateSelectionBoxTexture(int width, int height)
        {
            Texture2D selectionBox = new Texture2D(Game.GraphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Color);
            int count = selectionBox.Width * selectionBox.Height;
            Color[] colorArray = new Color[count];
            for (int i = 0; i < count; i++)
                colorArray[i] = _selectionBoxColor;
            selectionBox.SetData<Color>(colorArray);
            return selectionBox;
        }


        private void UpdateCursorPosition()
        {
            // Find the line that we belong on
            _lineIndex = FindLineIndexFromPosition(CursorIndex);

            if (CurrentLineStartIndex + (CursorIndex - CurrentLineStartIndex) <= Text.Length)
            {
                Vector2 textLeftOfCursorSize = SpriteFont.MeasureString(Text.Substring(CurrentLineStartIndex, CursorIndex - CurrentLineStartIndex));
                CursorPosition = new Vector2(_textItems[_lineIndex].AbsoluteTransform.X + textLeftOfCursorSize.X, _textItems[_lineIndex].AbsoluteTransform.Y);
            }
        }


        private int FindLineIndexFromPosition(int cursorIndex)
        {
            int newIndex = 0;
            for (int i = 0; i < _lineStartIndexes.Count; i++)
            {
                if (_lineStartIndexes[i] < cursorIndex)
                {
                    newIndex = i;
                }
            }
            return newIndex;
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


        public override void SetText(string original, string changed)
        {
            base.SetText(original, changed);


            // Could be either adding or removing here.
            // Find the current line we're on (post-change).
            // Render all line indexes after this one null and void.
            // Recalculate each's starting index and set text accordingly.

            UpdateCursorPosition();


            // Recalculate each starting index and set text accordingly.
            for (int i = _lineIndex; i < _lineStartIndexes.Count; i++)
            {
                // Find the maximum substring we can display
                int subStringCount = 0;
                bool foundWholeString = false;
                bool endOfText = false;
                string wholeString = string.Empty;
                do
                {
                    if (_lineStartIndexes[i] + subStringCount > Text.Length)
                    {
                        endOfText = true;
                        subStringCount--;
                        break;
                    }

                    wholeString = Text.Substring(_lineStartIndexes[i], subStringCount);
                    float length = SpriteFont.MeasureString(wholeString).X;
                    if (length >= (this.Size.X - (2 * LEFTPAD)))
                    {
                        foundWholeString = true;
                        subStringCount--;
                    }
                    else
                        subStringCount++;
                } while (!foundWholeString);


                // Set the text item
                _textItems[i].Text = Text.Substring(_lineStartIndexes[i], subStringCount);



                if (endOfText)
                {
                    // If we have more lines left
                    if (i < _lineStartIndexes.Count - 1)
                    {
                        // Cull all remaining lines
                        for (int remainingIndex = _lineStartIndexes.Count - 1; remainingIndex > i; remainingIndex--)
                        {
                            _lineStartIndexes.RemoveAt(remainingIndex);
                            this.Children.Remove(_textItems[remainingIndex]);
                            _textItems.RemoveAt(remainingIndex);
                            if (_lineIndex == remainingIndex)
                                _lineIndex--;
                        }
                    }
                }
                else if (i < _lineStartIndexes.Count - 1)
                {
                    // We're not at EOT and we have more lines left. Set the next index and continue.
                    _lineStartIndexes[i + 1] = _lineStartIndexes[i] + subStringCount;
                }
                else
                {
                    // It is the last line and we're not at EOT.
                    // Add a new line
                    string remainingString = Text.Substring(_lineStartIndexes[i], Text.Length - _lineStartIndexes[i]);
                    float length = SpriteFont.MeasureString(remainingString).X;
                    if (length >= (this.Size.X - (2 * LEFTPAD)))
                    {
                        _lineStartIndexes.Add(_lineStartIndexes[i] + subStringCount);
                        DText newLine = CreateNewLineText();
                        newLine.Position = _textItems[i].Position + new Vector2(0, Cursor.Height);
                        this.AddPanel(newLine);
                        _textItems.Add(newLine);
                    }
                }
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
                            int lineIndex;
                            int closestLetterIndex = GetClosestIndexToMouse(out lineIndex);
                            if (closestLetterIndex != -1 && lineIndex != -1)
                            {
                                closestLetterIndex += _lineStartIndexes[lineIndex];
                                if (closestLetterIndex != CursorIndex)
                                {
                                    CursorIndex = closestLetterIndex;
                                    UpdateCursorPosition();
                                }
                            }
                        }
                        _lastClickTime = gameTime.TotalGameTime;
                    }
                }
            }
            else if (ms.LeftButton == ButtonState.Pressed && !_mouseCursorPlaceDown)
            {
                // Click outside bounds. Unfocus
                ////_cursorIndex = 0;
                ////_selection = false;
                //UpdateCursorPosition();
            }





            //// Updates whether hovering or not
            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (IsFocused)
                {
                    int lineIndex;
                    int closestLetterIndex = GetClosestIndexToMouse(out lineIndex);
                    if (closestLetterIndex != -1 && lineIndex != -1)
                    {
                        closestLetterIndex += _lineStartIndexes[lineIndex];

                        // Only update selection box if we got a valid index and we changed selection range
                        if (closestLetterIndex != CursorIndex)
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
                                    if (CursorIndex < SelectionIndex)
                                    {
                                        //SetSelectionRectangle(new RectangleF(dragLocation.X, dragLocation.Y,
                                        //    _selectionPosition.X - dragLocation.X,
                                        //    Cursor.Height));
                                    }
                                    else
                                    {
                                        //SetSelectionRectangle(new RectangleF(_selectionPosition.X, dragLocation.Y,
                                        //    dragLocation.X - _selectionPosition.X,
                                        //    Cursor.Height));
                                    }
                                    Selection = true;
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
            }
            else
            {
                _mouseCursorPlaceDown = false;
                _allSelected = false;
            }
        }




        private void SetSelection(int selectionStart, int selectionEnd)
        {
            // Find the lines that these selections will include
            // Create selection textures and positions
            int start = selectionStart;
            int end = selectionEnd;
            if (start > end)
            {
                start = selectionEnd;
                end = selectionStart;
            }

            int startLine = FindLineIndexFromPosition(start);
            int endLine = FindLineIndexFromPosition(end);

            // TODO: create some selection textures and positions
            if (startLine == endLine)
            {
                // Single line select
            }
            else
            {
                // Multi line select
                for (int i = startLine; i <= endLine; i++)
                {
                    if (i == startLine)
                    {
                        // Create selection from start point to line end
                    }
                    else if (i == endLine)
                    {
                        // Create selection from line start to end point
                    }
                    else
                    {
                        // Create selection spanning the whole line.
                    }
                }
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
                CursorPosition = new Vector2(_textItems[0].AbsoluteTransform.X, _textItems[0].AbsoluteTransform.Y);
                //_selectionPosition = new Vector2(CursorPosition.X + SpriteFont.MeasureString(_textItem.Text).X, CursorPosition.Y);
                //SetSelectionRectangle(new RectangleF(CursorPosition.X, CursorPosition.Y, _selectionPosition.X - CursorPosition.X, Cursor.Height));
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
                //if (Selection)
                //{
                //    _guiManager.SpriteBatch.Draw(_selectionBox,
                //        new Vector2(_selectionBoxRectangle.X, _selectionBoxRectangle.Y),
                //        null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                //}
            }
        }


        private int GetClosestIndexToMouse(out int lineIndex)
        {
            MouseState ms = Mouse.GetState();

            float closestLineDistance = 0f;
            int closestLineIndex = -1;
            for (int i = 0; i < _textItems.Count; i++)
            {
                if (!_textItems[i].IsTotallyObscured)
                {
                    float distanceAbs = Math.Abs(ms.Y - (_textItems[i].AbsoluteTransform.Y + (_textItems[i].Size.Y / 2)));
                    if (distanceAbs < closestLineDistance || closestLineIndex == -1)
                    {
                        closestLineDistance = distanceAbs;
                        closestLineIndex = i;
                    }
                }
            }


            int closestLetterIndex = -1;
            if (closestLineIndex != -1)
            {
                // Iterate through substrings of the text to see if we can find the closest point where the cursor lies
                string substring = Text.Substring(_lineStartIndexes[closestLineIndex], Text.Length - _lineStartIndexes[closestLineIndex]);
                float closestXValue = 0f;

                for (int i = 0; i <= substring.Length; i++)
                {
                    Vector2 lettersSize = _textItems[closestLineIndex].SpriteFont.MeasureString(substring.Substring(0, i));
                    float lettersX = _textItems[closestLineIndex].AbsoluteTransform.X + lettersSize.X;
                    float distance = Math.Abs(ms.X - lettersX);
                    if (distance < closestXValue || closestLetterIndex == -1)
                    {
                        closestXValue = distance;
                        closestLetterIndex = i;
                    }
                }
            }

            lineIndex = closestLineIndex;
            return closestLetterIndex;
        }



        private Vector2 GetClosestCursorPositionToMouse()
        {
            int lineIndex;
            int closestIndex = GetClosestIndexToMouse(out lineIndex);
            if (closestIndex != -1 && lineIndex != -1)
            {
                Vector2 lettersSize = SpriteFont.MeasureString(Text.Substring(_lineStartIndexes[lineIndex], closestIndex));
                Vector2 result = new Vector2(_textItems[lineIndex].AbsoluteTransform.X + lettersSize.X, _textItems[lineIndex].AbsoluteTransform.Y);
                return result;
            }
            return Vector2.Zero;
        }




    }
}

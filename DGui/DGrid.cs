using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

 

namespace DGui
{
    public class DGrid : DPanel
    {
        public enum DGridFillType { None, DText, DButton };

        protected DGridFillType _fillType = DGridFillType.None;
        protected DPanel[,] _panelArray;

        protected int _columns = 2;
        protected int _rows = 2;
        protected int _gridLineWidth = 1;

        protected float[] _cellWidths;
        protected float[] _cellHeights;

        protected int _cellWidth = 100;
        protected int _cellHeight = 40;

        protected Texture2D[] _columnLines;
        protected Texture2D[] _rowLines;

        #region Public properties
        public DPanel[,] PanelArray
        {
            get { return _panelArray; }
        }
        public int Columns
        {
            get { return _columns; }
            set
            {
                if (value > 0)
                {
                    _columns = value;
                    CreateGrid();
                }
            }
        }
        public int Rows
        {
            get { return _rows; }
            set
            {
                if (value > 0)
                {
                    _rows = value;
                    CreateGrid();
                }
            }
        }
        public int GridLineWidth
        {
            get { return _gridLineWidth; }
            set
            {
                if (value >= 0)
                {
                    _gridLineWidth = value;
                    CreateGrid();
                }
            }
        }
        public Color GridColor
        {
            get;
            set;
        }
        public int CellWidth
        {
            get { return _cellWidth; }
            set
            {
                if (value > 0)
                {
                    _cellWidth = value;
                    CreateGrid();
                }
            }
        }
        public int CellHeight
        {
            get { return _cellHeight; }
            set
            {
                if (value > 0)
                {
                    _cellHeight = value;
                    CreateGrid();
                }
            }
        }
        #endregion



        #region Constructor
        public DGrid(DGuiManager guiManager, int columns, int rows, DGridFillType fillType)
            : this(guiManager, columns, rows)
        {
            _fillType = fillType;
        }

        public DGrid(DGuiManager guiManager, int columns, int rows) : base(guiManager)
        {
            _guiManager = guiManager;
            _columns = columns;
            _rows = rows;
            if (_columns <= 0 || _rows <= 0)
                throw new Exception("Cannot create a DGrid with zero or negative columns/rows.");

            GridColor = ColorTheme.FillColor;
        }
        #endregion


        public override void Initialize()
        {
            this.BorderWidth = 0;
            this.ColorTheme.FillColor = Color.TransparentWhite;
            base.Initialize();
        }


        protected override void LoadContent()
        {
            base.LoadContent();

            CreateGrid();
        }



        protected Vector2 GridPosition(int x, int y)
        {
            return new Vector2((x * _cellWidth) + ((x + 1) * _gridLineWidth),
                               (y * _cellHeight) + ((y + 1) * _gridLineWidth));
        }


        protected void CreateGrid()
        {
            _panelArray = new DPanel[_columns, _rows];
            _cellWidths = new float[_columns];
            _cellHeights = new float[_rows];
            _columnLines = new Texture2D[_columns + 1];
            _rowLines = new Texture2D[_rows + 1];

            for (int i = 0; i < _columns; i++)
            {
                _cellWidths[i] = _cellWidth;
            }
            for (int i = 0; i < _rows; i++)
            {
                _cellHeights[i] = _cellHeight;
            }
            this.Size = new Vector2(_cellWidths.Length * (_cellWidth + _gridLineWidth), _cellHeights.Length * (_cellHeight + _gridLineWidth));

            if (_gridLineWidth > 0)
            {
                for (int i = 0; i < _columnLines.Length; i++)
                {
                    _columnLines[i] = DrawingSystem.DrawingHelper.CreateLineTexture(Game.GraphicsDevice, _gridLineWidth, (int)this.Size.Y, GridColor);
                }
                for (int i = 0; i < _rowLines.Length; i++)
                {
                    _rowLines[i] = DrawingSystem.DrawingHelper.CreateLineTexture(Game.GraphicsDevice, (int)this.Size.X + _gridLineWidth, _gridLineWidth, GridColor);
                }
            }



            for (int x = 0; x < _columns; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    if (_fillType != DGridFillType.None)
                    {
                        Vector2 gridPosition = GridPosition(x,y);

                        if (_fillType == DGridFillType.DButton)
                        {
                            DButton button = new DButton(_guiManager,
                                gridPosition.X,
                                gridPosition.Y,
                                x.ToString() + ", " + y.ToString(),
                                _cellWidth,
                                _cellHeight);
                            button.FontName = "Miramonte";
                            _panelArray[x, y] = button;
                            this.AddPanel(button);
                        }
                        else if (_fillType == DGridFillType.DText)
                        {
                            DText text = new DText(_guiManager,
                                gridPosition.X + (_cellWidth / 2),
                                gridPosition.Y + (_cellHeight / 2),
                                _cellWidth,
                                _cellHeight,
                                x.ToString() + ", " + y.ToString());
                            text.HorizontalAlignment = DText.DHorizontalAlignment.Center;
                            text.VerticalAlignment = DText.DVerticalAlignment.Center;
                            text.FontName = "Miramonte";
                            _panelArray[x, y] = text;
                            this.AddPanel(text);
                        }
                    }
                }
            }
        }




        public void AddGridPanel(int x, int y, DPanel panel)
        {
            if (x >= 0 && x < _columns && y >= 0 && y < _rows && _panelArray[x, y] == null)
            {
                _panelArray[x, y] = panel;
                panel.Position = GridPosition(x,y);
                panel.Size = new Vector2(_cellWidth, _cellHeight);


                if (panel is DText) //&& 
                    //(panel as DText).HorizontalAlignment == DText.DHorizontalAlignment.Center && 
                    //(panel as DText).VerticalAlignment == DText.DVerticalAlignment.Center)
                    panel.Position += new Vector2((float)(_cellWidth / 2), (float)(_cellHeight / 2));

                this.AddPanel(panel);
                //panel.RecreateTexture();
            }
        }

        public void RemoveGridPanel(int x, int y)
        {
            if (x >= 0 && x < _columns && y >= 0 && y < _rows && _panelArray[x, y] == null)
            {
                this.Children.Remove(_panelArray[x, y]);
                _panelArray[x, y] = null;
            }
        }



        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_gridLineWidth > 0)
            {
                for (int i = 0; i < _columnLines.Length; i++)
                {
                    Vector2 drawPos = new Vector2(AbsoluteTransform.X + (i * (_cellWidth + _gridLineWidth)), AbsoluteTransform.Y);
                    // Draw the panel
                    _guiManager.SpriteBatch.Draw(_columnLines[i],
                                        drawPos + Offset,
                                        _sourceRectangle,
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        1,
                                        SpriteEffects.None,
                                        0);
                }
                for (int i = 0; i < _rowLines.Length; i++)
                {
                    Vector2 drawPos = new Vector2(AbsoluteTransform.X, AbsoluteTransform.Y + (i * (_cellHeight + _gridLineWidth)));
                    // Draw the panel
                    _guiManager.SpriteBatch.Draw(_rowLines[i],
                                        drawPos + Offset,
                                        _sourceRectangle,
                                        Color.White,
                                        0,
                                        Vector2.Zero,
                                        1,
                                        SpriteEffects.None,
                                        0);
                }
            }
        }



    }
}

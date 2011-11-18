using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Xna.Framework;

 

namespace DGui
{
    /// <summary>
    /// Grid layout for DPanel and derivatives
    /// </summary>
    public class DLayoutFlow
    {
        public enum DLayoutFlowStyle { Horizontally, Vertically };
        public enum DCellStyle { Fixed, Dynamic };
        public enum DLayoutVerticalAlignment { TopDown, BottomUp };
        public enum DLayoutHorizontalAlignment { LeftToRight, RightToLeft };

        protected DLayoutFlowStyle layoutFlow = DLayoutFlowStyle.Vertically;
        protected DCellStyle cellStyle = DCellStyle.Dynamic;
        protected DLayoutVerticalAlignment verticalAlignment = DLayoutVerticalAlignment.TopDown;
        protected DLayoutHorizontalAlignment horizontalAlignment = DLayoutHorizontalAlignment.LeftToRight;

        protected Vector2 position;

        protected int maxColumns = 3;
        protected int maxRows = 12;
        protected int cellWidth = 140;
        protected int cellHeight = 40;
        protected int cellPadding = 20; // interior padding
        protected bool useRowPercentages = false;
        protected bool useColumnPercentages = false;
        protected Collection<float> rowWidthPercentages = new Collection<float>();
        protected Collection<float> columnWidthPercentages = new Collection<float>();


        protected DPanel[,] panelArray;
        private bool[,] populatedArray;


        #region Public properties
        public bool UseColumnPercentages
        {
            get
            {
                return useColumnPercentages;
            }
            set
            {
                useColumnPercentages = value;
            }
        }
        public bool UseRowPercentages
        {
            get
            {
                return useRowPercentages;
            }
            set
            {
                useRowPercentages = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public int MaxColumns
        {
            get
            {
                return maxColumns;
            }
            set
            {
                maxColumns = value;
            }
        }
        public int MaxRows
        {
            get
            {
                return maxRows;
            }
            set
            {
                maxRows = value;
            }
        }
        public int CellWidth
        {
            get
            {
                return cellWidth;
            }
            set
            {
                cellWidth = value;
            }
        }
        public int CellHeight
        {
            get
            {
                return cellHeight;
            }
            set
            {
                cellHeight = value;
            }
        }
        /// <summary>
        /// Cell interior padding.
        /// </summary>
        public int CellPadding
        {
            get
            {
                return cellPadding;
            }
            set
            {
                cellPadding = value;
            }
        }
        public DCellStyle CellStyle
        {
            get
            {
                return cellStyle;
            }
            set
            {
                cellStyle = value;
            }
        }
        public DLayoutFlowStyle LayoutFlow
        {
            get
            {
                return layoutFlow;
            }
            set
            {
                layoutFlow = value;
            }
        }
        public DPanel[,] PanelArray
        {
            get
            {
                return panelArray;
            }
            set
            {
                panelArray = value;
            }
        }
        #endregion



        #region Constructor
        public DLayoutFlow(int columns, int rows)
        {
            maxColumns = columns;
            maxRows = rows;
            panelArray = new DPanel[maxColumns, maxRows];
            populatedArray = new bool[maxColumns, maxRows];
        }

        public DLayoutFlow(int columns, int rows, DLayoutFlowStyle flow) : this(columns, rows)
        {
            layoutFlow = flow;
        }

        public DLayoutFlow(int columns, int rows, int itemWidth, int itemHeight)
            : this(columns, rows)
        {
            cellWidth = itemWidth;
            cellHeight = itemHeight;
        }

        public DLayoutFlow(int columns, int rows, int itemWidth, int itemHeight, DLayoutFlowStyle flow)
            : this(columns, rows, itemWidth, itemHeight)
        {
            layoutFlow = flow;
        }
        #endregion


        #region Add
        /// <summary>
        /// Add the panel to the layout.
        /// Position it according to the layout flow and the existing items
        /// </summary>
        /// <param name="panel"></param>
        public void Add(DPanel panel)
        {
            bool added = false;
            if (layoutFlow == DLayoutFlowStyle.Horizontally)
            {
                // y,x iteration
                for (int y = 0; y < maxRows; y++)
                {
                    for (int x = 0; x < maxColumns; x++)
                    {
                        if (populatedArray[x, y] == false)
                        {
                            AddToCell(x, y, panel);
                            added = true;
                            break;
                        }
                    }
                    if (added)
                        break;
                }
            }
            else if (layoutFlow == DLayoutFlowStyle.Vertically)
            {
                // x,y iteration
                for (int x = 0; x < maxColumns; x++)
                {
                    for (int y = 0; y < maxRows; y++)
                    {
                        if (populatedArray[x, y] == false)
                        {
                            AddToCell(x, y, panel);
                            added = true;
                            break;
                        }
                    }
                    if (added)
                        break;
                }
            }
        }
        #endregion


        #region Clear
        public void Clear()
        {
            populatedArray = new bool[maxColumns, maxRows];
        }
        #endregion


        #region AddToCell
        void AddToCell(int x, int y, DPanel panel)
        {
            // TODO: Reposition all cells if inserting in the middle
            // separate function probably

            if (x < populatedArray.GetLength(0) && y < populatedArray.GetLength(1))
            {
                populatedArray[x, y] = true;

                float xPos = Position.X;
                float yPos = Position.Y;
                if (cellStyle == DCellStyle.Fixed)
                {
                    xPos = Position.X + (x * (cellWidth + cellPadding));
                    yPos = Position.Y + (y * (cellHeight + cellPadding));
                }
                else if (cellStyle == DCellStyle.Dynamic)
                {
                    xPos = Position.X;
                    yPos = Position.Y;

                    // Calc X dimension
                    for (int i = 0; i < x; i++)
                    {                 
                        if (populatedArray[i, y] == true)
                            xPos += cellPadding + panelArray[i, y].Width;
                    }

                    // Calc Y dimension
                    for (int i = 0; i < y; i++)
                    {
                        if (populatedArray[x, i] == true)
                            yPos += cellPadding + panelArray[x, i].Height;
                    }
                }

                // Set the top left corner of this cell
                Vector2 cellPosition = new Vector2(xPos, yPos);
                panel.Position = cellPosition;
                panelArray[x, y] = panel;
            }
        }
        #endregion
    }
}

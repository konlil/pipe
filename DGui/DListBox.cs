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

using DGui.DrawingSystem;

 

namespace DGui
{
    // Delegate functions (for communicating with other classes)
    public delegate void ListBoxChangeEventHandler();

    // List of selectable items that can be image/text
    public class DListBox : DPanel
    {
        const float PADDING = 5;
        const float LINESPACING = 2;
        const int WIDTH = 180;
        const int HEIGHT = 200;


        ContentManager content;
        //bool Focused = false;
        public event ListBoxChangeEventHandler OnItemSelect;
        int _selectedIndex = -1;
        int _totalListHeight = 0;
        //int listScrollValue = 0;

        // List of items - image, text, name
        protected Collection<DListBoxItem> _listItems = new Collection<DListBoxItem>();
        protected DScrollBar _verticalScrollBar = null;


        protected SpriteFont _spriteFont;
        protected Color selectorFillColor = Color.White;
        protected Color selectorBorderColor = Color.Black;
        protected Color fontColor = Color.MidnightBlue;

        protected float _itemHeight = 35f;


        #region Public properties
        public float ItemHeight
        {
            get
            {
                return _itemHeight;
            }
            set
            {
                _itemHeight = value;
            }
        }
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
            }
        }
        public Collection<DListBoxItem> Items
        {
            get
            {
                return _listItems;
            }
            set
            {
                _listItems = value;
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
        public Color FontColor
        {
            get
            {
                return fontColor;
            }
            set
            {
                fontColor = value;
            }
        }
        #endregion






        public DListBox(DGuiManager guiManager, float x, float y, int _width, int _height, 
            Color _fillColor, Color _borderColor)
            : this(guiManager, x, y, _width, _height)
        {
            this.FillColor = _fillColor;
            this.BorderColor = _borderColor;
        }

        public DListBox(DGuiManager guiManager, float x, float y, int _width, int _height)
            : this(guiManager, x, y)
        {
            this.Size = new Vector2(_width, _height);
        }

        public DListBox(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            this.Position = new Vector2(x, y);
        }

        public DListBox(DGuiManager guiManager)
            : base(guiManager)
        {
            FillColor = Color.White;
            BorderColor = Color.Black;
            Size = new Vector2(WIDTH, HEIGHT);

            content = new ContentManager(guiManager.Game.Services);
            content.RootDirectory = "Content";

            _verticalScrollBar = new DScrollBar(guiManager, 0, 0);
            _verticalScrollBar.Max = 0;
            _verticalScrollBar.Step = 0;
        }



        protected void ResizeHandler(Vector2 newSize)
        {
            _verticalScrollBar.Position = new Vector2(newSize.X - _verticalScrollBar.ScrollbarWidth, 0);
            _verticalScrollBar.Size = new Vector2(_verticalScrollBar.ScrollbarWidth, newSize.Y);
        }


        public void AddListItem(DListBoxItem item)
        {
            // Police position and size
            // Obtain next Y position
            Vector2 nextPos = NextLineTextPos();
            item.ListBox = this;
            item.Visible = false;
            item.UseInteriorClipping = true; // make sure this item is clipped at the interior edge of its parent's border, not the absolute edge

            item.Position = nextPos;
            _listItems.Add(item);

            // Ensure scrollbar uses relevant scroll area
            int oldTotalListHeight = _totalListHeight;
            _totalListHeight += (int) (item.Size.Y + LINESPACING);
            if (_totalListHeight > (Height - LINESPACING))
            {
                _verticalScrollBar.Visible = true;
                UpdateScrollIndicator();
 
                RefreshAllListBoxSizes();
            }
            else
                // Size item based on scrollbar visibility
                RefreshListBoxItemSize(item);

            item.Initialize();
            this.Children.Add(item);


            this.Update(new GameTime());
            //item.Update(new GameTime());
            item.Visible = true;
            
            item.OnSelect += new DListBoxItemSelectedEventHandler(ItemSelected);

            // Resize handler to deal with extra shit
            this.OnResize += new DPanelResizeHandler(ResizeHandler);
        }


        /// <summary>
        /// Scale max and step so we do not go over the max distance or under the min
        /// </summary>
        protected void UpdateScrollIndicator()
        {
            if (_totalListHeight > 0)
            {
                // Scale max and step so we do not go over the max distance or under the min
                float step = (Height - (PADDING + LINESPACING));
                float max = _totalListHeight;
                float indicatorRatio = (max - step) / max;

                _verticalScrollBar.Step = step * indicatorRatio;
                _verticalScrollBar.Max = max * indicatorRatio;
            }
        }


        /// <summary>
        /// Refresh the X size based on whether the vertical scrollbar is visible
        /// </summary>
        /// <param name="item"></param>
        void RefreshListBoxItemSize(DListBoxItem item)
        {
            if (_verticalScrollBar.Visible)
                item.Size = new Vector2(Size.X - (2f * PADDING) - _verticalScrollBar.ScrollbarWidth, item.Size.Y);
            else
                item.Size = new Vector2(Size.X - (2f * PADDING), item.Size.Y);
            item.RecreateTexture();
        }

        void RefreshAllListBoxSizes()
        {
            foreach (DListBoxItem item in Items)
            {
                RefreshListBoxItemSize(item);
            }
            if (_selectedIndex >= 0 && _selectedIndex < _listItems.Count)
            {
                _listItems[_selectedIndex].Select(); // restore selected texture
            }
        }



        public void ClearItems()
        {
            foreach (DListBoxItem cmbItem in Items)
            {
                this.Children.Remove(cmbItem);
                cmbItem.Dispose();
            }
            Items.Clear();
            _verticalScrollBar.Visible = false;
            _totalListHeight = (int)PADDING;
        }


        public void RemoveListItem(DListBoxItem item)
        {
            bool removedItem = false;
            foreach (DListBoxItem cmbItem in Items)
            {
                if (cmbItem == item)
                {
                    this.Children.Remove(cmbItem);

                    // Ensure scrollbar uses relevant scroll area
                    _totalListHeight -= (int)(cmbItem.Size.Y + LINESPACING);
                    if (_totalListHeight < (Height - LINESPACING))
                    {
                        _verticalScrollBar.Visible = false;
                    }
                    UpdateScrollIndicator();

                    removedItem = true;
                    break;
                }
            }
            if (removedItem)
            {
                Items.Remove(item);
                RefreshAllListBoxSizes();
            }
        }



        // Unselect all the others!
        protected void ItemSelected(DListBoxItem item)
        {
            int i = 0;
            foreach (DListBoxItem li in _listItems)
            {
                if (item != li)
                    li.Selected = false;
                else
                    _selectedIndex = i;
                i++;
            }

            // Trigger change item event
            if (OnItemSelect != null)
                OnItemSelect();
        }



        protected void MoveItems(DScrollBar sender, float y)
        {
            // Must scale move to prevent scrolling too far
            //if (_totalListHeight > Height)
            //{
            //    float ratio = (_totalListHeight - (Height - (PADDING + LINESPACING))) / _totalListHeight;
            //    y *= ratio;
            //}


            foreach (DListBoxItem li in _listItems)
            {
                //li.NeedsUpdating = true;
                li.Position = new Vector2(li.Position.X, li.Position.Y - y);
            }
        }


        public List<DListBoxItem> SelectedItems()
        {
            List<DListBoxItem> selectedItems = new List<DListBoxItem>();
            foreach (DListBoxItem li in _listItems)
            {
                if (li.Selected == true)
                    selectedItems.Add(li);
            }
            if (selectedItems.Count > 0)
                return selectedItems;
            else
                return null;
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

            // Load the scrollbar with the new position!
            _verticalScrollBar = new DScrollBar(_guiManager, Size.X - 24, 0, 24, (int)Size.Y);
            _verticalScrollBar.OnScroll += new ScrollBarEventHandler(MoveItems);
            this.AddPanel(_verticalScrollBar);
            _verticalScrollBar.Visible = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        { 
            //lineHeight = spriteFont.MeasureString("T").Y;
            base.LoadContent();

            _spriteFont = content.Load<SpriteFont>("Miramonte");
        }



        protected override void UnloadContent()
        {
            _verticalScrollBar.Dispose();
            foreach (DListBoxItem listItem in _listItems)
            {
                listItem.Dispose();
            }
            base.UnloadContent();
        }


        public void UnselectAll()
        {
            foreach (DListBoxItem li in _listItems)
            {
                li.Selected = false;
            }
            _selectedIndex = -1;
        }



        private Vector2 NextLineTextPos()
        {
            // Get next line
            float nextY = PADDING;// +(_listItems.Count * (_itemHeight + LINESPACING));
            if (_listItems.Count > 0)
                nextY = _listItems[_listItems.Count - 1].Position.Y + _itemHeight + LINESPACING;

            Vector2 nextPos = new Vector2(PADDING, nextY);
            return nextPos;
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}

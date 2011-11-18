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

using DGui.DrawingSystem;
 

namespace DGui
{
    // Delegate functions (for communicating with other classes)
    public delegate void ComboBoxEventHandler(string value);
    public delegate void ComboBoxToggleHandler(bool open);

    /// <summary>
    /// Combo box dropdown list.
    /// A panel for the text, a button for dropdown,
    /// and a DListBox for the dropdown itself.
    /// </summary>
    public class DComboBox : DPanel
    {
        const int DEFAULT_HEIGHT = 28;
        const int DEFAULT_WIDTH = 180;
        const int DEFAULT_LIST_HEIGHT = 220;
        const int _PADDING = 4;

        DImage imageValue;
        DText textValue;
        DToggleButton dropDownButton;
        DListBox dropDownList;


        public event ComboBoxEventHandler OnChange;
        public event ComboBoxToggleHandler OnShowHide;

        protected string text = null;
        protected string imageName = null;
        protected SpriteFont spriteFont;
        protected bool listShown = false;

        #region Public properties
        public DListBox DropDownList
        {
            get
            {
                return dropDownList;
            }
            set
            {
                dropDownList = value;
            }
        }
        public string ImageName
        {
            get
            {
                return imageName;
            }
            set
            {
                imageName = value;
            }
        }
        public int SelectedIndex
        {
            get
            {
                return dropDownList.SelectedIndex;
            }
            set
            {
                if (value >= 0 && value < dropDownList.Items.Count)
                {
                    dropDownList.SelectedIndex = value;
                    dropDownList.Items[dropDownList.SelectedIndex].Selected = true;
                }
            }
        }
        public Collection<DListBoxItem> Items
        {
            get
            {
                return dropDownList.Items;
            }
        }
        public SpriteFont SpriteFont
        {
            get
            {
                return spriteFont;
            }
            set
            {
                spriteFont = value;
            }
        }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;

                // Also try select the item in the drop down list
                if (dropDownList != null)
                {
                    foreach (DListBoxItem item in dropDownList.Items)
                    {
                        if (item.Text == value)
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }
        }
        #endregion



        public DComboBox(DGuiManager guiManager, float x, float y)
            : this(guiManager)
        {
            this.Position = new Vector2(x, y);
        }

        public DComboBox(DGuiManager guiManager)
            : base(guiManager)
        {
            Size = new Vector2(DEFAULT_WIDTH, DEFAULT_HEIGHT);
            Text = "";
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
            float textXOffset = _PADDING;

            if (imageName != null)
            {
                imageValue = new DImage(_guiManager, _PADDING, _PADDING, imageName, (int)(Size.X - (2 * _PADDING)), (int)(Size.Y - (2 * _PADDING)));
                imageValue.Initialize();
                
                this.Children.Add(imageValue);

                textXOffset += imageValue.Position.X + imageValue.Size.X;
            }
            

            textValue = new DText(_guiManager);
            textValue.Text = text;
            textValue.FontName = "Miramonte";
            // Left justified
            textValue.HorizontalAlignment = DText.DHorizontalAlignment.Left;
            textValue.VerticalAlignment = DText.DVerticalAlignment.Center; 
            textValue.Size = new Vector2(Size.X, Size.Y);
            textValue.Initialize();
            textValue.Position = new Vector2(textXOffset, _PADDING);
            this.AddPanel(textValue);

            dropDownButton = new DToggleButton(_guiManager);
            dropDownButton.Text = "";
            dropDownButton.Size = new Vector2(Size.Y, Size.Y);
            dropDownButton.Initialize();
            dropDownButton.Position = new Vector2(Size.X - Size.Y, 0);
            this.AddPanel(dropDownButton);
            

            dropDownButton.OnToggle += new ToggleButtonEventHandler(dropDownButton_OnToggle);
            

            dropDownList = new DListBox(_guiManager);
            dropDownList.Size = new Vector2(Size.X, DEFAULT_LIST_HEIGHT);
            dropDownList.Initialize();
            dropDownList.Position = new Vector2(0, this.Position.Y + Size.Y);
            dropDownList.Visible = false;
            dropDownList.ApplyChildClipping = false;

            dropDownList.OnItemSelect += new ListBoxChangeEventHandler(dropDownList_OnItemSelect);
            listShown = false;

            this.FillColor = Color.White;

            base.LoadContent();
        }


        
        void dropDownList_OnItemSelect()
        {
            dropDownButton.Toggle(DButton.DButtonState.Off);
            HideList();


            int index = dropDownList.SelectedIndex;
            if (index > -1 && index < dropDownList.Items.Count)
            {
                textValue.Text = dropDownList.Items[index].Text;
                imageName = dropDownList.Items[index].ImageName;
                Text = textValue.Text;

                if (OnChange != null)
                    OnChange(textValue.Text);

                UpdateImage();
            }

            _guiManager.FocusedControl = dropDownButton;
        }


        protected void UpdateImage()
        {
            // Also set our image value if we can
            if (imageValue != null)
                this.Children.Remove(imageValue);
            imageValue = new DImage(_guiManager, 3, 3, imageName, (int)(Size.X - 6), (int)(Size.Y - 6));
            imageValue.Initialize();
            this.Children.Add(imageValue);

            // Shift text over
            if (imageName != null && textValue.Position.X == 5f)
            {
                textValue.Position = new Vector2(imageValue.Position.X + imageValue.Size.X + 5f, textValue.Position.Y);
            }
        }


        protected override void UnloadContent()
        {
            //((DEngine.Engine)Game).StaticSceneGraph.RemoveNode(dropDownList);
            if (imageValue != null)
                imageValue.Dispose();

            dropDownButton.OnToggle -= dropDownButton_OnToggle;
            textValue.Dispose();
            dropDownButton.Dispose();
            dropDownList.Dispose();
            base.UnloadContent();
        }



        void dropDownButton_OnToggle(DButton.DButtonState state)
        {
            if (state == DButton.DButtonState.On)
            {
                ShowList();
            }
            else
            {
                HideList();
            }
        }



        private void ShowList()
        {
            // Display the drop down list
            
            dropDownList.Position = new Vector2(this.AbsoluteTransform.X, this.AbsoluteTransform.Y + this.Size.Y);
            dropDownList.AbsoluteTransform = new Vector3(dropDownList.Position, 0);
            _guiManager.AddForegroundControl(dropDownList);
            listShown = true;
            dropDownList.Visible = true;

            if (OnShowHide != null)
                OnShowHide(true);
        }

        private void HideList()
        {
            dropDownList.Visible = false;
            _guiManager.ForegroundSceneNode.Children.Remove(dropDownList);
            if (listShown)
            {
                listShown = false;
            }
            if (OnShowHide != null)
                OnShowHide(false);
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState ms = Mouse.GetState();
            if (!dropDownButton.IsMouseHoveringOver && !dropDownList.IsMouseHoveringOver)
            {
                if (ms.LeftButton == ButtonState.Pressed && listShown)
                {
                    dropDownButton.Toggle(DButton.DButtonState.Off);
                    dropDownButton_OnToggle(DButton.DButtonState.Off);
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }


        /// <summary>
        /// Add an item to the combo box.
        /// </summary>
        /// <param name="value">Text of the item.</param>
        /// <param name="imageName">Optional image to load.</param>
        public void AddItem(string value, string imageName)
        {
            dropDownList.AddListItem(new DListBoxItem(_guiManager, value, imageName));
        }
    }
}

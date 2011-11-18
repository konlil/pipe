using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DGui
{
    public delegate void DGuiManagerFocusQueueHandler(DPanel panel);

    public class DGuiManager : DrawableGameComponent
    {
        Game _game;
        DGuiSceneGraph _sceneGraph;
        DGuiSceneNode _controlsSceneNode;
        DGuiSceneNode _controlTextSceneNode;
        DGuiSceneNode _foregroundSceneNode;
        DGuiSceneNode _foregroundTextSceneNode;

        DPanel _focusedControl;

        protected SpriteBatch _spriteBatch;

        // List of controls which have received a focusing click
        // Used to set focused control
        protected List<DPanel> _focusList = new List<DPanel>();

        public event DGuiManagerFocusQueueHandler OnFocusQueueReject;

        #region Public Properties

        public DGuiSceneNode ControlsSceneNode
        {
            get { return _controlsSceneNode; }
            set { _controlsSceneNode = value; }
        }
        public DGuiSceneNode ForegroundSceneNode
        {
            get { return _foregroundSceneNode; }
            set { _foregroundSceneNode = value; }
        }
        public DPanel FocusedControl
        {
            get { return _focusedControl; }
            set
            {
                if (_focusedControl != null && _focusedControl != value)
                {
                    _focusedControl = value;
                }
            }
        }
        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
            set { _spriteBatch = value; }
        }
        #endregion

        public DGuiManager(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _game = game;
            _sceneGraph = new DGuiSceneGraph(game);
            _controlsSceneNode = new DGuiSceneNode(game);
            _foregroundSceneNode = new DGuiSceneNode(game);
            _controlTextSceneNode = new DGuiSceneNode(game);
            _foregroundTextSceneNode = new DGuiSceneNode(game);
            _sceneGraph.RootNode.Children.Add(_controlsSceneNode);
            _sceneGraph.RootNode.Children.Add(_controlTextSceneNode);
            _sceneGraph.RootNode.Children.Add(_foregroundSceneNode);
            _sceneGraph.RootNode.Children.Add(_foregroundTextSceneNode);
            _spriteBatch = spriteBatch;
        }


        public override void Update(GameTime gameTime)
        {
            if (_sceneGraph != null)
            {
                _sceneGraph.Update(gameTime);
            }

            // Calculate focus based on update index
            DPanel focusedPanel = null;
            foreach (DPanel panel in _focusList)
            {
                if (focusedPanel == null || (panel.AcceptsFocus && panel.UpdateIndex > focusedPanel.UpdateIndex))
                    focusedPanel = panel;
            }

            // Notify of rejctions
            foreach (DPanel panel in _focusList)
            {
                if (OnFocusQueueReject != null)
                    OnFocusQueueReject(panel);
            }

            _focusList.Clear();
            if (focusedPanel != null)
                _focusedControl = focusedPanel;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_sceneGraph != null)
            {
                _spriteBatch.Begin();
                _sceneGraph.Draw(gameTime);
                _spriteBatch.End();

                // Reset various things stuffed up by SpriteBatch
                _game.GraphicsDevice.RenderState.DepthBufferEnable = true;
                _game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
                _game.GraphicsDevice.RenderState.AlphaTestEnable = false;

                _game.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
                _game.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
            }

            base.Draw(gameTime);
        }


        /// <summary>
        /// Add a control to the manager's main scene graph.
        /// Typically you would add a DForm item with this method and attach controls to the form itself.
        /// </summary>
        /// <param name="panel"></param>
        public void AddControl(DPanel panel)
        {
            if (_sceneGraph != null)
            {
                _controlsSceneNode.Children.Add(panel);
            }
        }

        /// <summary>
        /// Add a control to the scenegraph as a foreground item which will be rendered after all controls.
        /// </summary>
        /// <param name="panel"></param>
        public void AddForegroundControl(DPanel panel)
        {
            if (_sceneGraph != null)
            {
                _foregroundSceneNode.Children.Add(panel);
            }
        }

        
        /// <summary>
        /// Add this item to the GUI manager's list of items that require focus.
        /// List is evaluated and cleared every update - the highest item receives focus.
        /// All losers are notified so that they can reset mouse state flags for another focus click.
        /// </summary>
        /// <param name="panel"></param>
        public void FocusListEnqueue(DPanel panel)
        {
            if (panel != null)
                _focusList.Add(panel);
        }
    }
}

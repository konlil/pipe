//Copyright (c) 2007 GfxStorm.com
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.



using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Drawing;

using DGui.DrawingSystem;

namespace DGui
{
    /// <summary>
    /// Defines a scene node.
    /// </summary>
    public class DGuiSceneNode : DrawableGameComponent
    {
        #region Private Fields

        protected Game _game;

        protected SceneNodeCollection children = new SceneNodeCollection();
        protected Vector2 position = Vector2.Zero;
        protected Vector2 offset = Vector2.Zero;
        protected float rotation;
        protected Vector3 absoluteTransform = Vector3.Zero;

        protected Object tag;
        protected bool alwaysVisible = false; // Immune to scenegraph cull

        protected RectangleF _rectangle;
        long _updateIndex;
        #endregion



        #region Public Properties

        public long UpdateIndex
        {
            get { return _updateIndex; }
            set { _updateIndex = value; }
        }

        /// <summary>
        /// The children of this scene node.
        /// </summary>
        public SceneNodeCollection Children
        {
            get { return children; }
        }

        /// <summary>
        /// RectangleF for use in QuadTree
        /// </summary>
        public RectangleF Rectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }

        /// <summary>
        /// Whether this node can be culled by the scenegraph occluder or not.
        /// </summary>
        public bool AlwaysVisible
        {
            get { return alwaysVisible; }
            set { alwaysVisible = value; }
        }

        /// <summary>
        /// The scene node's location in space.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// The location in local space to move and center the scene node when displayed.
        /// </summary>
        public Vector2 Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        /// <summary>
        /// The scene node's rotation around its center.
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        /// <summary>
        /// Indicates if the scene node should be drawn.
        /// </summary>
        /*public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }*/

        /// <summary>
        /// A sphere that indicates the bounds of the scene node.
        /// </summary>
        public BoundingSphere BoundingSphere
        {
            get { return GetBoundingSphere(); }
        }

        /// <summary>
        /// The scene node's calculated world transform.
        /// </summary>
        public Vector3 AbsoluteTransform
        {
            get { return absoluteTransform; }
            set { absoluteTransform = value; }
        }

        /// <summary>
        /// A user definable object.
        /// </summary>
        public Object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// The controller that will manipulate this scene node.
        /// </summary>
        /*public IController Controller
        {
            get { return controller; }
            set { controller = value; }
        }*/


        #endregion



        #region Default Constructor

        /// <summary>
        /// Intializes an instance of the <see cref="SceneNode"/> class.
        /// </summary>
        public DGuiSceneNode(Game game)
            : base(game)
        {
            _game = game;
        }

        #endregion



        #region Protected Methods

        /// <summary>
        /// In a derived class, allows the class to specify a custom <see cref="BoundingSphere"/>.
        /// </summary>
        /// <returns>A BoundingSphere structure that contains the bounds of the scene.</returns>
        protected virtual BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Vector3.Zero, 0);
        }

        #endregion



        #region Public Methods

        /// <summary>
        /// Allows a scene node to update its state.
        /// </summary>
        /// <param name="sceneGraph">The <see cref="SceneGraph"/> that is updating the node.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Allows a scene node to display itself.
        /// </summary>
        /// <param name="sceneGraph">The <see cref="SceneGraph"/> that is drawing the node.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public void DrawRecursive(GameTime gameTime)
        {
            DrawRecursivePrivate(gameTime, this);
        }

        void DrawRecursivePrivate(GameTime gameTime, DGuiSceneNode node)
        {
            //Draw
            if (node.Visible || node.AlwaysVisible)
            {
                node.Draw(gameTime);

                for (int i = 0; i < node.Children.Count; i++)
                {
                    DrawRecursivePrivate(gameTime, node.Children[i]);
                }
            }
        }

        #endregion


    }
}

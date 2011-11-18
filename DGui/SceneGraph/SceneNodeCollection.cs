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



namespace DGui
{
    /// <summary>
    /// Defines a collection of <see cref="SceneNode"/> items.
    /// </summary>
    public class SceneNodeCollection : ICollection<DGuiSceneNode>
    {
        #region Private Fields

        List<DGuiSceneNode> sceneNodes = new List<DGuiSceneNode>();

        #endregion



        #region Public Properties

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the entry to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public DGuiSceneNode this[int index]
        {
            get
            {
                if (index < 0 || index > sceneNodes.Count - 1)
                    throw new ArgumentOutOfRangeException("index");

                return sceneNodes[index];
            }

            set
            {
                if (index < 0 || index > sceneNodes.Count - 1)
                    throw new ArgumentOutOfRangeException("index");

                sceneNodes[index] = value;
            }
        }

        #endregion



        #region Default Constructor

        /// <summary>
        /// Intializes an instance of the <see cref="SceneNodeCollection"/> class.
        /// </summary>
        public SceneNodeCollection()
        {
        }

        #endregion



        #region ICollection<SceneNode> Members

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        public void Add(DGuiSceneNode item)
        {
            sceneNodes.Add(item);
        }

        /// <summary>
        /// Insert an item at a specified position to the collection.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        public void Insert(int index, DGuiSceneNode item)
        {
            sceneNodes.Insert(index, item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            sceneNodes.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise, false.</returns>
        public bool Contains(DGuiSceneNode item)
        {
            return sceneNodes.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an <see cref="System.Array"/>, starting at a particular index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied from the collection. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(DGuiSceneNode[] array, int arrayIndex)
        {
            sceneNodes.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return sceneNodes.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>true if item was successfully removed from the collection; otherwise, false. This method also returns false if item is not found in the collection.</returns>
        public bool Remove(DGuiSceneNode item)
        {
            return sceneNodes.Remove(item);
        }

        #endregion



        #region IEnumerable<SceneNode> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.Generic.IEnumerator{SceneNode}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<DGuiSceneNode> GetEnumerator()
        {
            return sceneNodes.GetEnumerator();
        }

        #endregion



        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.IEnumerator"/> that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return sceneNodes.GetEnumerator();
        }

        #endregion
    }

}

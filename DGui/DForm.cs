using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using DGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

 

namespace DGui
{
    public delegate void OnFormShowHandler(object sender);
    public delegate void OnFormHideHandler(object sender);


    /// <summary>
    /// Form with sub-controls.
    /// Maintains form hierarchy and provides a standard interface for form show/hide
    /// </summary>
    public class DForm : DPanel
    {
        public const int FORM_WIDTH = 400;
        public const int FORM_HEIGHT = 300;

        protected bool shown;
        protected string name;
        protected DForm parentForm;
        protected Dictionary<string,DForm> childForms;

        public event OnFormShowHandler OnFormShow;
        public event OnFormHideHandler OnFormHide;

        #region Public properties
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public bool Shown
        {
            get { return shown; }
            //set { shown = value; }
        }
        public Dictionary<string, DForm> ChildForms
        {
            get
            {
                return childForms;
            }
            set
            {
                childForms = value;
            }
        }
        public DForm ParentForm
        {
            get
            {
                return parentForm;
            }
            set
            {
                parentForm = value;
            }
        }
        #endregion

        public DForm(DGuiManager guiManager, string formName, DForm parent)
            : base(guiManager)
        {
            _isForm = true;
            name = formName;
            parentForm = parent;
            childForms = new Dictionary<string,DForm>();
            Size = new Vector2(FORM_WIDTH, FORM_HEIGHT);

            // Center by default
            this.Position = new Vector2((guiManager.Game.Window.ClientBounds.Width - Size.X) / 2f,
                                        (guiManager.Game.Window.ClientBounds.Height - Size.Y) / 2f);
        }

        public virtual void ShowForm()
        {
            shown = true;
            if (OnFormShow != null)
                OnFormShow(this);
        }

        public virtual void HideForm()
        {
            shown = false;
            if (OnFormHide != null)
                OnFormHide(this);
        }

        public void ShowParentForm()
        {
            if (parentForm != null)
            {
                HideForm();
                parentForm.ShowForm();
            }
        }

        public void ShowChildForm(string formName)
        {
            foreach (KeyValuePair<string,DForm> childFormPair in childForms)
            {
                if (childFormPair.Key == formName)
                {
                    HideForm();
                    childFormPair.Value.ShowForm();
                    break;
                }
            }
        }


    }
}

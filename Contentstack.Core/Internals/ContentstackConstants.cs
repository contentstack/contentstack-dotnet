using System;
using System.IO;

namespace Contentstack.Core.Internals
{
    internal class ContentstackConstants
    {
        #region Private Variable
        private string _ContentTypes = "content_types";
      
        #endregion

        #region Private Constructors
        private ContentstackConstants()
        {

        }
        #endregion

        #region Public Properties
        public string ContentTypeUid { get; set; }
        public string EntryUid { get; set; }


        //public string CacheFolderName
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(this._CacheFolderName))
        //        {
        //            this._CacheFolderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ContentstackCache");
        //        }

        //        return this._CacheFolderName;
        //    }
        //    set
        //    {
        //        this._CacheFolderName = value;
        //    }
        //}

        public string Content_Types
        {
            get { return this._ContentTypes ?? "content_types"; }
            set { this._ContentTypes = value; }
        }

        public string Entries
        {
            get { return this._ContentTypes ?? "entries"; }
            set { this._ContentTypes = value; }
        }

        // error messages

        #endregion

        #region Public Functions
        internal static ContentstackConstants Instance
        {
            get
            {
                return new ContentstackConstants();
            }
        }
        #endregion
    }
}

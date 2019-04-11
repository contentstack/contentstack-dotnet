using System;
using System.Collections.Generic;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Models
{
    public class AssetLibrary
    {
        
        #region Private Variables

        //private CachePolicy _CachePolicy;
        private Dictionary<string, string> _PostParams;

        #endregion

        public AssetLibrary()
        {
            this._PostParams = new Dictionary<string, string>();
        }

        #region Public Functions
        public void SortWithKeyAndOrderBy(String key, OrderBy order)
        {
           
        }
        #endregion


        public void ObjectsCount()
        {

        }

        public void IncludeCount()
        {

        }

        public void IncludeRelativeUrls()
        {
            
        }

        public void SetHeaderForKey(String key, String value){
        
        }

        public void AddHeadersWithDictionary(){
            
        }

    }
}

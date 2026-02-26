using System;
using System.Collections.Generic;
using Contentstack.Core.Models;

namespace Contentstack.Core.Tests.Models
{
    /// <summary>
    /// Generic model for testing medium-complexity content type operations
    /// </summary>
    public class MediumContentTypeModel
    {
        public string Uid { get; set; }
        public string Title { get; set; }
        public object[] Tags { get; set; }
        public DateTime Created_at { get; set; }
        public string Created_by { get; set; }
        public DateTime Updated_at { get; set; }
        public string Updated_by { get; set; }
        
        #region Basic Fields
        
        /// <summary>
        /// URL field
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// Date field
        /// </summary>
        public string Date { get; set; }
        
        /// <summary>
        /// Byline text field
        /// </summary>
        public string Byline { get; set; }
        
        #endregion
        
        #region Reference Fields
        
        /// <summary>
        /// Single reference field
        /// </summary>
        public Entry Reference { get; set; }
        
        #endregion
        
        #region Global Fields
        
        /// <summary>
        /// Content block global field
        /// Multiple content blocks
        /// </summary>
        public List<Dictionary<string, object>> ContentBlock { get; set; }
        
        /// <summary>
        /// Image gallery global field
        /// </summary>
        public Dictionary<string, object> ImageGallery { get; set; }
        
        /// <summary>
        /// Video experience global field
        /// </summary>
        public Dictionary<string, object> VideoExperience { get; set; }
        
        #endregion
        
        #region File Fields
        
        /// <summary>
        /// Image presets field (extension)
        /// </summary>
        public Dictionary<string, object> ImagePresets { get; set; }
        
        #endregion
        
        #region Metadata Fields
        
        /// <summary>
        /// SEO global field
        /// </summary>
        public Dictionary<string, object> Seo { get; set; }
        
        /// <summary>
        /// Search global field
        /// </summary>
        public Dictionary<string, object> Search { get; set; }
        
        /// <summary>
        /// Referenced data global field
        /// Data used when this entry is referenced elsewhere
        /// </summary>
        public Dictionary<string, object> ReferencedData { get; set; }
        
        #endregion
        
        #region Taxonomy
        
        /// <summary>
        /// Taxonomy field
        /// </summary>
        public List<Dictionary<string, object>> Taxonomies { get; set; }
        
        #endregion
    }
}


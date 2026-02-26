using System;
using System.Collections.Generic;
using Contentstack.Core.Models;

namespace Contentstack.Core.Tests.Models
{
    /// <summary>
    /// Generic model for testing complex content type operations with multiple field types
    /// </summary>
    public class ComplexContentTypeModel
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
        /// Boolean field - Featured flag
        /// </summary>
        public bool Featured { get; set; }
        
        /// <summary>
        /// Boolean field - Double wide layout
        /// </summary>
        public bool DoubleWide { get; set; }
        
        /// <summary>
        /// Number field
        /// </summary>
        public double Number { get; set; }
        
        /// <summary>
        /// Media type dropdown/enum field
        /// </summary>
        public string MediaType { get; set; }
        
        /// <summary>
        /// Topics multi-select enum field
        /// </summary>
        public List<string> Topics { get; set; }
        
        #endregion
        
        #region Reference Fields
        
        /// <summary>
        /// Multiple reference field - Related content
        /// Can contain multiple entries from different content types
        /// </summary>
        public List<Entry> RelatedContent { get; set; }
        
        /// <summary>
        /// Multiple reference field - Authors
        /// </summary>
        public List<Entry> Authors { get; set; }
        
        /// <summary>
        /// Single reference field - Page footer
        /// References a page_footer entry
        /// </summary>
        public Entry PageFooter { get; set; }
        
        #endregion
        
        #region Complex/Nested Fields
        
        /// <summary>
        /// Global field - Page header
        /// Contains nested structure from global field
        /// </summary>
        public Dictionary<string, object> PageHeader { get; set; }
        
        /// <summary>
        /// Multiple global field - Content blocks
        /// Array of content block structures
        /// </summary>
        public List<Dictionary<string, object>> ContentBlock { get; set; }
        
        /// <summary>
        /// Modular blocks field
        /// Contains different block types with varied schemas
        /// </summary>
        public List<Dictionary<string, object>> ModularBlocks { get; set; }
        
        /// <summary>
        /// Group field
        /// Contains nested field structure
        /// </summary>
        public Dictionary<string, object> Group { get; set; }
        
        /// <summary>
        /// Video experience global field
        /// Multiple video configurations
        /// </summary>
        public List<Dictionary<string, object>> VideoExperience { get; set; }
        
        /// <summary>
        /// Podcast global field
        /// Multiple podcast links
        /// </summary>
        public List<Dictionary<string, object>> Podcast { get; set; }
        
        #endregion
        
        #region Rich Text Fields
        
        /// <summary>
        /// HTML/Rich text field
        /// </summary>
        public string Html { get; set; }
        
        /// <summary>
        /// Article references (rich text)
        /// </summary>
        public string ArticleReferences { get; set; }
        
        /// <summary>
        /// JSON RTE field
        /// Can contain embedded entries and assets
        /// </summary>
        public Dictionary<string, object> JsonRte { get; set; }
        
        #endregion
        
        #region Metadata Fields
        
        /// <summary>
        /// SEO global field
        /// Contains SEO metadata
        /// </summary>
        public Dictionary<string, object> Seo { get; set; }
        
        /// <summary>
        /// Search global field
        /// Contains search-related metadata
        /// </summary>
        public Dictionary<string, object> Search { get; set; }
        
        #endregion
        
        #region Taxonomy
        
        /// <summary>
        /// Taxonomy field
        /// Hierarchical taxonomy terms
        /// </summary>
        public List<Dictionary<string, object>> Taxonomies { get; set; }
        
        #endregion
        
        #region File Fields
        
        /// <summary>
        /// Image/file field
        /// </summary>
        public Dictionary<string, object> Image { get; set; }
        
        /// <summary>
        /// Image presets field (extension)
        /// </summary>
        public Dictionary<string, object> ImagePresets { get; set; }
        
        #endregion
    }
}


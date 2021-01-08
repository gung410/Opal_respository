using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class Activity.
    /// </summary>
    
    public class ActivityEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityEntity"/> class.
        /// </summary>
        public ActivityEntity()
        {
            LtActivities = new List<LtActivityEntity>();
            Surveys = new List<SurveyEntity>();
            LevelGroups = new List<LevelGroupEntity>();
        }

        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>The activity identifier.</value>
        public int ActivityID { get; set; }
        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>The language identifier.</value>
        public int LanguageID { get; set; }
        /// <summary>
        /// Gets or sets the owner identifier.
        /// </summary>
        /// <value>The owner identifier.</value>
        public int OwnerID { get; set; }
        /// <summary>
        /// Gets or sets the style sheet.
        /// </summary>
        /// <value>The style sheet.</value>
        public string StyleSheet { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public short Type { get; set; }
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>The no.</value>
        public short No { get; set; }
        /// <summary>
        /// Gets or sets the type of the tooltip.
        /// </summary>
        /// <value>The type of the tooltip.</value>
        public short TooltipType { get; set; }
        /// <summary>
        /// Gets or sets the listview.
        /// </summary>
        /// <value>The listview.</value>
        public short Listview { get; set; }
        /// <summary>
        /// Gets or sets the descview.
        /// </summary>
        /// <value>The descview.</value>
        public short Descview { get; set; }
        /// <summary>
        /// Gets or sets the chartview.
        /// </summary>
        /// <value>The chartview.</value>
        public short Chartview { get; set; }
        /// <summary>
        /// Gets or sets the olap server.
        /// </summary>
        /// <value>The olap server.</value>
        public string OLAPServer { get; set; }
        /// <summary>
        /// Gets or sets the olapdb.
        /// </summary>
        /// <value>The olapdb.</value>
        public string OLAPDB { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the type of the selection header.
        /// </summary>
        /// <value>The type of the selection header.</value>
        public short SelectionHeaderType { get; set; }
        /// <summary>
        /// Gets or sets the ext identifier.
        /// </summary>
        /// <value>The ext identifier.</value>
        public string ExtID { get; set; }
        /// <summary>
        /// gets or sets the Use olap field
        /// </summary>
        public bool UseOLAP { set; get; }
        /// <summary>
        /// Gets or sets the archetype identifier.
        /// </summary>
        /// <value>The created.</value>
        public int? ArchetypeId { get; set; }
      
        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public virtual ICollection<SurveyEntity> Surveys { get; set; }

        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        /// <value>The pages.</value>
        public virtual ICollection<LevelGroupEntity> LevelGroups { get; set; }
        /// <summary>
        /// Gets or sets the l t_ activity.
        /// </summary>
        /// <value>The l t_ activity.</value>
        public virtual ICollection<LtActivityEntity> LtActivities { get; set; }
        
    }
}

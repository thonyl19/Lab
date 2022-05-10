namespace UnitTestProject.Case
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WP_IPQC_CHECKITEM_RAW
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(255)]
        public string ACTION_LINK_SID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(255)]
        public string OPERATION { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(255)]
        public string QC_ITEM_SID { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal QC_SEQ { get; set; }

        [StringLength(255)]
        public string QC_DATA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(255)]
        public string CREATE_USER { get; set; }

        [Key]
        [Column(Order = 5)]
        public DateTime CREATE_DATE { get; set; }
    }
}

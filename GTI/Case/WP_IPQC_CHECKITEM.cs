namespace MDL.MES
{
	using System;
	using System.ComponentModel.DataAnnotations;

	public partial class WP_IPQC_CHECKITEM
	{
		[Key]
		[StringLength(255)]
		public string WP_IPQC_CHECKITEM_SID { get; set; }

		[Required]
		[StringLength(255)]
		public string ROUTE_VER_OPER_SID { get; set; }

		[Required]
		[StringLength(255)]
		public string OPERATION { get; set; }

		[StringLength(255)]
		public string ACTION { get; set; }

		[Required]
		[StringLength(255)]
		public string APPLICATION_NAME { get; set; }

		[Required]
		[StringLength(255)]
		public string ACTION_LINK_SID { get; set; }

		[StringLength(255)]
		public string ACTION_REASON { get; set; }

		[StringLength(2000)]
		public string ACTION_DESCRIPTION { get; set; }

		[Required]
		[StringLength(255)]
		public string QC_ITEM_SID { get; set; }

		[Required]
		[StringLength(255)]
		public string ITEM_NO { get; set; }

		[Required]
		[StringLength(255)]
		public string ITEM { get; set; }

		[Required]
		[StringLength(1)]
		public string DATATYPE { get; set; }

		public decimal? DATA_COUNT { get; set; }

		[StringLength(2000)]
		public string DISPLAY_POINT_NAME { get; set; }

		[StringLength(255)]
		public string STATUS { get; set; }

		[Required]
		[StringLength(255)]
		public string CREATE_USER { get; set; }

		public DateTime CREATE_DATE { get; set; }

		[Required]
		[StringLength(255)]
		public string UPDATE_USER { get; set; }

		public DateTime UPDATE_DATE { get; set; }
	}
}

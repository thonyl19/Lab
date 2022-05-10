namespace MDL.MES
{
	using System;
	using System.ComponentModel.DataAnnotations;

	public partial class WP_IPQC
	{
		[Key]
		[StringLength(255)]
		public string QC_NO { get; set; }

		[StringLength(255)]
		public string QC_AQL_SID { get; set; }

		[StringLength(255)]
		public string QC_AQL_NO { get; set; }

		public decimal? AC { get; set; }

		public decimal? RE { get; set; }

		public decimal? REALL_AC { get; set; }

		public decimal? REALL_RE { get; set; }

		[Required]
		[StringLength(255)]
		public string QC_RESULT { get; set; }

		public decimal? SAMPLE_SIZE { get; set; }

		[StringLength(255)]
		public string QC_CODE_LETTER_SID { get; set; }

		[StringLength(255)]
		public string QC_CODE_LETTER_NO { get; set; }

		[StringLength(255)]
		public string QC_INSP_TYPE { get; set; }

		[StringLength(255)]
		public string QC_INSP_METHOD { get; set; }

		public decimal TOTAL_PCS_QTY { get; set; }

		public decimal TOTAL_BATCH_QTY { get; set; }

		[StringLength(255)]
		public string QC_LEVEL_SID { get; set; }

		[StringLength(255)]
		public string QC_LEVEL_NO { get; set; }

		[StringLength(255)]
		public string IN_ACTION_LINK_SID { get; set; }

		[StringLength(255)]
		public string OUT_ACTION_LINK_SID { get; set; }

		[StringLength(255)]
		public string OPERATION { get; set; }

		[StringLength(2000)]
		public string DESCRIPTION { get; set; }

		[Required]
		[StringLength(255)]
		public string CREATE_USER { get; set; }

		public DateTime CREATE_DATE { get; set; }

		[Required]
		[StringLength(255)]
		public string UPDATE_USER { get; set; }

		public DateTime UPDATE_DATE { get; set; }

		[Required]
		[StringLength(255)]
		public string INSP_STATUS { get; set; }
	}
}

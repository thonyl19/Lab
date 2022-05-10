namespace UnitTestProject.Case
{
	using System;
	using System.Data.Entity;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;

	public partial class Model1 : DbContext
	{
		public Model1()
			: base("name=Model1")
		{
		}

		public virtual DbSet<WP_IPQC> WP_IPQC { get; set; }
		public virtual DbSet<WP_IPQC_CHECKITEM> WP_IPQC_CHECKITEM { get; set; }
		public virtual DbSet<WP_IPQC_CHECKITEM_RAW> WP_IPQC_CHECKITEM_RAW { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<WP_IPQC>()
				.Property(e => e.AC)
				.HasPrecision(18, 0);

			modelBuilder.Entity<WP_IPQC>()
				.Property(e => e.RE)
				.HasPrecision(18, 0);

			modelBuilder.Entity<WP_IPQC>()
				.Property(e => e.REALL_AC)
				.HasPrecision(18, 0);

			modelBuilder.Entity<WP_IPQC>()
				.Property(e => e.REALL_RE)
				.HasPrecision(18, 0);

			modelBuilder.Entity<WP_IPQC>()
				.Property(e => e.SAMPLE_SIZE)
				.HasPrecision(18, 0);

			modelBuilder.Entity<WP_IPQC>()
				.Property(e => e.TOTAL_PCS_QTY)
				.HasPrecision(18, 0);

			modelBuilder.Entity<WP_IPQC>()
				.Property(e => e.TOTAL_BATCH_QTY)
				.HasPrecision(18, 0);

			modelBuilder.Entity<WP_IPQC_CHECKITEM>()
				.Property(e => e.DATATYPE)
				.IsFixedLength();

			modelBuilder.Entity<WP_IPQC_CHECKITEM>()
				.Property(e => e.DATA_COUNT)
				.HasPrecision(18, 0);

			modelBuilder.Entity<WP_IPQC_CHECKITEM_RAW>()
				.Property(e => e.QC_SEQ)
				.HasPrecision(18, 0);
		}
	}
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MvcDriverVerify.Models
{
    public partial class VerifyDriverContext : DbContext
    {
        public VerifyDriverContext()
        {
        }

        public VerifyDriverContext(DbContextOptions<VerifyDriverContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comments> Comments { get; set; }
        public virtual DbSet<Platform> Platform { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserTypes> UserTypes { get; set; }
        public virtual DbSet<Vehicle> Vehicle { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlite("DataSource=VerifyDriver.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Comments>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("_Comments");

                entity.Property(e => e.CId)
                    .HasColumnName("cID")
                    .HasColumnType("int")
                    .ValueGeneratedNever();

                entity.Property(e => e.CDateTime)
                    .HasColumnName("cDateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.CPid)
                    .HasColumnName("c_Pid")
                    .HasColumnType("int");

                entity.Property(e => e.CText)
                    .IsRequired()
                    .HasColumnName("cText")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.CUid)
                    .HasColumnName("c_Uid")
                    .HasColumnType("int");

                entity.HasOne(d => d.CP)
                    .WithMany(p => p.CommentsCP)
                    .HasForeignKey(d => d.CPid)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CU)
                    .WithMany(p => p.CommentsCU)
                    .HasForeignKey(d => d.CUid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Platform>(entity =>
            {
                entity.HasKey(e => e.PId);

                entity.ToTable("_Platform");

                entity.Property(e => e.PId)
                    .HasColumnName("pID")
                    .HasColumnType("int")
                    .ValueGeneratedNever();

                entity.Property(e => e.PName)
                    .IsRequired()
                    .HasColumnName("pName")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.PType)
                    .HasColumnName("pType")
                    .HasColumnType("varchar(20)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UId);

                entity.ToTable("_User");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("int")
                    .ValueGeneratedNever();

                entity.Property(e => e.UAge)
                    .HasColumnName("uAge")
                    .HasColumnType("varchar(2)");

                entity.Property(e => e.UGender)
                    .HasColumnName("uGender")
                    .HasColumnType("varchar(6)");

                entity.Property(e => e.UNames)
                    .IsRequired()
                    .HasColumnName("uNames")
                    .HasColumnType("varchar(35)");

                entity.Property(e => e.UPartnerId)
                    .HasColumnName("uPartner_ID")
                    .HasColumnType("int");

                entity.Property(e => e.UPlatformId)
                    .HasColumnName("uPlatform_ID")
                    .HasColumnType("int");

                entity.Property(e => e.URating)
                    .HasColumnName("uRating")
                    .HasColumnType("decimal(2,2)");

                entity.Property(e => e.UUsertypeId)
                    .HasColumnName("uUsertype_ID")
                    .HasColumnType("int");

                entity.Property(e => e.UVid)
                    .HasColumnName("uVID")
                    .HasColumnType("int");

                entity.HasOne(d => d.UPartner)
                    .WithMany(p => p.InverseUPartner)
                    .HasForeignKey(d => d.UPartnerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.UPlatform)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UPlatformId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.UUsertype)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UUsertypeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.UV)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UVid)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserTypes>(entity =>
            {
                entity.HasKey(e => e.UTId);

                entity.ToTable("_User_types");

                entity.Property(e => e.UTId)
                    .HasColumnName("U_T_ID")
                    .HasColumnType("int")
                    .ValueGeneratedNever();

                entity.Property(e => e.UTDescription)
                    .IsRequired()
                    .HasColumnName("U_T_description")
                    .HasColumnType("varchar(10)");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.VId);

                entity.ToTable("_Vehicle");

                entity.Property(e => e.VId)
                    .HasColumnName("vID")
                    .HasColumnType("int")
                    .ValueGeneratedNever();

                entity.Property(e => e.VMake)
                    .HasColumnName("vMake")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.VModelName)
                    .HasColumnName("vModel_name")
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.VModelYear)
                    .HasColumnName("vModel_year")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.VParterId)
                    .HasColumnName("vParter_ID")
                    .HasColumnType("int");

                entity.Property(e => e.VPlatformId)
                    .HasColumnName("vPlatform_ID")
                    .HasColumnType("int");

                entity.Property(e => e.Vregistration)
                    .IsRequired()
                    .HasColumnName("vregistration")
                    .HasColumnType("varchar(10)");

                entity.HasOne(d => d.VParter)
                    .WithMany(p => p.Vehicle)
                    .HasForeignKey(d => d.VParterId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.VPlatform)
                    .WithMany(p => p.Vehicle)
                    .HasForeignKey(d => d.VPlatformId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

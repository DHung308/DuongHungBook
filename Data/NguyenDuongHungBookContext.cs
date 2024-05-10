using System;
using System.Collections.Generic;
using DHungBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace DHungBooks.Data;

public partial class NguyenDuongHungBookContext : DbContext
{
    public NguyenDuongHungBookContext()
    {
    }

    public NguyenDuongHungBookContext(DbContextOptions<NguyenDuongHungBookContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Page> Pages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TransactStatus> TransactStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-JVTPCC1\\NDH;Database=NguyenDuongHung_Book;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_100_CI_AI_KS_WS_SC_UTF8");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Salt)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Accounts_Roles");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.CaId).HasColumnName("CaID");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateModified).HasColumnType("datetime");

            entity.HasOne(d => d.Ca).WithMany(p => p.Books)
                .HasForeignKey(d => d.CaId)
                .HasConstraintName("FK_Books_Categories");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CaId);

            entity.Property(e => e.CaId).HasColumnName("CaID");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Salt)
                .HasMaxLength(8)
                .IsFixedLength();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.ShipDate).HasColumnType("datetime");
            entity.Property(e => e.TotalMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransactStatusId).HasColumnName("TransactStatusID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.TransactStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TransactStatusId)
                .HasConstraintName("FK_Orders_TransactStatus");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ShipDate).HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_OrderDetails_Orders");
        });

        modelBuilder.Entity<Page>(entity =>
        {
            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
        });

        modelBuilder.Entity<TransactStatus>(entity =>
        {
            entity.ToTable("TransactStatus");

            entity.Property(e => e.TransactStatusId).HasColumnName("TransactStatusID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

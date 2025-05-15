using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.DbContext;

public partial class DBContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DBContext(DbContextOptions<DBContext> options) : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Book> Books { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .HasOne(b => b.CreatedByNavigation)
            .WithMany(u => u.BookCreatedByNavigations)
            .HasForeignKey(b => b.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.UpdatedByNavigation)
            .WithMany(u => u.BookUpdatedByNavigations)
            .HasForeignKey(b => b.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict); 

        base.OnModelCreating(modelBuilder);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Data
{
    public class BookShopContext:DbContext
    {
        public BookShopContext(DbContextOptions<BookShopContext> options) : base(options)
        {

        }
        public DbSet<Author> authors => Set<Author>();
        public DbSet<Book> books => Set<Book>();
        public DbSet<BookCategory> bookCategories => Set<BookCategory>();
        public DbSet<Categories> categories => Set<Categories>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.AuthorId).HasName("PK__Authors");
                entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired(false);
                entity.Property(e => e.LastName).HasMaxLength(50);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.BookId).HasName("PK__Books");

                entity.Property(e => e.Title).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ReleaseDate);
                entity.Property(e => e.Copies);
                entity.Property(e => e.Price);
                entity.Property(e => e.EditionType);
                entity.Property(e => e.AgeRestriction);
                entity.HasOne(d => d.Author).WithMany(p => p.Books)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Books_Authors");
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(e => e.CategoryId).HasName("PK__Categories");

                entity.Property(e => e.CategoryName).HasMaxLength(50);
            });

            modelBuilder.Entity<BookCategory>(entity =>
            {
                entity.HasKey(e => new { e.BookId, e.CategoryId }).HasName("PK__BookCategories");
                entity.HasOne(d => d.Book).WithMany(p => p.BookCategories)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookCategories_Books");
                entity.HasOne(d => d.Categories).WithMany(p => p.BookCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookCategories_Categories");
            });
        }
    }
}

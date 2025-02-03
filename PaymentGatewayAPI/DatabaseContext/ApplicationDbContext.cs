﻿using Microsoft.EntityFrameworkCore;
using PaymentGatewayAPI.Models;

namespace PaymentGatewayAPI.DatabaseContext;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);

            entity.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(50);



            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(u => u.CreatedOn)
                .IsRequired();
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.TransactionId);

            entity.HasOne(u => u.User)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(t => t.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(t => t.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .IsFixedLength();

            entity.Property(t => t.PaymentMode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(t => t.CreatedOn)
                .IsRequired();
        });

    }
}

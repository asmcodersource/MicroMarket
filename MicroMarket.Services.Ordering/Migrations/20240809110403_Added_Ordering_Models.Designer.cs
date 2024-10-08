﻿// <auto-generated />
using System;
using MicroMarket.Services.Ordering.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MicroMarket.Services.Ordering.Migrations
{
    [DbContext(typeof(OrderingDbContext))]
    [Migration("20240809110403_Added_Ordering_Models")]
    partial class Added_Ordering_Models
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MicroMarket.Services.Ordering.Models.Item", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid");

                    b.Property<int>("OrderedQuantity")
                        .HasColumnType("integer");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("ProductPrice")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("MicroMarket.Services.Ordering.Models.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDelivered")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("MicroMarket.Services.Ordering.Models.OrderState", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("OccuredAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderStates");
                });

            modelBuilder.Entity("MicroMarket.Services.Ordering.Models.Item", b =>
                {
                    b.HasOne("MicroMarket.Services.Ordering.Models.Order", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("MicroMarket.Services.Ordering.Models.OrderState", b =>
                {
                    b.HasOne("MicroMarket.Services.Ordering.Models.Order", null)
                        .WithMany("StateHistory")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MicroMarket.Services.Ordering.Models.Order", b =>
                {
                    b.Navigation("Items");

                    b.Navigation("StateHistory");
                });
#pragma warning restore 612, 618
        }
    }
}

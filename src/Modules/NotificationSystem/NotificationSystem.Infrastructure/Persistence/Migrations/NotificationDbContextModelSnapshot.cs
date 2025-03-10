﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NotificationSystem.Infrastructure.Persistence;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NotificationSystem.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(NotificationDbContext))]
    partial class NotificationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "hstore");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("NotificationSystem.Domain.Entities.Notification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("character varying(21)");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<string>("Recipient")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("SentAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Notifications");

                    b.HasDiscriminator().HasValue("Notification");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("NotificationSystem.Domain.Entities.EmailNotification", b =>
                {
                    b.HasBaseType("NotificationSystem.Domain.Entities.Notification");

                    b.Property<string>("HtmlContent")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PlainTextContent")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue("EmailNotification");
                });

            modelBuilder.Entity("NotificationSystem.Domain.Entities.PushNotification", b =>
                {
                    b.HasBaseType("NotificationSystem.Domain.Entities.Notification");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Dictionary<string, string>>("Data")
                        .IsRequired()
                        .HasColumnType("hstore");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue("PushNotification");
                });

            modelBuilder.Entity("NotificationSystem.Domain.Entities.SmsNotification", b =>
                {
                    b.HasBaseType("NotificationSystem.Domain.Entities.Notification");

                    b.HasDiscriminator().HasValue("SmsNotification");
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TelegramBot.dbutils;

#nullable disable

namespace TelegramBot.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-preview.3.23174.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TelegramBot.dbutils.Notes", b =>
                {
                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("TgId")
                        .HasColumnType("bigint");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("TelegramBot.dbutils.Users", b =>
                {
                    b.Property<long>("TgId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("TgId"));

                    b.Property<long>("TgChatId")
                        .HasColumnType("bigint");

                    b.HasKey("TgId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}

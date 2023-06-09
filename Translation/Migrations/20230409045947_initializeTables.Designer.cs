﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Translation.Data;

namespace Translation.Migrations
{
    [DbContext(typeof(TranslationContext))]
    [Migration("20230409045947_initializeTables")]
    partial class initializeTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("translation")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "6.0.0-preview.7.21378.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Translation.Models.Language", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("code");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("bit")
                        .HasColumnName("is_locked");

                    b.Property<bool>("IsRtl")
                        .HasColumnType("bit")
                        .HasColumnName("is_rtl");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("label");

                    b.Property<int?>("Order")
                        .HasColumnType("int")
                        .HasColumnName("order");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("languages");
                });

            modelBuilder.Entity("Translation.Models.TranslationFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<string>("FileHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("file_hash");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("file_path");

                    b.HasKey("Id");

                    b.HasIndex("FilePath")
                        .IsUnique();

                    b.ToTable("translation_files");
                });

            modelBuilder.Entity("Translation.Models.TranslationKey", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("key");

                    b.Property<string>("Module")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("module");

                    b.HasKey("Id");

                    b.ToTable("translation_keys");
                });

            modelBuilder.Entity("Translation.Models.TranslationValue", b =>
                {
                    b.Property<Guid>("TranslationKeyId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("translation_key_id");

                    b.Property<Guid>("LanguageId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("language_id");

                    b.Property<string>("DefaultValue")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("default_value");

                    b.Property<string>("OverriddenValue")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("overridden_value");

                    b.HasKey("TranslationKeyId", "LanguageId");

                    b.HasIndex("LanguageId");

                    b.ToTable("translation_values");
                });

            modelBuilder.Entity("Translation.Models.TranslationValue", b =>
                {
                    b.HasOne("Translation.Models.Language", "Language")
                        .WithMany("TranslationValues")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Translation.Models.TranslationKey", "TranslationKey")
                        .WithMany("Values")
                        .HasForeignKey("TranslationKeyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Language");

                    b.Navigation("TranslationKey");
                });

            modelBuilder.Entity("Translation.Models.Language", b =>
                {
                    b.Navigation("TranslationValues");
                });

            modelBuilder.Entity("Translation.Models.TranslationKey", b =>
                {
                    b.Navigation("Values");
                });
#pragma warning restore 612, 618
        }
    }
}

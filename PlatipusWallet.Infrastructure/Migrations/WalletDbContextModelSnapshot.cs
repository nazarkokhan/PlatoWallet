﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PlatipusWallet.Infrastructure.Persistence;

#nullable disable

namespace PlatipusWallet.Infrastructure.Migrations
{
    [DbContext(typeof(WalletDbContext))]
    partial class WalletDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Award", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Awards", (string)null);
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.AwardRound", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AwardId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RoundId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AwardId")
                        .IsUnique();

                    b.HasIndex("RoundId")
                        .IsUnique();

                    b.ToTable("AwardRounds", (string)null);
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Casino", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int?>("Provider")
                        .HasColumnType("integer");

                    b.Property<string>("SignatureKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Casinos", (string)null);
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.CasinoCurrencies", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CasinoId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CasinoId");

                    b.HasIndex("CurrencyId");

                    b.ToTable("CasinoCurrencies", (string)null);
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Currency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Currencies", (string)null);
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.MockedError", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<int>("HttpStatusCode")
                        .HasColumnType("integer");

                    b.Property<int>("Method")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("MockedErrors", (string)null);
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Round", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool>("Finished")
                        .HasColumnType("boolean");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Rounds", (string)null);
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Sessions", (string)null);
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<decimal>("Amount")
                        .HasPrecision(38, 2)
                        .HasColumnType("numeric(38,2)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RoundId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoundId");

                    b.ToTable("Transactions", (string)null);
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Balance")
                        .HasPrecision(38, 2)
                        .HasColumnType("numeric(38,2)");

                    b.Property<string>("CasinoId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CasinoId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Award", b =>
                {
                    b.HasOne("PlatipusWallet.Domain.Entities.User", "User")
                        .WithMany("Awards")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.AwardRound", b =>
                {
                    b.HasOne("PlatipusWallet.Domain.Entities.Award", "Award")
                        .WithOne("AwardRound")
                        .HasForeignKey("PlatipusWallet.Domain.Entities.AwardRound", "AwardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PlatipusWallet.Domain.Entities.Round", "Round")
                        .WithOne("AwardRound")
                        .HasForeignKey("PlatipusWallet.Domain.Entities.AwardRound", "RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Award");

                    b.Navigation("Round");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.CasinoCurrencies", b =>
                {
                    b.HasOne("PlatipusWallet.Domain.Entities.Casino", "Casino")
                        .WithMany("CasinoCurrencies")
                        .HasForeignKey("CasinoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PlatipusWallet.Domain.Entities.Currency", "Currency")
                        .WithMany("CasinoCurrencies")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Casino");

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.MockedError", b =>
                {
                    b.HasOne("PlatipusWallet.Domain.Entities.User", "User")
                        .WithMany("MockedErrors")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Round", b =>
                {
                    b.HasOne("PlatipusWallet.Domain.Entities.User", "User")
                        .WithMany("Rounds")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Session", b =>
                {
                    b.HasOne("PlatipusWallet.Domain.Entities.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Transaction", b =>
                {
                    b.HasOne("PlatipusWallet.Domain.Entities.Round", "Round")
                        .WithMany("Transactions")
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Round");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.User", b =>
                {
                    b.HasOne("PlatipusWallet.Domain.Entities.Casino", "Casino")
                        .WithMany("Users")
                        .HasForeignKey("CasinoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PlatipusWallet.Domain.Entities.Currency", "Currency")
                        .WithMany("Users")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Casino");

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Award", b =>
                {
                    b.Navigation("AwardRound");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Casino", b =>
                {
                    b.Navigation("CasinoCurrencies");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Currency", b =>
                {
                    b.Navigation("CasinoCurrencies");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.Round", b =>
                {
                    b.Navigation("AwardRound");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("PlatipusWallet.Domain.Entities.User", b =>
                {
                    b.Navigation("Awards");

                    b.Navigation("MockedErrors");

                    b.Navigation("Rounds");

                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}

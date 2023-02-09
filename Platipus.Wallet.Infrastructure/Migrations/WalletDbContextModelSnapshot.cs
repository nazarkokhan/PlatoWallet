﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Platipus.Wallet.Infrastructure.Persistence;

#nullable disable

namespace Platipus.Wallet.Infrastructure.Migrations
{
    [DbContext(typeof(WalletDbContext))]
    partial class WalletDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Award", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("valid_until");

                    b.HasKey("Id")
                        .HasName("pk_awards");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_awards_user_id");

                    b.ToTable("awards", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.AwardRound", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AwardId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("award_id");

                    b.Property<string>("RoundId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("round_id");

                    b.HasKey("Id")
                        .HasName("pk_award_rounds");

                    b.HasIndex("AwardId")
                        .IsUnique()
                        .HasDatabaseName("ix_award_rounds_award_id");

                    b.HasIndex("RoundId")
                        .IsUnique()
                        .HasDatabaseName("ix_award_rounds_round_id");

                    b.ToTable("award_rounds", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Casino", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<int?>("Provider")
                        .HasColumnType("integer")
                        .HasColumnName("provider");

                    b.Property<string>("SignatureKey")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("signature_key");

                    b.Property<int?>("SwProviderId")
                        .HasColumnType("integer")
                        .HasColumnName("sw_provider_id");

                    b.HasKey("Id")
                        .HasName("pk_casinos");

                    b.ToTable("casinos", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.CasinoCurrencies", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("CasinoId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("casino_id");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uuid")
                        .HasColumnName("currency_id");

                    b.HasKey("Id")
                        .HasName("pk_casino_currencies");

                    b.HasIndex("CasinoId")
                        .HasDatabaseName("ix_casino_currencies_casino_id");

                    b.HasIndex("CurrencyId")
                        .HasDatabaseName("ix_casino_currencies_currency_id");

                    b.ToTable("casino_currencies", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.CasinoGames", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("CasinoId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("casino_id");

                    b.Property<int>("GameId")
                        .HasColumnType("integer")
                        .HasColumnName("game_id");

                    b.HasKey("Id")
                        .HasName("pk_casino_games");

                    b.HasIndex("CasinoId")
                        .HasDatabaseName("ix_casino_games_casino_id");

                    b.HasIndex("GameId")
                        .HasDatabaseName("ix_casino_games_game_id");

                    b.ToTable("casino_games", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Currency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_currencies");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_currencies_name");

                    b.ToTable("currencies", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer")
                        .HasColumnName("category_id");

                    b.Property<int>("GameServerId")
                        .HasColumnType("integer")
                        .HasColumnName("game_server_id");

                    b.Property<string>("LaunchName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("launch_name");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_game");

                    b.HasIndex("GameServerId")
                        .IsUnique()
                        .HasDatabaseName("ix_game_game_server_id");

                    b.HasIndex("LaunchName")
                        .IsUnique()
                        .HasDatabaseName("ix_game_launch_name");

                    b.ToTable("game", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.MockedError", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("body");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content_type");

                    b.Property<int>("Count")
                        .HasColumnType("integer")
                        .HasColumnName("count");

                    b.Property<int>("HttpStatusCode")
                        .HasColumnType("integer")
                        .HasColumnName("http_status_code");

                    b.Property<int>("Method")
                        .HasColumnType("integer")
                        .HasColumnName("method");

                    b.Property<TimeSpan?>("Timeout")
                        .HasColumnType("interval")
                        .HasColumnName("timeout");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_mocked_errors");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_mocked_errors_user_id");

                    b.ToTable("mocked_errors", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Request", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_requests");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_requests_user_id");

                    b.ToTable("requests", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Round", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<bool>("Finished")
                        .HasColumnType("boolean")
                        .HasColumnName("finished");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_rounds");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_rounds_user_id");

                    b.ToTable("rounds", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiration_date");

                    b.Property<bool>("IsTemporaryToken")
                        .HasColumnType("boolean")
                        .HasColumnName("is_temporary_token");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_sessions");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_sessions_user_id");

                    b.ToTable("sessions", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<decimal>("Amount")
                        .HasPrecision(38, 2)
                        .HasColumnType("numeric(38,2)")
                        .HasColumnName("amount");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<string>("InternalId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("internal_id");

                    b.Property<string>("RoundId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("round_id");

                    b.HasKey("Id")
                        .HasName("pk_transactions");

                    b.HasIndex("RoundId")
                        .HasDatabaseName("ix_transactions_round_id");

                    b.ToTable("transactions", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal>("Balance")
                        .HasPrecision(38, 2)
                        .HasColumnType("numeric(38,2)")
                        .HasColumnName("balance");

                    b.Property<string>("CasinoId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("casino_id");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uuid")
                        .HasColumnName("currency_id");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean")
                        .HasColumnName("is_disabled");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<int?>("SwUserId")
                        .HasColumnType("integer")
                        .HasColumnName("sw_user_id");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("CasinoId")
                        .HasDatabaseName("ix_users_casino_id");

                    b.HasIndex("CurrencyId")
                        .HasDatabaseName("ix_users_currency_id");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasDatabaseName("ix_users_user_name");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Award", b =>
                {
                    b.HasOne("Platipus.Wallet.Domain.Entities.User", "User")
                        .WithMany("Awards")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_awards_user_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.AwardRound", b =>
                {
                    b.HasOne("Platipus.Wallet.Domain.Entities.Award", "Award")
                        .WithOne("AwardRound")
                        .HasForeignKey("Platipus.Wallet.Domain.Entities.AwardRound", "AwardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_award_rounds_awards_award_id");

                    b.HasOne("Platipus.Wallet.Domain.Entities.Round", "Round")
                        .WithOne("AwardRound")
                        .HasForeignKey("Platipus.Wallet.Domain.Entities.AwardRound", "RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_award_rounds_round_round_id");

                    b.Navigation("Award");

                    b.Navigation("Round");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.CasinoCurrencies", b =>
                {
                    b.HasOne("Platipus.Wallet.Domain.Entities.Casino", "Casino")
                        .WithMany("CasinoCurrencies")
                        .HasForeignKey("CasinoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_casino_currencies_casino_casino_id");

                    b.HasOne("Platipus.Wallet.Domain.Entities.Currency", "Currency")
                        .WithMany("CasinoCurrencies")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_casino_currencies_currency_currency_id");

                    b.Navigation("Casino");

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.CasinoGames", b =>
                {
                    b.HasOne("Platipus.Wallet.Domain.Entities.Casino", "Casino")
                        .WithMany("CasinoGames")
                        .HasForeignKey("CasinoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_casino_games_casino_casino_id");

                    b.HasOne("Platipus.Wallet.Domain.Entities.Game", "Game")
                        .WithMany("CasinoGames")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_casino_games_game_game_id");

                    b.Navigation("Casino");

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.MockedError", b =>
                {
                    b.HasOne("Platipus.Wallet.Domain.Entities.User", "User")
                        .WithMany("MockedErrors")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_mocked_errors_user_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Request", b =>
                {
                    b.HasOne("Platipus.Wallet.Domain.Entities.User", "User")
                        .WithMany("Requests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_requests_user_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Round", b =>
                {
                    b.HasOne("Platipus.Wallet.Domain.Entities.User", "User")
                        .WithMany("Rounds")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_rounds_user_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Session", b =>
                {
                    b.HasOne("Platipus.Wallet.Domain.Entities.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_sessions_user_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Transaction", b =>
                {
                    b.HasOne("Platipus.Wallet.Domain.Entities.Round", "Round")
                        .WithMany("Transactions")
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_transactions_rounds_round_id");

                    b.Navigation("Round");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.User", b =>
                {
                    b.HasOne("Platipus.Wallet.Domain.Entities.Casino", "Casino")
                        .WithMany("Users")
                        .HasForeignKey("CasinoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_users_casinos_casino_id");

                    b.HasOne("Platipus.Wallet.Domain.Entities.Currency", "Currency")
                        .WithMany("Users")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_users_currencies_currency_id");

                    b.Navigation("Casino");

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Award", b =>
                {
                    b.Navigation("AwardRound");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Casino", b =>
                {
                    b.Navigation("CasinoCurrencies");

                    b.Navigation("CasinoGames");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Currency", b =>
                {
                    b.Navigation("CasinoCurrencies");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Game", b =>
                {
                    b.Navigation("CasinoGames");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.Round", b =>
                {
                    b.Navigation("AwardRound");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("Platipus.Wallet.Domain.Entities.User", b =>
                {
                    b.Navigation("Awards");

                    b.Navigation("MockedErrors");

                    b.Navigation("Requests");

                    b.Navigation("Rounds");

                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}

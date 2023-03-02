using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Platipus.Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "game_environments",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    baseurl = table.Column<string>(name: "base_url", type: "text", nullable: false),
                    uisbaseurl = table.Column<string>(name: "uis_base_url", type: "text", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_environments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gameserviceid = table.Column<int>(name: "game_service_id", type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    launchname = table.Column<string>(name: "launch_name", type: "text", nullable: false),
                    categoryid = table.Column<int>(name: "category_id", type: "integer", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_games", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "casinos",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    provider = table.Column<int>(type: "integer", nullable: false),
                    signaturekey = table.Column<string>(name: "signature_key", type: "text", nullable: false),
                    internalid = table.Column<int>(name: "internal_id", type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gameenvironmentid = table.Column<string>(name: "game_environment_id", type: "text", nullable: false),
                    @params = table.Column<Dictionary<string, JsonNode>>(name: "params", type: "jsonb", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_casinos", x => x.id);
                    table.ForeignKey(
                        name: "fk_casinos_game_environment_game_environment_id",
                        column: x => x.gameenvironmentid,
                        principalTable: "game_environments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "casino_currencies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    casinoid = table.Column<string>(name: "casino_id", type: "text", nullable: false),
                    currencyid = table.Column<string>(name: "currency_id", type: "text", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_casino_currencies", x => x.id);
                    table.ForeignKey(
                        name: "fk_casino_currencies_casino_casino_id",
                        column: x => x.casinoid,
                        principalTable: "casinos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_casino_currencies_currency_currency_id",
                        column: x => x.currencyid,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "casino_games",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    casinoid = table.Column<string>(name: "casino_id", type: "text", nullable: false),
                    gameid = table.Column<int>(name: "game_id", type: "integer", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_casino_games", x => x.id);
                    table.ForeignKey(
                        name: "fk_casino_games_casino_casino_id",
                        column: x => x.casinoid,
                        principalTable: "casinos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_casino_games_game_game_id",
                        column: x => x.gameid,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    balance = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    isdisabled = table.Column<bool>(name: "is_disabled", type: "boolean", nullable: false),
                    currencyid = table.Column<string>(name: "currency_id", type: "text", nullable: false),
                    casinoid = table.Column<string>(name: "casino_id", type: "text", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_casinos_casino_id",
                        column: x => x.casinoid,
                        principalTable: "casinos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_currencies_currency_id",
                        column: x => x.currencyid,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "awards",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    validuntil = table.Column<DateTime>(name: "valid_until", type: "timestamp with time zone", nullable: false),
                    userid = table.Column<int>(name: "user_id", type: "integer", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_awards", x => x.id);
                    table.ForeignKey(
                        name: "fk_awards_user_user_id",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mocked_errors",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    method = table.Column<int>(type: "integer", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    httpstatuscode = table.Column<int>(name: "http_status_code", type: "integer", nullable: false),
                    contenttype = table.Column<string>(name: "content_type", type: "text", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false),
                    executionorder = table.Column<int>(name: "execution_order", type: "integer", nullable: false),
                    timeout = table.Column<TimeSpan>(type: "interval", nullable: true),
                    userid = table.Column<int>(name: "user_id", type: "integer", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mocked_errors", x => x.id);
                    table.ForeignKey(
                        name: "fk_mocked_errors_user_user_id",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "requests",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    userid = table.Column<int>(name: "user_id", type: "integer", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_requests_user_user_id",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rounds",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    internalid = table.Column<string>(name: "internal_id", type: "text", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    finished = table.Column<bool>(type: "boolean", nullable: false),
                    userid = table.Column<int>(name: "user_id", type: "integer", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rounds", x => x.id);
                    table.ForeignKey(
                        name: "fk_rounds_user_user_id",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    userid = table.Column<int>(name: "user_id", type: "integer", nullable: false),
                    expirationdate = table.Column<DateTime>(name: "expiration_date", type: "timestamp with time zone", nullable: false),
                    istemporarytoken = table.Column<bool>(name: "is_temporary_token", type: "boolean", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_sessions_user_user_id",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "award_rounds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    awardid = table.Column<string>(name: "award_id", type: "text", nullable: false),
                    roundid = table.Column<string>(name: "round_id", type: "text", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_award_rounds", x => x.id);
                    table.ForeignKey(
                        name: "fk_award_rounds_awards_award_id",
                        column: x => x.awardid,
                        principalTable: "awards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_award_rounds_round_round_id",
                        column: x => x.roundid,
                        principalTable: "rounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    iscanceled = table.Column<bool>(name: "is_canceled", type: "boolean", nullable: false),
                    internalid = table.Column<string>(name: "internal_id", type: "text", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    roundid = table.Column<string>(name: "round_id", type: "text", nullable: false),
                    createddate = table.Column<DateTime>(name: "created_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastupdateddate = table.Column<DateTime>(name: "last_updated_date", type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_transactions_rounds_round_id",
                        column: x => x.roundid,
                        principalTable: "rounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "currencies",
                column: "id",
                values: new object[]
                {
                    "ADA",
                    "AED",
                    "ALL",
                    "AMD",
                    "AOA",
                    "ARS",
                    "AUD",
                    "AZN",
                    "BAM",
                    "BDT",
                    "BGN",
                    "BHD",
                    "BOB",
                    "BRL",
                    "BTT",
                    "BYN",
                    "CAD",
                    "CDF",
                    "CHF",
                    "CLP",
                    "CNY",
                    "COP",
                    "CRC",
                    "CZK",
                    "DKK",
                    "DOGE",
                    "DOP",
                    "DZD",
                    "EGP",
                    "ETB",
                    "EUR",
                    "GBP",
                    "GEL",
                    "GHS",
                    "GMD",
                    "GTQ",
                    "HKD",
                    "HNL",
                    "HRK",
                    "HTG",
                    "HUF",
                    "ICX",
                    "IDR",
                    "ILS",
                    "INR",
                    "IQD",
                    "IRR",
                    "ISK",
                    "JOD",
                    "JPY",
                    "KES",
                    "KGS",
                    "KHR",
                    "KRW",
                    "KWD",
                    "KZT",
                    "LAK",
                    "LKR",
                    "MAD",
                    "MATIC",
                    "mBCH",
                    "mBNB",
                    "MDL",
                    "mEOS",
                    "MGA",
                    "MKD",
                    "mLTC",
                    "MMK",
                    "MNT",
                    "mXMR",
                    "MXN",
                    "MYR",
                    "MZN",
                    "NGN",
                    "NIO",
                    "NOK",
                    "NZD",
                    "OMR",
                    "PAB",
                    "PEN",
                    "PGK",
                    "PHP",
                    "PKR",
                    "PLN",
                    "PYG",
                    "QAR",
                    "RON",
                    "RSD",
                    "RUB",
                    "SAR",
                    "SCR",
                    "SDG",
                    "SEK",
                    "SGD",
                    "SOS",
                    "THB",
                    "TJS",
                    "TMT",
                    "TND",
                    "TRX",
                    "TRY",
                    "TWD",
                    "TZS",
                    "UAH",
                    "uBTC",
                    "uETH",
                    "UGX",
                    "USD",
                    "USDT",
                    "UYU",
                    "UZS",
                    "VND",
                    "XAF",
                    "XOF",
                    "XRP",
                    "XVG",
                    "ZAR",
                    "ZMW"
                });

            migrationBuilder.InsertData(
                table: "game_environments",
                columns: new[] { "id", "base_url", "uis_base_url" },
                values: new object[,]
                {
                    { "local", "http://localhost:5143/", "https://platipusgaming.cloud/qa/integration/vivo/test/index.html" },
                    { "test", "https://test.platipusgaming.com/", "https://platipusgaming.cloud/qa/integration/vivo/test/index.html" }
                });

            migrationBuilder.InsertData(
                table: "games",
                columns: new[] { "id", "category_id", "game_service_id", "launch_name", "name" },
                values: new object[,]
                {
                    { 1, 4, 99, "baccarat", "Baccarat PRO" },
                    { 2, 3, 110, "rouletteeuropean", "European Roulette" },
                    { 3, 1, 386, "cleosgold", "Cleo's Gold" },
                    { 4, 1, 392, "fieryplanet", "Fiery Planet" },
                    { 5, 1, 393, "mistressofamazon", "Mistress of Amazon" },
                    { 6, 1, 394, "magicalwolf", "Magical Wolf" },
                    { 7, 1, 395, "jewelbang", "Jewel Bang" },
                    { 8, 1, 400, "fairyforest", "Fairy Forest" },
                    { 9, 1, 401, "princessofbirds", "Princess of Birds" },
                    { 10, 1, 409, "crocoman", "Crocoman" },
                    { 11, 1, 417, "arabiantales", "Arabian Tales" },
                    { 12, 1, 423, "junglespin", "Jungle Spin" },
                    { 13, 1, 424, "fruitysevens", "Fruity Sevens" },
                    { 14, 1, 425, "crystalsevens", "Crystal Sevens" },
                    { 15, 1, 426, "safariadventures", "Safari Adventures" },
                    { 16, 1, 427, "luckydolphin", "Lucky Dolphin" },
                    { 17, 1, 428, "juicyspins", "Juicy Spins" },
                    { 18, 1, 429, "richywitchy", "Richy Witchy" },
                    { 19, 1, 442, "legendofatlantis", "Legend of Atlantis" },
                    { 20, 1, 443, "crazyjelly", "Crazy Jelly" },
                    { 21, 1, 444, "tripledragon", "Triple Dragon" },
                    { 22, 1, 446, "magicalmirror", "Magical Mirror" },
                    { 23, 1, 448, "aztectemple", "Aztec Temple" },
                    { 24, 1, 450, "megadrago", "Mega Drago" },
                    { 25, 1, 452, "powerofposeidon", "Power of Poseidon" },
                    { 26, 1, 465, "bookofegypt", "Book of Egypt" },
                    { 27, 1, 469, "sakurawind", "Sakura Wind" },
                    { 28, 1, 475, "luckymoney", "Lucky Money" },
                    { 29, 1, 476, "loveis", "Love Is" },
                    { 30, 1, 477, "cinderella", "Cinderella" },
                    { 31, 1, 480, "monkeysjourney", "Monkey's Journey" },
                    { 32, 1, 483, "powerofgods", "Power of Gods" },
                    { 33, 1, 485, "jadevalley", "Jade Valley" },
                    { 34, 1, 486, "greatocean", "Great Ocean" },
                    { 35, 2, 487, "blackjackvip", "Blackjack VIP" },
                    { 36, 1, 488, "neonclassic", "Neon Classic" },
                    { 37, 4, 489, "baccaratmini", "Baccarat Mini" },
                    { 38, 4, 490, "baccaratvip", "Baccarat VIP" },
                    { 39, 1, 491, "bisontrail", "Bison Trail" },
                    { 40, 1, 492, "pharaohsempire", "Pharaoh's Empire" },
                    { 41, 1, 526, "webbyheroes", "Webby Heroes" },
                    { 42, 1, 527, "luckycat", "Lucky Cat" },
                    { 43, 1, 528, "rhinomania", "Rhino Mania" },
                    { 44, 1, 529, "azteccoins", "Aztec Coins" },
                    { 45, 1, 530, "chinesetigers", "Chinese Tigers" },
                    { 46, 1, 531, "jackpotlab", "Jackpot Lab" },
                    { 47, 1, 532, "piratesmap", "Pirate's Map" },
                    { 48, 1, 533, "dragonselement", "Dragon's Element" },
                    { 49, 1, 534, "caishensgifts", "Caishen's Gifts" },
                    { 50, 1, 535, "theancientfour", "The Ancient Four" },
                    { 51, 1, 536, "wildspin", "Wild Spin" },
                    { 52, 1, 537, "dajidali", "Da Ji Da Li" },
                    { 53, 1, 538, "dynastywarriors", "Dynasty Warriors" },
                    { 54, 1, 539, "chillifiesta", "Chilli Fiesta" },
                    { 55, 1, 540, "wealthofwisdom", "Wealth of Wisdom" },
                    { 56, 1, 541, "royallotus", "Royal Lotus" },
                    { 57, 1, 542, "santasbag", "Santa's Bag" },
                    { 58, 1, 543, "hawaiiannight", "Hawaiian Night" },
                    { 59, 1, 544, "lordofthesun", "Lord of the Sun" },
                    { 60, 1, 545, "hotfruits", "7 & Hot Fruits" },
                    { 61, 1, 546, "thousandonespins", "1001 spins" },
                    { 62, 1, 547, "guisesofdracula", "Guises of Dracula" },
                    { 63, 1, 548, "mightofzeus", "Might of Zeus" },
                    { 64, 1, 549, "bamboogrove", "Bamboo Grove" },
                    { 65, 1, 550, "leprechauns", "Leprechaun's Coins" },
                    { 66, 1, 551, "wildjustice", "Wild Justice" },
                    { 67, 1, 552, "littlewitchy", "Little Witchy" },
                    { 68, 1, 553, "undiademuertos", "Un Dia de Muertos" },
                    { 69, 1, 554, "frozenmirror", "Frozen Mirror" },
                    { 70, 1, 555, "diamondhunt", "Diamond Hunt" },
                    { 71, 1, 556, "jokerchase", "Joker Chase" },
                    { 72, 1, 557, "poshcats", "Posh Cats" },
                    { 73, 1, 558, "justsweets", "Just Sweets" },
                    { 74, 1, 559, "piedradelsol", "Piedra del Sol" },
                    { 75, 1, 560, "ninedragonkings", "9 Dragon Kings" },
                    { 76, 1, 561, "ninegems", "9 Gems" },
                    { 77, 5, 562, "jacksorbetter", "Jacks or Better" },
                    { 78, 1, 563, "waysofthegauls", "Ways of the Gauls" },
                    { 79, 1, 564, "wildcrowns", "Wild Crowns" },
                    { 80, 1, 565, "pearlsoftheocean", "Pearls of the Ocean" },
                    { 81, 5, 566, "bonusdeuceswild", "Bonus Deuces Wild" },
                    { 82, 1, 567, "bookoflight", "Book of Light" },
                    { 83, 1, 568, "extragems", "Extra Gems" },
                    { 84, 1, 569, "coinfest", "Coinfest" },
                    { 85, 1, 570, "hallowin", "Hallowin" },
                    { 86, 1, 571, "xmasavalanche", "Xmas Avalanche" },
                    { 87, 1, 572, "fruitboost", "Fruit Boost" },
                    { 88, 1, 573, "booksofgiza", "Books of Giza" },
                    { 89, 1, 574, "thebigscore", "The Big Score" },
                    { 90, 1, 575, "catchtheleprechaun", "Catch the Leprechaun" },
                    { 91, 1, 576, "pirateslegacy", "Pirate's Legacy" },
                    { 92, 5, 577, "acesandfaces", "Aces and Faces" },
                    { 93, 1, 578, "wildspindeluxe", "Wild Spin Deluxe" },
                    { 94, 1, 579, "bitstarzelement", "Bitstarz Element" },
                    { 95, 5, 580, "twowaysroyal", "Two Ways Royal" },
                    { 96, 1, 581, "skycrown", "Skycrown" },
                    { 97, 1, 582, "missgypsy", "Miss Gypsy" },
                    { 98, 1, 583, "thorturbopower", "Thor Turbo Power" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_award_rounds_award_id",
                table: "award_rounds",
                column: "award_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_award_rounds_created_date",
                table: "award_rounds",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_award_rounds_last_updated_date",
                table: "award_rounds",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_award_rounds_round_id",
                table: "award_rounds",
                column: "round_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_awards_created_date",
                table: "awards",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_awards_last_updated_date",
                table: "awards",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_awards_user_id",
                table: "awards",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_casino_currencies_casino_id",
                table: "casino_currencies",
                column: "casino_id");

            migrationBuilder.CreateIndex(
                name: "ix_casino_currencies_created_date",
                table: "casino_currencies",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_casino_currencies_currency_id",
                table: "casino_currencies",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_casino_currencies_last_updated_date",
                table: "casino_currencies",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_casino_games_casino_id",
                table: "casino_games",
                column: "casino_id");

            migrationBuilder.CreateIndex(
                name: "ix_casino_games_created_date",
                table: "casino_games",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_casino_games_game_id",
                table: "casino_games",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_casino_games_last_updated_date",
                table: "casino_games",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_casinos_created_date",
                table: "casinos",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_casinos_game_environment_id",
                table: "casinos",
                column: "game_environment_id");

            migrationBuilder.CreateIndex(
                name: "ix_casinos_last_updated_date",
                table: "casinos",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_currencies_created_date",
                table: "currencies",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_currencies_last_updated_date",
                table: "currencies",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_game_environments_created_date",
                table: "game_environments",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_game_environments_last_updated_date",
                table: "game_environments",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_games_created_date",
                table: "games",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_games_game_service_id",
                table: "games",
                column: "game_service_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_games_last_updated_date",
                table: "games",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_games_launch_name",
                table: "games",
                column: "launch_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_mocked_errors_created_date",
                table: "mocked_errors",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_mocked_errors_last_updated_date",
                table: "mocked_errors",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_mocked_errors_method_user_id_execution_order",
                table: "mocked_errors",
                columns: new[] { "method", "user_id", "execution_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_mocked_errors_user_id",
                table: "mocked_errors",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_requests_created_date",
                table: "requests",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_requests_last_updated_date",
                table: "requests",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_requests_user_id",
                table: "requests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_created_date",
                table: "rounds",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_last_updated_date",
                table: "rounds",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_user_id",
                table: "rounds",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_sessions_created_date",
                table: "sessions",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_sessions_last_updated_date",
                table: "sessions",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_sessions_user_id",
                table: "sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_created_date",
                table: "transactions",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_last_updated_date",
                table: "transactions",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_round_id",
                table: "transactions",
                column: "round_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_casino_id",
                table: "users",
                column: "casino_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_created_date",
                table: "users",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_users_currency_id",
                table: "users",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_last_updated_date",
                table: "users",
                column: "last_updated_date");

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "award_rounds");

            migrationBuilder.DropTable(
                name: "casino_currencies");

            migrationBuilder.DropTable(
                name: "casino_games");

            migrationBuilder.DropTable(
                name: "mocked_errors");

            migrationBuilder.DropTable(
                name: "requests");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "awards");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "rounds");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "casinos");

            migrationBuilder.DropTable(
                name: "currencies");

            migrationBuilder.DropTable(
                name: "game_environments");
        }
    }
}

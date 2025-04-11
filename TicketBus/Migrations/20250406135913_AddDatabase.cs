using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    IdBrand = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameBrand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.IdBrand);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    IdCity = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameCity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.IdCity);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    IdCoupon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.IdCoupon);
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    IdPassenger = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PassengerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamePassenger = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCard = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.IdPassenger);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    IdPos = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PosCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamePos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bonus = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.IdPos);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    IdService = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameService = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.IdService);
                });

            migrationBuilder.CreateTable(
                name: "TypeNews",
                columns: table => new
                {
                    IdTypeNews = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameTypeNews = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeNews", x => x.IdTypeNews);
                });

            migrationBuilder.CreateTable(
                name: "VehicleTypes",
                columns: table => new
                {
                    IdType = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeatCount = table.Column<int>(type: "int", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTypes", x => x.IdType);
                });

            migrationBuilder.CreateTable(
                name: "RegistForms",
                columns: table => new
                {
                    IdRegist = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdBrand = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistForms", x => x.IdRegist);
                    table.ForeignKey(
                        name: "FK_RegistForms_Brands_IdBrand",
                        column: x => x.IdBrand,
                        principalTable: "Brands",
                        principalColumn: "IdBrand",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    IdDistrict = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameDistrict = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.IdDistrict);
                    table.ForeignKey(
                        name: "FK_Districts_Cities_IdCity",
                        column: x => x.IdCity,
                        principalTable: "Cities",
                        principalColumn: "IdCity",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    IdBill = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdPassenger = table.Column<int>(type: "int", nullable: true),
                    SeatQuantity = table.Column<int>(type: "int", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.IdBill);
                    table.ForeignKey(
                        name: "FK_Bills_Passengers_IdPassenger",
                        column: x => x.IdPassenger,
                        principalTable: "Passengers",
                        principalColumn: "IdPassenger",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    IdFeedback = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedbackCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdPassenger = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.IdFeedback);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Passengers_IdPassenger",
                        column: x => x.IdPassenger,
                        principalTable: "Passengers",
                        principalColumn: "IdPassenger");
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    IdEmployee = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEmployee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdBrand = table.Column<int>(type: "int", nullable: true),
                    IdPos = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.IdEmployee);
                    table.ForeignKey(
                        name: "FK_Employees_Brands_IdBrand",
                        column: x => x.IdBrand,
                        principalTable: "Brands",
                        principalColumn: "IdBrand",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Positions_IdPos",
                        column: x => x.IdPos,
                        principalTable: "Positions",
                        principalColumn: "IdPos");
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    IdNews = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NewsCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdTypeNews = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.IdNews);
                    table.ForeignKey(
                        name: "FK_News_TypeNews_IdTypeNews",
                        column: x => x.IdTypeNews,
                        principalTable: "TypeNews",
                        principalColumn: "IdTypeNews",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDetails",
                columns: table => new
                {
                    IdType = table.Column<int>(type: "int", nullable: false),
                    IdService = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDetails", x => new { x.IdType, x.IdService });
                    table.ForeignKey(
                        name: "FK_ServiceDetails_Services_IdService",
                        column: x => x.IdService,
                        principalTable: "Services",
                        principalColumn: "IdService",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceDetails_VehicleTypes_IdType",
                        column: x => x.IdType,
                        principalTable: "VehicleTypes",
                        principalColumn: "IdType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusRoutes",
                columns: table => new
                {
                    IdRoute = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameRoute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Distance = table.Column<int>(type: "int", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    IdRegist = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusRoutes", x => x.IdRoute);
                    table.ForeignKey(
                        name: "FK_BusRoutes_RegistForms_IdRegist",
                        column: x => x.IdRegist,
                        principalTable: "RegistForms",
                        principalColumn: "IdRegist");
                });

            migrationBuilder.CreateTable(
                name: "Coaches",
                columns: table => new
                {
                    IdCoach = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoachCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberPlate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    IdType = table.Column<int>(type: "int", nullable: true),
                    IdRegist = table.Column<int>(type: "int", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coaches", x => x.IdCoach);
                    table.ForeignKey(
                        name: "FK_Coaches_RegistForms_IdRegist",
                        column: x => x.IdRegist,
                        principalTable: "RegistForms",
                        principalColumn: "IdRegist",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Coaches_VehicleTypes_IdType",
                        column: x => x.IdType,
                        principalTable: "VehicleTypes",
                        principalColumn: "IdType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DropOffs",
                columns: table => new
                {
                    IdDropOff = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DropOffCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DropOffName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCity = table.Column<int>(type: "int", nullable: true),
                    IdRegist = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropOffs", x => x.IdDropOff);
                    table.ForeignKey(
                        name: "FK_DropOffs_Cities_IdCity",
                        column: x => x.IdCity,
                        principalTable: "Cities",
                        principalColumn: "IdCity",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DropOffs_RegistForms_IdRegist",
                        column: x => x.IdRegist,
                        principalTable: "RegistForms",
                        principalColumn: "IdRegist",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pickups",
                columns: table => new
                {
                    IdPickup = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCity = table.Column<int>(type: "int", nullable: true),
                    IdRegist = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pickups", x => x.IdPickup);
                    table.ForeignKey(
                        name: "FK_Pickups_Cities_IdCity",
                        column: x => x.IdCity,
                        principalTable: "Cities",
                        principalColumn: "IdCity",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pickups_RegistForms_IdRegist",
                        column: x => x.IdRegist,
                        principalTable: "RegistForms",
                        principalColumn: "IdRegist",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountDetails",
                columns: table => new
                {
                    IdCoupon = table.Column<int>(type: "int", nullable: false),
                    IdBill = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountDetails", x => new { x.IdCoupon, x.IdBill });
                    table.ForeignKey(
                        name: "FK_DiscountDetails_Bills_IdBill",
                        column: x => x.IdBill,
                        principalTable: "Bills",
                        principalColumn: "IdBill",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountDetails_Coupons_IdCoupon",
                        column: x => x.IdCoupon,
                        principalTable: "Coupons",
                        principalColumn: "IdCoupon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteStops",
                columns: table => new
                {
                    IdStop = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StopCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdRoute = table.Column<int>(type: "int", nullable: true),
                    StopName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopOrder = table.Column<int>(type: "int", nullable: true),
                    Time = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStops", x => x.IdStop);
                    table.ForeignKey(
                        name: "FK_RouteStops_BusRoutes_IdRoute",
                        column: x => x.IdRoute,
                        principalTable: "BusRoutes",
                        principalColumn: "IdRoute");
                });

            migrationBuilder.CreateTable(
                name: "ScheduleDetails",
                columns: table => new
                {
                    IdCoach = table.Column<int>(type: "int", nullable: false),
                    IdRoute = table.Column<int>(type: "int", nullable: false),
                    DepartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    ArriveTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleDetails", x => new { x.IdCoach, x.IdRoute });
                    table.ForeignKey(
                        name: "FK_ScheduleDetails_BusRoutes_IdRoute",
                        column: x => x.IdRoute,
                        principalTable: "BusRoutes",
                        principalColumn: "IdRoute",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleDetails_Coaches_IdCoach",
                        column: x => x.IdCoach,
                        principalTable: "Coaches",
                        principalColumn: "IdCoach",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    IdSeat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeatCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<int>(type: "int", nullable: true),
                    IdCoach = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.IdSeat);
                    table.ForeignKey(
                        name: "FK_Seats_Coaches_IdCoach",
                        column: x => x.IdCoach,
                        principalTable: "Coaches",
                        principalColumn: "IdCoach",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    IdPrice = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdRoute = table.Column<int>(type: "int", nullable: true),
                    IdStopStart = table.Column<int>(type: "int", nullable: true),
                    IdStopEnd = table.Column<int>(type: "int", nullable: true),
                    IdCoach = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.IdPrice);
                    table.ForeignKey(
                        name: "FK_Prices_BusRoutes_IdRoute",
                        column: x => x.IdRoute,
                        principalTable: "BusRoutes",
                        principalColumn: "IdRoute");
                    table.ForeignKey(
                        name: "FK_Prices_Coaches_IdCoach",
                        column: x => x.IdCoach,
                        principalTable: "Coaches",
                        principalColumn: "IdCoach");
                    table.ForeignKey(
                        name: "FK_Prices_RouteStops_IdStopEnd",
                        column: x => x.IdStopEnd,
                        principalTable: "RouteStops",
                        principalColumn: "IdStop");
                    table.ForeignKey(
                        name: "FK_Prices_RouteStops_IdStopStart",
                        column: x => x.IdStopStart,
                        principalTable: "RouteStops",
                        principalColumn: "IdStop");
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    IdTicket = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdSeat = table.Column<int>(type: "int", nullable: true),
                    IdPrice = table.Column<int>(type: "int", nullable: true),
                    IdPassenger = table.Column<int>(type: "int", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    IdEmployee = table.Column<int>(type: "int", nullable: true),
                    IdRoute = table.Column<int>(type: "int", nullable: true),
                    IdCoach = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.IdTicket);
                    table.ForeignKey(
                        name: "FK_Tickets_BusRoutes_IdRoute",
                        column: x => x.IdRoute,
                        principalTable: "BusRoutes",
                        principalColumn: "IdRoute");
                    table.ForeignKey(
                        name: "FK_Tickets_Coaches_IdCoach",
                        column: x => x.IdCoach,
                        principalTable: "Coaches",
                        principalColumn: "IdCoach");
                    table.ForeignKey(
                        name: "FK_Tickets_Employees_IdEmployee",
                        column: x => x.IdEmployee,
                        principalTable: "Employees",
                        principalColumn: "IdEmployee");
                    table.ForeignKey(
                        name: "FK_Tickets_Passengers_IdPassenger",
                        column: x => x.IdPassenger,
                        principalTable: "Passengers",
                        principalColumn: "IdPassenger");
                    table.ForeignKey(
                        name: "FK_Tickets_Prices_IdPrice",
                        column: x => x.IdPrice,
                        principalTable: "Prices",
                        principalColumn: "IdPrice");
                    table.ForeignKey(
                        name: "FK_Tickets_Seats_IdSeat",
                        column: x => x.IdSeat,
                        principalTable: "Seats",
                        principalColumn: "IdSeat");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bills_IdPassenger",
                table: "Bills",
                column: "IdPassenger");

            migrationBuilder.CreateIndex(
                name: "IX_BusRoutes_IdRegist",
                table: "BusRoutes",
                column: "IdRegist");

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_IdRegist",
                table: "Coaches",
                column: "IdRegist");

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_IdType",
                table: "Coaches",
                column: "IdType");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountDetails_IdBill",
                table: "DiscountDetails",
                column: "IdBill");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_IdCity",
                table: "Districts",
                column: "IdCity");

            migrationBuilder.CreateIndex(
                name: "IX_DropOffs_IdCity",
                table: "DropOffs",
                column: "IdCity");

            migrationBuilder.CreateIndex(
                name: "IX_DropOffs_IdRegist",
                table: "DropOffs",
                column: "IdRegist");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IdBrand",
                table: "Employees",
                column: "IdBrand");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IdPos",
                table: "Employees",
                column: "IdPos");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_IdPassenger",
                table: "Feedbacks",
                column: "IdPassenger");

            migrationBuilder.CreateIndex(
                name: "IX_News_IdTypeNews",
                table: "News",
                column: "IdTypeNews");

            migrationBuilder.CreateIndex(
                name: "IX_Pickups_IdCity",
                table: "Pickups",
                column: "IdCity");

            migrationBuilder.CreateIndex(
                name: "IX_Pickups_IdRegist",
                table: "Pickups",
                column: "IdRegist");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_IdCoach",
                table: "Prices",
                column: "IdCoach");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_IdRoute",
                table: "Prices",
                column: "IdRoute");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_IdStopEnd",
                table: "Prices",
                column: "IdStopEnd");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_IdStopStart",
                table: "Prices",
                column: "IdStopStart");

            migrationBuilder.CreateIndex(
                name: "IX_RegistForms_IdBrand",
                table: "RegistForms",
                column: "IdBrand");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_IdRoute",
                table: "RouteStops",
                column: "IdRoute");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDetails_IdRoute",
                table: "ScheduleDetails",
                column: "IdRoute");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_IdCoach",
                table: "Seats",
                column: "IdCoach");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetails_IdService",
                table: "ServiceDetails",
                column: "IdService");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdCoach",
                table: "Tickets",
                column: "IdCoach");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdEmployee",
                table: "Tickets",
                column: "IdEmployee");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdPassenger",
                table: "Tickets",
                column: "IdPassenger");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdPrice",
                table: "Tickets",
                column: "IdPrice");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdRoute",
                table: "Tickets",
                column: "IdRoute");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdSeat",
                table: "Tickets",
                column: "IdSeat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountDetails");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "DropOffs");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Pickups");

            migrationBuilder.DropTable(
                name: "ScheduleDetails");

            migrationBuilder.DropTable(
                name: "ServiceDetails");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "TypeNews");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Passengers");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "RouteStops");

            migrationBuilder.DropTable(
                name: "Coaches");

            migrationBuilder.DropTable(
                name: "BusRoutes");

            migrationBuilder.DropTable(
                name: "VehicleTypes");

            migrationBuilder.DropTable(
                name: "RegistForms");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}

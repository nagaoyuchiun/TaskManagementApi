using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JWTCliam",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    iss = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sub = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    aud = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    exp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nbf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    iat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    jti = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JWTCliam", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JWTConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Issuer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireDateTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JWTConfig", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "JWTCliam");

            migrationBuilder.DropTable(
                name: "JWTConfig");
        }
    }
}

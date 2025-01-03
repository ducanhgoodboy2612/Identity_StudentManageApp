using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagement_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "TuitionFees",
                type: "DECIMAL(18, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MarksObtained",
                table: "Grades",
                type: "DECIMAL(18, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalMarks",
                table: "Exams",
                type: "DECIMAL(18, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18,2)");

            migrationBuilder.AlterColumn<bool>(
                name: "TwoFactorEnabled",
                schema: "STUDENT_MANAGE_DB",
                table: "AspNetUsers",
                type: "NUMBER(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "BOOLEAN");

            migrationBuilder.AlterColumn<bool>(
                name: "PhoneNumberConfirmed",
                schema: "STUDENT_MANAGE_DB",
                table: "AspNetUsers",
                type: "NUMBER(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "BOOLEAN");

            migrationBuilder.AlterColumn<bool>(
                name: "LockoutEnabled",
                schema: "STUDENT_MANAGE_DB",
                table: "AspNetUsers",
                type: "NUMBER(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "BOOLEAN");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailConfirmed",
                schema: "STUDENT_MANAGE_DB",
                table: "AspNetUsers",
                type: "NUMBER(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "BOOLEAN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "TuitionFees",
                type: "DECIMAL(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MarksObtained",
                table: "Grades",
                type: "DECIMAL(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalMarks",
                table: "Exams",
                type: "DECIMAL(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18, 2)");

            migrationBuilder.AlterColumn<bool>(
                name: "TwoFactorEnabled",
                schema: "STUDENT_MANAGE_DB",
                table: "AspNetUsers",
                type: "BOOLEAN",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "NUMBER(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "PhoneNumberConfirmed",
                schema: "STUDENT_MANAGE_DB",
                table: "AspNetUsers",
                type: "BOOLEAN",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "NUMBER(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "LockoutEnabled",
                schema: "STUDENT_MANAGE_DB",
                table: "AspNetUsers",
                type: "BOOLEAN",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "NUMBER(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailConfirmed",
                schema: "STUDENT_MANAGE_DB",
                table: "AspNetUsers",
                type: "BOOLEAN",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "NUMBER(1)");
        }
    }
}

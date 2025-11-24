using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerAdvisorApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateCareerPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                Create table CareerPlan(
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    UserId NVARCHAR(450) NOT NULL,
                    PlanDetails NVARCHAR(MAX) NOT NULL,
                    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
                    UpdatedAt DATETIME NULL,
                    CONSTRAINT FK_CareerPlan_User FOREIGN KEY (UserId)
                        REFERENCES AspNetUsers(Id) ON DELETE CASCADE
                )
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop table CareerPlan");
        }
    }
}

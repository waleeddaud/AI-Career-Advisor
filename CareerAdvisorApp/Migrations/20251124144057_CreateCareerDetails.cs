using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerAdvisorApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateCareerDetails : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE CareerDetails (
                    Id INT IDENTITY(1,1) PRIMARY KEY,

                    UserId NVARCHAR(450) NOT NULL, -- Foreign key to AspNetUsers.Id

                    EducationLevel NVARCHAR(50) NOT NULL,
                    Skills NVARCHAR(MAX) NOT NULL,
                    Interests NVARCHAR(MAX) NOT NULL,
                    CareerGoals NVARCHAR(MAX) NOT NULL,
                    Experience NVARCHAR(50) NULL,
                    Industry NVARCHAR(100) NULL,
                    WorkStyle NVARCHAR(100) NULL,
                    Salary NVARCHAR(50) NULL,
                    Timeline NVARCHAR(50) NULL,

                    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
                    UpdatedAt DATETIME NULL,

                    CONSTRAINT FK_CareerDetails_User FOREIGN KEY (UserId)
                        REFERENCES AspNetUsers(Id) ON DELETE CASCADE
                )
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE CareerDetails");
        }

    }
}

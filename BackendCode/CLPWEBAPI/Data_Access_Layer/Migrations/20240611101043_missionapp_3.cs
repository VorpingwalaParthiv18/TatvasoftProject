using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data_Access_Layer.Migrations
{
    /// <inheritdoc />
    public partial class missionapp_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissionApplication_Missions_MissionId",
                table: "MissionApplication");

            migrationBuilder.DropIndex(
                name: "IX_MissionApplication_MissionId",
                table: "MissionApplication");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MissionApplication_MissionId",
                table: "MissionApplication",
                column: "MissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MissionApplication_Missions_MissionId",
                table: "MissionApplication",
                column: "MissionId",
                principalTable: "Missions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

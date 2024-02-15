using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTest.Migrations
{
    /// <inheritdoc />
    public partial class DB5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_ticket_TicketID",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "Attachment",
                table: "ticket");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "ticket");

            migrationBuilder.DropColumn(
                name: "Attachment",
                table: "Message");

            migrationBuilder.CreateTable(
                name: "attachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attachment_Message_MessageID",
                        column: x => x.MessageID,
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attachment_MessageID",
                table: "attachment",
                column: "MessageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_ticket_TicketID",
                table: "Message",
                column: "TicketID",
                principalTable: "ticket",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_ticket_TicketID",
                table: "Message");

            migrationBuilder.DropTable(
                name: "attachment");

            migrationBuilder.AddColumn<byte[]>(
                name: "Attachment",
                table: "ticket",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "ticket",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "Attachment",
                table: "Message",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_ticket_TicketID",
                table: "Message",
                column: "TicketID",
                principalTable: "ticket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

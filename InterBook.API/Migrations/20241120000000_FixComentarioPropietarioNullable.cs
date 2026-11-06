using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterBook.API.Migrations
{
    /// <inheritdoc />
    public partial class FixComentarioPropietarioNullable : Migration
    {
   /// <inheritdoc />
   protected override void Up(MigrationBuilder migrationBuilder)
  {
  // Hacer que ComentarioPropietario permita NULL
            migrationBuilder.AlterColumn<string>(
                name: "ComentarioPropietario",
     table: "Intercambios",
    type: "nvarchar(500)",
            maxLength: 500,
        nullable: true,  // ? ESTO ES LO IMPORTANTE
         oldClrType: typeof(string),
    oldType: "nvarchar(500)",
         oldMaxLength: 500,
       oldNullable: false);
   }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ComentarioPropietario",
            table: "Intercambios",
         type: "nvarchar(500)",
     maxLength: 500,
    nullable: false,
 oldClrType: typeof(string),
                oldType: "nvarchar(500)",
            oldMaxLength: 500,
      oldNullable: true);
 }
    }
}

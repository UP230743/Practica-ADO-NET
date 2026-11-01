using Microsoft.Data.SqlClient;

namespace ProductosApi.Data;


// Se encarga de la conexión a SQL Server.
// Lee la cadena de conexión de appsettings.json y entrega objetos SqlConnection.

public class ConexionDb
{
    private readonly string _cadenaConexion;

    public ConexionDb(IConfiguration configuration)
    {
        _cadenaConexion = configuration.GetConnectionString("ProductosDB")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'ProductosDB'.");
    }
    // Crea una nueva conexión (cerrada). Quien la usa es responsable de abrirla y liberarla.
    public SqlConnection ObtenerConexion() => new SqlConnection(_cadenaConexion);
}

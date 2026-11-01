using System.Data;
using Microsoft.Data.SqlClient;
using ProductosApi.Data;
using ProductosApi.Models;

namespace ProductosApi.Repositories;

/// <summary>
/// Implementación de IProductoRepository usando ADO.NET (SqlConnection, SqlCommand, SqlDataReader).
/// Todas las consultas son parametrizadas para evitar inyección SQL.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    private readonly ConexionDb _conexionDb;

    public ProductoRepository(ConexionDb conexionDb)
    {
        _conexionDb = conexionDb;
    }

    // Consulta: obtiene todos los productos
    public List<Producto> ObtenerTodos()
    {
        const string sql = @"SELECT Id, Nombre, Descripcion, Categoria, Precio, Stock, Activo, FechaRegistro
                             FROM dbo.Productos
                             ORDER BY Id";

        var productos = new List<Producto>();

        using var conexion = _conexionDb.ObtenerConexion();
        using var comando = new SqlCommand(sql, conexion);

        conexion.Open();
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            productos.Add(MapearProducto(lector));
        }

        return productos;
    }

    // Buscar: obtiene un producto por su Id (null si no existe)
    public Producto? ObtenerPorId(int id)
    {
        const string sql = @"SELECT Id, Nombre, Descripcion, Categoria, Precio, Stock, Activo, FechaRegistro
                             FROM dbo.Productos
                             WHERE Id = @Id";

        using var conexion = _conexionDb.ObtenerConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        conexion.Open();
        using var lector = comando.ExecuteReader();

        return lector.Read() ? MapearProducto(lector) : null;
    }

    // Insert: agrega un producto y devuelve el Id generado
    public int Insertar(Producto producto)
    {
        const string sql = @"INSERT INTO dbo.Productos (Nombre, Descripcion, Categoria, Precio, Stock, Activo)
                             OUTPUT INSERTED.Id
                             VALUES (@Nombre, @Descripcion, @Categoria, @Precio, @Stock, @Activo)";

        using var conexion = _conexionDb.ObtenerConexion();
        using var comando = new SqlCommand(sql, conexion);
        AgregarParametros(comando, producto);

        conexion.Open();
        return (int)comando.ExecuteScalar();
    }

    // Actualizar: modifica un producto existente; devuelve true si se afectó alguna fila
    public bool Actualizar(Producto producto)
    {
        const string sql = @"UPDATE dbo.Productos
                             SET Nombre = @Nombre,
                                 Descripcion = @Descripcion,
                                 Categoria = @Categoria,
                                 Precio = @Precio,
                                 Stock = @Stock,
                                 Activo = @Activo
                             WHERE Id = @Id";

        using var conexion = _conexionDb.ObtenerConexion();
        using var comando = new SqlCommand(sql, conexion);
        AgregarParametros(comando, producto);
        comando.Parameters.Add("@Id", SqlDbType.Int).Value = producto.Id;

        conexion.Open();
        return comando.ExecuteNonQuery() > 0;
    }

    // Eliminar: borra un producto por Id; devuelve true si se eliminó
    public bool Eliminar(int id)
    {
        const string sql = "DELETE FROM dbo.Productos WHERE Id = @Id";

        using var conexion = _conexionDb.ObtenerConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        conexion.Open();
        return comando.ExecuteNonQuery() > 0;
    }

    // Parámetros comunes de Insertar y Actualizar
    private static void AgregarParametros(SqlCommand comando, Producto producto)
    {
        comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = producto.Nombre;
        comando.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 250).Value = (object?)producto.Descripcion ?? DBNull.Value;
        comando.Parameters.Add("@Categoria", SqlDbType.NVarChar, 50).Value = producto.Categoria;

        var precio = comando.Parameters.Add("@Precio", SqlDbType.Decimal);
        precio.Precision = 10;
        precio.Scale = 2;
        precio.Value = producto.Precio;

        comando.Parameters.Add("@Stock", SqlDbType.Int).Value = producto.Stock;
        comando.Parameters.Add("@Activo", SqlDbType.Bit).Value = producto.Activo;
    }

    // Convierte la fila actual del lector en un objeto Producto
    private static Producto MapearProducto(SqlDataReader lector)
    {
        return new Producto
        {
            Id = lector.GetInt32(lector.GetOrdinal("Id")),
            Nombre = lector.GetString(lector.GetOrdinal("Nombre")),
            Descripcion = lector.IsDBNull(lector.GetOrdinal("Descripcion"))
                ? null
                : lector.GetString(lector.GetOrdinal("Descripcion")),
            Categoria = lector.GetString(lector.GetOrdinal("Categoria")),
            Precio = lector.GetDecimal(lector.GetOrdinal("Precio")),
            Stock = lector.GetInt32(lector.GetOrdinal("Stock")),
            Activo = lector.GetBoolean(lector.GetOrdinal("Activo")),
            FechaRegistro = lector.GetDateTime(lector.GetOrdinal("FechaRegistro"))
        };
    }
}

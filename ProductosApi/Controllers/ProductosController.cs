using Microsoft.AspNetCore.Mvc;
using ProductosApi.Models;
using ProductosApi.Repositories;

namespace ProductosApi.Controllers;

// Endpoints REST para administrar productos.

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductosController : ControllerBase
{
    private readonly IProductoRepository _repositorio;

    public ProductosController(IProductoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Producto>), StatusCodes.Status200OK)]
    public ActionResult<List<Producto>> ObtenerTodos()
    {
        return Ok(_repositorio.ObtenerTodos());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Producto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Producto> ObtenerPorId(int id)
    {
        var producto = _repositorio.ObtenerPorId(id);

        if (producto is null)
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });

        return Ok(producto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Producto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Producto> Insertar([FromBody] Producto producto)
    {
        var nuevoId = _repositorio.Insertar(producto);
        var creado = _repositorio.ObtenerPorId(nuevoId);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoId }, creado);
    }


    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Actualizar(int id, [FromBody] Producto producto)
    {
        if (id != producto.Id)
            return BadRequest(new { mensaje = "El Id de la ruta no coincide con el Id del producto." });

        if (!_repositorio.Actualizar(producto))
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Eliminar(int id)
    {
        if (!_repositorio.Eliminar(id))
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });

        return NoContent();
    }
}

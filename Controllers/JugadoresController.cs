using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MarcadorFaseIIApi.Services;
using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Models.DTOs.Jugador;
using MarcadorFaseIIApi.Models.DTOs.Common;

namespace MarcadorFaseIIApi.Constrollers
{
    /// <summary>
    /// Gestión de jugadores: listado, detalle, paginación y CRUD (con autorización en entorno no-DEBUG).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class JugadoresController : ControllerBase
    {
        private readonly JugadorService _service;

        /// <summary>
        /// Crea el controlador de jugadores con su servicio de dominio.
        /// </summary>
        /// <param name="service">Servicio de jugadores.</param>
        public JugadoresController(JugadorService service)
        {
            _service = service;
        }

        // ---------- GETs ----------

        /// <summary>
        /// Lista jugadores con filtros opcionales.
        /// </summary>
        /// <param name="search">Texto de búsqueda.</param>
        /// <param name="equipoNombre">Nombre del equipo.</param>
        /// <param name="equipoId">Id del equipo.</param>
        /// <param name="posicion">Posición del jugador.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>200 con colección de <see cref="JugadorResponseDto"/>.</returns>
        // GET api/jugadores?search=&equipoNombre=&equipoId=&posicion=
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JugadorResponseDto>>> Get(
            [FromQuery] string? search,
            [FromQuery] string? equipoNombre,
            [FromQuery] int? equipoId,
            [FromQuery] string? posicion,
            CancellationToken ct)
        {
            var list = await _service.GetListAsync(search, equipoNombre, equipoId, posicion, ct);
            return Ok(list.Select(ToDto));
        }

        /// <summary>
        /// Obtiene un jugador por id.
        /// </summary>
        /// <param name="id">Id del jugador.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>200 con jugador; 404 si no existe.</returns>
        // GET api/jugadores/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<JugadorResponseDto>> GetById(int id, CancellationToken ct)
        {
            var j = await _service.GetByIdAsync(id, ct);
            if (j is null) return NotFound();
            return Ok(ToDto(j));
        }

        /// <summary>
        /// Devuelve jugadores paginados con filtros y ordenamiento.
        /// </summary>
        /// <param name="page">Página (>=1).</param>
        /// <param name="pageSize">Tamaño de página.</param>
        /// <param name="search">Texto de búsqueda.</param>
        /// <param name="equipoNombre">Nombre del equipo.</param>
        /// <param name="equipoId">Id del equipo.</param>
        /// <param name="posicion">Posición del jugador.</param>
        /// <param name="sortBy">Campo de ordenamiento.</param>
        /// <param name="sortDir">Dirección: <c>asc</c> / <c>desc</c>.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>200 con <see cref="PagedResult{T}"/> de <see cref="JugadorResponseDto"/>.</returns>
        // GET api/jugadores/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<JugadorResponseDto>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? equipoNombre = null,
            [FromQuery] int? equipoId = null,
            [FromQuery] string? posicion = null,
            [FromQuery] string? sortBy = "nombre",
            [FromQuery] string sortDir = "asc",
            CancellationToken ct = default)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            var asc = (sortDir ?? "asc").Equals("asc", StringComparison.OrdinalIgnoreCase);

            var (items, total) = await _service.GetPagedAsync(search, equipoNombre, equipoId, posicion, sortBy, asc, page, pageSize, ct);
            var dtos = items.Select(ToDto).ToList();
            return Ok(new PagedResult<JugadorResponseDto>(dtos, total, page, pageSize));
        }

        // ---------- Mutaciones (auth desactivada en DEBUG) ----------

        /// <summary>
        /// Crea un jugador.
        /// </summary>
        /// <param name="dto">DTO con datos del jugador.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>201 con jugador creado; 409 si ya existe uno con el mismo nombre (según validación de servicio).</returns>
#if DEBUG
        [AllowAnonymous]
#else
        [Authorize(Roles = "Admin")]
#endif
        [HttpPost]
        public async Task<ActionResult<JugadorResponseDto>> Create([FromBody] JugadorCreateDto dto, CancellationToken ct)
        {
            try
            {
                var j = await _service.CreateAsync(dto.Nombre, dto.EquipoId, dto.Posicion, ct);
                return CreatedAtAction(nameof(GetById), new { id = j.Id }, ToDto(j));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("existe un jugador", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un jugador existente.
        /// </summary>
        /// <param name="id">Id del jugador.</param>
        /// <param name="dto">DTO con datos a actualizar.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>200 con jugador; 404 si no existe; 409 si conflicto por duplicidad (según servicio).</returns>
#if DEBUG
        [AllowAnonymous]
#else
        [Authorize(Roles = "Admin")]
#endif
        [HttpPut("{id:int}")]
        public async Task<ActionResult<JugadorResponseDto>> Update(int id, [FromBody] JugadorUpdateDto dto, CancellationToken ct)
        {
            try
            {
                var j = await _service.UpdateAsync(id, dto.Nombre, dto.EquipoId, dto.Posicion, ct);
                if (j is null) return NotFound();
                return Ok(ToDto(j));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("existe un jugador", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un jugador por id.
        /// </summary>
        /// <param name="id">Id del jugador.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>204 si se elimina; 404 si no existe.</returns>
#if DEBUG
        [AllowAnonymous]
#else
        [Authorize(Roles = "Admin")]
#endif
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok) return NotFound();
            return NoContent();
        }

        // ---------- Helper ----------
        /// <summary>
        /// Proyección de <see cref="Jugador"/> a <see cref="JugadorResponseDto"/>.
        /// </summary>
        private static JugadorResponseDto ToDto(Jugador j) => new()
        {
            Id = j.Id,
            Nombre = j.Nombre,
            Puntos = j.Puntos,
            Faltas = j.Faltas,
            Posicion = j.Posicion,
            EquipoId = j.EquipoId,
            EquipoNombre = j.Equipo?.Nombre ?? string.Empty
        };
    }
}

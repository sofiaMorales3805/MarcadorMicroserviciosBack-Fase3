using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MarcadorFaseIIApi.Services;
using MarcadorFaseIIApi.Models;
using MarcadorFaseIIApi.Models.DTOs.Jugador;
using MarcadorFaseIIApi.Models.DTOs.Common;
using MarcadorFaseIIApi.Data;

namespace MarcadorFaseIIApi.Controllers
{
    /// <summary>
    /// Gestión de jugadores: listado, detalle y paginado con filtros.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class JugadoresController : ControllerBase
    {
        private readonly JugadorService _service;
	private readonly MarcadorDbContext _db;

	public JugadoresController(JugadorService service, MarcadorDbContext db)
        {
           _service = service;
           _db = db;
        }
        // ---------------------------------------------------------------------
        // GET api/jugadores
        // ---------------------------------------------------------------------
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<JugadorResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<JugadorResponseDto>>> Get(
            [FromQuery] string? search,
            [FromQuery] string? equipoNombre,
            [FromQuery] int? equipoId,
            [FromQuery] string? posicion,
            CancellationToken ct)
        {
            var jugadores = await _service.GetListAsync(search, equipoNombre, equipoId, posicion, ct);
            return Ok(jugadores.Select(ToDto));
        }

        // ---------------------------------------------------------------------
        // GET api/jugadores/{id}
        // ---------------------------------------------------------------------
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(JugadorResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JugadorResponseDto>> GetById(int id, CancellationToken ct)
        {
            var j = await _service.GetByIdAsync(id, ct);
            if (j is null) return NotFound();
            return Ok(ToDto(j));
        }

        // ---------------------------------------------------------------------
        // GET api/jugadores/
        // ---------------------------------------------------------------------
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<JugadorResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<JugadorResponseDto>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? equipoNombre = null,
            [FromQuery] int? equipoId = null,
            [FromQuery] string? posicion = null,
            CancellationToken ct = default)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            
            (IEnumerable<Jugador> items, int total) = await _service.GetPagedAsync(
                search, equipoNombre, equipoId, posicion,
                sortBy: "nombre", asc: true,
                page: page, pageSize: pageSize, ct: ct);

            var dtos = items.Select(ToDto).ToList();
            return Ok(new PagedResult<JugadorResponseDto>(dtos, total, page, pageSize));
        }
       
	// --------------------------------------------------------------
	// POST api/jugadores
	// --------------------------------------------------------------
	[HttpPost]
        [AllowAnonymous]
	[ProducesResponseType(typeof(JugadorResponseDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<JugadorResponseDto>> Create([FromBody] JugadorCreateDto dto, CancellationToken ct)
	{
	
	if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.EquipoId <= 0)
		return BadRequest("Nombre y EquipoId son obligatorios.");

	var existeEquipo = await _db.Equipos.AnyAsync(e => e.Id == dto.EquipoId, ct);
	if (!existeEquipo)
	  return BadRequest("El equipo no existe.");
	
	var entity = new Jugador
        {   
	    Nombre = dto.Nombre.Trim(),
            EquipoId = dto.EquipoId,
	    Posicion = dto.Posicion,
            numero = dto.Numero?.ToString() ?? string.Empty,
	    edad = dto.Edad ?? 0,
            estatura = dto.Estatura ?? 0,
	    nacionalidad = dto.Nacionalidad ?? string.Empty,
            // Puntos/Faltas arrancan en 0
		    Puntos = 0,
	            Faltas = 0
	};

	        _db.Jugadores.Add(entity);
	        await _db.SaveChangesAsync(ct);

	        entity.Equipo = await _db.Equipos.FindAsync(new object?[] { entity.EquipoId }, ct);
	        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
	    }
        // ---------------------------------------------------------------------
        // Mapeando entidad -> DTO
        // ---------------------------------------------------------------------
        private static JugadorResponseDto ToDto(Jugador j)
        {
            
            int? numero = null;
            if (!string.IsNullOrWhiteSpace(j.numero) && int.TryParse(j.numero, out var n))
                numero = n;

            return new JugadorResponseDto
            {
                Id = j.Id,
                Nombre = j.Nombre,
                Puntos = j.Puntos,
                Faltas = j.Faltas,
                Posicion = j.Posicion,
                EquipoId = j.EquipoId,
                EquipoNombre = j.Equipo?.Nombre ?? string.Empty,

                Numero = numero,
                Edad = j.edad,
                Estatura = j.estatura,
                Nacionalidad = string.IsNullOrWhiteSpace(j.nacionalidad) ? null : j.nacionalidad
            };
        }
    }
}

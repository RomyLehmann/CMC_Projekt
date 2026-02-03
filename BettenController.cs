using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMC_Backend.Data;
using CMC_Backend.Models;

namespace CMC_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BettenController : ControllerBase
    {
        private readonly BettenDbContext _context;

        public BettenController(BettenDbContext context)
        {
            _context = context;
        }

        // GET: api/Betten
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bett>>> GetAlleBetten()
        {
            return await _context.Betten.OrderBy(b => b.ZimmerNummer).ToListAsync();
        }

        // GET: api/Betten/B001
        [HttpGet("{bettNummer}")]
        public async Task<ActionResult<Bett>> GetBett(string bettNummer)
        {
            var bett = await _context.Betten.FindAsync(bettNummer);

            if (bett == null)
            {
                return NotFound(new { message = $"Bett {bettNummer} nicht gefunden" });
            }

            return bett;
        }

        // GET: api/Betten/Zimmer/5
        [HttpGet("Zimmer/{zimmerNummer}")]
        public async Task<ActionResult<IEnumerable<Bett>>> GetBettenNachZimmer(int zimmerNummer)
        {
            var betten = await _context.Betten
                .Where(b => b.ZimmerNummer == zimmerNummer)
                .ToListAsync();

            if (!betten.Any())
            {
                return NotFound(new { message = $"Keine Betten in Zimmer {zimmerNummer} gefunden" });
            }

            return betten;
        }

        // PUT: api/Betten/B001
        [HttpPut("{bettNummer}")]
        public async Task<ActionResult<Bett>> UpdateBett(string bettNummer, [FromBody] BettUpdateDto updateDto)

        {
            var bett = await _context.Betten.FindAsync(bettNummer);
            if (bett == null)
            {
                return NotFound(new { message = $"Bett {bettNummer} nicht gefunden" });
            }

            // Update nur erlaubte Felder
            if (!string.IsNullOrEmpty(updateDto.Status))
            {
                if (updateDto.Status != "Frei" && updateDto.Status != "Belegt")
                {
                    return BadRequest(new { message = "Status muss 'Frei' oder 'Belegt' sein" });
                }
                bett.Status = updateDto.Status;
            }

            if (!string.IsNullOrEmpty(updateDto.Wartung))
            {
                if (updateDto.Wartung != "Sauber" && updateDto.Wartung != "Nicht sauber")
                {
                    return BadRequest(new { message = "Wartung muss 'Sauber' oder 'Nicht sauber' sein" });
                }
                bett.Wartung = updateDto.Wartung;
            }

            bett.LetztGeaendert = DateTime.UtcNow;

            try
            {
                _context.Entry(bett).State = EntityState.Modified; // ← NEU!
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Betten.AnyAsync(b => b.BettNummer == bettNummer))
                {
                    return NotFound();
                }
                throw;
            }

            return Ok(bett);
        }


        // POST: api/Betten
        [HttpPost]
        public async Task<ActionResult<Bett>> CreateBett([FromBody] Bett bett)
        {
            if (await _context.Betten.AnyAsync(b => b.BettNummer == bett.BettNummer))
            {
                return Conflict(new { message = $"Bett {bett.BettNummer} existiert bereits" });
            }

            bett.LetztGeaendert = DateTime.UtcNow;
            _context.Betten.Add(bett);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBett), new { bettNummer = bett.BettNummer }, bett);
        }

        // DELETE: api/Betten/B001
        [HttpDelete("{bettNummer}")]
        public async Task<IActionResult> DeleteBett(string bettNummer)
        {
            var bett = await _context.Betten.FindAsync(bettNummer);

            if (bett == null)
            {
                return NotFound(new { message = $"Bett {bettNummer} nicht gefunden" });
            }

            _context.Betten.Remove(bett);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTO für Updates
    public class BettUpdateDto
    {
        public string? Status { get; set; }
        public string? Wartung { get; set; }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Controllers
{
   public class ContosoUniversityController : Controller
   {
      private readonly ContosoUniversityContext _context;

      public ContosoUniversityController(ContosoUniversityContext context)
      {
         _context = context;
      }

      // GET: ContosoUniversity
      public async Task<IActionResult> Index(string movieGenre, string sortOrder, string searchString)
      {


         // Use LINQ to get list of genres.
         IQueryable<string> genreQuery = from m in _context.Movie
                                         orderby m.Genre
                                         select m.Genre;

         var ContosoUniversity = from m in _context.Movie
                                 select m;

         if (!string.IsNullOrEmpty(searchString))
         {
            ContosoUniversity = ContosoUniversity.Where(s => s.Title.Contains(searchString));
         }

         if (!string.IsNullOrEmpty(movieGenre))
         {
            ContosoUniversity = ContosoUniversity.Where(x => x.Genre == movieGenre);
         }

         switch (sortOrder)
         {
            case "ReleaseDate_Asc_Sort":
               ContosoUniversity = ContosoUniversity.OrderBy(m => m.ReleaseDate);
               break;
            case "ReleaseDate_Desc_Sort":
               ContosoUniversity = ContosoUniversity.OrderByDescending(s => s.ReleaseDate);
               break;
            default:
               ContosoUniversity = ContosoUniversity.OrderBy(s => s.Title);
               break;
         }

         var movieGenreVM = new MovieGenreViewModel
         {
            Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
            ContosoUniversity = await ContosoUniversity.ToListAsync()
         };

         movieGenreVM.SortDate = sortOrder == "ReleaseDate_Asc_Sort" ? "ReleaseDate_Desc_Sort" : "ReleaseDate_Asc_Sort";

         return View(movieGenreVM);
      }

      // GET: ContosoUniversity/Details/5
      public async Task<IActionResult> Details(int? id)
      {
         if (id == null)
         {
            return NotFound();
         }

         var movie = await _context.Movie
             .FirstOrDefaultAsync(m => m.Id == id);
         if (movie == null)
         {
            return NotFound();
         }

         return View(movie);
      }

      // GET: ContosoUniversity/Create
      public IActionResult Create()
      {
         return View();
      }

      // POST: ContosoUniversity/Create
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
      {
         if (ModelState.IsValid)
         {
            _context.Add(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
         }
         return View(movie);
      }

      // GET: ContosoUniversity/Edit/5
      public async Task<IActionResult> Edit(int? id)
      {
         if (id == null)
         {
            return NotFound();
         }

         var movie = await _context.Movie.FindAsync(id);
         if (movie == null)
         {
            return NotFound();
         }
         return View(movie);
      }

      // POST: ContosoUniversity/Edit/5
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
      {
         if (id != movie.Id)
         {
            return NotFound();
         }

         if (ModelState.IsValid)
         {
            try
            {
               _context.Update(movie);
               await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
               if (!MovieExists(movie.Id))
               {
                  return NotFound();
               }
               else
               {
                  throw;
               }
            }
            return RedirectToAction(nameof(Index));
         }
         return View(movie);
      }

      // GET: ContosoUniversity/Delete/5
      public async Task<IActionResult> Delete(int? id)
      {
         if (id == null)
         {
            return NotFound();
         }

         var movie = await _context.Movie
             .FirstOrDefaultAsync(m => m.Id == id);
         if (movie == null)
         {
            return NotFound();
         }

         return View(movie);
      }

      // POST: ContosoUniversity/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> DeleteConfirmed(int id)
      {
         var movie = await _context.Movie.FindAsync(id);
         _context.Movie.Remove(movie);
         await _context.SaveChangesAsync();
         return RedirectToAction(nameof(Index));
      }

      private bool MovieExists(int id)
      {
         return _context.Movie.Any(e => e.Id == id);
      }
   }
}

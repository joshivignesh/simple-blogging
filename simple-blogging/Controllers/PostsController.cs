using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using simple_blogging.Data;
using Microsoft.AspNetCore.Authorization;
using simple_blogging.DTO;
using Microsoft.AspNetCore.Identity;

namespace simple_blogging.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Posts
        /// <summary>
        /// Retrieves a specific product by unique id
        /// </summary>
        /// <remarks>Awesomeness!</remarks>
        /// <response code="404">Product not found</response>
        /// <response code="500">Oops! Can't lookup your product right now</response>
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Post.ToListAsync());
        }

        // GET: Posts/Details/5
        /// <summary>
        /// Retrieves a specific product by unique id
        /// </summary>
        /// <remarks>Awesomeness!</remarks>
        /// <param name="id" example="5">The post id</param>
        /// <response code="200">Product retrieved</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Oops! Can't lookup your product right now</response>
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.Include(au=>au.Author).Include(pc=>pc.PostCategories).ThenInclude(c=>c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            var categoryList = _context.Category.ToList();
            CreatePostDto createPostDto = new CreatePostDto();
            createPostDto.Categories = categoryList.Select(c=> new SelectListItem()
            {
                Text = c.Title, Value = c.Id.ToString()
            }).ToList();
            return View(createPostDto);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostDto createPostDto)
        {
            if (ModelState.IsValid)
            {
                var selectedCategory = createPostDto.Categories.Where(sc => sc.Selected).Select(scv => scv.Value).Select(int.Parse).ToList();
                var post = new Post
                {
                    Title = createPostDto.Title,
                    Description = createPostDto.Description,
                    PublishedDate = createPostDto.PublishedDate,
                    ApplicationUserId = Guid.Parse(_userManager.GetUserId(HttpContext.User)),
                };
                foreach(var categoryItem in selectedCategory)
                {
                    post.PostCategories.Add(new PostCategory()
                    {
                        Post = post,
                        CategoryId = categoryItem,
                    });
                }
               
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,PublishedDate,ApplicationUserId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            if (post != null)
            {
                _context.Post.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}

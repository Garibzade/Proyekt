using JobFind.DAL;
using JobFind.Models;
using JobFind.ViewModel.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Protocol;

namespace JobFind.Areas.Admin.Controllers
{
        [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class CategoryController(JobFindContext _context,IWebHostEnvironment environment) : Controller
    {
        public IWebHostEnvironment Environment { get; } = environment;



        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.Select(c => new GetCategoryAdminVM
            {
                Id = c.Id,
                Name = c.Name,
                Icon = c.Icon,
                JobCount = c.JobCount,


            }).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Create(CreateCategoryVM CreateVM)
        {
           if (CreateVM.IconFile != null) 
            {
                ModelState.AddModelError("IconFile", "The Icon file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(CreateVM);
            }

            string NewFileName= DateTime.Now.ToString("yyyyMMddHHmmssfff");
            NewFileName += Path.GetExtension(CreateVM.IconFile!.FileName);

            string IconFullPath=Environment.WebRootPath + "/category/"+ NewFileName;
            using (var stream=System.IO.File.Create(IconFullPath))
            {
                CreateVM.IconFile.CopyTo(stream);
            }
            Category category = new Category()
            {
                Icon = NewFileName,
                Name = NewFileName,
                JobCount=1,
                
                
            };
            _context.Categories.Add(category);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id<=0)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(Id);    
            if (category == null)
            {
                return BadRequest();
            }
            var EditVM = new EditCategoryVM()
            {
                Name = category.Name,
                IconFile=null,
               
                

            };


            return View(EditVM);
        }
        [HttpPost]  
        public async Task<IActionResult> Edit(int? id,EditCategoryVM editCategoryVM)
        {
            if (id<=0)
            {
                return BadRequest();
            }
            var Category  = await _context.Categories.FindAsync(id);
            if (Category == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid) 
            {
                return View(editCategoryVM);
            }
            string newFileName = Category.Icon;
            if (editCategoryVM.IconFile!=null)
            {
                newFileName= DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(editCategoryVM.IconFile.FileName);
                string iconFullPath = Environment.WebRootPath + "/category/" + newFileName;
                using (var stream = System.IO.File.Create(iconFullPath))
                {
                    editCategoryVM.IconFile.CopyTo(stream);
                }
                string oldImage= Environment.WebRootPath + "/category/" + newFileName;
                System.IO.File.Delete(oldImage);

            }
            Category.Name = editCategoryVM.Name;
            Category.Icon = newFileName;  
            
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id<=0)
            { return BadRequest(); }
            var category=_context.Categories.Find(id);
            if (category == null)
            { return NotFound(); }
            string IconFullPath=Environment.WebRootPath + "/category/" + category.Icon;   
            System.IO.File.Delete(IconFullPath);
            
             _context.Remove(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }



    }
}
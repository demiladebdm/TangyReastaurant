﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using Tangy.Data;
using Tangy.Models;
using Tangy.Models.MenuItemViewModels;
//using Tangy.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Tangy.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;


        [BindProperty]
        public MenuItemViewModel MenuItemVM
        {
            get; set;
        }
        public MenuItemsController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category.ToList(),
                MenuItem = new Models.MenuItem()
            };
        }


        //GET : MenuItems
        public async Task<IActionResult> Index()
        {
            var menuItems = _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory);
            return View(await menuItems.ToListAsync());
        }


        //GET : MenuItems Create
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        //POST : MenuItem Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            //if (!ModelState.IsValid)
            //{
            //    return View(MenuItemVM);
            //}

            //_db.MenuItem.Add(MenuItemVM.MenuItem);
            //await _db.SaveChangesAsync();

            //Image Being Saved
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = _db.MenuItem.Find(MenuItemVM.MenuItem.Id);

            //if (files[0] != null && files[0].Length > 0)
            //{
            //    //When user Uploads an Image
            //    var uploads = Path.Combine(webRootPath, "images");
            //    var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

            //    using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
            //    {
            //        files[0].CopyTo(filestream);
            //    }
            //    menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;

            //}
            //else
            //{
            //    //When user does now Upload any Image
            //    var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
            //    System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".jpg");
            //    menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".jpg";

            //}

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public JsonResult GetSubCategory(int CategoryId)
        {
            List<SubCategory> subCategoryList = new List<SubCategory>();

            subCategoryList = (from subCategory in _db.SubCategory
                               where subCategory.CategoryId == CategoryId
                               select subCategory).ToList();

            return Json(new SelectList(subCategoryList, "Id", "Name"));
        }


        //GET : Edit MenuItem
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToList();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }


        //POST : Edit MenuItems
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (id != MenuItemVM.MenuItem.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        string webRootPath = _hostingEnvironment.WebRootPath;
            //        var files = HttpContext.Request.Form.Files;
            //        var menuItemFromDb = _db.MenuItem.Where(m => m.Id == MenuItemVM.MenuItem.Id).FirstOrDefault();

            //        if (files[0].Length > 0 && files[0] != null)
            //        {
            //            //If user uploads a new image
            //            var uploads = Path.Combine(webRootPath, "images");

            //            var extension_New = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));
            //            var extension_Old = menuItemFromDb.Image.Substring(menuItemFromDb.Image.LastIndexOf("."), menuItemFromDb.Image.Length - menuItemFromDb.Image.LastIndexOf("."));

            //            if(System.IO.File.Exists(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_Old)))
            //            {
            //                System.IO.File.Delete(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_Old));
            //            }
            //            using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_New), FileMode.Create))
            //            {
            //                files[0].CopyTo(filestream);
            //            }
            //            MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_New;
            //        }

            //        if(MenuItemVM.MenuItem.Image != null)
            //        {
            //            menuItemFromDb.Image = MenuItemVM.MenuItem.Image;
            //        }

            var menuItemFromDb = _db.MenuItem.Where(m => m.Id == MenuItemVM.MenuItem.Id).FirstOrDefault();
            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

            await _db.SaveChangesAsync();
            //}
            //catch(Exception ex)
            //{

            //}

            MenuItemVM.SubCategory = _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToList();
            return RedirectToAction(nameof(Index));
            //}
            //MenuItemVM.SubCategory = _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToList();
            //return View(MenuItemVM);
        }


        //GET : Details MenuItem
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }


        //GET : Delete MenuItem
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //PST : Delete MenuItem
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //string webRootPath = _hostingEnvironment.WebRootPath;
            //MenuItem menuItem = await _db.MenuItem.FindAsync(id);

            //if (menuItem != null)
            //{
            //    var uploads = Path.Combine(webRootPath, "images");
            //    var extension = menuItem.Image.Substring(menuItem.Image.LastIndexOf("."), menuItem.Image.Length - menuItem.Image.LastIndexOf("."));

            //    var imagePath = Path.Combine(uploads, menuItem.Id + extension);
            //    if (System.IO.File.Exists(imagePath))
            //    {
            //        System.IO.File.Delete(imagePath);
            //    }
            //    _db.MenuItem.Remove(menuItem);
            //    await _db.SaveChangesAsync();
            //}

            MenuItem menuItem = await _db.MenuItem.FindAsync(id);
            _db.MenuItem.Remove(menuItem);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
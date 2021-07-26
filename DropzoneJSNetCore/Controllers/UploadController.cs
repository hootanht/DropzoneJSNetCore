using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DropzoneJSNetCore.Controllers
{
    public class UploadController : Controller
    {
        #region Properties
        public IWebHostEnvironment _env { get; set; }
        #endregion

        #region Constructor
        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }
        #endregion


        #region UploadImage
        /// <summary>
        /// This method upload images file to the server
        /// </summary>
        /// <param name="files">images files for upload</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(ICollection<IFormFile> files)
        {
            var imagesList = new List<string>();
            if (!Directory.Exists(Path.Combine(_env.ContentRootPath, "wwwroot", "images")))
            {
                Directory.CreateDirectory(Path.Combine(_env.ContentRootPath, "wwwroot", "images"));
            }
            string uploads = Path.Combine(_env.ContentRootPath, "wwwroot", "images");
            foreach (IFormFile image in files)
            {
                if (image.Length > 0)
                {
                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    imagesList.Add(imageName);
                    string savePath = Path.Combine(uploads, imageName);
                    using (FileStream stream = new FileStream(savePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                }
            }

            return Ok(JsonSerializer.Serialize(imagesList));
        }
        #endregion

        #region DeleteUploadedImage
        /// <summary>
        /// This method delete image file from the server
        /// </summary>
        /// <param name="fileName">name of image file</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUploadedImage(string fileName)
        {
            if (System.IO.File.Exists(Path.Combine(_env.ContentRootPath, "wwwroot", "images", fileName)))
            {
                System.IO.File.Delete((Path.Combine(_env.ContentRootPath, "wwwroot", "images", fileName)));
                return Ok();
            }
            return NotFound();
        }
        #endregion
    }
}

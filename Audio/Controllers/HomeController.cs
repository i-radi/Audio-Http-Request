using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Audio.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Audio.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment host;
        public HomeController(IHostingEnvironment hostEnv)
        {
            host = hostEnv;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Models.Audio audio)
        {
            if (ModelState.IsValid)
            {
                UploadFile(audio);

                if (audio.File != null)
                {
                    HttpClient client = new HttpClient();
                    var requestContent = new MultipartFormDataContent();
                    var fileContent = new StreamContent(audio.File.OpenReadStream());
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("files")
                    {
                        Name = "\"file\"",
                        FileName = $"\"{audio.File.FileName}\""
                    };
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(audio.File.ContentType);
                    requestContent.Add(fileContent);
                    HttpResponseMessage response = await client.PostAsync("http://192.168.1.200:5000/predict?with_statistics_details=true", fileContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.Content;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(audio);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        void UploadFile(Models.Audio audio)
        {
            if (audio.File != null)
            {
                string uploadsFolder = Path.Combine(host.WebRootPath, "Song");
                string uniqueFileName = Guid.NewGuid() + ".mp3";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    audio.File.CopyTo(fileStream);
                }
            }
        }
    }
}

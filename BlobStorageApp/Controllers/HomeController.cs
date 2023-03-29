using Azure;
using AzureBlobStorage.Dtos;
using BlobStorageApp.Models;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;

namespace BlobStorageApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HttpClient client = new HttpClient();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var api = await client.GetStringAsync(Constant.FileUrl);
                var model = JsonConvert.DeserializeObject<List<BlobDto>>(api);
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }

            return View();

        }

        public async Task<IActionResult> Upload()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                content.Add(fileContent, "file", file.FileName);
                var respone = await client.PostAsync(Constant.FileUrl, content);
                if (respone.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ViewBag.msg = "Upload successfully!";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Download(string name)
        {
            try
            {
                var response = await client.GetAsync(Constant.FileUrl + "/filename?filename=" + name);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    var contentType = response.Content.Headers.ContentType.ToString();
                    return File(contentStream, contentType, name);
                }


                ViewBag.msg = "Upload successfully!";
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpGet]
        public async Task<IActionResult> Delete(string name)
        {
            try
            {
                var response = await client.DeleteAsync(Constant.FileUrl + "/filename?filename=" + name);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    ViewBag.msg = "Delete successfully!";
                    //khuc nay co return ko a

                }
                else
                {
                    ViewBag.msg = "Delete Failed!";
                }
               
                return RedirectToAction("Index", "Home");


            }
            catch (Exception ex)
            {
                throw;
            }

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
    }
}
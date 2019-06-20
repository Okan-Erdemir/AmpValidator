using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Text;

namespace AmpValidator.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CheckAmpValidator(string url)
        {
            Console.WriteLine("Start!");

            string htmlContent = string.Empty;
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                htmlContent = client.DownloadString(url);
            }

            string result = AmpTools.ValidateHtmlForAmp(htmlContent);
            Console.WriteLine(result);

            var ResultConvert = JsonConvert.DeserializeObject<RootObject>(result);

            Console.WriteLine($"Result: {ResultConvert.valid}");
            return RedirectToAction("Index", new { valid = ResultConvert.valid });
        }


    }
}

public static class AmpTools
{
    public static string ValidateHtmlForAmp(string htmlContent)
    {
        RestClient client = new RestClient("https://amp.cloudflare.com");
        RestRequest request = new RestRequest("/q", Method.POST);

        request.AddParameter("text/html; charset=UTF-8", htmlContent, ParameterType.RequestBody);
        request.RequestFormat = DataFormat.None;

        try
        {
            IRestResponse restResponse = client.Execute(request);

            return restResponse.Content;
        }
        catch (Exception exception)
        {
            // Log
            Console.WriteLine(exception.Message);
            return string.Empty;
        }
    }
}

public class RootObject
{
    public string version { get; set; }
    public string source { get; set; }
    public bool valid { get; set; }
}
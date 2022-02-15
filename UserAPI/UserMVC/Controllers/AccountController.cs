using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UserMVC.ViewModels;

namespace UserMVC.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Login(string username, string password)
        {
            LoginViewModel loginVM = new LoginViewModel
            {
                UserName = username,
                Password = password
            };

            StringContent requestContent = new StringContent(JsonConvert.SerializeObject(loginVM), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = null;
            string endpoint = "https://localhost:44300/admin/api/accounts/login/";

            using (HttpClient client = new HttpClient())
            {
                responseMessage = await client.PostAsync(endpoint, requestContent);
            }

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await responseMessage.Content.ReadAsStringAsync();

                LoginResponseViewModel responseVM = JsonConvert.DeserializeObject<LoginResponseViewModel>(content);
                Response.Cookies.Append("AuthToken", responseVM.Token);

                return RedirectToAction("index", "category");
            }


            return NotFound();
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace MvcDriverVerify.Controllers
{
    public class VerifyDriverController : Controller
    {
        // 
        // GET: /VerifyDriver/

        public string Index()
        {
            return "This is my default action...";
        }

        // 
        // GET: /VerifyDriver/Welcome/ 

        public string Welcome()
        {
            return "This is the Welcome action method...";
        }        
    }
}
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JsonPath.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetData(string Jsondata, string Value)
        {
            try
            {
                string jsonData = Jsondata;
                string searchValue = Value; // Replace this with the value you want to search for

                JObject jsonObject = JObject.Parse(jsonData);

                List<string> foundParentNames = new List<string>();

                foreach (var property in jsonObject.Properties())
                {
                    if (property.Value.Type == JTokenType.Array)
                    {
                        var jsonArray = (JArray)property.Value;

                        for (int index = 0; index < jsonArray.Count; index++)
                        {
                            JObject colObject = (JObject)jsonArray[index];

                            var matchingProperties = colObject.Properties()
                                .Where(p => p.Value.ToString() == searchValue)
                                .Select(p => p.Name);

                            foreach (string propertyName in matchingProperties)
                            {
                                string parentName = property.Name;
                                string indexFormat = $"{parentName}[{index}].{propertyName}";
                                foundParentNames.Add(indexFormat);
                            }
                        }
                    }
                }

                if (foundParentNames.Count > 0)
                {
                    // Construct a JSON object to return the results
                    var result = new
                    {
                        Found = true,
                        ParentNames = foundParentNames
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Return a JSON object indicating that the value was not found
                    var result = new
                    {
                        Found = false,
                        Message = "Value not found in JSON data."
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
                var errorResult = new
                {
                    Found = false,
                    Message = "An error occurred: " + ex.Message
                };

                return Json(errorResult, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MSTUApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckController : Controller
    {
        [Route("group")]
        public string group(string name)
        {
            try
            {
                if (name != null)
                {
                    return JsonConvert.SerializeObject(new SendedCheckedGroup() { Status = 0, Data = new CheckedGroup() { GroupName = name, isRight = GroupIsRight(name) } }) ;
                }
                else
                {
                    return JsonConvert.SerializeObject(new SendedCheckedGroup() { Status = -1, Data = null, Message = "Name must not be empty" });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new SendedCheckedGroup() { Status = -2, Data = null, Message = $"Error: {ex.Message}" });
            }
        }

        private List<Group> GroupParser()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var data = BrowsingContext.New(config).OpenAsync("https://students.bmstu.ru/schedule/list").GetAwaiter().GetResult();
            var Tables = data.QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("btn-group"));
            List<List<IElement>> elements = new List<List<IElement>>();
            foreach (var item in Tables)
            {
                elements.Add(item.QuerySelectorAll("a").ToList());
            }
            List<Group> temp = new List<Group>();
            foreach (var item in elements)
            {
                foreach (var item2 in item)
                {
                    temp.Add(new Group() { GroupName = item2.TextContent.TrimStart().TrimEnd(), Url = "https://students.bmstu.ru" + item2.Attributes[0].Value });
                }
            }
            return temp;
        }

        private bool GroupIsRight(string name)
        {
            var data = GroupParser();
            foreach (var item in data)
            {
                if (item.GroupName == name)
                    return true;
            }
            return false;
        }
    }
}
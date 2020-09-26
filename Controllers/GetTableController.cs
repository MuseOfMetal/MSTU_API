using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.Mvc;
using MSTUAPi.Models;
using Newtonsoft.Json;

namespace MSTUApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetController : Controller
    {
        [Route("timeTable")]
        public string getTimeTable(string url, string group)
        {
            try
            {
                if (url != null)
                {
                    var data = TimeTableParser(url);
                    if (data.Count == 0)
                        return JsonConvert.SerializeObject(new SendedTimeTable() { Status = -1, Data = null, Message = "Wrong url" });
                    else
                        return JsonConvert.SerializeObject(new SendedTimeTable() { Status = 0, Data = data });
                }
                else if (group != null)
                {
                    string _url = FindUrlGroup(group);
                    if (string.IsNullOrEmpty(_url))
                        return JsonConvert.SerializeObject(new SendedTimeTable() { Status = -1, Data = null, Message = "Wrong group name" });
                    else
                        return JsonConvert.SerializeObject(new SendedTimeTable() { Status = 0, Data = TimeTableParser(FindUrlGroup(group)) });
                }
                else
                {
                    return JsonConvert.SerializeObject(new SendedTimeTable() { Status = -1, Data = null, Message = "url or group are must not be empty" }); ;
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new SendedTimeTable() { Status = -2, Data = null, Message = $"Error: {ex.Message}" }); ;
            }
            
        }

        [Route("groupList")]
        public string getGroupList()
        {
            try
            {
                return JsonConvert.SerializeObject(new SendedGroupList() { Status = 0, Data = GroupParser() });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new SendedGroupList() { Status = -2, Data = null, Message = ex.Message}) ;
            }
        }

        [Route("week")]
        public string week(string group)
        {
            string _url = FindUrlGroup(group);
            if (_url == null)
                return JsonConvert.SerializeObject(new SendedWeek() { Status = -1, Data = null, Message = "Group invalid"});
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var data = BrowsingContext.New(config).OpenAsync(_url).GetAwaiter().GetResult();
                var Data = data.QuerySelectorAll("i").Where(item => item.TextContent.Contains("числитель") || item.TextContent.Contains("знаменатель"));
                return JsonConvert.SerializeObject(new SendedWeek() { Status = 0, Data = Data.ToList()[0].TextContent });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new SendedWeek() { Status = -2, Data = null, Message = ex.Message });
            }

        }

        private List<LectionDB> TimeTableParser(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var c = Configuration.Default;
            var data = BrowsingContext.New(config).OpenAsync(url).GetAwaiter().GetResult();
            var Tables = data.QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("col-md-6 hidden-xs"));
            List<List<IElement>> elements = new List<List<IElement>>();
            for (int i = 0; i < Tables.ToList().Count; i++)
            {
                elements.Add(new HtmlParser().Parse(Tables.ToList()[i].OuterHtml).QuerySelectorAll("tr").ToList());
            }
            for (int i = 0; i < elements.Count; i++)
            {
                var current = elements[i];
                for (int j = 0; j < current.Count; j++)
                {
                    var datalist = current[j].QuerySelectorAll("td").ToList();
                    for (int k = 0; k < datalist.Count; k++)
                    {
                        if (string.IsNullOrWhiteSpace(datalist[k].TextContent))
                            datalist[k].TextContent = "There nothing!!!!!!";
                    }
                }
            }
            List<List<List<string>>> sorted = new List<List<List<string>>>();
            for (int i = 0; i < elements.Count; i++)
            {
                var current = elements[i];
                List<List<string>> temparr1 = new List<List<string>>();
                for (int j = 0; j < current.Count; j++)
                {
                    var datalist = current[j].QuerySelectorAll("td").ToList();
                    List<string> temparr2 = new List<string>();
                    bool inProgress = false;
                    bool end = false;
                    for (int k = 0; k < datalist.Count; k++)
                    {
                        if (end)
                        {
                            inProgress = false;
                            end = false;
                        }
                        if (inProgress)
                        {
                            temparr2.Add(datalist[k].TextContent);
                        }
                        else if (IsTime(datalist[k].TextContent))
                        {
                            inProgress = true;
                            temparr2.Add(datalist[k].TextContent);
                        }
                    }
                    temparr1.Add(temparr2);
                    temparr2 = new List<string>();
                }
                sorted.Add(temparr1);
                temparr1 = new List<List<string>>();
            }

            List<LectionDB> lections = new List<LectionDB>();
            int ii = 0;
            foreach (var item in sorted)
            {
                LectionDB tempDB = new LectionDB();
                tempDB.DayOfTheWeek = Base.DaysFull[ii];
                foreach (var item2 in item)
                {
                    Lection temp = new Lection();

                    if (item2.Count == 2)
                    {
                        temp.Time = item2[0];
                        temp.Both = item2[1];
                    }
                    else if (item2.Count == 3)
                    {
                        temp.Time = item2[0];
                        if (item2[1] == "There nothing!!!!!!" && item2[2] != "There nothing!!!!!!")
                        {
                            temp.Denominator = item2[2];
                        }
                        else if (item2[2] == "There nothing!!!!!!" && item2[1] != "There nothing!!!!!!")
                        {
                            temp.Numerator = item2[1];
                        }
                        else if (item2[1] != "There nothing!!!!!!" && item2[2] != "There nothing!!!!!!")
                        {
                            temp.Numerator = item2[1];
                            temp.Denominator = item2[2];
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    tempDB.Lections.Add(temp);
                    temp = new Lection();
                }
                lections.Add(tempDB);
                tempDB = new LectionDB();
                ii++;
            }
            return lections;
        }
        private string FindUrlGroup(string group)
        {
            var groups = GroupParser();
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].GroupName.ToLower() == group.ToLower())
                    return groups[i].Url;
            }
            return null;
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
        private bool IsTime(string Data)
        {
            for (int i = 0; i < Base.Times.Count; i++)
            {
                if (Base.Times.Contains(Data.TrimStart().TrimEnd()))
                    return true;
            }
            return false;
        }
    }
}
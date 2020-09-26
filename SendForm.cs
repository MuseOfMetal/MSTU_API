using MSTUAPi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSTUApi
{
    class SendedTimeTable
    {
        public int Status;
        public string Message;
        public List<LectionDB> Data = new List<LectionDB>();
    }

    class SendedGroupList
    {
        public int Status;
        public string Message;
        public List<Group> Data = new List<Group>();
    }

    class SendedWeek
    {
        public int Status;
        public string Message;
        public string Data;
    }
    class SendedCheckedGroup
    {
        public int Status;
        public string Message;
        public CheckedGroup Data;
    }
}

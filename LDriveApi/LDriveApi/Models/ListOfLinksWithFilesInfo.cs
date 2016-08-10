using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LDriveWebApi.Models
{
    public class ListOfLinksWithFilesInfo
    {
        public string Key { get; set; }
        public int Less10Mb { get; set; }
        public int From10To50Mb { get; set; }
        public int More100Mb { get; set; }
        public string CurrentPath { get; set; }
        public List<LinkModel> Links { get; set; }
    }
}
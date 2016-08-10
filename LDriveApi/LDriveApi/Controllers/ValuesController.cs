using LDriveWebApi.Helpers;
using LDriveWebApi.Hubs;
using LDriveWebApi.Models;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;

namespace LDriveWebApi.Controllers
{
    public class ValuesController : ApiController
    {
        public ListOfLinksWithFilesInfo Post(DataModel data)
        {
            return LDriveHelper.GetDirectoriInfo(data.Value, data.Key);
        }      
    }
}

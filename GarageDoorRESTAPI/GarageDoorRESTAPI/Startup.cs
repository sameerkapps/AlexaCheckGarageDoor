using GarageDoorRESTAPI.Storage;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GarageDoorRESTAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            QueueUtil.EnsureCreated();
            app.MapSignalR();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using wojilu.Web.Mvc;
using wojilu.Sample.Domain;

namespace wojilu.Web.Controller {

    public class MainController : ControllerBase {

        public void Index() {
            set( "time", DateTime.Now );
        }


    }

}

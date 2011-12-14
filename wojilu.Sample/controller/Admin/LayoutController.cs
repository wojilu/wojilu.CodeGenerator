
using System;
using System.Collections.Generic;
using System.Text;
using wojilu.Web.Mvc;

namespace wojilu.Web.Controller.Admin {

    public class LayoutController : ControllerBase {

        public override void Layout() {

            
            set( "user.AdminLink", to( new Admin.UserController().List ) );
            set( "article.AdminLink", to( new Admin.ArticleController().List ) );
            set( "category.AdminLink", to( new Admin.CategoryController().List ) );

        }
    }
}

using System;

namespace wojilu.Coder.Service {

    public class ControllerTemplate : IControllerTemplate {


        public string GetLayoutController() {

            return @"
using System;
using System.Collections.Generic;
using System.Text;
using wojilu.Web.Mvc;

namespace wojilu.Web.Controller.Admin {

    public class LayoutController : ControllerBase {

        public override void Layout() {

            <!-- BEGIN list -->
            set( ""#{lmName}.AdminLink"", to( new Admin.#{mName}Controller().List ) );<!-- END list -->

        }
    }
}
";
        }


        public string GetController() {
            return @"using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using wojilu.Web.Mvc;
using wojilu.Web.Mvc.Attr;
using #{domainNamespace};

namespace #{namespace} {

    public class #{controllerName}Controller : ControllerBase {

        public void List() {
            #{listCode}
        }

        public void Add() {
            #{addCode}
            bind( ""x"", new  #{controllerName}() );
        }

        [HttpPost]
        public void Create() {
            #{createCode}
        }

        public void Edit( int id ) {
             #{editCode}
        }

        [HttpPost]
        public void Update( int id ) {
            #{updateCode}
        }

        [HttpDelete]
        public void Delete( int id ) {
             #{deleteCode}
        }

    }
}";
        }


        public string GetAddAction() {
            return @"target( Create );
#{entityProperty}
            <!-- BEGIN editor -->editorFull( ""#{Name}"", """", ""350px"" );
            <!-- END editor -->";
        }

        public string GetEditAction() {
            return @"
            target( Update, id );

            #{m.Name} data = #{m.Name}.findById( id );
            if (data == null) {
                echo( ""数据不存在"" );
                return;
            }
            bind( ""x"", data );
#{entityProperty}
            <!-- BEGIN editor -->editorFull( ""#{Name}"", data.#{PName}, ""350px"" );
            <!-- END editor -->";
        }


        public string GetCreateAction() {
            return @"
            #{m.Name} data = ctx.PostObject<#{m.Name}>( ""x"" );
            if (ctx.HasErrors) {
                echoError();
                return;
            }

            data.insert();
            redirect( List );";
        }

        public string GetUpdateAction() {
            return @"
            #{m.Name} data = #{m.Name}.findById( id );
            if (data == null) {
                echo( ""数据不存在"" );
                return;
            }

            data = ctx.PostObject( data, ""x"" ) as #{m.Name};
            if (ctx.HasErrors) {
                echoError();
                return;
            }

            data.update();
            redirect( List );";
        }

        public string GetDeleteAction() {
            return @"
            #{m.Name} data = #{m.Name}.findById( id );
            if (data == null) {
                echo( ""数据不存在"" );
                return;
            }

            data.delete();
            redirect( List );";
        }


        public string GetListAction() {
            return @"
            set( ""addLink"", to( Add ) );

            DataPage<#{model.Name}> list = #{model.Name}.findPage("""");
            list.Results.ForEach( x => {
                x.data.edit = to( Edit, x.Id );
                x.data.delete = to( Delete, x.Id );
            } );

            bindList( ""list"", ""x"", list.Results );

            set( ""page"", list.PageBar );";
        }




    }
}


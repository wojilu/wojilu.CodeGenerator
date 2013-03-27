using System;

namespace wojilu.Coder.Service {

    public class CrudActionTemplate {

        public static string GetController() {
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

        public static string GetAddAction() {
            return @"target( Create );
#{entityProperty}
            <!-- BEGIN editor -->editorFull( ""#{Name}"", """", ""350px"" );
            <!-- END editor -->";
        }

        public static string GetEditAction() {
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


        public static string GetCreateAction() {
            return @"
            #{m.Name} data = ctx.PostObject<#{m.Name}>( ""x"" );
            if (ctx.HasErrors) {
                echoError();
                return;
            }

            data.insert();
            redirect( List );";
        }

        public static string GetUpdateAction() {
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

        public static string GetDeleteAction() {
            return @"
            #{m.Name} data = #{m.Name}.findById( id );
            if (data == null) {
                echo( ""数据不存在"" );
                return;
            }

            data.delete();
            redirect( List );";
        }


        public static string GetListAction() {
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


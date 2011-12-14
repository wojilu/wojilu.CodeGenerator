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

        private void bindList( List<#{controllerName}> list ) {
            IBlock block = getBlock( ""list"" );
            foreach (#{controllerName} data in list) {
                <!-- BEGIN setList -->
                block.Set( ""d.#{propertyName}"", #{propertyValue} );<!-- END setList -->
                block.Set( ""d.LinkEdit"", to( Edit, data.Id ) );
                block.Set( ""d.LinkDelete"", to( Delete, data.Id ) );
                block.Next();
            }
        }

        public void Add() {
            #{addCode}
            bind( ""#{domainCamelName}"", new  #{controllerName}() );
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
            bind( ""#{domainCamelName}"", data );
#{entityProperty}
            <!-- BEGIN editor -->editorFull( ""#{Name}"", data.#{PName}, ""350px"" );
            <!-- END editor -->";
        }


        public static string GetCreateAction() {
            return @"
            #{m.Name} data = ctx.PostValue<#{m.Name}>();
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

            data = ctx.PostValue( data ) as #{m.Name};
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
            bindList( list.Results );
            set( ""page"", list.PageBar );";
        }




    }
}


using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using wojilu.Web.Mvc;
using wojilu.Web.Mvc.Attr;
using wojilu.Sample.Domain;

namespace wojilu.Web.Controller.Admin {

    public class ArticleController : ControllerBase {

        public void List() {
            
            set( "addLink", to( Add ) );

            DataPage<Article> list = Article.findPage("");
            list.Results.ForEach( x => {
                x.data["Summary"] = strUtil.ParseHtml( x.Content, 30 );
                x.data.edit = to( Edit, x.Id );
                x.data.delete = to( Delete, x.Id );
            } );

            bindList( "list", "x", list.Results );

            set( "page", list.PageBar );
        }

        public void Add() {
            target( Create );
            dropList( "x.Category", Category.findAll(), "Name=Id", null );

            editorFull( "x.Content", "", "350px" );
            
            bind( "x", new  Article() );
        }

        [HttpPost]
        public void Create() {
            
            Article data = ctx.PostObject<Article>( "x" );
            if (ctx.HasErrors) {
                echoError();
                return;
            }

            data.insert();
            redirect( List );
        }

        public void Edit( int id ) {
             
            target( Update, id );

            Article data = Article.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }
            bind( "x", data );
            dropList( "x.Category", Category.findAll(), "Name=Id", data.Category.Id );

            editorFull( "x.Content", data.Content, "350px" );
            
        }

        [HttpPost]
        public void Update( int id ) {
            
            Article data = Article.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }

            data = ctx.PostObject( data, "x" ) as Article;
            if (ctx.HasErrors) {
                echoError();
                return;
            }

            data.update();
            redirect( List );
        }

        [HttpDelete]
        public void Delete( int id ) {
             
            Article data = Article.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }

            data.delete();
            redirect( List );
        }

    }
}
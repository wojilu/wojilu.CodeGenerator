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
            bindList( list.Results );
            set( "page", list.PageBar );
        }

        private void bindList( List<Article> list ) {
            IBlock block = getBlock( "list" );
            foreach (Article data in list) {
                
                block.Set( "d.Title", data.Title );
                block.Set( "d.Category", data.Category.Name );
                block.Set( "d.Content", strUtil.ParseHtml( data.Content, 50 ) );
                block.Set( "d.Created", data.Created );
                block.Set( "d.Id", data.Id );
                block.Set( "d.LinkEdit", to( Edit, data.Id ) );
                block.Set( "d.LinkDelete", to( Delete, data.Id ) );
                block.Next();
            }
        }

        public void Add() {
            target( Create );
            dropList( "article.Category", Category.findAll(), "Name=Id", null );

            editorFull( "article.Content", "", "350px" );
            
            bind( "article", new  Article() );
        }

        [HttpPost]
        public void Create() {
            
            Article data = ctx.PostValue<Article>();
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
            bind( "article", data );
            dropList( "article.Category", Category.findAll(), "Name=Id", data.Category.Id );

            editorFull( "article.Content", data.Content, "350px" );
            
        }

        [HttpPost]
        public void Update( int id ) {
            
            Article data = Article.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }

            data = ctx.PostValue( data ) as Article;
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
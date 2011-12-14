using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using wojilu.Web.Mvc;
using wojilu.Web.Mvc.Attr;
using wojilu.Sample.Domain;

namespace wojilu.Web.Controller.Admin {

    public class UserController : ControllerBase {

        public void List() {
            
            set( "addLink", to( Add ) );

            DataPage<User> list = User.findPage("");
            bindList( list.Results );
            set( "page", list.PageBar );
        }

        private void bindList( List<User> list ) {
            IBlock block = getBlock( "list" );
            foreach (User data in list) {
                
                block.Set( "d.Name", data.Name );
                block.Set( "d.Pwd", data.Pwd );
                block.Set( "d.Gender", data.Gender );
                block.Set( "d.Description", data.Description );
                block.Set( "d.Created", data.Created );
                block.Set( "d.Id", data.Id );
                block.Set( "d.LinkEdit", to( Edit, data.Id ) );
                block.Set( "d.LinkDelete", to( Delete, data.Id ) );
                block.Next();
            }
        }

        public void Add() {
            target( Create );

            
            bind( "user", new  User() );
        }

        [HttpPost]
        public void Create() {
            
            User data = ctx.PostValue<User>();
            if (ctx.HasErrors) {
                echoError();
                return;
            }

            data.insert();
            redirect( List );
        }

        public void Edit( int id ) {
             
            target( Update, id );

            User data = User.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }
            bind( "user", data );

            
        }

        [HttpPost]
        public void Update( int id ) {
            
            User data = User.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }

            data = ctx.PostValue( data ) as User;
            if (ctx.HasErrors) {
                echoError();
                return;
            }

            data.update();
            redirect( List );
        }

        [HttpDelete]
        public void Delete( int id ) {
             
            User data = User.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }

            data.delete();
            redirect( List );
        }

    }
}
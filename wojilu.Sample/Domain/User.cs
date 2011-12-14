using System;
using System.Collections.Generic;
using System.Text;
using wojilu.ORM;

namespace wojilu.Sample.Domain {

    [Table("Users")]
    [Label( "用户" )]
    public class User : ObjectBase<User> {

        [NotNull( "请填写名称" )]
        public String Name { get; set; }
        public String Pwd { get; set; }
        public int Gender { get; set; }
        public String Description { get; set; }

        public DateTime Created { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using wojilu.ORM;

namespace wojilu.Sample.Domain {

    [Label( "文章分类" )]
    public class Category : ObjectBase<Category> {

        [NotNull( "请填写名称" )]
        public String Name { get; set; }

        [LongText]
        public String Description { get; set; }

        public DateTime Created { get; set; }
    }

}

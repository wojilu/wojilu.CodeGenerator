using System;

namespace wojilu.Coder.Service {

    public interface ICodeService {

        ICrudViewTemplate crudViewTemplate { get; set; }

        ICodeService Init( string codePath, string nsName );
        void Make();
        void MakeSingle( string typeName );
    }

}

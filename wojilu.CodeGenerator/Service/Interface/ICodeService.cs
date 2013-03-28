using System;

namespace wojilu.Coder.Service {

    public interface ICodeService {

        IViewTemplate viewTemplate { get; set; }

        ICodeService Init( string codePath, string nsName );
        void Make();
        void MakeSingle( string typeName );
    }

}

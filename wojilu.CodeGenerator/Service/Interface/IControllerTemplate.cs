using System;

namespace wojilu.Coder.Service {

    public interface IControllerTemplate {

        string GetController();

        string GetLayoutController();

        string GetListAction();
        string GetAddAction();
        string GetCreateAction();
        string GetEditAction();
        string GetUpdateAction();
        string GetDeleteAction();

    }

}

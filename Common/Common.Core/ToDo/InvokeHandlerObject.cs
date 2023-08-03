using System;
using Common.Core.Views;

namespace Common.Core.ToDo
{
    public class InvokeHandlerObject
    {
        public InvokeHandlerObject(Action action, ViewModelBase view)
        {
            Action = action;
            View = view;
        }

        public Action Action { get; set; }
        public ViewModelBase View { get; set; }
    }
}
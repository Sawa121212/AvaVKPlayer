using System;
using System.Threading.Tasks;

namespace Common.Core.ToDo
{
    public static class InvokeHandler
    {
        public delegate void TaskErrorResponsed(InvokeHandlerObject handlerObject, Exception ex);

        public static event TaskErrorResponsed? TaskErrorResponsedEvent;

        public static async void Start(InvokeHandlerObject handlerObject)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (handlerObject.View != null) ;
                    // ToDo handlerObject.View.IsLoading = true;

                    handlerObject.Action.Invoke();
                }
                catch (Exception ex)
                {
                    TaskErrorResponsedEvent?.Invoke(handlerObject, ex);
                }
                finally
                {
                    if (handlerObject.View != null) ;
                    // ToDo  handlerObject.View.IsLoading = false;
                }
            });
        }
    }
}
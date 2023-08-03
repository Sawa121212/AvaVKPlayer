using Prism.Ioc;
using Prism.Modularity;

namespace VkProvider.Module
{
    public class VkProviderModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<IVkApiManager, VkApiManager>();
            //containerRegistry.RegisterInstance(typeof(MailViewModel));
        }
    }
}
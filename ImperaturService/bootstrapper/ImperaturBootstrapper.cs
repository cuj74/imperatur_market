using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2;
using Nancy.Conventions;

namespace ImperaturService.bootstrapper
{
    /*
        public class Bootstrapper : NinjectNancyBootstrapper
        {
            protected override void ConfigureApplicationContainer(IKernel existingContainer)
            {
                ImperaturSystemLocation.SystemLocation;

                ImperaturContainer.BuildImperaturContainer(SystemLocation);


                //application singleton
                existingContainer.Bind<IImperaturMarket>()
                    .To<IImperaturMarket>().InSingletonScope();

                //transient binding
               // existingContainer.Bind<ICommandHandler>().To<CommandHandler>();
            }


            protected override void ConfigureRequestContainer(IKernel container, NancyContext context)
            {
                //container here is a child container. I.e. singletons here are in request scope.
                //IDisposables will get disposed at the end of the request when the child container does.
                container.Bind<IPerRequest>().To<PerRequest>().InSingletonScope();
            }
        }*/

   /* public class ApplicationBootstrapper : DefaultNancyBootstrapper
    {
        protected override void  ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Static", @"Static"));
            base.ConfigureConventions(nancyConventions);
        }
    }
    */
    public class ImperaturBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("static", @"static"));
            base.ConfigureConventions(nancyConventions);
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            container.Register<IImperaturMarket>(ImperaturContainer.BuildImperaturContainer(@"F:\dev\test4"));
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {

            //CORS Enable
            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                                .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                                .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");

            });
        }
    }
}

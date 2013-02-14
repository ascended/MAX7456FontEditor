namespace MaxFontEditor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Autofac;
    using Caliburn.Micro;
    using MaxFontEditor.Framework;
    using MaxFontEditor.Shell;
    using System.Windows.Shapes;
    using MaxFontEditor.Services;
    using Caliburn.Micro.Autofac;

    public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        static readonly ILog Log = LogManager.GetLog(typeof(AppBootstrapper));

        static AppBootstrapper()
        {
            // Add DebugLogger to CM
            LogManager.GetLog = type => new DebugLogger(type);

            FrameworkExtensions.Message.Attach.AllowXamlSyntax();
        }

        public AppBootstrapper()
        {
        }


        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor(typeof(ShellViewModel));
        }

        #region IOC (Autofac)

        private IContainer container;

        protected override void Configure()
        {
            //  configure container
            var builder = new ContainerBuilder();

            /*var entry = Assembly.GetEntryAssembly();
            var root = Path.GetDirectoryName(entry.Location);
            AssemblySource.Instance.Add(Assembly.LoadFrom(Path.Combine("ref.dll")));
            */

            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                .Where(type => type.Name.EndsWith("Service"))
                .AsImplementedInterfaces();

            //  register view models
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                //  must be a type with a name that ends with ViewModel
              .Where(type => type.Name.EndsWith("ViewModel"))
                //  registered as self
              .AsSelf()
                //  always create a new one
              .InstancePerDependency();

            //  register views
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                //  must be a type with a name that ends with View
              .Where(type => type.Name.EndsWith("View"))
                //  registered as self
              .AsSelf()
                //  always create a new one
              .InstancePerDependency();

            //  register the single window manager for this container
            builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();
            //  register the single event aggregator for this container
            builder.Register<IEventAggregator>(c => new EventAggregator()).InstancePerLifetimeScope();

            //  allow derived classes to add to the container
            ConfigureContainer(builder);

            container = builder.Build();
        }

        protected override object GetInstance(Type service, string key)
        {
            object instance;
            if (string.IsNullOrWhiteSpace(key))
            {
                if (container.TryResolve(service, out instance))
                    return instance;
            }
            else
            {
                if (container.TryResolveNamed(key, service, out instance))
                    return instance;
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            container.InjectProperties(instance);
        }

        #endregion

        private IEnumerable<ExportAttribute> GetExportAttribute(ICustomAttributeProvider attrProvider)
        {
            return attrProvider.GetCustomAttributes(typeof(ExportAttribute), true).Cast<ExportAttribute>();
        }

        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
            Assembly[] assemblies = AssemblySource.Instance.ToArray();

            builder
                .RegisterAssemblyTypes(assemblies)
                .Where(t => GetExportAttribute(t).Any())
                .As(t => GetExportAttribute(t).Select(e => e.ContractType));


            //builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
        }
    }
}

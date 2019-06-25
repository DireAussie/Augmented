﻿using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using DavidFidge.MonoGame.Core.Components;
using DavidFidge.MonoGame.Core.Configuration;
using DavidFidge.MonoGame.Core.Graphics;
using DavidFidge.MonoGame.Core.Graphics.Quads;
using DavidFidge.MonoGame.Core.Interfaces;
using DavidFidge.MonoGame.Core.Interfaces.Components;
using DavidFidge.MonoGame.Core.Interfaces.Services;
using DavidFidge.MonoGame.Core.Interfaces.UserInterface;
using DavidFidge.MonoGame.Core.Services;
using DavidFidge.MonoGame.Core.UserInterface;
using GeonBit.UI.Entities;
using InputHandlers.Keyboard;
using InputHandlers.Mouse;

using Serilog;

namespace DavidFidge.MonoGame.Core.Installers
{
    public class CoreInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var config = new LoggerConfiguration()
                .WriteTo.Seq("http://localhost:5341/")
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            container.AddFacility<TypedFactoryFacility>();

            container.Install(new MediatorInstaller());

            container.Register(
                Component.For<IRootPanel<Entity>>()
                    .ImplementedBy<RootGeonBitPanel>()
                    .LifeStyle.Transient,

                Component.For<IGameTimeService>()
                    .ImplementedBy<GameTimeService>(),

                Component.For<IRandom>()
                    .ImplementedBy<Random>(),

                Component.For<ISaveGameService>()
                    .ImplementedBy<SaveGameService>()
                    .LifeStyle.Transient,

                Component.For<IGameInputService>()
                    .ImplementedBy<GameInputService>(),

                Component.For<IStopwatchProvider>()
                    .ImplementedBy<StopwatchProvider>()
                    .LifeStyle.Transient,

                Component.For<IKeyboardInput>()
                    .ImplementedBy<KeyboardInput>(),

                Component.For<IMouseInput>()
                    .ImplementedBy<MouseInput>(),

                Component.For<IUserInterface>()
                    .ImplementedBy<UserInterface.UserInterface>(),

                Component.For<ILogger>()
                    .Instance(config),

                Component.For<IGameProvider>()
                    .ImplementedBy<GameProvider>(),

                Component.For<IGameOptionsStore>()
                    .ImplementedBy<GameOptionsStore>(),

                Component.For<MaterialQuadTemplate>()
                    .LifeStyle.Transient,

                Component.For<TexturedQuadTemplate>()
                    .LifeStyle.Transient,

                Component.For<IConfigurationSettings>()
                    .ImplementedBy<ConfigurationSettings>(),

                Component.For<IGraphicsSettings>()
                    .UsingFactoryMethod(k => BaseConfigurationSectionHandler.Load<GraphicsSettings>())
            );
        }
    }
}

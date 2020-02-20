using System.Linq;
using System.Reflection;
using Autofac;
using CorePool.Api;
using CorePool.Banning;
using CorePool.Blockchain.Bitcoin;
using CorePool.Blockchain.Bitcoin.DaemonResponses;
using CorePool.Blockchain.Ethereum;
using CorePool.Blockchain.Cryptonote;
using CorePool.Blockchain.Equihash;
using CorePool.Blockchain.Equihash.DaemonResponses;
using CorePool.Configuration;
using CorePool.Crypto;
using CorePool.Crypto.Hashing.Equihash;
using CorePool.Messaging;
using CorePool.Mining;
using CorePool.Notifications;
using CorePool.Payments;
using CorePool.Payments.PaymentSchemes;
using CorePool.Time;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Module = Autofac.Module;
using Microsoft.AspNetCore.Mvc;
using CorePool.Api.WebSocketNotifications;

namespace CorePool
{
    public class AutofacModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            var thisAssembly = typeof(AutofacModule).GetTypeInfo().Assembly;

            builder.RegisterInstance(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            builder.RegisterType<MessageBus>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<PayoutManager>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<StandardClock>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<IntegratedBanManager>()
                .Keyed<IBanManager>(BanManagerKind.Integrated)
                .SingleInstance();

            builder.RegisterType<ShareRecorder>()
                .SingleInstance();

            builder.RegisterType<ShareReceiver>()
                .SingleInstance();

            builder.RegisterType<BtStreamReceiver>()
                .SingleInstance();

            builder.RegisterType<ShareRelay>()
                .SingleInstance();

            builder.RegisterType<StatsRecorder>()
                .SingleInstance();

            builder.RegisterType<NotificationService>()
                .SingleInstance();

            builder.RegisterType<MetricsPublisher>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.GetCustomAttributes<CoinFamilyAttribute>().Any() && t.GetInterfaces()
                    .Any(i =>
                        i.IsAssignableFrom(typeof(IMiningPool)) ||
                        i.IsAssignableFrom(typeof(IPayoutHandler)) ||
                        i.IsAssignableFrom(typeof(IPayoutScheme))))
                .WithMetadataFrom<CoinFamilyAttribute>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IHashAlgorithm))))
                .PropertiesAutowired()
                .AsSelf();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsAssignableTo<EquihashSolver>())
                .PropertiesAutowired()
                .AsSelf();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsAssignableTo<ControllerBase>())
                .PropertiesAutowired()
                .AsSelf();

            builder.RegisterType<WebSocketNotificationsRelay>()
                .PropertiesAutowired()
                .AsSelf()
                .SingleInstance();

            //////////////////////
            // Payment Schemes

            builder.RegisterType<PPLNSPaymentScheme>()
                .Keyed<IPayoutScheme>(PayoutScheme.PPLNS)
                .SingleInstance();

            builder.RegisterType<SoloPaymentScheme>()
                .Keyed<IPayoutScheme>(PayoutScheme.Solo)
                .SingleInstance();

            //////////////////////
            // Bitcoin and family

            builder.RegisterType<BitcoinJobManager>()
                .AsSelf();

            //////////////////////
            // Cryptonote

            builder.RegisterType<CryptonoteJobManager>()
                .AsSelf();

            //////////////////////
            // Ethereum

            builder.RegisterType<EthereumJobManager>()
                .AsSelf();

            //////////////////////
            // ZCash

            builder.RegisterType<EquihashJobManager>()
                .AsSelf();

            base.Load(builder);
        }
    }
}

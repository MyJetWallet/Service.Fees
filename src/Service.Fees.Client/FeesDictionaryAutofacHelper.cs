﻿using Autofac;
using MyNoSqlServer.DataReader;
using Service.Fees.MyNoSql;

namespace Service.Fees.Client
{
    public static class FeesDictionaryAutofacHelper
    {
        /// <summary>
        /// Register interface:
        ///   * IFeesSettingsClient
        /// </summary>
        public static void RegisterAssetPaymentSettingsClients(this ContainerBuilder builder,
            IMyNoSqlSubscriber myNoSqlSubscriber)
        {
            var subs = new MyNoSqlReadRepository<FeesSettingsNoSqlEntity>(myNoSqlSubscriber,
                FeesSettingsNoSqlEntity.TableName);

            builder
                .RegisterInstance(new FeesSettingsClient(subs))
                .As<IFeesSettingsClient>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
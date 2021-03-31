using System;
using Autofac;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataWriter;
using Service.Fees.MyNoSql;

namespace Service.Fees.Modules
{
    public class MyNoSqlModule : Module
    {
        private readonly Func<string> _myNoSqlServerWriterUrl;

        public MyNoSqlModule(Func<string> myNoSqlServerWriterUrl)
        {
            _myNoSqlServerWriterUrl = myNoSqlServerWriterUrl;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterMyNoSqlWriter<FeesSettingsNoSqlEntity>(builder, FeesSettingsNoSqlEntity.TableName);
            RegisterMyNoSqlWriter<AssetFeesNoSqlEntity>(builder, AssetFeesNoSqlEntity.TableName);
            RegisterMyNoSqlWriter<SpotInstrumentFeesNoSqlEntity>(builder, SpotInstrumentFeesNoSqlEntity.TableName);
        }

        private void RegisterMyNoSqlWriter<TEntity>(ContainerBuilder builder, string table)
            where TEntity : IMyNoSqlDbEntity, new()
        {
            builder.Register(ctx =>
                    new MyNoSqlServerDataWriter<TEntity>(_myNoSqlServerWriterUrl, table, true))
                .As<IMyNoSqlServerDataWriter<TEntity>>()
                .SingleInstance();
        }
    }
}
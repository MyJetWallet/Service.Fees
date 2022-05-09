using System;
using Autofac;
using MyJetWallet.Sdk.NoSql;
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
            builder.RegisterMyNoSqlWriter<FeesSettingsNoSqlEntity>(_myNoSqlServerWriterUrl, FeesSettingsNoSqlEntity.TableName);
            builder.RegisterMyNoSqlWriter<AssetFeesNoSqlEntity>(_myNoSqlServerWriterUrl, AssetFeesNoSqlEntity.TableName);
            builder.RegisterMyNoSqlWriter<SpotInstrumentFeesNoSqlEntity>(_myNoSqlServerWriterUrl, SpotInstrumentFeesNoSqlEntity.TableName);
            builder.RegisterMyNoSqlWriter<FeeProfilesNoSqlEntity>(_myNoSqlServerWriterUrl, FeeProfilesNoSqlEntity.TableName);

        }
    }
}
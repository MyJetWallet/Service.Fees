using SimpleTrading.SettingsReader;

namespace Service.Fees.Settings
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        [YamlProperty("Fees.SeqServiceUrl")] public string SeqServiceUrl { get; set; }

        [YamlProperty("Fees.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("Fees.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }

        [YamlProperty("Fees.ZipkinUrl")]
        public string ZipkinUrl { get; set; }
    }
}
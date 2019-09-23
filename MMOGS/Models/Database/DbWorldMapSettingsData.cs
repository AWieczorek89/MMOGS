namespace MMOGS.Models.Database
{
    public class DbWorldMapSettingsData
    {
        public int WmsId { get; private set; } = -1;
        public int Type { get; private set; } = -1;
        public string Description { get; private set; } = "";
        public string Value { get; private set; } = "";

        public DbWorldMapSettingsData
        (
            int wmsId,
            int type,
            string description,
            string value
        )
        {
            this.WmsId = wmsId;
            this.Type = type;
            this.Description = description;
            this.Value = value;
        }
    }
}

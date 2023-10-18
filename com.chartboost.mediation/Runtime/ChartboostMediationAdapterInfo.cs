namespace Chartboost
{
    /// <summary>
    /// Data class holding Adapter Info
    /// </summary>
    // Note : Json deserialization (JsonConvert.DeserializeObject<T>) using generics when type argument is value doesn't
    // work with IL2CPP. Therefore, we use class here instead of struct
    // https://stackoverflow.com/a/56185978
    // TODO: As per the above link this is fixed in 2022.2 so we should update this to struct when we update Unity
    public class ChartboostMediationAdapterInfo
    {
        public string AdapterVersion { get; set; }
        public string PartnerVersion { get; set; }
        public string PartnerDisplayName { get; set; }
        public string PartnerIdentifier { get; set; }
        
        // Android uses "PartnerId", iOS uses "PartnerIdentifier"
        public string PartnerId {set => PartnerIdentifier = value; }
    }
}

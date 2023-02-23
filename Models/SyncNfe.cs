using Redis.OM.Modeling;

namespace Redis.OM.Skeleton.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "SyncNfe" })]
    public class SyncNfe
    {
        [RedisIdField][Indexed] public string ChaveNfe { get; set; }
        [Indexed(Sortable = true)] public long TimeStamp { get; set; }
    }

    public class SyncNfeModel
    {
        public string ChaveNfe { get; set; }
    }
}

using MongoDB.Bson;

namespace Domain
{
    public static class IdGenerator
    {
        public static string GetNewId() => ObjectId.GenerateNewId().ToString();
    }
}
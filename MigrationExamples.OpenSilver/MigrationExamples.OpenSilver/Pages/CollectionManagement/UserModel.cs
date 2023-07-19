namespace MigrationExamples.OpenSilver.Pages.CollectionManagement
{
    public class UserModel
    {
        public string Name { get; set; }

        public int Order => Name.GetHashCode() & 0xFFFF;
    }
}

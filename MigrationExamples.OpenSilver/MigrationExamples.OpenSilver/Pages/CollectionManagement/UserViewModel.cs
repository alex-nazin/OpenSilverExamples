namespace MigrationExamples.OpenSilver.Pages.CollectionManagement
{
    public class UserViewModel
    {
        public UserViewModel(UserModel model)
        {
            Model = model;
        }

        internal UserModel Model { get; }

        public string Name => Model.Name;

        public int Order => Model.Order;

        public string Image => Order % 2 == 0 ? "UserMale.png" : "UserFemale.png";


    }
}

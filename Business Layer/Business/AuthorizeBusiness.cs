using Data_Layer.Data;

namespace Business_Layer.Business
{
    public class AuthorizeBusiness
    {
        public AuthorizeData AuthorizeData { get; }

        public AuthorizeBusiness(AuthorizeData authorizeData)
        {
            AuthorizeData = authorizeData;
        }


        public async Task<List<int>> GetPermissions(int userId)
        {
            if(userId == 0)
            {
                return [];
            }

            return await AuthorizeData.GetPermissions(userId);
        }

    }
}

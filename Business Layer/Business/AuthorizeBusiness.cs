using Business_Layer.Interfaces;
using Data_Layer.Data;
using Data_Layer.Interfaces;

namespace Business_Layer.Business
{
    public class AuthorizeBusiness : IAuthorizeBusiness
    {
        public IAuthorizeData AuthorizeData { get; }

        public AuthorizeBusiness(IAuthorizeData authorizeData)
        {
            AuthorizeData = authorizeData;
        }


        public async Task<List<int>> GetPermissions(int userId)
        {
            return await AuthorizeData.GetPermissions(userId);
        }

    }
}

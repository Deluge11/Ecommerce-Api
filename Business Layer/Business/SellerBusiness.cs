
using Data_Layer.Data;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Business
{
    public class SellerBusiness 
    {

        public SellerData SellerData { get; }
        public UsersBusiness UsersBusiness { get; }

        public SellerBusiness(SellerData sellerData, UsersBusiness usersBusiness)
        {
            SellerData = sellerData;
            UsersBusiness = usersBusiness;
        }

        public async Task<bool> ApplyForSeller()
        {
            int userId = UsersBusiness.GetUserId();

            if(userId == 0)
            {
                return false;
            }

            return await SellerData.ApplyForSeller(userId);
        }

        public async Task<bool> ConfirmSeller(int requestId)
        {
            if (requestId == 0)
            {
                return false;
            }
            return await SellerData.ConfirmSeller(requestId);
        }

        public async Task<bool> RefuseSeller(int requestId)
        {
            if (requestId == 0)
            {
                return false;
            }
            return await SellerData.RefuseSeller(requestId);
        }
    }
}

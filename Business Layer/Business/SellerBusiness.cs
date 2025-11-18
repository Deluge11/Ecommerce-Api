using Business_Layer.Interfaces;
using Data_Layer.Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Business
{
    public class SellerBusiness : ISellerBusiness
    {

        public ISellerData SellerData { get; }
        public IUsersBusiness UsersBusiness { get; }

        public SellerBusiness(ISellerData sellerData, IUsersBusiness usersBusiness)
        {
            SellerData = sellerData;
            UsersBusiness = usersBusiness;
        }

        public async Task<bool> ApplyForSeller()
        {
            return await SellerData.ApplyForSeller(UsersBusiness.GetUserId());
        }

        public async Task<bool> ConfirmSeller(int requestId)
        {
            return await SellerData.ConfirmSeller(requestId);
        }

        public async Task<bool> RefuseSeller(int requestId)
        {
            return await SellerData.RefuseSeller(requestId);
        }
    }
}

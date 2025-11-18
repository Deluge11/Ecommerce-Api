using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Interfaces
{
    public interface ISellerBusiness
    {
        Task<bool> ApplyForSeller();
        Task<bool> ConfirmSeller(int requestId);
        Task<bool> RefuseSeller(int requestId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.Interfaces
{
    public interface ISellerData
    {
        Task<bool> ApplyForSeller(int userId);
        Task<bool> ConfirmSeller(int requestId);
        Task<bool> RefuseSeller(int requestId);
    }
}

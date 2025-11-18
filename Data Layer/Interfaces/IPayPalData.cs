
using Models;

namespace Data_Layer.Interfaces;

public interface IPayPalData
{
    Task<bool> UpdatePaymentStateId(string paymentId, int state);
    Task<PaymentDetails> GetPaymentDetails(string paymentId);
    Task<bool> SaveOrderPayment(string paymentId, int orderId);
    Task SaveTransferPayout(string payoutId, int transferId);
}




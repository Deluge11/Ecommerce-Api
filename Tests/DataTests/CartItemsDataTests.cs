using Data_Layer.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DataTests
{
    public class CartItemsDataTests
    {
        private const string TestsConnectionString = "Server=.;Database=TestsDB;TrustServerCertificate=True;Trusted_Connection = True;";


        [Fact]
        public async Task DeleteCartItem_ValidIds_DeletesRow()
        {
            var service = new CartItemsData(TestsConnectionString, null, null, null);

            bool result = await service.DeleteCartItem(1, 123);

            Assert.True(result);
            var countQuery = "SELECT COUNT(*) FROM CartItems WHERE id=1";
            await using var checkConn = new SqlConnection(TestsConnectionString);
            await checkConn.OpenAsync();
            var count = (int)await new SqlCommand(countQuery, checkConn).ExecuteScalarAsync();
            Assert.Equal(0, count);
        }



    }
}

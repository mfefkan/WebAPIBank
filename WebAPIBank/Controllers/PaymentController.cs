using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using WebAPIBank.DesignPatterns;
using WebAPIBank.Models.Context;
using WebAPIBank.Models.Entities;
using WebAPIBank.RequestModel;
using WebAPIBank.ResponseModel;

namespace WebAPIBank.Controllers
{
    public class PaymentController : ApiController
    {
        MyContext _db;

        public PaymentController()
        {
            _db = DBTool.DbInstance;
        }


        //Development testi içindir....aşağıdaki action

        //[HttpGet]
        //public List<PaymentResponseModel> GetAll()
        //{
        //    return _db.Cards.Select(x => new PaymentResponseModel
        //    {
        //        CardExpiryMonth = x.CardExpiryMonth,
        //        CardExpiryYear = x.CardExpiryYear,
        //        CardNumber = x.CardNumber
        //    }).ToList();
        //}



        [HttpPost]
        public IHttpActionResult RecievePayment(PaymentRequestModel item)
        {
            CardInfo ci = _db.Cards.FirstOrDefault(x=> x.CardUserName == item.CardUserName && x.SecurityNumber == item.SecurityNumber && x.CardNumber == item.CardNumber && x.CardExpiryMonth == item.CardExpiryMonth && x.CardExpiryYear == item.CardExpiryYear );

            if (ci != null )
            {
                if (ci.CardExpiryYear< DateTime.Now.Year)
                {
                    return BadRequest("Expired Card");
                }
                else if (ci.CardExpiryYear == DateTime.Now.Year)
                {
                    if (ci.CardExpiryMonth < DateTime.Now.Month)
                    {
                        return BadRequest("Expired Card(Month)");
                    }

                    if(ci.Balance >= item.ShoppingPrice)
                    {
                        SetBalance(item, ci);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Balance exceeded");
                    }
                }


                else if(ci.Balance>= item.ShoppingPrice)
                {
                    SetBalance(item, ci);
                    return Ok();

                }

                return BadRequest("Balance exceeded");
            }

            return BadRequest("Card Info Wrong!!");
        }
        


        private void SetBalance(PaymentRequestModel item,CardInfo ci)
        {
            ci.Balance-=item.ShoppingPrice;
            _db.SaveChanges();
        }

    }
}

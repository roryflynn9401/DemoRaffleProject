using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace SportsRaffles.Controllers
{
    [Route("create-checkout-session")]
    [ApiController]
    public class CheckoutController : Controller
    {
        [HttpPost]
        public IActionResult Create()
        {
            var domain = "http://localhost:7001";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                    Price = "price_1NuxmsE5C5eLEIJwRNflu4l1",
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                SuccessUrl = domain + "/success",
                CancelUrl = domain + "/cancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}

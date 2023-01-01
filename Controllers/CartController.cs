using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using Bonsal_Gardent.Areas.Admin.Data;
using System.Configuration;
using Bonsal_Gardent.Others;
using PayPal.Api;
using Bonsal_Gardent.Models;

namespace My_Web.Controllers
{
    public class CartController : Controller
    {
        Bonsal_GardentEntities db = new Bonsal_GardentEntities();
        // GET: Cart


        public ActionResult Index()
        {
            return View();
        }
        public List<Cart> TakeCart()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }
        public ActionResult AddCart(int ms, string url, FormCollection f)
        {
            List<Cart> lstCart = TakeCart();
            Cart sp = lstCart.Find(n => n.ProductID == ms);
            if (sp == null)
            {
                var product = db.Products.FirstOrDefault(n => n.ID.Equals(ms));
                sp = new Cart();
                sp.ProductID = ms;
                sp.Amount = 1;
                sp.Name = product.Name;
                sp.Pice = product.Price;
                lstCart.Add(sp);
            }
            else
            {
                sp.Amount++;
            }
            return Redirect(url);
        }
        public ActionResult AddCartAll(int ms, string url, FormCollection f)
        {
            List<Cart> lstCart = TakeCart();
            Cart sp = lstCart.Find(n => n.ProductID == ms);
            if (sp == null)
            {
                var product = db.Products.FirstOrDefault(n => n.ID.Equals(ms));
                sp = new Cart();
                sp.ProductID = ms;
                sp.Amount = int.Parse(f["Amount"].ToString());
                sp.Name = product.Name;
                sp.Pice = product.Price;
                lstCart.Add(sp);
            }
            else
            {
                sp.Amount++;
            }
            return Redirect(url);
        }
        private int SumCart()
        {
            int sum = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                sum = (int)lstCart.Sum(n => n.Amount);
            }
            return sum;
        }
        private double Total()
        {
            double sum = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                sum = (int)lstCart.Sum(n => n.Amount) * (double)lstCart.Sum(n => n.Pice);
            }
            return sum;
        }

        public ActionResult Cart()
        {

            List<Cart> lstCart = TakeCart();

            ViewBag.SumCart = SumCart();
            ViewBag.Total = Total();
            return View(lstCart);
        }
        public ActionResult DeleteCart(int? iProductID)
        {
            List<Cart> lstCart = TakeCart();
            Cart sp = lstCart.SingleOrDefault(n => n.ProductID == iProductID);
            if (sp != null)
            {
                lstCart.RemoveAll(n => n.ProductID == iProductID);
                if (lstCart.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Cart");
        }
        public ActionResult Cart_partial()
        {
            ViewBag.SumCart = SumCart();
            ViewBag.Total = Total();
            return PartialView();
        }
        public ActionResult UpdateCart(int? iProductID, FormCollection f)
        {
            List<Cart> lstCart = TakeCart();
            Cart sp = lstCart.SingleOrDefault(n => n.ProductID == iProductID);
            if (sp != null)
            {
                sp.Amount = int.Parse(f["Amount"].ToString());
            }
            return RedirectToAction("Cart");
        }
        public ActionResult DeleteCart1()
        {
            List<Cart> lstCart = TakeCart();
            lstCart.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Order()
        {
            if (Session["CEmail"] == null || Session["CEmail"].ToString() == "")
            {
                return Redirect("~/Security/Login");
            }
            //Kiểm tra có hàng trong giỏ chưa
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Cart> lstCart = TakeCart();
            ViewBag.SumCart = SumCart();
            ViewBag.Total = Total();
            return View(lstCart);
        }


        public ActionResult OrderPay(FormCollection f)
        {

            int CustomerID = Convert.ToInt32((Session["CInfo"] as AccCustomer).ID.ToString());
            //Create New Order
            CustomerOrder ddh = new CustomerOrder();
            ddh.AccCustomerID = CustomerID;
            ddh.CreateAtTime = DateTime.Now;
            db.CustomerOrders.Add(ddh);
            db.SaveChanges();

            string NewOrderID = db.CustomerOrders.OrderByDescending(n => n.ID).FirstOrDefault().ID.ToString();

            //Create new Order Detail
            List<Cart> lstCart = TakeCart();

            foreach (var item in lstCart)
            {
                OrderDetail order = new OrderDetail();
                order.CustomerOrderID = Convert.ToInt32(NewOrderID);
                order.ProductID = item.ProductID;
                order.Quantity = (int)item.Amount;
                order.Price = item.Pice;
                order.Amount = item.Amount * item.Pice;
                order.Status = 1;
                order.Note = f["Note"];
                db.OrderDetails.Add(order);
            }
            db.SaveChanges();
            Session["Cart"] = null;
            ViewBag.ThongBao = "Order successfully";
            return RedirectToAction("Orderconfirmation", "Cart");
        }
        public ActionResult Orderconfirmation()
        {
            return View();
        }
        public ActionResult Payment()
        {

            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();
            List<Cart> temp = (List<Cart>)Session["Cart"];
            var total = temp.Sum(x => x.Amount * x.Pice * 100);

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", total.ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", "NCB"); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);


        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                        Session["Cart"] = null;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }
        private Payment payment;

        // Create a paypment using an APIContext
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var listItems = new ItemList() { items = new List<Item>() };

            List<Cart> listCarts = (List<Cart>)Session["Cart"];
            foreach (var cart in listCarts)
            {
                listItems.items.Add(new Item()
                {
                    name = cart.Name,
                    currency = "USD",
                    price = cart.Pice.ToString(),
                    quantity = cart.Amount.ToString(),
                    sku = "sku"
                });
            }

            var payer = new Payer() { payment_method = "paypal" };

            // Do the configuration RedirectURLs here with redirectURLs object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            // Create details object
            var details = new Details()
            {
                tax = "3",
                shipping = "2",
                subtotal = listCarts.Sum(x => x.Amount * x.Pice).ToString()
            };

            // Create amount object
            var amount = new Amount()
            {


                currency = "USD",
                total = (Convert.ToDouble(details.tax) + Convert.ToDouble(details.shipping) + Convert.ToDouble(details.subtotal)).ToString(),// tax + shipping + subtotal
                details = details
            };

            // Create transaction
            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = " Transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000)),
                amount = amount,
                item_list = listItems
            });

            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return payment.Create(apiContext);
        }

        // Create ExecutePayment method
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }

        // Create PaymentWithPaypal method
        public ActionResult PaymentWithPaypal()
        {
            // Gettings context from the paypal bases on clientId and clientSecret for payment
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    // Creating a payment
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/ShoppingCart/PaymentWithPaypal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid);

                    // Get links returned from paypal response to create call funciton
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = string.Empty;

                    while (links.MoveNext())
                    {
                        Links link = links.Current;
                        if (link.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = link.href;
                        }
                    }

                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This one will be executed when we have received all the payment params from previous call
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        //Remove shopping cart session
                        Session.Remove("Cart");
                        return View("Failure");
                    }
                }
            }
            catch (Exception ex)
            {
                PaypalLogger.Log("Error: " + ex.Message);
                //Remove shopping cart session
                Session.Remove("Cart");
                return View("Failure");
            }

            //Remove shopping cart session
            Session.Remove("Cart");
            return View("Success");
        }


    }

}

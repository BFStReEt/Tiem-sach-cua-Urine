using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBANSACH.Controllers
{
    public class GiohangController : Controller
    {
        DbQuanLyBanSachDataContext db = new DbQuanLyBanSachDataContext();
        //
        // GET: /Giohang/
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lsGiohang = Session["Giohang"] as List<Giohang>;
            if(lsGiohang == null)
            {
                lsGiohang = new List<Giohang>();
                Session["Giohang"] = lsGiohang;
            }
            return lsGiohang;
        }

        public ActionResult Themgiohang(int Masach, string strURL)
        {
            List<Giohang> lsGiohang = Laygiohang();
            Giohang sanpham = lsGiohang.Find(n => n.iMasach == Masach);
            if(sanpham == null)
            {
                sanpham = new Giohang(Masach);
                lsGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lsGiohang = Session["Giohang"] as List<Giohang>;
            if(lsGiohang!=null)
            {
                iTongSoLuong = lsGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }

        private double TongTien()
        {
            double dTongtien = 0;
            List<Giohang> lsGiohang = Session["Giohang"] as List<Giohang>;
            if(lsGiohang != null)
            {
                dTongtien = lsGiohang.Sum(n => n.dThanhtien);
            }
            return dTongtien;
        }

        public ActionResult GioHang()
        {
            List<Giohang> lsGiohang = Laygiohang();
            if(lsGiohang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lsGiohang);
        }

        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            List<Giohang> lsGiohang = Laygiohang();
            Giohang sanpham = lsGiohang.SingleOrDefault(n => n.iMasach == iMaSP);
            if(sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }

        public ActionResult XoaGiohang(int iMaSp)
        {
            List < Giohang > lsGiohang = Laygiohang();
            Giohang sanpham = lsGiohang.SingleOrDefault(n => n.iMasach == iMaSp);
            if(sanpham != null)
            {
                lsGiohang.RemoveAll(n => n.iMasach == iMaSp);
                return RedirectToAction("Giohang");
            }
            return RedirectToAction("Giohang");
        }

        public ActionResult SoLuongGioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Soluong = lstGiohang.Count();
            return PartialView();
        }

        public ActionResult XoaTatcaGiohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("index", "BookStore");
        }

        public ActionResult Dathang()
        {
            if(Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "NguoiDung");
            }
            List<Giohang> lstGiohang = Laygiohang();
            if(lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            //List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        [HttpPost]
        public ActionResult Dathang(FormCollection collection)
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<Giohang> lstGiohang = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.TenNguoiNhan = kh.HoTen;
            ddh.DiaChiNhan = kh.DiaChiKH;
            ddh.DienThoaiNhan = kh.DienThoaiKH;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.NgayGiao = DateTime.Parse(ngaygiao);
            ddh.TinhTrangGiaoHang = false;
            ddh.DaThanhToan = false;
            db.DONDATHANGs.InsertOnSubmit(ddh);
            db.SubmitChanges();
            foreach(var item in lstGiohang)
            {
                CHITIETDONHANG ctdh = new CHITIETDONHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaSach = item.iMasach;
                ctdh.SoLuong = item.iSoluong;
                ctdh.DonGia = (decimal)item.dDongia;
                db.CHITIETDONHANGs.InsertOnSubmit(ctdh);
            }
            db.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
        public ActionResult Xacnhandonhang()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }

    }
}

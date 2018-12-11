using iTextSharp.text;
using iTextSharp.text.pdf;
using org.in2bits.MyXls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WCAPP.Libs;
using WCAPP.Models.Database;
using WCAPP.Models.Home;
using WCAPP.Models.HomeModels;
using WCAPP.Models.UserModels;
using WCAPP.Types;
using WCAPP.Utils;

namespace WCAPP.Controllers
{
    public class UserController : Controller
    {             
        public ActionResult Index(int? pageNo, string searchName,int? pageSize)
        {
            var db = new Context();
            List<int> lint = new List<int>();
            lint.Add(10);
            lint.Add(20);
            lint.Add(50);
            lint.Add(100);
            lint.Add(200);

            int pNo = pageNo ?? 1;
            int pSize = pageSize ?? 10;

            SelectList lints = new SelectList(lint, pSize);
            ViewBag.PageSize = lints;
            ViewBag.PageSizes = pSize;

            if (searchName!=null)
            {
                int rowsCount = db.Users.Where(x => x.Name.Contains(searchName)).Count();
                // int pCount = (int)Math.Ceiling(1.0 * rowsCount / pSize);//取天花板数
                // 调整当前页码不超过总页数
                int crtNo = Common.FixPageNo(pNo, pSize, rowsCount);
                crtNo = crtNo <= 0 ? 1 : crtNo;
                //总记录数
                List<User> list = db.Users.Where(x => x.Name.Contains(searchName)).Include(x => x.AuthorityRs).OrderBy(a => a.Id).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                //总页数
                int pageCount = (rowsCount - 1) / pSize + 1;
                int nextPageNo = crtNo >= pageCount ? pageCount : crtNo + 1;//计算下一页页号
                int prevPageNo = crtNo == 1 ? 1 : crtNo - 1;//计算上一页页号
                ViewBag.NextPageNo = nextPageNo;
                ViewBag.PrevPageNo = prevPageNo;
                ViewBag.PageCount = pageCount;//总页数
                ViewBag.PageNo = crtNo;//当前页号
                ViewBag.SearchName = searchName;//搜索员工姓名
                List<int> listPage = new List<int>();
                for (int i = 1; i <= pageCount; i++)
                {
                    listPage.Add(i);
                }
                SelectList li = new SelectList(listPage, crtNo);
                ViewBag.PageList = li;

                return View(list);
            }
            else
            {
                int rowsCount = db.Users.Count();
                // int pCount = (int)Math.Ceiling(1.0 * rowsCount / pSize);//取天花板数
                // 调整当前页码不超过总页数
                int crtNo = Common.FixPageNo(pNo,pSize, rowsCount);
                crtNo = crtNo <= 0 ? 1 : crtNo;
                //总记录数
                List<User> list = db.Users.Include(x => x.AuthorityRs).OrderBy(a => a.Id).Skip((crtNo - 1) * pSize).Take(pSize).ToList();
                //总页数
                int pageCount = (rowsCount - 1) / pSize + 1;
                int nextPageNo = crtNo >= pageCount ? pageCount : crtNo + 1;//计算下一页页号
                int prevPageNo = crtNo == 1 ? 1 : crtNo - 1;//计算上一页页号
                ViewBag.NextPageNo = nextPageNo;
                ViewBag.PrevPageNo = prevPageNo;
                ViewBag.PageCount = pageCount;//总页数
                ViewBag.PageNo = crtNo;//当前页号
                ViewBag.SearchName = searchName;//搜索员工姓名
                List<int> listPage = new List<int>();
                for (int i = 1; i <= pageCount; i++)
                {
                    listPage.Add(i);
                }
                SelectList li = new SelectList(listPage, crtNo);
                ViewBag.PageList = li;

                return View(list);
            }

        }
       

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(AddUserModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var db = new Context();
            var user = db.Users.SingleOrDefault(x => x.Id == model.Id);

            if (user != null)
            {
                ModelState.AddModelError("Error", "相同工号的用户已经存在!");
                return View();
            }

            user = new User
            {
                Id = model.Id,
                Name = model.Name,
                Sex = model.Sex,
                Department = model.Department,
                Password = "123456".Md532(),
                Position = model.Position,
                AuthorityRs = model.Authorities.ToAuthorityRs()
            };

            db.Users.Add(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Modify(string userId,int pageNo)
        {
            ViewBag.Title = "Home Page";

            ViewBag.pageNo = pageNo;

            var db = new Context();

            var user = db.Users.Include(x => x.AuthorityRs).SingleOrDefault(x => x.Id == userId);

            if (user == null)
            {
                return PartialView("index");
            }

            ViewBag.Authorities = user.Authorities.ToList();

            var model = new ModifyUserModel
            {
                Id = userId,
                Name = user.Name,
                Sex = user.Sex,
                Department = user.Department,
                Position = user.Position,
                Authorities = user.Authorities,
                pageNo=pageNo
            };

            return View("Modify", model);
        }

        [HttpPost]
        public ActionResult Modify(ModifyUserModel model)
        {
            var db = new Context();
            var user = db.Users.Include(x => x.AuthorityRs)
                .SingleOrDefault(x => x.Id == model.Id);

            if (user == null) 
                return View(model);
            ViewBag.pageNo = model.pageNo;
            user.Name = model.Name;
            user.Sex = model.Sex;
            user.Department = model.Department;
            user.Position = model.Position;

            db.Authorities.RemoveRange(user.AuthorityRs);
            user.AuthorityRs = model.Authorities.ToAuthorityRs();
            foreach (var auth in user.AuthorityRs)
            {
                auth.User = user;
            }

            db.SaveChanges();
            return RedirectToAction("Index" + "/" + model.pageNo);
        }

        public ActionResult NowUser()
        {
            ViewBag.Title = "Home Page";

            var userId = Session.GetSessionUser().Id;

            var db = new Context();
            
            ViewBag.Useres = db.Users.Include(x => x.AuthorityRs).SingleOrDefault(x => x.Id == userId);
            
            var user = db.Users.Include(x => x.AuthorityRs).SingleOrDefault(x => x.Id == userId);
            
            ViewBag.Authorities = user.Authorities.ToList();
           
            return View("NowUser");
        }
        [HttpPost]
        public ActionResult NowUser(LoginModel model)
        {
            var db = new Context();
            var userId = Session.GetSessionUser().Id;

            var user = db.Users.SingleOrDefault(x => x.Id == userId);
            if (model.Password == null && model.Password0 == null && model.Password1 == null)
            {
                return Json(new { succeed = false, error = "请输入密码！" });
            }
                if (model.Password == "" && model.Password0 == "" && model.Password1 == "")
            {
                return Json(new { succeed = false, error = "请输入密码！" });
               // ModelState.AddModelError("Password1", "请输入密码!");
                //return View();
            }
            else
            {
                if (user != null && user.Password == model.Password && model.Password0 != null && model.Password0 != null)
                {
                    if (user != null && model.Password0 == model.Password1)
                    {
                        user.Password = model.Password1;
                        db.SaveChanges();
                        return Json(new { succeed = true, error = "修改成功！" });
                        //return View();
                    }
                    else
                    {
                        return Json(new { succeed = false, error = "新密码两次输入不一致，请再次输入！" });
                       // ModelState.AddModelError("Password0", "新密码两次输入不一致，请再次输入！");
                        //return View();
                    }
                }
                else
                {
                    return Json(new { succeed = false, error = "用户密码输入错误！" });
                   // ModelState.AddModelError("Password", "用户密码输入错误!");
                    //return View();
                }
            }
        }
        public ActionResult ModifyPassword()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [HttpPost]
        public ActionResult ModifyPassword(LoginModel model)
        {
            var db = new Context();
            var userId = Session.GetSessionUser().Id;

            var user = db.Users.SingleOrDefault(x => x.Id == userId);
            if (model.Password == "" && model.Password0 == ""&& model.Password1=="")
            {
                ModelState.AddModelError("Password1", "请输入密码!");
                return View("NowUser", "User");
            }
            else
            {
                if (user != null && user.Password == model.Password && model.Password0 != null && model.Password0 != null)
                {
                    if (user != null && model.Password0 == model.Password1)
                    {
                        user.Password = model.Password0;
                        db.SaveChanges();
                        //return Json(new { succeed = false, error = "该指定工艺规程不是当前用户所创建！" });
                        return View("NowUser", "User");
                    }
                    else
                    {
                        ModelState.AddModelError("Password0", "新密码两次输入不一致，请再次输入！");
                        return View("NowUser", "User");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "用户密码输入错误!");
                    return View("NowUser", "User");
                }               
            }
            
        }

        [HttpPost]
        public ActionResult DeleteUser(string id)
        {
            var db = new Context();
            var userId = Session.GetSessionUser().Id;

            var user = db.Users.SingleOrDefault(x => x.Id == id);

            db.Users.Remove(user);

            db.SaveChanges();

            return Json(new {succeed = true});
        }

        [HttpPost]
        public ActionResult ResetPassword(string id)
        {
            var db = new Context();
            var userId = Session.GetSessionUser().Id;

            var user = db.Users.SingleOrDefault(x => x.Id == id);

            user.Password = "123456".Md532();

            db.SaveChanges();

            return Json(new {succeed = true});
        }
        [HttpPost]
        public ActionResult SuccessExcel(HttpPostedFileBase file)
        {
            var dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "ExcelFiles");
            if (!dirInfo.Exists)
                dirInfo.Create();

            var fileName = AppDomain.CurrentDomain.BaseDirectory + "ExcelFiles/" + Guid.NewGuid() + ".xls";

            var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            file.InputStream.CopyTo(fs);
            fs.Close();

            try
            {
                var users = UserExcel.Import(fileName);
                var db = new Context();
                List<string> userList = new List<string>();
                foreach (var user in users)
                {
                    var userdb = db.Users.SingleOrDefault(x => x.Id == user.Id);
                    if (userdb != null)
                    {
                        //相同编号工艺规程编号的同一版本不能重复导入
                        userList.Add($"{user.Id}({user.Name})");
                        continue;
                    }
                    var userExcel = new User
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Sex = user.Sex,
                        Password = user.Password,
                        Position = user.Position,
                        AuthorityRs = user.AuthorityRs,
                        Department=user.Department
                    };
                    db.Users.Add(userExcel);
                    db.SaveChanges();
                }
               
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new {succeed = false, error = e.Message}, "text/html");
            }

            return Json(new {succeed = true}, "text/html");
        }
       
        public void GetUserPdfs(string id)
        {
            HttpContext.Response.ContentType = "application/pdf";
            List<User> list = new List<User>();
            if (id!=null)
            {
                string[] ids = id.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                var users = new Context().Users.Where(x => ids.Contains(x.Id)).ToArray();
                foreach (var user in users)
                {
                    list.Add(user);
                }               
            }
            try
            {
                HttpContext.Response.BinaryWrite(CreateUserPdfList(list));
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
            
        }
        public byte[] CreateUserPdfList(List<User> users)
        {
            try
            {
                var ms = new MemoryStream();//为系统内存提供流式的读写操作。常作为其他流数据交换时的中间对象操作
                var document = new Document(PageSize.A4, 40, 60, 40, 40);//

                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                var currentPage = 1;
                var outterTable = new PdfPTable(1)
                {
                    WidthPercentage = 100
                };

                var innerTable = new PdfPTable(1);

                foreach (var user in users)
                {
                    PdfPTable bottomTable = new PdfPTable(2);

                    StringBuilder sb = new StringBuilder();
                    sb.Append("User:");
                    sb.Append(user.Id);
                    var image = Image.GetInstance(QrCoder.Encode(sb.ToString()));
                    image.ScaleAbsolute(50, 50);
                    bottomTable.AddCell(new PdfPCell(image)
                    {
                        BorderWidthLeft = 0,
                        BorderWidthTop = 0,
                        FixedHeight = 120,
                        PaddingBottom = 6,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                    });

                    bottomTable.SetWidths(new float[] { 0.5f, 0.5f });
                    bottomTable.AddCell(new PdfPCell(new Phrase(new Phrase(user.Name.ToString() + "(" + user.Id.ToString() + ")")))
                    {
                        BorderWidthLeft = 0,
                        BorderWidthTop = 0,
                        FixedHeight = 120,
                        PaddingBottom = 6,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                    });

                    innerTable.AddCell(new PdfPCell(bottomTable)
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        BorderWidthTop = 2,
                        BorderWidthBottom = 0,
                    });

                }
                outterTable.AddCell(new PdfPCell(innerTable)
                {
                    PaddingLeft = 10,
                    PaddingRight = 5,
                    PaddingBottom = 10,
                    PaddingTop = 10
                });
                document.Add(outterTable);
                document.NewPage();
                currentPage++;
                document.Close();
                return ms.ToArray();
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return null;
            }
           

        }

        public void ImportUserExcel(string userIdes)
        {
            try
            {
                string fileName = string.Empty;
                fileName = DateTime.Now.ToString() + ".xls";
                string[] ids = userIdes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);//剔除空值
                ExportExcelForPercentForWebs(fileName, fileName, ids);
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
        }
        public static void ExportExcelForPercentForWebs(string sheetName, string xlsname, string[] areaid)
        {

            XlsDocument xls = new XlsDocument();
            DataTable table = GetDataTableForPercents(areaid);
            Worksheet sheet;
            try
            {
                sheet = xls.Workbook.Worksheets.Add("员工信息表");
                
                if (table == null || table.Rows.Count == 0) { return; }

                //填充表头  
                foreach (DataColumn col in table.Columns)
                {
                    sheet.Cells.Add(1, col.Ordinal + 1, col.ColumnName);
                }

                //填充内容  
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        sheet.Cells.Add(i + 2, j + 1, table.Rows[i][j].ToString());
                    }
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    xls.Save(ms);
                    ms.Flush();
                    ms.Position = 0;
                    sheet = null;
                    xls = null;
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();

                    response.Charset = "UTF-8";
                    response.ContentType = "application/vnd-excel";
                    System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + xlsname));

                    byte[] data = ms.ToArray();
                    System.Web.HttpContext.Current.Response.BinaryWrite(data);
                }

            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
            finally
            {
                sheet = null;
                xls = null;
            }

        }

        private static DataTable GetDataTableForPercents(string[] areaid)
        {
            var db = new Context();
            
            var userdbs = db.Users.Include(x => x.AuthorityRs).Where(x => areaid.Contains(x.Id)).ToArray();
            
            DataTable dt = new DataTable();
            dt.Columns.Add("企业代码", typeof(String));
            dt.Columns.Add("姓名", typeof(String));
            dt.Columns.Add("性别", typeof(String));
            dt.Columns.Add("所在部门", typeof(String));
            dt.Columns.Add("岗位", typeof(String));
            dt.Columns.Add("权限", typeof(String));
            try
            {
                foreach (var user in userdbs)
                {                    
                    DataRow dr=dt.NewRow();

                    dr["企业代码"] = user.Id;
                    dr["姓名"] = user.Name;
                    dr["性别"] = user.Sex;
                    dr["所在部门"] = user.Department;
                    dr["岗位"] = user.Position.ToString();
                    string author = "";
                    List<Authority> auths = user.Authorities.ToList();
                    foreach (Authority auth in Enum.GetValues(typeof(Authority)))
                    {
                        if (auths.Contains(auth))
                        {
                            author += auth.ToString() + ",";
                        }                        
                    }                    
                    dr["权限"] = author;
                    dt.Rows.Add(dr);
                }                
                return dt;
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return null;
            }

        }

    }
}
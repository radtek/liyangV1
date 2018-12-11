using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf.qrcode;
using WCAPP.Attributes;
using WCAPP.Models.Database;
using WCAPP.Models.ProcedureModels;
using WCAPP.Models.ProcessModels;
using WCAPP.Types;
using WCAPP.Utils;

namespace WCAPP.Controllers
{
    public class ProcedureController : Controller
    {
        public ActionResult Creating(int id)
        {
            var db = new Context();
            var userId = Session.GetSessionUser().Id;
            var procedure = db.Procedures
                .Include(x => x.Process)
                .Include(x => x.Seams)
                .Include("Seams.InitialParams")
                .SingleOrDefault(x => x.Id == id && x.ApprovalState != ApprovalState.审核通过);

            ViewBag.Procedure = procedure;
            ViewBag.ProcessId = procedure.Process.Id;
            ViewBag.Editable = procedure != null && (procedure.Process.ApprovalState == ApprovalState.审核不通过 ||
                                                     procedure.Process.ApprovalState == ApprovalState.未提交审核);

            return View();
        }//ReviseCreating
        public ActionResult ReviseCreating(int id)
        {
            var db = new Context();
            var userId = Session.GetSessionUser().Id;
            var procedure = db.Procedures
                .Include(x => x.Process)
                .Include(x => x.Seams)
                .Include("Seams.InitialParams")
                 .Include("Seams.RevisedParams")
                .SingleOrDefault(x => x.Id == id && x.ApprovalState != ApprovalState.审核通过);

            ViewBag.Procedure = procedure;
            ViewBag.ProcessId = procedure.Process.Id;
            ViewBag.Editable = procedure != null && (procedure.Process.ApprovalState == ApprovalState.审核不通过 ||
                                                     procedure.Process.ApprovalState == ApprovalState.未提交审核);

            return View();
        }
        [Authority(Authority.编制)]
        public ActionResult Detail(int id)
        {
            var db = new Context();
            var procedure = db.Procedures
                .Include(x => x.Process)
                .Include("Seams.InitialParams")
                .Include("Seams.RevisedParams")
                .SingleOrDefault(x => x.Id == id && x.ApprovalState != ApprovalState.审核通过);

            ViewBag.Procedure = procedure;
            ViewBag.ProcessId = procedure.Process.Id;
            return View();
        }
        public ActionResult CheckReviseEditParam(int sid)
        {
            var db = new Context();
            var seam = db.Seams
                .Include(x => x.Procedure)
                .Include(x => x.InitialParams)
                .Include(x => x.RevisedParams)
                .SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return PartialView("_ReviseEditParam");

            ViewBag.WeldMethod = seam.Procedure.WeldMethod;

            if (!seam.InitialParams.Any())
                return PartialView("_ReviseEditParam");

            var model = new EditParamModel();

            var paramDict = seam.InitialParams.ToDictionary(param => param.Enum, param => param.Value);
            
            foreach (var prop in model.GetType().GetProperties())
            {
                if (!(prop.Name.Contains("原因") || prop.Name.Contains("原因序号")))
                {
                    prop.SetValue(model, paramDict.GetValue(prop.Name.ToEnum<WeldParam>()), null);
                }

            }

            return PartialView("_ReviseEditParam", model);
        }
        public ActionResult ReviseEditParam(int sid)
        {
            var db = new Context();
            var seam = db.Seams
                .Include(x => x.Procedure)
                .Include(x => x.InitialParams)
                .Include(x => x.RevisedParams)
                .SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return PartialView("_ReviseEditParam");

            ViewBag.WeldMethod = seam.Procedure.WeldMethod;

            if (!seam.InitialParams.Any())
                return PartialView("_ReviseEditParam");

            var model = new EditParamModel();

            var paramDict = seam.InitialParams.ToDictionary(param => param.Enum, param => param.Value);

           
            foreach (var prop in model.GetType().GetProperties())
            {
                if (!(prop.Name.Contains("原因") || prop.Name.Contains("原因序号")))
                {
                    prop.SetValue(model, paramDict.GetValue(prop.Name.ToEnum<WeldParam>()), null);
                }

            }

            return PartialView("_ReviseEditParam", model);
        }

        [HttpPost]
        public ActionResult ReviseEditParam(int sid, EditParamModel model)
        {
            var db = new Context();
            var seam = db.Seams
                .Include(x => x.Procedure)
                .Include(x => x.InitialParams)
                .SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return PartialView("_EditParam");

            if (!seam.InitialParams.Any())
            {
                seam.InitialParams.AddRange(GlobalData.InitialMethodParams(seam.Procedure.WeldMethod));
            }

            var props = model.GetType().GetProperty("原因");
            seam.Reason = (string)props.GetValue(model, null);

            var propsNo = model.GetType().GetProperty("原因序号");
            seam.ReasonNo = (string)propsNo.GetValue(model, null);

            foreach (var param in seam.InitialParams)
            {
                var t = model.GetType();
                if (seam.Procedure.WeldMethod == WeldMethod.高频钎焊)
                {
                    if (param.Enum == WeldParam.填充材料牌号)
                    {
                        param.Enum = WeldParam.钎料牌号;
                    }
                    if (param.Enum == WeldParam.填充材料规格)
                    {
                        param.Enum = WeldParam.钎料规格;
                    }
                }
                if (seam.Procedure.WeldMethod == WeldMethod.氩弧焊)
                {
                    if (param.Enum == WeldParam.钎料牌号)
                    {
                        param.Enum = WeldParam.填充材料牌号;
                    }
                    if (param.Enum == WeldParam.钎料规格)
                    {
                        param.Enum = WeldParam.填充材料规格;
                    }
                }
                var prop = t.GetProperty(param.Enum.ToString());

                param.Value = (string)prop.GetValue(model, null);

            }
            // seam.Reason=model.
            db.SaveChanges();
            return Json(new { succeed = true });
        }
        public ActionResult EditParam(int sid)
        {
            var db = new Context();
            var seam = db.Seams
                .Include(x => x.Procedure)
                .Include(x => x.InitialParams)
                .SingleOrDefault(x => x.Id == sid);
            //var seam = db.Seams
            //    .Include(x => x.Procedure)
            //    .SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return PartialView("_EditParam");

            ViewBag.WeldMethod = seam.Procedure.WeldMethod;
            ViewBag.ResistType = seam.Procedure.ResistType;

            if (seam.InitialParams?.Any() != true)
                return PartialView("_EditParam");

            var model = new EditParamModel();
            var paramDict = seam.InitialParams.ToDictionary(param => param.Enum, param => param.Value);

            foreach (var prop in model.GetType().GetProperties())
            {
                if (!(prop.Name.Contains("原因") || prop.Name.Contains("原因序号")))
                {
                    prop.SetValue(model, paramDict.GetValue(prop.Name.ToEnum<WeldParam>()), null);
                }
            }

            return PartialView("_EditParam", model);
        }

        [HttpPost]
        public ActionResult EditParam(int sid, EditParamModel model)
        {
            var db = new Context();
            var seam = db.Seams
                .Include(x => x.Procedure)
                .Include(x => x.InitialParams)
                .SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return PartialView("_EditParam");

            if (!seam.InitialParams.Any())
            {
                seam.InitialParams.AddRange(GlobalData.InitialMethodParams(seam.Procedure.WeldMethod));
            }

            foreach (var param in seam.InitialParams)
            {
                var t = model.GetType();
                if (seam.Procedure.WeldMethod == WeldMethod.高频钎焊)
                {
                    if (param.Enum == WeldParam.填充材料牌号)
                    {
                        param.Enum = WeldParam.钎料牌号;
                    }
                    if (param.Enum == WeldParam.填充材料规格)
                    {
                        param.Enum = WeldParam.钎料规格;
                    }
                }
                if (seam.Procedure.WeldMethod == WeldMethod.氩弧焊)
                {
                    if (param.Enum == WeldParam.钎料牌号)
                    {
                        param.Enum = WeldParam.填充材料牌号;
                    }
                    if (param.Enum == WeldParam.钎料规格)
                    {
                        param.Enum = WeldParam.填充材料规格;
                    }
                }
                var prop = t.GetProperty(param.Enum.ToString());
                var prop2 = t.GetProperty(param.Enum.ToString() + "yy");
                param.Value = (string)prop.GetValue(model, null);
            }

            db.SaveChanges();
            return Json(new { succeed = true });
        }

        [HttpPost]
        public ActionResult GenParam(int sid)
        {
            var db = new Context();

            var seam = db.Seams.Include(x => x.InitialParams).SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return Json(new { succeed = false, sid, error = "指定焊缝不存在" });
            if (seam.InitialParams.Any())
                return Json(new { succeed = false, sid, error = "指定焊缝已经有参数了" });

            try
            {
                var seam2s = db.Seams
                    .Include(x => x.Procedure.Process)
                    .Where(x => x.Procedure.Process.Published &&
                            x.CheckStandard == seam.CheckStandard &&
                            x.SeamLevel == seam.SeamLevel &&
                            x.JointForm == seam.JointForm &&
                            x.FillWire == seam.FillWire &&
                            x.BaseCount == seam.BaseCount &&
                            x.Gap == seam.Gap &&
                            x.WeldMachineClass == seam.WeldMachineClass &&
                            x.ElectrodeDiameter == seam.ElectrodeDiameter)
                    .OrderByDescending(x => x.Id)
                    .ToList();

                if (!seam2s.Any())
                    return Json(new { succeed = false, sid });

                var seam2 = seam2s.FirstOrDefault(x => x.SimilarTo(seam));

                if (seam2 == null)
                    return Json(new { succeed = false, sid });

                seam2 = db.Seams
                    .Include(x => x.InitialParams)
                    .Include(x => x.RevisedParams)
                    .Single(x => x.Id == seam2.Id);

                if (!seam2.InitialParams.Any())
                    return Json(new { succeed = false, sid });

                if (seam2.RevisedParams.Any())
                    seam.InitialParams.AddRange(seam2.RevisedParams.Select(x => new SeamParam1
                    {
                        Enum = x.Enum,
                        Value = x.Value,
                        Seam = seam
                    }));
                else
                    seam.InitialParams.AddRange(seam2.InitialParams.Select(x => new SeamParam1
                    {
                        Enum = x.Enum,
                        Value = x.Value,
                        Seam = seam
                    }));

                db.SaveChanges();

                return Json(new { succeed = true, sid });
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return Json(new { succeed = false, sid, error = e.Message });
            }

        }

        public ActionResult AddSeam(bool isResist, bool isPoint)
        {
            ViewBag.IsResist = isResist;
            ViewBag.IsPoint = isPoint;
            return PartialView("_AddSeam");
        }
        public ActionResult AddSeams(bool isResist, bool isPoint)
        {
            ViewBag.IsResist = isResist;
            ViewBag.IsPoint = isPoint;
            return PartialView("_AddSeams");
        }

        [HttpPost]
        public ActionResult AddSeam(AddSeamModel model, int prid)
        {
            var db = new Context();
            var userId = Session.GetSessionUser().Id;

            var procedure = db.Procedures.Include(x => x.Seams).SingleOrDefault(x => x.Id == prid);

            if (procedure == null)
            {
                return Json(new { succeed = false, error = "指定工序不存在" });
            }

            if (model.JointForm != JointForm.堆焊)
            {
                if (!ModelState.IsValid)
                {
                    if (procedure.WeldMethod == WeldMethod.氩弧焊)
                    {
                        if (procedure.AutoLevel == AutoLevel.手工)
                        {

                            return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.手工);

                        }
                        if (procedure.AutoLevel == AutoLevel.自动)
                        {
                            return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.自动);

                        }
                    }
                    if (procedure.WeldMethod == WeldMethod.电阻焊)
                    {
                        return AddSeam(procedure.WeldMethod == WeldMethod.电阻焊, procedure.ResistType == ResistType.点焊);
                    }
                    if (procedure.WeldMethod == WeldMethod.电子束焊)
                    {
                        return AddSeam(false, false);
                    }
                    if (procedure.WeldMethod == WeldMethod.高频钎焊)
                    {
                        return AddSeam(false, false);
                    }

                }

                if (model.Material3 != null && model.Material3.Trim() != "" && model.Thick3 == null)
                {
                    ModelState.AddModelError("Thick3", "Thick3 Required");

                    if (procedure.WeldMethod == WeldMethod.氩弧焊)
                    {
                        if (procedure.AutoLevel == AutoLevel.手工)
                        {
                            return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.手工);
                        }
                        if (procedure.AutoLevel == AutoLevel.自动)
                        {
                            return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.自动);
                        }

                    }
                    if (procedure.WeldMethod == WeldMethod.电阻焊)
                    {
                        return AddSeam(procedure.WeldMethod == WeldMethod.电阻焊, procedure.ResistType == ResistType.点焊);
                    }
                }

                if (model.Thick3 != null && (model.Material3 == null || model.Material3.Trim() == ""))
                {
                    ModelState.AddModelError("Material3", "Material3 Required");

                    if (procedure.WeldMethod == WeldMethod.氩弧焊)
                    {
                        if (procedure.AutoLevel == AutoLevel.手工)
                        {
                            return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.手工);
                        }
                        if (procedure.AutoLevel == AutoLevel.自动)
                        {
                            return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.自动);
                        }

                    }
                    if (procedure.WeldMethod == WeldMethod.电阻焊)
                    {
                        return AddSeam(procedure.WeldMethod == WeldMethod.电阻焊, procedure.ResistType == ResistType.点焊);
                    }
                }
                if (model.Material4 != null && model.Material4.Trim() != "" && model.Thick4 == null)
                {
                    ModelState.AddModelError("Thick4", "Thick4 Required");

                    if (procedure.WeldMethod == WeldMethod.氩弧焊)
                    {
                        if (procedure.AutoLevel == AutoLevel.手工)
                        {
                            return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.手工);
                        }
                        if (procedure.AutoLevel == AutoLevel.自动)
                        {
                            return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.自动);
                        }

                    }
                    if (procedure.WeldMethod == WeldMethod.电阻焊)
                    {
                        return AddSeam(procedure.WeldMethod == WeldMethod.电阻焊, procedure.ResistType == ResistType.点焊);
                    }
                }

                if (model.Thick4 != null && (model.Material4 == null || model.Material4.Trim() == ""))
                {
                    ModelState.AddModelError("Material4", "Material4 Required");

                    if (procedure.WeldMethod == WeldMethod.氩弧焊)
                    {
                        if (procedure.AutoLevel == AutoLevel.手工)
                        {
                            return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.手工);
                        }
                        if (procedure.AutoLevel == AutoLevel.自动)
                        {
                            return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.自动);
                        }

                    }
                    if (procedure.WeldMethod == WeldMethod.电阻焊)
                    {
                        return AddSeam(procedure.WeldMethod == WeldMethod.电阻焊, procedure.ResistType == ResistType.点焊);
                    }
                }

            }



            if (procedure.Seams.Exists(x => x.No == model.No))
            {
                ModelState.AddModelError("Error", "相同编号的焊缝已经存在");

                if (procedure.WeldMethod == WeldMethod.氩弧焊)
                {
                    if (procedure.AutoLevel == AutoLevel.手工)
                    {
                        return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.手工);
                    }
                    if (procedure.AutoLevel == AutoLevel.自动)
                    {
                        return AddSeams(procedure.WeldMethod == WeldMethod.氩弧焊, procedure.AutoLevel == AutoLevel.自动);
                    }

                }
                if (procedure.WeldMethod == WeldMethod.电阻焊)
                {
                    return AddSeam(procedure.WeldMethod == WeldMethod.电阻焊, procedure.ResistType == ResistType.点焊);
                }
                if (procedure.WeldMethod == WeldMethod.电子束焊)
                {
                    return AddSeam(false, false);
                }
                if (procedure.WeldMethod == WeldMethod.高频钎焊)
                {
                    return AddSeam(false, false);
                }
            }

            var seam = new Seam
            {
                No = model.No,
                TestState = TestState.尚未进行,
                JointForm = model.JointForm,
                SeamLevel = model.SeamLevel,
                CheckStandard = model.CheckStandard,
                Material1 = model.Material1,
                Material2 = model.Material2,
                Material3 = model.Material3,
                Material4 = model.Material4,
                Thick1 = model.Thick1,
                Thick2 = model.Thick2,
                Thick3 = model.Thick3,
                Thick4 = model.Thick4,
                Gap = model.Gap,
                WeldMachineClass = model.WeldMachineClass,
                ElectrodeDiameter = model.ElectrodeDiameter
            };

            procedure.Seams.Add(seam);
            db.SaveChanges();

            return Json(new { succeed = true });
        }

        public ActionResult HistoryDetail(int id, string ver)
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [HttpPost]
        public ActionResult StartTest(int sid)
        {
            var db = new Context();
            var seam = db.Seams.SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return Json(new { error = "指定焊缝不存在" });
            if (seam.TestState != TestState.尚未进行)
                return Json(new { error = "试焊已经开始" });
            seam.TestState = TestState.进行中;
            db.SaveChanges();
            return Json(new { succeed = true });
        }

        public ActionResult FinishTest(int sid)
        {
            var db = new Context();

            //操作者
            var oerators = db.Users.Include(m => m.AuthorityRs).ToList();
            List<User> user = new List<User>();
            foreach (var oerator in oerators)
            {
                foreach (var item in oerator.AuthorityRs)
                {
                    if (item.Authority == Authority.试焊)
                    {
                        user.Add(oerator);
                    }
                }
            }
            ViewBag.User = user;
            SelectList lints = new SelectList(user, 1);
            ViewBag.Oerators = lints;
            ViewBag.SID = sid;
            return PartialView("_FinishTest");
        }

        /// 将 Stream 转成 byte[]

        public byte[] StreamToBytes(Stream stream)

        {
            try
            {
                byte[] bytes = new byte[stream.Length];

                stream.Read(bytes, 0, bytes.Length);

                // 设置当前流的位置为流的开始 

                stream.Seek(0, SeekOrigin.Begin);

                return bytes;
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return null;
            }



        }
        [HttpPost]
        public ActionResult FinishTest(int sid, FinishTestModel model, HttpPostedFileBase file)
        {
            if (file == null)
            {
                return Json(new { succeed = false, error = "检验报告为空！" });
            }
            byte[] bytes = StreamToBytes(file.InputStream);

            if (model.Checker == "")
            {
                return Json(new { succeed = false, error = "检验者不能为空！" });
            }
            if (model.Welder == "")
            {
                return Json(new { succeed = false, error = "操作者不能为空！" });
            }
            if (model.ReportNo == "")
            {
                return Json(new { succeed = false, error = "报告编号不能为空！" });
            }
            var db = new Context();
            var seam = db.Seams
                .Include(x => x.Procedure.Process)
                .SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return Json(new { succeed = false, error = "指定焊缝不存在" });

            if (seam.TestState == TestState.已完成)
                return Json(new { succeed = false, error = "指定焊缝已经完成试焊" });
            if (model.Welder == model.Checker)
            {
                return Json(new { succeed = false, error = "检验者和操作者不能同是一个人" });
            }
            seam.TestState = TestState.已完成;
            seam.TestWelder = model.Welder;
            seam.TestChecker = model.Checker;
            seam.SpecialReportFileNo = model.ReportNo;
            seam.TestByteFile = bytes;

            var seams = db.Seams.Where(x => x.Id != sid && x.ProcedureId == seam.ProcedureId);
            var allready = true;
            foreach (var s in seams)
            {
                if (s.TestState == TestState.已完成)
                    continue;
                allready = false;
                break;
            }

            if (allready)
            {
                seam.Procedure.TestState = ProgramTestState.已完成;

                var procedures =
                    db.Procedures.Where(x => x.Id != seam.Procedure.Id && x.ProcessId == seam.Procedure.ProcessId);

                foreach (var p in procedures)
                {
                    if (p.TestState == ProgramTestState.已完成)
                        continue;
                    allready = false;
                    break;
                }
                if (allready)
                    seam.Procedure.Process.TestState = ProgramTestState.已完成;
            }

            db.SaveChanges();
            return Json(new { succeed = true });
        }
        public void CheckPDF(int seamId)
        {
            try
            {
                var db = new Context();
                var seam = db.Seams
                    .Include(x => x.Procedure)
                    .Include(x => x.InitialParams)
                    .Include(x => x.RevisedParams)
                    .SingleOrDefault(x => x.Id == seamId);
                if (seam == null)
                {
                    throw new Exception("指定ID的焊缝不存在");
                }
                byte[] bytePdf = seam.TestByteFile;

                if (bytePdf != null)
                {
                    HttpContext.Response.ContentType = "application/pdf";
                    HttpContext.Response.BinaryWrite(bytePdf);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }


        }
        public ActionResult ModifyParam(int sid)
        {
            var db = new Context();
            var seam = db.Seams
                .Include(x => x.Procedure)
                .Include(x => x.InitialParams)
                .Include(x => x.RevisedParams)
                .SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return PartialView("_ModifyParam");

            ViewBag.WeldMethod = seam.Procedure.WeldMethod;

            if (!seam.InitialParams.Any())
                return PartialView("_ModifyParam");

            var model = new EditParamModel();
            var paramDict = seam.RevisedParams.Any()
                ? seam.RevisedParams.ToDictionary(param => param.Enum, param => param.Value)
                : seam.InitialParams.ToDictionary(param => param.Enum, param => param.Value);

            foreach (var prop in model.GetType().GetProperties())
            {
                if (!(prop.Name.Contains("原因") || prop.Name.Contains("原因序号")))
                {
                    prop.SetValue(model, paramDict.GetValue(prop.Name.ToEnum<WeldParam>()), null);
                }
            }

            return PartialView("_ModifyParam", model);
        }

        [HttpPost]
        public ActionResult ModifyParam(int sid, EditParamModel model)
        {
            var db = new Context();
            var seam = db.Seams
                .Include(x => x.Procedure)
                .Include(x => x.InitialParams)
                .Include(x => x.RevisedParams)
                .SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return PartialView("_ModifyParam");

            if (!seam.InitialParams.Any())
            {
                return Json(new { error = "没有初始值" });
            }

            if (!seam.RevisedParams.Any())
            {
                seam.RevisedParams.AddRange(seam.InitialParams.Select(x =>
                    new SeamParam2 { Enum = x.Enum, Value = x.Value }));
            }

            foreach (var param in seam.RevisedParams)
            {
                var t = model.GetType();
                var prop = t.GetProperty(param.Enum.ToString());
                param.Value = (string)prop.GetValue(model, null);
            }

            db.SaveChanges();
            return Json(new { succeed = true });
        }
        public ActionResult ReviseModifyParam(int sid)
        {
            var db = new Context();
            var seam = db.Seams
                .Include(x => x.Procedure)
                .Include(x => x.InitialParams)
                .Include(x => x.RevisedParams)
                .SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return PartialView("_ReviseModifyParam");

            ViewBag.WeldMethod = seam.Procedure.WeldMethod;

            if (!seam.InitialParams.Any())
                return PartialView("_ReviseModifyParam");

            var model = new EditParamModel();
            var paramDict = seam.RevisedParams.Any()
                ? seam.RevisedParams.ToDictionary(param => param.Enum, param => param.Value)
                : seam.InitialParams.ToDictionary(param => param.Enum, param => param.Value);

            foreach (var prop in model.GetType().GetProperties())
            {
                if (!(prop.Name.Contains("原因") || prop.Name.Contains("原因序号")))
                {
                    prop.SetValue(model, paramDict.GetValue(prop.Name.ToEnum<WeldParam>()), null);
                }
                if (prop.Name.Contains("原因序号") && seam.ReasonNo != null)
                {
                    model.原因序号 = seam.ReasonNo;
                }
                if (prop.Name.Contains("原因") && seam.Reason != null)
                {
                    model.原因 = seam.Reason;
                }

            }
            return PartialView("_ReviseModifyParam", model);
        }

        [HttpPost]
        public ActionResult ReviseModifyParam(int sid, EditParamModel model)
        {
            var db = new Context();
            var seam = db.Seams
                .Include(x => x.Procedure)
                .Include(x => x.InitialParams)
                .Include(x => x.RevisedParams)
                .SingleOrDefault(x => x.Id == sid);
            if (seam == null)
                return PartialView("_ReviseModifyParam");

            if (!seam.InitialParams.Any())
            {
                return Json(new { error = "没有初始值" });
            }

            if (!seam.RevisedParams.Any())
            {
                seam.RevisedParams.AddRange(seam.InitialParams.Select(x =>
                    new SeamParam2 { Enum = x.Enum, Value = x.Value }));
            }

            foreach (var param in seam.RevisedParams)
            {
                var t = model.GetType();
                var prop = t.GetProperty(param.Enum.ToString());
                param.Value = (string)prop.GetValue(model, null);
            }
            var props = model.GetType().GetProperty("原因");
            seam.Reason = (string)props.GetValue(model, null);

            var propsNo = model.GetType().GetProperty("原因序号");
            seam.ReasonNo = (string)propsNo.GetValue(model, null);

            db.SaveChanges();
            return Json(new { succeed = true });
        }
    }
}
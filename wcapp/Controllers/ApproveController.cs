using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WCAPP.Models.ApproveModels;
using WCAPP.Models.Database;
using WCAPP.Models.ProcessModels;
using WCAPP.Sevices;
using WCAPP.Types;
using WCAPP.Utils;

namespace WCAPP.Controllers
{
    public class ApproveController : Controller
    {
        public ActionResult Submit()
        {
            ViewBag.Approvers = new UserManager().GetApprovers()
                .Select(x => new SelectListItem {Value = x.Id, Text = x.Name + "(" + x.Id + ")"}).ToList();
            return PartialView("_SubmitApprove");
        }

        [HttpPost]
        public ActionResult Submit(SubmitApproveModel model, int pid)
        {
            if (!ModelState.IsValid)
                return Submit();

            try
            {
                var db = new Context();
                var process = db.Processes.Include("Procedures.Seams.InitialParams").SingleOrDefault(x => x.Id == pid);

                if (process == null)
                    throw new Exception("指定工艺规程不存在");

                var set = new HashSet<string>();
                var approvers = new[] {model.ProoferId, model.ApproverId};
                var userId = Session.GetSessionUser().Id;
                foreach (var it in approvers)
                {
                    if (it == userId)
                        throw new Exception("不能提交给自己审核");

                    if (set.Contains(it))
                        throw new Exception("存在相同的审核人");

                    var usr = db.Users.Include(x => x.AuthorityRs).SingleOrDefault(x => x.Id == it);
                    if (usr == null)
                        throw new Exception("所选审核人不存在");

                    if (!usr.Authorities.Contains(Authority.审核))
                        throw new Exception("所选审核人没有审核权限");

                    set.Add(it);
                }

                if (process.ApprovalState == ApprovalState.审核中)
                    throw new Exception("指定工艺规程已经处于审核中");
                if (process.ApprovalState == ApprovalState.审核通过)
                    throw new Exception("指定工艺规程已经审核通过，不能再提交审核");

                if (!process.Procedures.Any())
                    throw new Exception("指定工艺规程没有工序，不能提交审核");

                foreach (var procedure in process.Procedures)
                {
                    if(!procedure.Seams.Any())
                        throw new Exception("指定工艺规程存在无焊缝的工序，不能提交审核");
                    foreach (var seam in procedure.Seams)
                    {
                        if(!seam.InitialParams.Any())
                            throw new Exception("指定工艺规程存在无参数的焊缝，不能提交审核");
                    }
                }

                process.ApprovalState = ApprovalState.审核中;
                db.Approves.Add(new Approve
                {
                    SubmiterId = userId,
                    ProoferId = approvers[0],
                    ApproverId = approvers[1],
                    Process = process,
                    CurrenterId = approvers[0],
                    ApproveState = ApproveState.进行中
                });

                db.SaveChanges();
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                ModelState.AddModelError("Error", e.Message);
                return Submit();
            }

            return Json(new {succeed = true});
        }

        public ActionResult Waiting()
        {
            var db = new Context();
            var usrid = Session.GetSessionUser().Id;
            ViewBag.Approves = db.Approves.Include(x => x.Process).Where(x => x.CurrenterId == usrid).ToList();

            return View();
        }

        public ActionResult Approve(int id)
        {
            var db = new Context();
            ViewBag.Approve = db.Approves.Include("Process.Procedures").SingleOrDefault(x => x.Id == id);

            return View();
        }

        [HttpPost]
        public ActionResult Approve(int id, ApproveModel model)
        {
            var db = new Context();
            var appr = db.Approves.Include("Process").SingleOrDefault(x => x.Id == id);
            var usrid = Session.GetSessionUser().Id;

            if (appr == null)
                return Json(new {error = "指定审批流程不存在"});

            if (usrid != appr.CurrenterId)
                return Json(new {error = "您不是该工艺规程的审批者"});

            if (usrid == appr.ProoferId)
            {
                if (!model.Agree)
                {
                    appr.CurrenterId = null;
                    appr.ApproveState = ApproveState.未通过;
                    appr.Interrupt = true;
                }
                else
                {
                    appr.CurrenterId = appr.ApproverId;
                }
                appr.ProofNote = model.Note;
            }
            else
            {
                appr.CurrenterId = null;
                appr.ApproveNote = model.Note;
                appr.ApproveState = model.Agree ? ApproveState.通过 : ApproveState.未通过;
            }

            if (appr.ApproveState == ApproveState.未通过)
                appr.Process.ApprovalState = ApprovalState.审核不通过;
            if (appr.ApproveState == ApproveState.通过)
                appr.Process.ApprovalState = ApprovalState.审核通过;

            db.SaveChanges();

            return Json(new {succeed = true});
        }

        public ActionResult Detail(int pid)
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}
using WCAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using WCAPP.Utils;
using WCAPP.Libs;
using WCAPP.Types;
using WCAPP.Models.Database;

namespace WCAPP.Sevices
{
    public class WpsManager
    {
        public Wps[] ListAllWps()
        {
            return new Context().Wpses
                .ToArray();
        }

        public Wps[] ListWps(string userId)
        {
            return new Context().Wpses
                .ToArray();
        }

        public Wps GetWps(int wpsId)
        {
            var wps = new Context().Wpses.SingleOrDefault(x => x.Id == wpsId);

            if (wps == null)
                throw new Exception("指定ID的工艺规程不存在");

            return wps;
        }

        public void GetWpsPdf(int wpsId)
        {
            HttpContext.Current.Response.ContentType = "application/pdf";
            var wps = new Context().Wpses.Include(x => x.ParamList).SingleOrDefault(x => x.Id == wpsId);

            if (wps == null)
                throw new Exception("指定ID的工艺规程不存在");

            HttpContext.Current.Response.BinaryWrite(new byte[] {0});
        }

        public Wps GetWpsDetail(int wpsId)
        {
            var wps = new Context().Wpses
                .Include(x => x.ParamList)
                .SingleOrDefault(x => x.Id == wpsId);

            if (wps == null)
                throw new Exception("指定ID的工艺规程不存在");

            return wps;
        }

        public Wps[] GetAllWpsDetail()
        {
            return new Context().Wpses
                .Include(x => x.ParamList)
                .ToArray();
        }

        public Wps GenWps(Wps input)
        {
            return new Deduction().GenWps(input);
        }


        public void CreateWps(string userId, Wps wps)
        {
            Context db = new Context();
            wps.MakeTime = DateTime.Now;
            wps.CurrTestSeqNo = null;
            wps.Tests = null;
            wps.TestState = TestState.尚未进行;
            if (wps.CloneFrom != null)
            {
                var w = new Context().Wpses
                    .Include(x => x.ParamList)
                    .SingleOrDefault(x => x.Id == wps.CloneFrom);

                if (w == null)
                    throw new Exception("用于复制的源工艺规程不存在");

                wps.ParamList = w.CloneParams();
            }

            db.Wpses.Add(wps);
            db.SaveChanges();
        }

        public Wps PdmGetWpsDetail(string pdmToken, int wpsId)
        {
            if (pdmToken != GlobalData.Token)
                throw new Exception("没有PDM权限");
            return GetWpsDetail(wpsId);
        }

        public Wps PdmGenWps(string pdmToken, Wps input)
        {
            if (pdmToken != GlobalData.Token)
                throw new Exception("没有PDM权限");
            return GenWps(input);
        }

        public void PdmCreateWps(string pdmToken, string pdmUserId, Wps wps)
        {
            if (pdmToken != GlobalData.Token)
                throw new Exception("没有PDM权限");
            CreateWps(pdmUserId, wps);
        }

        public Wps PdmGetWpsDetailByJointKey(string pdmToken, string partNo, string procNo)
        {
            if (pdmToken != GlobalData.Token)
                throw new Exception("没有PDM权限");

            var wps = new Context().Wpses
                .Include(x => x.ParamList)
                .SingleOrDefault(x => x.PartNo == partNo && x.ProcNo == procNo);

            return wps;
        }

        public string PdmGetWpsPdf(string pdmToken, int wpsId)
        {
            if (pdmToken != GlobalData.Token)
                throw new Exception("没有PDM权限");

            var wps = new Context().Wpses.Include(x => x.ParamList).SingleOrDefault(x => x.Id == wpsId);

            if (wps == null)
                throw new Exception("指定ID的工艺规程不存在");

            if (wps.ApprovalState != ApprovalState.审核通过)
                throw new Exception("指定工艺规程还未通过审核");

            return new byte []{0}.Base64();
        }
    }
}
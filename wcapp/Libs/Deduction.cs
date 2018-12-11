using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using WCAPP.Types;
using WCAPP.Models.Database;

namespace WCAPP.Libs
{
    public class Deduction
    {
        public Wps GenWps(Wps input)
        {
            Context db = new Context();
            var wps = db.Wpses
                .Include(x => x.ParamList)
                .Where(o => o.JointForm == input.JointForm &&
                            o.ApprovalState == ApprovalState.审核通过 &&
                            o.WeldMethod == input.WeldMethod &&
                            o.ProcName == input.ProcName &&
                            ((o.Material1 == input.Material1 &&
                            o.Material2 == input.Material2 &&
                            o.Thick1 == input.Thick1 &&
                            o.Thick2 == input.Thick2) ||
                            (o.Material1 == input.Material2 &&
                            o.Material2 == input.Material1 &&
                            o.Thick1 == input.Thick2 &&
                            o.Thick2 == input.Thick1)))
                            .FirstOrDefault();

            if (wps != null)
            {
                input.CloneFrom = wps.Id;
                input.ParamList = wps.ParamList;
            }

            return input;
        }
    }
}
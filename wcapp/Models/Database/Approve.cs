using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WCAPP.Types;

namespace WCAPP.Models.Database
{
    public class Approve
    {
        public int Id { get; set; }
        public string SubmiterId { get; set; }        //提交人
        public string ProoferId { get; set; }         //校对
        public string ApproverId { get; set; }        //审核
        public string CurrenterId { get; set; }       //当前处理人
        public DateTime? SubmitedTime { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public int ProcessId { get; set; }
        public Process Process { get; set; }
        public ApproveState ApproveState { get; set; }
        public string ProofNote { get; set; }
        public string ApproveNote { get; set; }
        public bool Interrupt { get; set; }            //标识是否在校对过程中就被驳回
    }
}
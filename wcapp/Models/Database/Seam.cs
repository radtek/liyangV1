using WCAPP.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using WCAPP.Utils;

namespace WCAPP.Models.Database
{
    public class Seam
    {
        public int Id { get; set; }

        [Index("seam_key", IsUnique = true, Order = 0)]
        [MaxLength(255)]
        public string No { get; set; }

        public Seam CloneFrom { get; set; }

        [Required] [ScriptIgnore] public Procedure Procedure { get; set; }

        [Index("seam_key", IsUnique = true, Order = 1)]
        public int ProcedureId { get; set; }

        public string CheckStandard { get; set; }//检验标准
        public SeamLevel SeamLevel { get; set; }//焊缝等级
        public JointForm JointForm { get; set; }//接头类型
        public bool FillWire { get; set; }

        public int BaseCount { get; set; }

        public string Material1 { get; set; }//材料牌号1
        public string Material2 { get; set; }//材料牌号2
        public string Material3 { get; set; }//材料牌号3
        public string Material4 { get; set; }//材料牌号4
        public double? Gap { get; set; }//
        public double Thick1 { get; set; }//厚度1
        public double Thick2 { get; set; }//厚度2
        public double? Thick3 { get; set; }
        public double? Thick4 { get; set; }
        public string WeldMachineClass { get; set; }//焊机型别
        public string ElectrodeDiameter { get; set; }//滚轮宽度

        public string Reason { get; set; }//原因
        public string ReasonNo { get; set; }

        public List<SeamParam1> InitialParams { get; set; }
        public List<SeamParam2> RevisedParams { get; set; }

        #region 试焊

        public TestState TestState { get; set; }//试焊状态
        public string TestWelder { get; set; }//操作者
        public string TestChecker { get; set; }//检验者
        public string SpecialReportFileNo { get; set; }//报告编号
        public string CoverReportFileNo { get; set; }
        public byte[] TestByteFile { get; set; }//报告编号
        #endregion

        public Seam()
        {
            BaseCount = 2;
        }        

        public bool SimilarTo(Seam seam)
        {
            if (CheckStandard != seam.CheckStandard ||
                SeamLevel != seam.SeamLevel ||
                JointForm != seam.JointForm ||
                FillWire != seam.FillWire ||
                BaseCount != seam.BaseCount ||
                Gap != seam.Gap ||
                WeldMachineClass != seam.WeldMachineClass ||
                ElectrodeDiameter != seam.ElectrodeDiameter)
                return false;

            if (BaseCount == 2)
            {
                if (Material1 == seam.Material1 && Material2 == seam.Material2)
                {
                    if (Thick1.DoubleEquals(seam.Thick1) && Thick2.DoubleEquals(seam.Thick2))
                        return true;
                }
                if (Material1 == seam.Material2 && Material2 == seam.Material1)
                {
                    if (Thick1.DoubleEquals(seam.Thick2) && Thick2.DoubleEquals(seam.Thick1))
                        return true;
                }
            }
            else
            {
                if (Material1 == seam.Material1 && Material2 == seam.Material2 && Material3 == seam.Material3)
                {
                    if (Thick1.DoubleEquals(seam.Thick1) && Thick2.DoubleEquals(seam.Thick2) && Thick3.DoubleEquals(seam.Thick3))
                        return true;
                }
                if (Material1 == seam.Material1 && Material2 == seam.Material3 && Material3 == seam.Material2)
                {
                    if (Thick1.DoubleEquals(seam.Thick1) && Thick2.DoubleEquals(seam.Thick3) && Thick3.DoubleEquals(seam.Thick2))
                        return true;
                }
                if (Material1 == seam.Material2 && Material2 == seam.Material1 && Material3 == seam.Material3)
                {
                    if (Thick1.DoubleEquals(seam.Thick2) && Thick2.DoubleEquals(seam.Thick1) && Thick3.DoubleEquals(seam.Thick3))
                        return true;
                }
                if (Material1 == seam.Material2 && Material2 == seam.Material3 && Material3 == seam.Material1)
                {
                    if (Thick1.DoubleEquals(seam.Thick2) && Thick2.DoubleEquals(seam.Thick3) && Thick3.DoubleEquals(seam.Thick1))
                        return true;
                }
                if (Material1 == seam.Material3 && Material2 == seam.Material1 && Material3 == seam.Material2)
                {
                    if (Thick1.DoubleEquals(seam.Thick3) && Thick2.DoubleEquals(seam.Thick1) && Thick3.DoubleEquals(seam.Thick2))
                        return true;
                }
                if (Material1 == seam.Material3 && Material2 == seam.Material2 && Material3 == seam.Material1)
                {
                    if (Thick1.DoubleEquals(seam.Thick3) && Thick2.DoubleEquals(seam.Thick2) && Thick3.DoubleEquals(seam.Thick1))
                        return true;
                }
            }

            return false;
        }
    }
}
using WCAPP.Models.Database;
using WCAPP.Types;
using WCAPP.Utils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using WCAPP.Models.ProcedureModels;

namespace WCAPP.Libs
{
    public class Reporter
    {
        private static readonly BaseFont BaseFont =
            BaseFont.CreateFont("C:/Windows/Fonts/simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        private static readonly Font ChFont = new Font(BaseFont, 12);

        private static PdfPTable CreateParamsTable(WeldMethod method, Seam seam)
        {
            switch (method)
            {
                case WeldMethod.氩弧焊:
                    return CreateTigParamsTable(seam);
                case WeldMethod.电子束焊:
                    return CreateBeamParamsTable(seam);
                case WeldMethod.电阻焊:
                    return CreateResistParamsTable(seam);
                case WeldMethod.高频钎焊:
                    return CreateBrazeParamsTable(seam);
                //case WeldMethod.等离子焊:
                //    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
            return new PdfPTable(1); 
        }

        private static PdfPTable CreateTigParamsTable(Seam seam)
        {
            var paramsTable = new PdfPTable(1);
            paramsTable.AddCell(new PdfPCell(new Phrase("熔焊工艺参数", ChFont))
            {
                BorderWidth = 0,
                PaddingBottom = 10,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            var paramsTable1 = new PdfPTable(6);
            var paramList = seam.RevisedParams.Select(x => (SeamParam) x).ToList();
            if (!paramList.Any())
            {
                paramList = seam.InitialParams.Select(x => (SeamParam) x).ToList();
            }

            var paramDict = paramList.ToDictionary(x => x.Enum);

            paramsTable1.AddCell(Crtc("焊接电流(A)", 2, 1));
            paramsTable1.AddCell(Crtc("焊接速度(m/min)", 2, 1));
            paramsTable1.AddCell(Crtc("氩气流量", 1, 2));
            paramsTable1.AddCell(Crtc("电流衰减(s)", 2, 1));
            paramsTable1.AddCell(Crtc("保护气滞后(m/s)", 2, 1));
            paramsTable1.AddCell(Crtc("正面(L/min)"));
            paramsTable1.AddCell(Crtc("反面(L/min)"));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.焊接电流)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.焊接速度)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.氩气流量正面)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.氩气流量反面)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.电流衰减)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.保护气滞后)));

            paramsTable.AddCell(new PdfPCell(paramsTable1)
            {
                BorderWidthRight = 0,
                BorderWidthBottom = 0
            });


            var paramsTable2 = new PdfPTable(5);
            paramsTable2.SetWidths(new[] {0.32f, 0.2f, 0.16f, 0.16f, 0.16f});

            paramsTable2.AddCell(Crtc("填充材料牌号"));
            paramsTable2.AddCell(Crtc("填充材料规格"));
            paramsTable2.AddCell(Crtc("送丝速度(m/s)"));
            paramsTable2.AddCell(Crtc("钨极直径(mm)"));
            paramsTable2.AddCell(Crtc("喷嘴直径(mm)"));

            paramsTable2.AddCell(Crtc(GetValue(paramDict, WeldParam.填充材料牌号)));
            paramsTable2.AddCell(Crtc(GetValue(paramDict, WeldParam.填充材料规格)));
            paramsTable2.AddCell(Crtc(GetValue(paramDict, WeldParam.送丝速度)));
            paramsTable2.AddCell(Crtc(GetValue(paramDict, WeldParam.钨极直径)));
            paramsTable2.AddCell(Crtc(GetValue(paramDict, WeldParam.喷嘴直径)));

            paramsTable.AddCell(new PdfPCell(paramsTable2)
            {
                BorderWidthRight = 0,
                BorderWidthBottom = 0,
                BorderWidthTop = 0
            });

            return paramsTable;
        }

        private static PdfPTable CreateResistParamsTable(Seam seam)
        {
            var paramsTable = new PdfPTable(1);
            paramsTable.AddCell(new PdfPCell(new Phrase("电阻焊工艺参数", ChFont))
            {
                BorderWidth = 0,
                PaddingBottom = 10,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            var paramsTable1 = new PdfPTable(7);
            var paramList = seam.RevisedParams.Select(x => (SeamParam) x).ToList();
            if (!paramList.Any())
            {
                paramList = seam.InitialParams.Select(x => (SeamParam) x).ToList();
            }

            var paramDict = paramList.ToDictionary(x => x.Enum);

            paramsTable1.AddCell(Crtc("焊机型别", 2, 2));
            paramsTable1.AddCell(Crtc("电极直径/滚轮宽度(mm)", 2, 1));
            paramsTable1.AddCell(Crtc("功率级数", 2, 1));
            paramsTable1.AddCell(Crtc("预压(ms)", 2, 1));
            paramsTable1.AddCell(Crtc("电极压力(Kpa)", 1, 2));
            paramsTable1.AddCell(Crtc("抬起"));
            paramsTable1.AddCell(Crtc("压下"));
            //paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.焊机型别), 1, 2));
            //paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.电极直径滚轮宽度)));
           
            paramsTable1.AddCell(Crtc(seam.WeldMachineClass,1,2));
            paramsTable1.AddCell(Crtc(seam.ElectrodeDiameter));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.功率级数)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.预压)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.抬起)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.压下)));

            paramsTable1.AddCell(Crtc("脉冲1(ms)"));
            paramsTable1.AddCell(Crtc("焊接电流1(A)"));
            paramsTable1.AddCell(Crtc("冷却(ms)"));
            paramsTable1.AddCell(Crtc("脉冲2(ms)"));
            paramsTable1.AddCell(Crtc("焊接电流2(A)"));
            paramsTable1.AddCell(Crtc("维持(ms)"));
            paramsTable1.AddCell(Crtc("休止(ms)"));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.脉冲1)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.焊接电流1)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.冷却)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.脉冲2)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.焊接电流2)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.维持)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.休止)));


            paramsTable1.AddCell(Crtc("气压(Kpa)", 1, 4));
            paramsTable1.AddCell(Crtc("焊接速度(m/min)", 2, 2));
            paramsTable1.AddCell(Crtc("焊点直径", 2, 1));
            paramsTable1.AddCell(Crtc("下气室", 1, 2));
            paramsTable1.AddCell(Crtc("中气室", 1, 2));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.下气室气压), 1, 2));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.中气室气压), 1, 2));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.焊接速度), 1, 2));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.熔核直径)));


            paramsTable.AddCell(new PdfPCell(paramsTable1)
            {
                BorderWidthRight = 0,
                BorderWidthBottom = 0
            });

            return paramsTable;
        }

        private static PdfPTable CreateBeamParamsTable(Seam seam)
        {
            var paramsTable = new PdfPTable(1);
            paramsTable.AddCell(new PdfPCell(new Phrase("电子束焊工艺参数", ChFont))
            {
                BorderWidth = 0,
                PaddingBottom = 10,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            var paramsTable1 = new PdfPTable(8);
            var paramList = seam.RevisedParams.Select(x => (SeamParam) x).ToList();
            if (!paramList.Any())
            {
                paramList = seam.InitialParams.Select(x => (SeamParam) x).ToList();
            }

            var paramDict = paramList.ToDictionary(x => x.Enum);

            paramsTable1.AddCell(Crtc("加速电压(kV)", 2, 1));
            paramsTable1.AddCell(Crtc("电子束流(mA)", 2, 1));
            paramsTable1.AddCell(Crtc("焊接速度(cm/min)", 2, 1));
            paramsTable1.AddCell(Crtc("聚焦电流(mA)", 2, 1));
            paramsTable1.AddCell(Crtc("工作距离(mm)", 2, 1));
            paramsTable1.AddCell(Crtc("束流斜率控制(S)", 1, 2));
            paramsTable1.AddCell(Crtc("焊接真空度(Pa)", 2, 1));
            paramsTable1.AddCell(Crtc("上升"));
            paramsTable1.AddCell(Crtc("下降"));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.加速电压)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.电子束流)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.焊接速度)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.聚焦电流)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.工作距离)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.上升)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.下降)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.焊接真空度)));

            paramsTable1.AddCell(Crtc("电子束扫描及偏转", 1, 6));
            paramsTable1.AddCell(Crtc("", 3, 2));
            paramsTable1.AddCell(Crtc("波形", 1, 2));
            paramsTable1.AddCell(Crtc("幅值", 1, 2));
            paramsTable1.AddCell(Crtc("频率", 1, 2));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.波形), 1, 2));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.幅值), 1, 2));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.频率), 1, 2));


            paramsTable.AddCell(new PdfPCell(paramsTable1)
            {
                BorderWidthRight = 0,
                BorderWidthBottom = 0
            });

            return paramsTable;
        }

        private static PdfPTable CreateBrazeParamsTable(Seam seam)
        {
            var paramsTable = new PdfPTable(1);
            paramsTable.AddCell(new PdfPCell(new Phrase("高频钎焊工艺参数", ChFont))
            {
                BorderWidth = 0,
                PaddingBottom = 10,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            var paramsTable1 = new PdfPTable(6);
            var paramList = seam.RevisedParams.Select(x => (SeamParam) x).ToList();
            if (!paramList.Any())
            {
                paramList = seam.InitialParams.Select(x => (SeamParam) x).ToList();
            }

            var paramDict = paramList.ToDictionary(x => x.Enum);

            paramsTable1.AddCell(Crtc("钎料牌号"));
            paramsTable1.AddCell(Crtc("填料规格"));
            paramsTable1.AddCell(Crtc("焊接电压"));
            paramsTable1.AddCell(Crtc("氩气流量"));
            paramsTable1.AddCell(Crtc("感应圈编号"));
            paramsTable1.AddCell(Crtc("管子规格"));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.填充材料牌号)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.填充材料规格)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.焊接电压)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.氩气流量)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.感应圈编号)));
            paramsTable1.AddCell(Crtc(GetValue(paramDict, WeldParam.管子规格)));

            paramsTable.AddCell(new PdfPCell(paramsTable1)
            {
                BorderWidthRight = 0,
                BorderWidthBottom = 0
            });

            return paramsTable;
        }

        public byte[] CreateProcessPdf(Process process)
        {
            var ms = new MemoryStream();//为系统内存提供流式的读写操作。常作为其他流数据交换时的中间对象操作
            var document = new Document(PageSize.A4, 40, 60, 40, 40);//

            var writer = PdfWriter.GetInstance(document, ms);
            document.Open();

            if (process.Procedures == null)
                return null;

            var currentPage = 1;
            var totalPage = 0;


            foreach (var procedure in process.Procedures)
            {
                totalPage += procedure.Seams.Count;
            }

            foreach (var procedure in process.Procedures)
            {
                foreach (var seam in procedure.Seams)
                {
                    var outterTable = new PdfPTable(1)
                    {
                        WidthPercentage = 100
                    };

                    var innerTable = new PdfPTable(1);

                    innerTable.AddCell(new PdfPCell(new Phrase("LY-XX"))
                    {
                        PaddingRight = 10,
                        PaddingBottom = 10,
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    });


                    PdfPTable baseTable1 = new PdfPTable(7);
                    baseTable1.AddCell(Crtc("单    位"));
                    baseTable1.AddCell(new PdfPCell(new Phrase("焊接图表", new Font(BaseFont, 16)))
                    {
                        PaddingBottom = 6,
                        BorderWidthTop = 0,
                        BorderWidthLeft = 0,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Rowspan = 2
                    });
                    baseTable1.AddCell(Crtc("零件号"));
                    baseTable1.AddCell(Crtc(process.PartNo));
                    baseTable1.AddCell(Crtc("工序号"));
                    baseTable1.AddCell(Crtc("焊缝编号"));
                    baseTable1.AddCell(Crtc("第 " + currentPage + " 页"));
                    baseTable1.AddCell(Crtc("燃烧分厂"));
                    baseTable1.AddCell(Crtc("零件名称"));
                    baseTable1.AddCell(Crtc(process.PartName));
                    baseTable1.AddCell(Crtc(procedure.No));
                    baseTable1.AddCell(Crtc(seam.No));
                    baseTable1.AddCell(Crtc("共 " + totalPage + " 页"));

                    innerTable.AddCell(new PdfPCell(baseTable1)
                    {
                        BorderWidthRight = 0,
                        BorderWidthBottom = 0
                    });


                    PdfPTable baseTable2 = new PdfPTable(4);

                    baseTable2.SetWidths(new float[] {0.2f, 0.35f, 0.2f, 0.25f});
                    var metal = seam.Material1 + " + " + seam.Material2 + "   " + seam.Thick1 + " + " +
                                seam.Thick2;

                    baseTable2.AddCell(Crtc("材料牌号"));
                    baseTable2.AddCell(Crtc(seam.Material1 + " + " + seam.Material2, 1, 3));
                    baseTable2.AddCell(Crtc("材料规格"));
                    baseTable2.AddCell(Crtc(seam.Thick1 + " + " + seam.Thick2));
                    baseTable2.AddCell(Crtc("焊缝间隙"));
                    baseTable2.AddCell(Crtc(seam.Gap.ToString()));
                    baseTable2.AddCell(Crtc("工序名称"));
                    baseTable2.AddCell(Crtc(procedure.Name));
                    baseTable2.AddCell(Crtc("接头形式"));
                    baseTable2.AddCell(Crtc(seam.JointForm.ToString()));
                    baseTable2.AddCell(Crtc("工艺规程编号"));
                    baseTable2.AddCell(Crtc(process.No));
                    baseTable2.AddCell(Crtc("设备"));
                    baseTable2.AddCell(Crtc(""));


                    innerTable.AddCell(new PdfPCell(baseTable2)
                    {
                        BorderWidthRight = 0,
                        BorderWidthBottom = 0,
                        BorderWidthTop = 0
                    });


                    innerTable.AddCell(new PdfPCell(CreateParamsTable(procedure.WeldMethod, seam))
                    {
                        PaddingLeft = 5,
                        PaddingRight = 5,
                        PaddingBottom = 20,
                        PaddingTop = 8,
                        BorderWidthTop = 2
                    });
                    StringBuilder sb = new StringBuilder();
                    sb.Append("wcapp:");
                    sb.Append(process.Id + "-");
                    //sb.Append(process.No + "-");
                    sb.Append(process.VersionString + "-");
                    sb.Append(process.Id + "-");
                    sb.Append(procedure.No + "-");
                    sb.Append(seam.No.ToString());
                    var image = Image.GetInstance(QrCoder.Encode(sb.ToString()));
                    image.ScaleAbsolute(100, 100);
                    innerTable.AddCell(new PdfPCell(image)
                    {
                        Padding = 15,
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        BorderWidthTop = 1
                    });

                    PdfPTable bottomTable = new PdfPTable(2);

                    var cell = Crtc("签名/日期", -1);
                    cell.PaddingLeft = 10;
                    cell.FixedHeight = 120;
                    bottomTable.AddCell(cell);

                    bottomTable.SetWidths(new float[] {0.7f, 0.3f});
                    bottomTable.AddCell(new PdfPCell(new Phrase("PDM印章", new Font(BaseFont, 32)))
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

                    outterTable.AddCell(new PdfPCell(innerTable)
                    {
                        PaddingLeft = 10,
                        PaddingRight = 5,
                        PaddingBottom = 30,
                        PaddingTop = 10
                    });

                    document.Add(outterTable);
                    document.NewPage();
                    currentPage++;
                }
            }

            document.Close();

            return ms.ToArray();
        }
        public byte[] CreateReviseProcessPdf(Process process)
        {
            try
            {
            var ms = new MemoryStream();//为系统内存提供流式的读写操作。常作为其他流数据交换时的中间对象操作
            var document = new Document(PageSize.A4, 40, 60, 40, 40);//

            var writer = PdfWriter.GetInstance(document, ms);
            document.Open();

            if (process.Procedures == null)
                return null;

            var currentPage = 1;
            var totalPage = 0;
            
                foreach (var procedure in process.Procedures)
            {
                foreach (var seam in procedure.Seams)
                {                    
                    if (seam.Reason!=null&&seam.ReasonNo!=null)
                    {
                        totalPage++;
                        var outterTable = new PdfPTable(1)
                        {
                            WidthPercentage = 100
                        };

                        var innerTable = new PdfPTable(1);

                        innerTable.AddCell(new PdfPCell(new Phrase("焊接参数更改更改单"))
                        {
                            PaddingRight = 10,
                            PaddingBottom = 10,
                            Border = 0,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        });


                        PdfPTable baseTable1 = new PdfPTable(9);
                        baseTable1.AddCell(new PdfPCell(new Phrase("黎阳航空动力", new Font(BaseFont, 10)))
                        {
                            PaddingBottom = 6,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Rowspan = 2
                        });
                        baseTable1.AddCell(Crtc("单    位"));
                        baseTable1.AddCell(new PdfPCell(new Phrase("焊接参数更改更改单", new Font(BaseFont, 16)))
                        {
                            PaddingBottom = 6,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Rowspan = 2,
                            Colspan = 3
                        });
                        baseTable1.AddCell(Crtc("零件名"));
                        baseTable1.AddCell(Crtc(process.PartName));
                        baseTable1.AddCell(Crtc("更改单号"));
                        baseTable1.AddCell(Crtc("第 " + currentPage + " 页"));
                        baseTable1.AddCell(Crtc("燃烧分厂"));
                        baseTable1.AddCell(Crtc("零件号"));
                        baseTable1.AddCell(Crtc(process.PartNo));
                        baseTable1.AddCell(Crtc(seam.No));
                        baseTable1.AddCell(Crtc("共 " + totalPage + " 页"));

                        innerTable.AddCell(new PdfPCell(baseTable1)
                        {
                            BorderWidthRight = 0,
                            BorderWidthBottom = 0
                        });


                        PdfPTable baseTable2 = new PdfPTable(4);
                        baseTable2.SetWidths(new float[] { 0.25f, 0.25f, 0.25f, 0.25f });
                        var metal = seam.Material1 + " + " + seam.Material2 + "   " + seam.Thick1 + " + " +
                                    seam.Thick2;

                        baseTable2.AddCell(Crtc("工艺规程编号"));
                        baseTable2.AddCell(new PdfPCell(new Phrase(process.No, new Font(BaseFont, 16)))
                        {
                            PaddingBottom = 6,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 3
                        });

                        innerTable.AddCell(new PdfPCell(baseTable2)
                        {
                            BorderWidthRight = 0,
                            BorderWidthBottom = 0,
                            BorderWidthTop = 0
                        });

                        var paramsTable1 = new PdfPTable(6);

                        paramsTable1.AddCell(Crtc("工序号", 2, 1));
                        paramsTable1.AddCell(Crtc("焊缝编号", 2, 1));
                        paramsTable1.AddCell(Crtc("原因序号", 2, 1));
                        paramsTable1.AddCell(Crtc("更改内容", 1, 2));
                        paramsTable1.AddCell(Crtc("更改原因", 2, 1));
                        paramsTable1.AddCell(Crtc("更改前"));
                        paramsTable1.AddCell(Crtc("更改后"));


                        var db = new Context();

                        foreach (var iniParam in seam.InitialParams)
                        {
                            foreach (var revParam in seam.RevisedParams)
                            {
                                if (iniParam.Enum == revParam.Enum)
                                {
                                    if (iniParam.Value != revParam.Value)
                                    {
                                        paramsTable1.AddCell(Crtc(procedure.No));
                                        paramsTable1.AddCell(Crtc(seam.No));
                                        paramsTable1.AddCell(Crtc(seam.ReasonNo));
                                        paramsTable1.AddCell(Crtc(iniParam.Value));
                                        paramsTable1.AddCell(Crtc(revParam.Value));
                                        paramsTable1.AddCell(Crtc(seam.Reason));
                                    }
                                }
                            }
                        }


                        innerTable.AddCell(new PdfPCell(paramsTable1)
                        {
                            BorderWidthRight = 0,
                            BorderWidthBottom = 0
                        });
                        var paramsTable2 = new PdfPTable(3);


                        var approve = db.Approves.Include("Process").SingleOrDefault(x => x.ProcessId == process.Id);
                        var ProoferName = db.Users.SingleOrDefault(x => x.Id == approve.ProoferId);
                        var ApproverName = db.Users.SingleOrDefault(x => x.Id == approve.ApproverId);

                        paramsTable2.AddCell(Crtc("编制", 2, 1));
                        paramsTable2.AddCell(Crtc("校对", 2, 1));
                        paramsTable2.AddCell(Crtc("审核", 2, 1));
                        paramsTable2.AddCell(Crtc(process.Author.ToString()));
                        if (ProoferName != null || ApproverName != null)
                        {
                            paramsTable2.AddCell(Crtc(ProoferName.ToString()));
                            paramsTable2.AddCell(Crtc(ApproverName.ToString()));
                        }
                        else
                        {
                            paramsTable2.AddCell(Crtc(""));
                            paramsTable2.AddCell(Crtc(""));
                        }

                        innerTable.AddCell(new PdfPCell(paramsTable2)
                        {
                            BorderWidthRight = 0,
                            BorderWidthBottom = 0
                        });

                        outterTable.AddCell(new PdfPCell(innerTable)
                        {
                            PaddingLeft = 10,
                            PaddingRight = 5,
                            PaddingBottom = 30,
                            PaddingTop = 10
                        });

                        document.Add(outterTable);
                        document.NewPage();
                        currentPage++;
                    }
                   
                }
            }
                document.Close();

                return ms.ToArray();
            }
            finally 
            {
                
            }
            
        }
        private static PdfPCell Crtc(string content, int align = 0)
        {
            return Crtc(content, 1, 1, align);
        }

        private static PdfPCell Crtc(string content, int rowspan, int colspan, int align = 0)
        {
            return new PdfPCell(new Phrase(content, ChFont))
            {
                MinimumHeight = 25,
                PaddingBottom = 6,
                BorderWidthTop = 0,
                BorderWidthLeft = 0,
                HorizontalAlignment =
                    align == 0 ? Element.ALIGN_CENTER : (align < 0 ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT),
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Rowspan = rowspan,
                Colspan = colspan
            };
        }

        private static string GetValue(IDictionary<WeldParam, SeamParam> dict, WeldParam key)
        {
            return dict.TryGetValue(key, out var p) ? p.Value : "";
        }
    }

    class Cell : PdfPCell
    {
    }
}
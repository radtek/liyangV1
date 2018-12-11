namespace WCAPP.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WCAPP.Models.Database;
    using WCAPP.Types;
    using WCAPP.Utils;

    internal sealed class Configuration : DbMigrationsConfiguration<WCAPP.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = GlobalData.AutomaticMigrationsEnabled;
            AutomaticMigrationDataLossAllowed = GlobalData.AutomaticMigrationDataLossAllowed;
        }

        protected override void Seed(WCAPP.Context context)
        {
            //  This method will be called after migrating to the latest version.

            //context.TestModels.AddOrUpdate(p => p.X, new TestModel { X = "X" });

            //var m1 = new Material { Grade = "40Cr30Ni20Ca10P6Ti" };
            //var m2 = new Material { Grade = "20Ca10P6Ti40Cr30Ni" };
            //var users = new[]
            //{
            //    new User
            //    {
            //        Id = "admin",
            //        Name = "张三",
            //        Position = WorkPosition.焊接主管,
            //        Password = "admin".Md532(),
            //        Department = "燃烧部件制造分厂",
            //        AuthorityRs = new[] {Authority.用户管理, Authority.编制, Authority.审核}.ToAuthorityRs()
            //    },
            //    new User
            //    {
            //        Id = "cj",
            //        Name = "李四",
            //        Position = WorkPosition.焊接工程师,
            //        Password = "cj".Md532(),
            //        Department = "燃烧部件制造分厂",
            //        AuthorityRs = new[] {Authority.编制}.ToAuthorityRs()
            //    },
            //    new User
            //    {
            //        Id = "sh",
            //        Name = "王五",
            //        Position = WorkPosition.高级焊接工程师,
            //        Password = "sh".Md532(),
            //        Department = "燃烧部件制造分厂",
            //        AuthorityRs = new[] {Authority.审核}.ToAuthorityRs()
            //    },
            //    new User
            //    {
            //        Id = "cjsh",
            //        Name = "赵六",
            //        Position = WorkPosition.焊接工程师,
            //        Password = "cjsh".Md532(),
            //        Department = "燃烧部件制造分厂",
            //        AuthorityRs = new[] {Authority.编制, Authority.审核}.ToAuthorityRs()
            //    },
            //    new User
            //    {
            //        Id = "yd",
            //        Name = "yd",
            //        Position = WorkPosition.焊接工程师,
            //        Password = "yd".Md532(),
            //        Department = "燃烧部件制造分厂",
            //        AuthorityRs = new[] {Authority.编制, Authority.审核,Authority.试焊}.ToAuthorityRs()
            //    },
            //    new User
            //    {
            //        Id = "wjq",
            //        Name = "wjq",
            //        Position = WorkPosition.焊接工程师,
            //        Password = "wjq".Md532(),
            //        Department = "燃烧部件制造分厂",
            //        AuthorityRs = new[] {Authority.编制, Authority.审核,Authority.试焊}.ToAuthorityRs()
            //    }
            //};
            //context.Users.AddOrUpdate(p => p.Id, users);
            //context.Processes.AddOrUpdate(p => new { p.No, p.Version }, new Process {
            //    No = "321343",
            //    PdmId = "",
            //    Author = users[1],
            //    PartNo = "123.431",
            //    PartName = "扇叶",
            //    Version = 1,
            //    ApprovalState = ApprovalState.未提交审核,
            //    TestState = ProgramTestState.未完成,
            //    Procedures = new List<Procedure>
            //    {
            //        new Procedure
            //        {
            //            No = "20",
            //            PdmId = "",
            //            CurrentType = CurrentType.交流,
            //            Name = "焊接",
            //            WeldMethod = WeldMethod.氩弧焊,
            //            AutoLevel = AutoLevel.手工,
            //            TestState = ProgramTestState.未完成,
            //            Seams = new List<Seam>
            //            {
            //                new Seam
            //                {
            //                    No = "1",
            //                    TestState = TestState.尚未进行,
            //                    CheckStandard = "HB6737",
            //                    FillWire = true,
            //                    SeamLevel = SeamLevel.Ⅲ,
            //                    JointForm = JointForm.对接,
            //                    Gap = 0.2,
            //                    Material1 = m1.Grade,
            //                    Material2 = m1.Grade,
            //                    Thick1 = 1.3,
            //                    Thick2 = 1.3,
            //                    TestWelder = "张三",
            //                    SpecialReportFileNo = "212",
            //                },
            //                new Seam
            //                {
            //                    No = "2",
            //                    TestState = TestState.尚未进行,
            //                    FillWire = true,
            //                    CheckStandard = "HB6737",
            //                    SeamLevel = SeamLevel.Ⅲ,
            //                    JointForm = JointForm.对接,
            //                    Gap = 0.2,
            //                    Material1 = m2.Grade,
            //                    Material2 = m2.Grade,
            //                    Thick1 = 1.3,
            //                    Thick2 = 1.3,
            //                    TestWelder = "张三",
            //                    SpecialReportFileNo = "211",
            //                }
            //            }
            //        }
            //    }
            //});
            //context.Processes.AddOrUpdate(p => new { p.No, p.Version }, new Process {
            //    No = "1234567890",
            //    PdmId = "",
            //    Author = users[1],
            //    PartNo = "123.433",
            //    PartName = "扇叶",
            //    Version = 1,
            //    Published = true,
            //    ApprovalState = ApprovalState.审核通过,
            //    TestState = ProgramTestState.未完成,
            //    Procedures = new List<Procedure>
            //    {
            //        new Procedure
            //        {
            //            No = "20",
            //            PdmId = "",
            //            CurrentType = CurrentType.交流,
            //            Name = "焊接",
            //            WeldMethod = WeldMethod.氩弧焊,
            //            AutoLevel = AutoLevel.手工,
            //            TestState = ProgramTestState.未完成,
            //            Seams = new List<Seam>
            //            {
            //                new Seam
            //                {
            //                    No = "1",
            //                    TestState = TestState.尚未进行,
            //                    CheckStandard = "HB6737",
            //                    FillWire = true,
            //                    SeamLevel = SeamLevel.Ⅲ,
            //                    JointForm = JointForm.对接,
            //                    Gap = 0.2,
            //                    Material1 = m1.Grade,
            //                    Material2 = m1.Grade,
            //                    Thick1 = 1.3,
            //                    Thick2 = 1.3,
            //                    TestWelder = "张三",
            //                    SpecialReportFileNo = "212",
            //                    InitialParams=GlobalData.InitialMethodParams(WeldMethod.氩弧焊)
            //                },
            //                new Seam
            //                {
            //                    No = "2",
            //                    TestState = TestState.尚未进行,
            //                    FillWire = true,
            //                    CheckStandard = "HB6737",
            //                    SeamLevel = SeamLevel.Ⅲ,
            //                    JointForm = JointForm.对接,
            //                    Gap = 0.2,
            //                    Material1 = m2.Grade,
            //                    Material2 = m2.Grade,
            //                    Thick1 = 1.3,
            //                    Thick2 = 1.3,
            //                    TestWelder = "张三",
            //                    SpecialReportFileNo = "211",
            //                    InitialParams=GlobalData.InitialMethodParams(WeldMethod.氩弧焊)
            //                }
            //            }
            //        },
            //        new Procedure
            //        {
            //            No = "30",
            //            PdmId = "",
            //            CurrentType = CurrentType.交流,
            //            Name = "焊接",
            //            WeldMethod = WeldMethod.氩弧焊,
            //            AutoLevel = AutoLevel.手工,
            //            TestState = ProgramTestState.未完成,
            //            Seams = new List<Seam>
            //            {
            //                new Seam
            //                {
            //                    No = "1",
            //                    TestState = TestState.尚未进行,
            //                    CheckStandard = "HB6737",
            //                    FillWire = true,
            //                    SeamLevel = SeamLevel.Ⅲ,
            //                    JointForm = JointForm.对接,
            //                    Gap = 0.2,
            //                    Material1 = m2.Grade,
            //                    Material2 = m2.Grade,
            //                    Thick1 = 1.3,
            //                    Thick2 = 1.3,
            //                    TestWelder = "张三",
            //                    SpecialReportFileNo = "122",
            //                    InitialParams=GlobalData.InitialMethodParams(WeldMethod.氩弧焊)
            //                },
            //                new Seam
            //                {
            //                    No = "2",
            //                    TestState = TestState.尚未进行,
            //                    FillWire = true,
            //                    CheckStandard = "HB6737",
            //                    SeamLevel = SeamLevel.Ⅲ,
            //                    JointForm = JointForm.对接,
            //                    Gap = 0.2,
            //                    Material1 = m1.Grade,
            //                    Material2 = m1.Grade,
            //                    Thick1 = 1.3,
            //                    Thick2 = 1.3,
            //                    TestWelder = "张三",
            //                    SpecialReportFileNo = "202",
            //                    InitialParams=GlobalData.InitialMethodParams(WeldMethod.氩弧焊)
            //                }
            //            }
            //        }
            //    }
            //});

            //context.Materials.AddOrUpdate(p => p.Grade, new[]
            //{
            //    new Material {Grade = "20Mn2"},
            //    new Material {Grade = "15Cr"},
            //    new Material {Grade = "20Cr"},
            //    new Material {Grade = "30Cr"},
            //    new Material {Grade = "40Cr"},
            //    new Material {Grade = "45Cr"},
            //    new Material {Grade = "30CrMo"},
            //    new Material {Grade = "35CrMo"},
            //    new Material {Grade = "42CrMo"},
            //    new Material {Grade = "38CrMoAl"},
            //    new Material {Grade = "50CrVA"},
            //    new Material {Grade = "40CrMnMo"},
            //    new Material {Grade = "Q295A"},
            //    new Material {Grade = "Q295B"},
            //    new Material {Grade = "Q345C"},
            //    new Material {Grade = "Q345E"},
            //    new Material {Grade = "Q420B"},
            //    new Material {Grade = "Q460D"},
            //    new Material {Grade = "10"},
            //    new Material {Grade = "15"},
            //    new Material {Grade = "20"},
            //    new Material {Grade = "25"},
            //    new Material {Grade = "40"},
            //    new Material {Grade = "45"},
            //    new Material {Grade = "50"},
            //    new Material {Grade = "15Mn"},
            //    new Material {Grade = "20Mn"},
            //    new Material {Grade = "30Mn"},
            //    new Material {Grade = "40Mn"},
            //    new Material {Grade = "45Mn"}
            //});

            //context.DispatchMessages.AddOrUpdate(p => p.TaskNo, new DispatchMessage {
            //    TaskNo = "TESTTASK1",
            //    BatchNo = "1",
            //    Count = "10",
            //    FacCode = "123",
            //    SeqNo = "12",
            //    PartNo = "1232213",
            //    WelderNo = "210021",
            //    WeldNo = "213",
            //    StartTime = DateTime.Now,
            //    EndTime = DateTime.Now
            //});
            //context.ERPs.AddOrUpdate(P => P.ID, new ERP {
            //    ID="ERP001",
            //    Desce="2081",
            //    Type= "2081",
            //    Kostl= "LYDL0032",
            //    KoDesc= "2081",
            //    TypeDesc= "2081",
            //    Eartx= "2081",
            //    State= "2081",
            //    Eqart = "2803"
            //});
            base.Seed(context);
        }
    }
}

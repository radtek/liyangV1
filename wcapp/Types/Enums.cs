using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCAPP.Types
{
    public enum ProcessState
    {
        创建中,
        审核中,
        审核不通过,
        已审核
    }

    public enum JointForm
    {
        对接,
        角接,
        搭接,
        端接,
        堆焊
    }

    public enum AutoLevel
    {
        手工,
        自动
    }

    public enum WeldMethod
    {
        氩弧焊,
        电阻焊,
        电子束焊,
        高频钎焊,
       // 等离子焊
    }

    public enum WeldType
    {
        正式焊,
        定位焊,
        封焊,
        修饰焊,
        补焊
    }

    public enum Sex
    {
        男,
        女
    }

    public enum CurrentType
    {
        交流,
        直流,
        脉冲,
        变极性
    }

    public enum ParamType
    {
        Integer,
        Real,
        String,
        Enum
    }

    public enum TestState
    {
        尚未进行,
        进行中,
        已完成
    }

    public enum ProgramTestState
    {
        未完成,
        已完成
    }

    public enum TestResult
    {
        不合格,
        合格
    }

    public enum ApprovalState
    {
        未提交审核,
        审核中,
        审核不通过,
        审核通过
    }

    public enum ApproveState
    {
        进行中,
        未通过,
        通过
    }

    public enum ApprovalResult
    {
        不通过,
        通过
    }

    public enum Entrance
    {
        工艺规程管理,
        试焊管理,
        审核管理,
        用户管理,
        同步管理
    }

    public enum ResistType
    {
        点焊,
        缝焊,
        凸焊
    }

    public enum Authority
    {
        编制,
        校对,
        审核,
        用户管理,
        试焊
    }

    public enum AuthorityState
    {
        通过,
        未登录,
        权限不够
    }

    public enum MaterialState
    {
        正常
    }

    public enum CheckStandard
    {
        正常
    }

    public enum SeamLevel
    {
        Ⅰ,
        Ⅱ,
        Ⅲ,
        Ⅳ,
        Ⅴ
    }

    public enum ProcedureType
    {
        焊接,
        补焊,
        点固
    }


    public enum WelderQualification
    {
        一级焊缝,
        补焊,
        点固
    }

    public enum WorkPosition
    {
        焊接工程师,
        高级焊接工程师,
        焊接主管,
        试焊人员
    }
}
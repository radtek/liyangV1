﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCAPP.Types
{
    public enum WeldParam
    {
        焊接速度,
        无效,
        无效枚举值,

        // TIG
        焊接电流,
        氩气流量正面,
        氩气流量反面,
        电流衰减,
        保护气滞后,
        填充材料牌号,
        填充材料规格,
        送丝速度,
        钨极直径,
        喷嘴直径,
        //焊接电流原因,
        //氩气流量正面原因,
        //氩气流量反面原因,
        //电流衰减原因,
        //保护气滞后原因,
        //填充材料牌号原因,
        //填充材料规格原因,
        //送丝速度原因,
        //钨极直径原因,
        //喷嘴直径原因,

        // 电阻焊

        //焊机型别,
        //电极直径滚轮宽度,

        功率级数,
        预压,
        抬起,
        压下,
        脉冲1,
        焊接电流1,
        冷却,
        脉冲2,
        焊接电流2,
        维持,
        休止,
        熔核直径,
        下气室气压,
        中气室气压,
        //功率级数原因,
        //预压原因,
        //抬起原因,
        //压下原因,
        //脉冲1yy,
        //焊接电流1原因,
        //冷却原因,
        //脉冲2原因,
        //焊接电流2原因,
        //维持原因,
        //休止原因,
        //熔核直径原因,
        //下气室气压原因,
        //中气室气压原因,

        // 电子束焊
        功率,
        加速电压,
        电子束流,
        聚焦电流,
        聚焦电流fd,
        工作距离,
        上升,
        下降,
        焊接真空度,
        波形,
        幅值,
        频率,
        //功率原因,
        //加速电压原因,
        //电子束流原因,
        //聚焦电流原因,
        //聚焦电流fd原因,
        //工作距离原因,
        //上升原因,
        //下降原因,
        //焊接真空度原因,
        //波形原因,
        //幅值原因,
        //频率原因,

        // 高频钎焊
        钎料牌号,
        钎料规格,
        管子规格,
        氩气流量,
        感应圈编号,
        焊接电压,
        //钎料牌号原因,
        //钎料规格原因,
        //管子规格原因,
        //氩气流量原因,
        //感应圈编号原因,
        //焊接电压原因,
    }
}
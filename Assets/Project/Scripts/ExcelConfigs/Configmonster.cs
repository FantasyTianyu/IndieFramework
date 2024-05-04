using UnityEngine;
using System;

namespace IndieFramework {
    public class Configmonster : ExcelData {
        //怪物Id
        public int Id;
        //怪物名字
        public string Name;
        //怪物描述
        public string Description;
        //怪物生命值
        public int Hp;
        //怪物法力值
        public int Mp;
        //怪物技能
        public string[] Skill;
        //怪物掉落物品
        public string[] DropItem;

        public override void ReadData(byte[] datas, ref int index) {
            Id = ExcelData.ReadInt(datas, ref index);
            Name = ExcelData.ReadString(datas, ref index);
            Description = ExcelData.ReadString(datas, ref index);
            Hp = ExcelData.ReadInt(datas, ref index);
            Mp = ExcelData.ReadInt(datas, ref index);
            Skill = ExcelData.ReadStringArray(datas, ref index);
            DropItem = ExcelData.ReadStringArray(datas, ref index);
        }
    }
}

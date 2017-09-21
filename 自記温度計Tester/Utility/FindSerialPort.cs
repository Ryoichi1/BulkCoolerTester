using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace 自記温度計Tester
{
    public static class FindSerialPort
    {
        public static List<string> GetDeviceNames()
        {
            var deviceNameList = new List<string>();
            var check = new System.Text.RegularExpressions.Regex("(COM[1-9][0-9]?[0-9]?)");


            ManagementClass mcPnPEntity = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection manageObjCol = mcPnPEntity.GetInstances();

            //全てのPnPデバイスを探索しシリアル通信が行われるデバイスを随時追加する
            foreach (ManagementObject manageObj in manageObjCol)
            {
                //Nameプロパティを取得
                var namePropertyValue = manageObj.GetPropertyValue("Name");
                if (namePropertyValue == null)
                {
                    continue;
                }

                //Nameプロパティ文字列の一部が"(COM1)～(COM999)"と一致するときリストに追加"
                string name = namePropertyValue.ToString();
                if (check.IsMatch(name))
                {
                    deviceNameList.Add(name);
                }
            }


            return new List<string>(deviceNameList.ToList<string>());
        }

        public static string GetComNo(string ID_NAME)
        {
            //Comポートの取得
            var ComPortList = GetDeviceNames();
            var buff = ComPortList.FirstOrDefault(a => a.Contains(ID_NAME));//一致する要素がない場合はnullを返す
            if (buff == null) return null;

            int i = buff.LastIndexOf("(");
            int j = buff.LastIndexOf(")");
            return buff.Substring(i + 1, j - i - 1);
        }

        public static List<string> GetComNoList(string ID_NAME)
        {
            //Comポートの取得
            var ComPortList = GetDeviceNames();
            var buff = ComPortList.Where(a => a.Contains(ID_NAME));//一致する要素がない場合はnullを返す
            if (buff == null) return null;

            var list = new List<string>();

            foreach (var b in buff)
            {
                int i = b.LastIndexOf("(");
                int j = b.LastIndexOf(")");
                list.Add(b.Substring(i + 1, j - i - 1));
            }

            return list;
        }


    }
}

using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace 自記温度計Tester
{
    public class PortInfo
    {
        public string Name;
        public string ClassGuid;
        public string DeviceId;
    }


    public static class FindSerialPort
    {
        public static List<PortInfo> GetDeviceNames()
        {
            var deviceNameList = new List<PortInfo>();
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
                    deviceNameList.Add(new PortInfo { Name = name, ClassGuid = manageObj.GetPropertyValue("ClassGuid").ToString(), DeviceId = manageObj.GetPropertyValue("DeviceID").ToString()});
                }
            }


            return deviceNameList;
        }

        public static string GetComNo(string ID_NAME)
        {
            //Comポートの取得
            var ComPortList = GetDeviceNames();
            var buff = ComPortList.FirstOrDefault(a => a.Name.Contains(ID_NAME));//一致する要素がない場合はnullを返す
            if (buff == null) return null;

            int i = buff.Name.LastIndexOf("(");
            int j = buff.Name.LastIndexOf(")");
            return buff.Name.Substring(i + 1, j - i - 1);
        }

        public static List<string> GetComNoList(string ID_NAME)
        {
            //Comポートの取得
            var ComPortList = GetDeviceNames();
            var buff = ComPortList.Where(a => a.Name.Contains(ID_NAME));//一致する要素がない場合はnullを返す
            if (buff == null) return null;

            var list = new List<string>();

            foreach (var b in buff)
            {
                int i = b.Name.LastIndexOf("(");
                int j = b.Name.LastIndexOf(")");
                list.Add(b.Name.Substring(i + 1, j - i - 1));
            }

            return list;
        }


    }
}

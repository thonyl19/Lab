using System;
using System.Web.UI.WebControls;
using Genesis.Gtimes.UserControl;
using Genesis.Gtimes.Portal.WIP;
using static Genesis.Gtimes.WIP.LotUtility;
using Genesis.Gtimes.Common;
using Genesis.Gtimes.WIP;

namespace Genesis.Gtimes.Portal.Extension
{

    public interface iCheckIn {
        iCheckIn CheckInfo_LotIsExist(LotUtility.LotInfo LotInfo);
        iCheckIn CheckInfo_STATUS_IsWait(LotUtility.LotInfo LotInfo);
    }

    public interface iLotCheckInWithPartNo: iCheckIn
    {
        T 檢查優先順序等級<T>();
    }

    public class ba_ {
        public Label lblLotNo;
        public TextBox txtLotNo;
        public RuleControlBar rcbCheckIn;
    }
    public class bzCheckIn:ba_ {
        
        public bzCheckIn(Label lblLotNo, TextBox txtLotNo, RuleControlBar rcbCheckIn) {
            this.lblLotNo = lblLotNo;
            this.txtLotNo = txtLotNo;
            this.rcbCheckIn = rcbCheckIn;
        }

        public static bzCheckIn Init(LotCheckInWithPartNo_t page) {
            var _page = new bzCheckIn(page.lblLotNo, page.txtLotNo, page.rcbCheckIn);
            return _page;
        }
        public bzCheckIn IsExist(LotInfo LotInfo)
        {
            if (LotInfo.IsExist == false)
            {
                this.txtLotNo.Text = "";
                this.rcbCheckIn.OKButtonEnable = false;
                throw new Exception(string.Format(Resources.Message.InputFieldNotExist, this.lblLotNo.Text));
            }
            return this;
        }

        public bzCheckIn IsSTATUS_Wait(LotInfo LotInfo)
        {
            if (LotInfo.STATUS != "Wait")
            {
                this.txtLotNo.Text = "";
                this.rcbCheckIn.OKButtonEnable = false;
                throw new Exception(string.Format(Resources.Message.LotStatusError, this.lblLotNo));
            }
            return this;
        }

    }
    public static class MyExten
    {
        public static T _IsExist<T>(this T page)
        {
            A _page = page as A;
            return page;
        }


        public static B _檢查優先順序等級(this B page)
        {
            return page;
        }
        public static LotInfo IsExist(this LotInfo LotInfo, Label lblLotNo, TextBox txtLotNo, RuleControlBar rcbCheckIn) {
            if (LotInfo.IsExist == false)
            {
                txtLotNo.Text = "";
                rcbCheckIn.OKButtonEnable = false;
                throw new Exception(string.Format(Resources.Message.InputFieldNotExist, lblLotNo.Text));
            }
            return LotInfo;
        }

        public static bool IsFunctionRightName(this PageBase _this, LotInfo LotInfo ,Action cb ) {

            bool g = LotInfo.FUN_CODE != _this.FunctionRightName;
            if (g) cb();
            return g;
            
        }

        public static T _IsExist<T>(this T page, LotInfo LotInfo)
        {
            CheckIn_t _page = page as CheckIn_t;
            return page;
        }


        public static LotCheckInWithPartNo_t _檢查優先順序等級(this LotCheckInWithPartNo_t page, LotInfo LotInfo)
        {
            return page;
        }




    }
}
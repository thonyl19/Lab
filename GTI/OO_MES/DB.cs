using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.OO_MES
{
    public class FlowStepDef
    {
        public int Instance { get; set; }
        public int Category { get; set; }
        public int Order { get; set; }
        public string StepCode { get; set; }
        public string ClassName { get; set; }
        public string Enable { get; set; }
    }

    public class LotFlowRepository
    {
        public IList<FlowStepDef> GetFlowSteps(string actionType)
        {
            
            // 模擬 DB 結果
            return new List<FlowStepDef>
            {
                new FlowStepDef{
                    Order = 1,
                    StepCode = "VALIDATE_ADD_QTY",
                    ClassName = "UnitTestProject.OO_MES.AddQtyValidateStep, UnitTestProject"
                },
                new FlowStepDef{
                    Order = 2,
                    StepCode = "MODIFY_ADD_QTY",
                    ClassName = "UnitTestProject.OO_MES.AddQtyValidateStep1, UnitTestProject"
                },
            
            };
        }
    }

    public class ProcessFlowRepository
    {
        public IList<FlowStepDef> GetProcessFlowSteps(string actionType)
        {
            // 模擬 DB 回傳
            return new List<FlowStepDef>{
                new FlowStepDef{
                    Order = 1,
                    StepCode = "VALIDATE_LOTS",
                    ClassName = "UnitTestProject.OO_MES.ValidateLotsStep, UnitTestProject"
                },
                new FlowStepDef{
                    Order = 2,
                    StepCode = "LOCK_LOTS",
                    ClassName = "UnitTestProject.OO_MES.LockLotsStep, UnitTestProject"
                },

            };
        }
    }

}

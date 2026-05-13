using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.OO_MES
{
    public class ProcessContext
    {
        public IList<string> LotNos { get; set; }
        public string Station { get; set; }
        public string UserNo { get; set; }

        public ProcessContext()
        {
            LotNos = new List<string>();
        }
    }

    public interface IProcessStep
    {
        void Execute(ProcessContext context);
    }

    public interface IProcessFlow
    {
        void Execute(ProcessContext context);
    }

    public class InStationProcess : IProcessFlow
    {
        public void Execute(ProcessContext context)
        {
            // 1. Load Lots
            var lots = LoadLots(context.LotNos);

            // 2. 進站檢核（跨 Lot）
            ValidateBeforeInStation(lots, context);

            // 3. 逐一驅動 Lot 行為
            foreach (var lot in lots)
            {
                lot.CheckIn(context.Station, context.UserNo);
            }

            // 4. 後處理（例如寫跨批號歷史）
            WriteProcessHistory(lots, context);
        }

        private IList<Lot> LoadLots(IList<string> lotNos)
        {
            // Repository / EF
            var result = new List<Lot>();
            foreach (var lotNo in lotNos)
            {
                result.Add(new Lot(lotNo));
            }
            return result;
        }

        private void ValidateBeforeInStation(
            IList<Lot> lots,
            ProcessContext context)
        {
            if (lots.Count != 2)
                throw new InvalidOperationException("進站必須兩個批號");
        }

        private void WriteProcessHistory(
            IList<Lot> lots,
            ProcessContext context)
        {
            Console.WriteLine("InStation Process Completed");
        }
    }

    public class ProcessFlowPipeline : IProcessFlow
    {
        private readonly IList<IProcessStep> _steps;

        public ProcessFlowPipeline(IEnumerable<IProcessStep> steps)
        {
            _steps = new List<IProcessStep>(steps);
        }

        public void Execute(ProcessContext context)
        {
            foreach (var step in _steps)
            {
                step.Execute(context);
            }
        }
    }

    public class ValidateLotsStep : IProcessStep
    {
        public void Execute(ProcessContext context)
        {
            if (context.LotNos.Count < 1)
                throw new InvalidOperationException("至少需要一個批號");

            Console.WriteLine("ValidateLots OK: " + string.Join(",", context.LotNos));
        }
    }

    public class LockLotsStep : IProcessStep
    {
        public void Execute(ProcessContext context){
            // 模擬鎖定批號
            Console.WriteLine("Locked Lots: " + string.Join(",", context.LotNos));
        }
    }

    public class ExecuteLotInStep : IProcessStep
    {
        public void Execute(ProcessContext context)
        {
            // 逐批號呼叫 Lot.In()
            foreach (var lotNo in context.LotNos)
            {
                var lot = new Lot(lotNo);
                lot.CheckIn(context.Station, context.UserNo);
            }
        }
    }

    public class WriteProcessHistoryStep : IProcessStep
    {
        public void Execute(ProcessContext context)
        {
            Console.WriteLine("Process History Written for Lots: " + string.Join(",", context.LotNos));
        }
    }

    public class ProcessFlowFactory
    {
        private readonly ProcessFlowRepository _repo;

        public ProcessFlowFactory()
        {
            _repo = new ProcessFlowRepository();
        }

        public IProcessFlow Create(string actionType)
        {
            // 從 DB 拿 Step 定義
            var stepDefs = _repo.GetProcessFlowSteps(actionType)
                                .OrderBy(x => x.Order)
                                .ToList();

            var steps = new List<IProcessStep>();

            foreach (var def in stepDefs)
            {
                var step = ProcessStepFactory.Create(def.ClassName);
                steps.Add(step);
            }

            return new ProcessFlowPipeline(steps);
        }
    }

    public static class ProcessStepFactory
    {
        public static IProcessStep Create(string className)
        {
            var type = Type.GetType(className);
            if (type == null)
                throw new InvalidOperationException(
                    "找不到 ProcessStep 類型：" + className);

            return (IProcessStep)Activator.CreateInstance(type);
        }
    }

    

}


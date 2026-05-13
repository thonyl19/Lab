using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.OO_MES
{
    public class Lot
    {
        public string LotNo { get; }

        public Lot(string lotNo)
        {
            LotNo = lotNo;
        }

        public virtual void AddQty(int qty, string userNo)
        {
            var context = new LotContext
            {
                LotNo = this.LotNo,
                Qty = qty,
                UserNo = userNo
            };

            var flow = LotFlowFactory_v0.CreateAddQtyFlow();
            flow.Execute(context);
        }

        public virtual void CheckIn(string station, string userNo)
        {
            var context = new LotContext{
                LotNo = this.LotNo,
            };

            var flow = LotFlowFactory_v0.CreateAddQtyFlow();
            flow.Execute(context);
        }
        public virtual void CheckOut()
        {
            var context = new LotContext{
                LotNo = this.LotNo,
            };

            var flow = LotFlowFactory_v0.CreateAddQtyFlow();
            flow.Execute(context);
        }
    }

    public class Lot_DB:Lot
    {
        public Lot_DB(string lotNo) : base(lotNo)
        {
        }

        public virtual void AddQty(int qty, string userNo)
        {
            var context = new LotContext
            {
                LotNo = this.LotNo,
                Qty = qty,
                UserNo = userNo
            };

            var factory = new LotFlowFactory_v1();
            var flow = factory.Create("AddQty");
            flow.Execute(context);
        }
    }

    public interface ILotFlow
    {
        void Execute(LotContext context);
    }


    public interface IFlowStep
    {
        void Execute(LotContext context);
    }
    public class LotContext
    {
        public string LotNo { get; set; }
        public int Qty { get; set; }
        public string UserNo { get; set; }

        // 流程中暫存用（避免 step 彼此耦合）
        public Dictionary<string, object> Bag { get; private set; }

        public LotContext()
        {
            Bag = new Dictionary<string, object>();
        }
    }

    public class FlowPipeline : ILotFlow
    {
        private readonly IList<IFlowStep> _steps;

        public FlowPipeline(IEnumerable<IFlowStep> steps)
        {
            _steps = new List<IFlowStep>(steps);
        }

        public void Execute(LotContext context)
        {
            foreach (var step in _steps)
            {
                step.Execute(context);
            }
        }
    }

    public class AddQtyValidateStep : IFlowStep
    {
        public void Execute(LotContext context)
        {
            if (context.Qty <= 0)
                throw new InvalidOperationException("加帳數量必須大於 0");

            if (string.IsNullOrEmpty(context.LotNo))
                throw new InvalidOperationException("LotNo 不可為空");
        }
    }
    public class AddQtyValidateStep1 : IFlowStep
    {
        public void Execute(LotContext context)
        {
            if (context.Qty <= 0)
                throw new InvalidOperationException("加帳數量必須大於 0");

            if (string.IsNullOrEmpty(context.LotNo))
                throw new InvalidOperationException("LotNo 不可為空");
        }
    }
    public static class LotFlowFactory_v0
    {
        public static ILotFlow CreateAddQtyFlow()
        {
            return new FlowPipeline(new IFlowStep[]{
                new AddQtyValidateStep(),
                new AddQtyModifyStep(),
            //new AddQtyHistoryStep()
            });
        }
    }

    public class LotFlowFactory_v1
    {
        private readonly LotFlowRepository _repo;

        public LotFlowFactory_v1()
        {
            _repo = new LotFlowRepository();
        }

        public ILotFlow Create(string actionType)
        {
            var stepDefs = _repo.GetFlowSteps(actionType)
                                .OrderBy(x => x.Order)
                                .ToList();

            var steps = new List<IFlowStep>();

            foreach (var def in stepDefs)
            {
                var step = FlowStepFactory.Create(def.ClassName);
                steps.Add(step);
            }

            return new FlowPipeline(steps);
        }
    }




    public class AddQtyModifyStep : IFlowStep
    {
        public void Execute(LotContext context)
        {
            // 模擬 DB 更新
            Console.WriteLine(
                "Lot:{0} Qty:+{1} User:{2}",
                context.LotNo,
                context.Qty,
                context.UserNo
            );

            // 可把結果放入 Bag
            context.Bag["AfterQty"] = 100; // 假設
        }
    }

    public static class FlowStepFactory
    {
        public static IFlowStep Create(string className)
        {
            var type = Type.GetType(className);
            if (type == null)
                throw new InvalidOperationException(
                    "找不到 FlowStep 類型：" + className);

            return (IFlowStep)Activator.CreateInstance(type);
        }
    }
}

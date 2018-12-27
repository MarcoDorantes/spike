using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace startup_spec
{
    class DeferredStep
    {
        public DeferredStep(Func<DeferredStep> a)
        {
            Execute_Step_And_Return_Deferred_Next_Step = a;
        }
        public Func<DeferredStep> Execute_Step_And_Return_Deferred_Next_Step;

        public static void Continue(IEnumerable<DeferredStep> steps)
        {
            if (steps.Count() <= 0) return;
            var next_steps = new List<DeferredStep>();
            foreach (DeferredStep step in steps)
            {
                DeferredStep deferred = step.Execute_Step_And_Return_Deferred_Next_Step();
                if (deferred != null)
                {
                    next_steps.Add(deferred);
                }
            }
            Continue(next_steps);
        }
    }

    interface ITarget
    {
        void Setup();
        void Start();
    }
    interface ISource
    {
        int Steps { get; }
        void Setup();
        void Start();
    }
    abstract class WriterBase
    {
        public ITarget target;
        public ISource source;
        public IDispatcher owner;
        public WriterBase(IDispatcher owner) { this.owner = owner; }

        public void SingleStepStart()
        {
            target = GetTarget();
            source = GetSource();
            target.Start();
            source.Start();
        }
        public DeferredStep Setup()
        {
            target = GetTarget();
            source = GetSource();
            target.Setup();
            source.Setup();
            return new DeferredStep(() => Start());
        }
        public virtual DeferredStep Start()
        {
            target.Start();
            source.Start();
            return null;
        }
        public abstract void SecondStepStart();
        protected abstract ITarget GetTarget();
        protected abstract ISource GetSource();
        public void Replace()
        {
            owner.Replace(this);
        }
    }
    class Writer : WriterBase
    {
        public Writer(IDispatcher owner):base(owner){}
        public override void SecondStepStart()
        {
            source.Setup();
        }
        protected override ITarget GetTarget() => new Target();
        protected override ISource GetSource() => new Source();
    }
    class Target : ITarget
    {
        public void Setup() { }
        public void Start(){}
    }
    class Source : ISource
    {
        public int Steps { get; set; }
        public void Setup()
        {
            ++Steps;
        }
        public void Start()
        {
            ++Steps;
        }
    }
    interface IDispatcher
    {
        void Replace(WriterBase s);
    }
    class Dispatcher: IDispatcher
    {
        public List<Writer> Writers;
        public Dispatcher(int count)
        {
            Writers = Enumerable.Range(1, count).Aggregate(new List<Writer>(), (whole, next) => { whole.Add(new Writer(this)); return whole; });
        }

        public void Start()
        {
            Writers.ForEach(w => w.SingleStepStart());
            Writers.Last().Replace();
        }
        public void Replace(WriterBase w)
        {
            var @new = new Writer(this);
            Writers.Add(@new);
            @new.SingleStepStart();
        }
    }
    class Dispatcher2 : IDispatcher
    {
        public List<Writer> Writers;
        public Dispatcher2(int count)
        {
            Writers = Enumerable.Range(1, count).Aggregate(new List<Writer>(), (whole, next) => { whole.Add(new Writer(this)); return whole; });
        }

        public void Start()
        {
            Writers.ForEach(w => w.SingleStepStart());
            Writers.ForEach(w => w.SecondStepStart());
            Writers.Last().Replace();
        }
        public void Replace(WriterBase s)
        {
            var @new = new Writer(this);
            Writers.Add(@new);
            @new.SingleStepStart();
            @new.SecondStepStart();
        }
    }
    class Dispatcher3 : IDispatcher
    {
        public List<Writer> Writers;
        public Dispatcher3(int count)
        {
            Writers = Enumerable.Range(1, count).Aggregate(new List<Writer>(), (whole, next) => { whole.Add(new Writer(this)); return whole; });
        }

        public void Start()
        {
            InitialStartWriters();
            Writers.Last().Replace();
        }
        private void InitialStartWriters()
        {
            StartWriters(Writers);
        }
        private void StartWriters(IEnumerable<WriterBase> writers)
        {
            var next_steps = writers.Aggregate(new List<DeferredStep>(), (whole, next) =>
            {
                DeferredStep deferred = StartWriter(next);
                if (deferred != null)
                {
                    whole.Add(deferred);
                }
                return whole;
            });
            DeferredStep.Continue(next_steps);
        }
        private DeferredStep StartWriter(WriterBase writer)
        {
            return writer.Setup();
        }
        public void Replace(WriterBase s)
        {
            var @new = new Writer(this);
            Writers.Add(@new);
            var newwriters = new List<WriterBase> { @new };
            StartWriters(newwriters);
        }
    }
    [TestClass]
    public class StartChainSpec
    {
        [TestMethod]
        public void AsOfNow_v1()
        {
            int w_count = 3;
            var dispatcher = new Dispatcher(w_count);

            dispatcher.Start();

            Assert.AreEqual<int>(w_count + 1, dispatcher.Writers.Count);
            foreach (var w in dispatcher.Writers)
            {
                Assert.AreEqual<int>(1, w.source.Steps);
            }
        }

        [TestMethod]
        public void AsOf_v2()
        {
            int w_count = 9;
            var dispatcher = new Dispatcher2(w_count);

            dispatcher.Start();

            Assert.AreEqual<int>(w_count + 1, dispatcher.Writers.Count);
            foreach (var w in dispatcher.Writers)
            {
                Assert.AreEqual<int>(2, w.source.Steps);
            }
        }

        [TestMethod]
        public void AsOf_v3()
        {
            int w_count = 9;
            var dispatcher = new Dispatcher3(w_count);

            dispatcher.Start();

            Assert.AreEqual<int>(w_count + 1, dispatcher.Writers.Count);
            foreach (var w in dispatcher.Writers)
            {
                Assert.AreEqual<int>(2, w.source.Steps);
            }
        }

        //
        static void start()
        {
            var steps = new List<DeferredStep>();
            DeferredStep s = step1();
            if (s != null)
                steps.Add(s);
            DeferredStep.Continue(steps);
        }
        static DeferredStep step1() { Debug.WriteLine("step1 "); return new DeferredStep(() => step2()); }
        static DeferredStep step2() { Debug.WriteLine("step2 "); return new DeferredStep(() => step3()); }
        static DeferredStep step3() { Debug.WriteLine("step3 "); return null; }

        [TestMethod]
        public void start_chain()
        {
            start();
        }
    }
}
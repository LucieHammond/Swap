using GameEngine.PMR.Basics.Content;
using GameEngine.PMR.Modules;
using GameEngine.PMR.Modules.Policies;
using GameEngine.PMR.Modules.Specialization;
using GameEngine.PMR.Process.Transitions;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Scheduling;
using GameEngine.PMR.Unity.Basics.Configuration;
using GameEngine.PMR.Unity.Basics.Content;
using GameEngine.PMR.Unity.Basics.Input;
using GameEngine.PMR.Unity.Modules;
using Swap.Rules.Services;
using System;
using System.Collections.Generic;

namespace Swap.Setups.Modes
{
    public class ServicesSetup : IGameModuleSetup
    {
        public string Name => "Services";

        public Type RequiredServiceSetup => null;

        public Type RequiredParentSetup => null;

        public void SetRules(ref RulesDictionary rules)
        {
            // Configuration
            rules.AddRule(new ConfigurationService());

            // Content
            rules.AddRule(new DescriptorContentService());

            // Input
            rules.AddRule(new InputService());

            // Progression
            rules.AddRule(new ProgressionService());
        }

        public List<Type> GetInitUnloadOrder()
        {
            return new List<Type>()
            {
                typeof(ConfigurationService),
                typeof(DataContentService),
                typeof(DescriptorContentService),
                typeof(AssetContentService),
                typeof(InputService),
                typeof(ProgressionService)
            };
        }

        public List<RuleScheduling> GetUpdateScheduler()
        {
            return new List<RuleScheduling>()
            {
                new RuleScheduling(typeof(InputService), 1, 0)
            };
        }

        public List<RuleScheduling> GetFixedUpdateScheduler()
        {
            return new List<RuleScheduling>();
        }

        public List<RuleScheduling> GetLateUpdateScheduler()
        {
            return new List<RuleScheduling>();
        }

        public ExceptionPolicy GetExceptionPolicy()
        {
            return new ExceptionPolicy()
            {
                ReactionDuringLoad = OnExceptionBehaviour.StopAll,
                ReactionDuringUpdate = OnExceptionBehaviour.PauseAll,
                ReactionDuringUnload = OnExceptionBehaviour.StopAll,
                FallbackModule = null
            };
        }

        public PerformancePolicy GetPerformancePolicy()
        {
            return new PerformancePolicy()
            {
                MaxFrameDuration = 20,
                CheckStallingRules = false,
                InitStallingTimeout = 200,
                UpdateStallingTimeout = 40,
                UnloadStallingTimeout = 150,
                NbWarningsBeforeException = 3
            };
        }

        public void SetSpecialTasks(ref List<SpecialTask> tasks)
        {
            UnityModule.SetSpecialTasks(ref tasks);
        }

        public Transition GetTransition()
        {
            return null;
        }
    }
}

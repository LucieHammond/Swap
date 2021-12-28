using GameEngine.PMR.Modules;
using GameEngine.PMR.Modules.Policies;
using GameEngine.PMR.Modules.Specialization;
using GameEngine.PMR.Process.Transitions;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Scheduling;
using GameEngine.PMR.Unity.Modules;
using Swap.Rules.Controls;
using Swap.Rules.Mechanics;
using Swap.Rules.World;
using System;
using System.Collections.Generic;

namespace Swap.Setups.Modes
{
    public class PlayModeSetup : IGameModuleSetup
    {
        public string Name => "GameLevel";

        public Type RequiredServiceSetup => typeof(ServicesSetup);

        public Type RequiredParentSetup => null;

        public void SetRules(ref RulesDictionary rules)
        {
            rules.AddRule(new LevelRule());

            rules.AddRule(new ControllerRule());
            rules.AddRule(new CharacterRule());
            rules.AddRule(new CameraRule());

            rules.AddRule(new SwapRule());
            rules.AddRule(new GemPickupRule());
        }

        public List<Type> GetInitUnloadOrder()
        {
            return new List<Type>()
            {
                typeof(LevelRule),
                typeof(ControllerRule),
                typeof(CharacterRule),
                typeof(CameraRule),
                typeof(SwapRule),
                typeof(GemPickupRule)
            };
        }

        public List<RuleScheduling> GetUpdateScheduler()
        {
            return new List<RuleScheduling>()
            {
                new RuleScheduling(typeof(CharacterRule), 1, 0),
                new RuleScheduling(typeof(CameraRule), 1, 0),
                new RuleScheduling(typeof(SwapRule), 1, 0),
                new RuleScheduling(typeof(GemPickupRule), 1, 0)
            };
        }

        public List<RuleScheduling> GetFixedUpdateScheduler()
        {
            return new List<RuleScheduling>();
        }

        public List<RuleScheduling> GetLateUpdateScheduler()
        {
            return new List<RuleScheduling>()
            {
                new RuleScheduling(typeof(ControllerRule), 1, 0)
            };
        }

        public ExceptionPolicy GetExceptionPolicy()
        {
            return new ExceptionPolicy()
            {
                ReactionDuringLoad = OnExceptionBehaviour.UnloadModule,
                ReactionDuringUpdate = OnExceptionBehaviour.PauseModule,
                ReactionDuringUnload = OnExceptionBehaviour.UnloadModule,
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

using GameEngine.PMR.Modules;
using GameEngine.PMR.Modules.Policies;
using GameEngine.PMR.Modules.Specialization;
using GameEngine.PMR.Process.Transitions;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Scheduling;
using GameEngine.PMR.Unity.Modules;
using Swap.Rules.Controls;
using Swap.Rules.Events;
using Swap.Rules.Mechanics;
using Swap.Rules.Operations;
using Swap.Rules.Skills;
using Swap.Rules.World;
using System;
using System.Collections.Generic;

namespace Swap.Setups.Modes
{
    public class PlayModeSetup : IGameModuleSetup
    {
        public string Name => "PlayMode";

        public Type RequiredServiceSetup => typeof(ServicesSetup);

        public Type RequiredParentSetup => null;

        public void SetRules(ref RulesDictionary rules)
        {
            rules.AddRule(new LevelRule());
            rules.AddRule(new LogicRule());

            rules.AddRule(new CameraRule());
            rules.AddRule(new CharacterRule());
            rules.AddRule(new ControllerRule());

            rules.AddRule(new SwapRule());

            rules.AddRule(new ButtonPressRule());
            rules.AddRule(new GemPickupRule());
            rules.AddRule(new GemReceptionRule());
            rules.AddRule(new DoorOpeningRule());
            rules.AddRule(new PlatformMoveRule());

            rules.AddRule(new MotionRule());
            rules.AddRule(new InteractionRule());

            rules.AddRule(new StartRule());
            rules.AddRule(new DefeatRule());
            rules.AddRule(new VictoryRule());
        }

        public List<Type> GetInitUnloadOrder()
        {
            return new List<Type>()
            {
                typeof(StartRule),

                typeof(LevelRule),
                typeof(LogicRule),

                typeof(ControllerRule),
                typeof(CharacterRule),

                typeof(SwapRule),

                typeof(ButtonPressRule),
                typeof(GemPickupRule),
                typeof(GemReceptionRule),
                typeof(DoorOpeningRule),
                typeof(PlatformMoveRule),

                typeof(MotionRule),
                typeof(InteractionRule),

                typeof(DefeatRule),
                typeof(VictoryRule),

                typeof(CameraRule)
            };
        }

        public List<RuleScheduling> GetUpdateScheduler()
        {
            return new List<RuleScheduling>()
            {
                new RuleScheduling(typeof(CharacterRule), 1, 0),
                new RuleScheduling(typeof(CameraRule), 1, 0),

                new RuleScheduling(typeof(SwapRule), 1, 0),

                new RuleScheduling(typeof(ButtonPressRule), 1, 0),
                new RuleScheduling(typeof(GemPickupRule), 1, 0),
                new RuleScheduling(typeof(GemReceptionRule), 1, 0),

                new RuleScheduling(typeof(DoorOpeningRule), 1, 0),
                new RuleScheduling(typeof(PlatformMoveRule), 1, 0),

                new RuleScheduling(typeof(MotionRule), 1, 0),
                new RuleScheduling(typeof(InteractionRule), 1, 0),

                new RuleScheduling(typeof(StartRule), 1, 0),
                new RuleScheduling(typeof(DefeatRule), 1, 0),
                new RuleScheduling(typeof(VictoryRule), 1, 0)
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
                new RuleScheduling(typeof(ControllerRule), 1, 0),
                new RuleScheduling(typeof(LogicRule), 1, 0)
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

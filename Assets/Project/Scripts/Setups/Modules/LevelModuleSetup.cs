using GameEngine.PMR.Modules;
using GameEngine.PMR.Modules.Policies;
using GameEngine.PMR.Modules.Specialization;
using GameEngine.PMR.Process.Transitions;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Scheduling;
using GameEngine.PMR.Unity.Modules;
using System;
using System.Collections.Generic;

public class LevelModuleSetup : IGameModuleSetup
{
    public string Name => "GameLevel";

    public Type RequiredServiceSetup => typeof(ServiceModuleSetup);

    public Type RequiredParentSetup => null;

    public void SetRules(ref RulesDictionary rules)
    {

    }

    public List<Type> GetInitUnloadOrder()
    {
        return new List<Type>();
    }

    public List<RuleScheduling> GetUpdateScheduler()
    {
        return new List<RuleScheduling>();
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
            CheckStallingRules = true,
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

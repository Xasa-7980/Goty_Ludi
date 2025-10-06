using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Action_Timing
{
    Start,
    OnGoing,
    End
}
public class Timer
{
    float curTime;
    MonoBehaviour owner;
    List<Coroutine> runningCoroutines;
    bool is_started;
    bool is_going;
    bool is_stopped;
    bool is_paused;
    public Timer(MonoBehaviour owner)
    {
        this.owner = owner;
        runningCoroutines = new List<Coroutine>();
        is_stopped = true;
    }
    public void StartTimer ( float maxTime, Action action, Action_Timing timing = Action_Timing.End )
    {
        switch (timing)
        {
            case Action_Timing.Start:
                runningCoroutines.Add(owner.StartCoroutine(StartAction_Timer(maxTime, action)));
                break;
            case Action_Timing.OnGoing:
                runningCoroutines.Add(owner.StartCoroutine(OnGoingAction_Timer(maxTime, action)));
                break;
            case Action_Timing.End:
                runningCoroutines.Add(owner.StartCoroutine(EndAction_Timer(maxTime, action)));
                break;
            default:
                break;
        }
    }
    public void StopTimer ( )
    {
        if (runningCoroutines != null)
        {
            for (int i = 0; i < runningCoroutines.Count; i++)
            {

                owner.StopCoroutine(runningCoroutines[i]);
            }
            runningCoroutines = null;
            is_stopped = true;
            is_started = false;
            is_going = false;
        }
    }
    public void PauseTimer ( )
    {
        if (is_going && !is_paused)
        {
            is_paused = true;
            is_going = false;
        }
    }
    public void ResumeTimer ( )
    {
        if (is_paused)
        {
            is_paused = false;
            is_going = true;
        }
    }
    public void ResetTimer ( )
    {
        curTime = 0;
    }
    public bool Timer_Started ( )
    {
        return is_started && !is_paused && !is_stopped;

    }
    public bool Timer_Finished ( )
    {
        return !is_started && !is_paused && !is_going && is_stopped;
    }
    public void DisableTimer( )
    {
        is_going = false;
    }
    IEnumerator StartAction_Timer ( float maxTime, Action action )
    {
        is_started = true;
        is_going = true;
        is_stopped = false;
        is_paused = false;
        curTime = 0;
        action?.Invoke();

        while (curTime < maxTime)
        {
            if (!is_paused)
            {
                curTime += Time.deltaTime;
            }
            yield return null;
        }

        is_started = false;
        is_going = false;
        is_stopped = true;
    }
    IEnumerator OnGoingAction_Timer ( float maxTime, Action action )
    {
        is_started = true;
        is_going = true;
        is_stopped = false;
        is_paused = false;
        curTime = 0;

        while (curTime < maxTime)
        {
            if (!is_paused)
            {
                curTime += Time.deltaTime;
                action?.Invoke();
            }
            yield return null;
        }

        is_started = false;
        is_going = false;
        is_stopped = true;
    }
    IEnumerator EndAction_Timer ( float maxTime, Action action )
    {
        is_started = true;
        is_going = true;
        is_stopped = false;
        is_paused = false;
        curTime = 0;

        while (curTime < maxTime)
        {
            if (!is_paused)
            {
                curTime += Time.deltaTime;
            }
            yield return null;
        }
        action?.Invoke();

        is_started = false;
        is_going = false;
        is_stopped = true;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    #region Properties

    public AudioTrack[] tracks;

    private readonly Dictionary<AudioType, AudioTrack> audioTable = new Dictionary<AudioType, AudioTrack>();
    private readonly Dictionary<AudioType, Coroutine> jobTable = new Dictionary<AudioType, Coroutine>();

    #endregion

    #region Unity Functions

    protected override void AwakeIfExist()
    {
        Configure();
    }

    // private void OnDisable() 
    // {
    //     //Dispose();
    // }

    #endregion

    #region Public Functions

    public bool IsPlaying(AudioType type)
    {
        return audioTable.ContainsKey(type) && audioTable[type].source.isPlaying;
    }

    [Button]
    public void PlayAudio(AudioType type, bool canReplayWhilePlaying = false, float cooldown = 0.0f, float delay = 0.0f, bool fade = false) 
    {
        if (!canReplayWhilePlaying && IsPlaying(type))
            return;
        
        AddJob(new AudioJob(AudioAction.Start, type, cooldown, delay, fade));
    }

    [Button]
    public void PauseAudio(AudioType type, float cooldown = 0.0f, float delay = 0.0f, bool fade = false) 
    {
        AddJob(new AudioJob(AudioAction.Pause, type, cooldown, delay, fade));
    }
    
    [Button]
    public void StopAudio(AudioType type, float cooldown = 0.0f, float delay = 0.0f, bool fade = false)
    {
        if (!IsPlaying(type))
            return;
        
        AddJob(new AudioJob(AudioAction.Stop, type, cooldown, delay, fade));
    }
    
    [Button]
    public void RestartAudio(AudioType type, float cooldown = 0.0f, float delay = 0.0f, bool fade = false) 
    {
        AddJob(new AudioJob(AudioAction.Restart, type, cooldown, delay, fade));
    }

    #endregion

    #region Private Functions

    private void Configure()
    {
        GenerateAudioTable();
    }

    private void GenerateAudioTable()
    {
        foreach (var track in tracks)
        {
            if (!audioTable.ContainsKey(track.type))
            {
                audioTable.Add(track.type, track);
            }
        }
    }

    private void Dispose()
    {
        // cancel all jobs in progress
        foreach (var job in jobTable.Select(kvp => kvp.Value).Where(job => job != null))
        {
            StopCoroutine(job);
        }
    }

    private void AddJob(AudioJob job)
    {
        // cancel any job that might be using this job's audio source
        RemoveConflictingJobs(job.Type);

        var jobRunner = StartCoroutine(RunAudioJob(job));
        jobTable.Add(job.Type, jobRunner);
    }

    private void RemoveJob(AudioType type)
    {
        if (!jobTable.ContainsKey(type))
            return;

        var runningJob = jobTable[type];
        if (runningJob != null)
            StopCoroutine(runningJob);

        jobTable.Remove(type);
    }

    private void RemoveConflictingJobs(AudioType type)
    {
        if (jobTable.ContainsKey(type))
            RemoveJob(type);

        var conflictAudio = AudioType.None;
        var audioTrackNeeded = GetAudioTrack(type);
        foreach (var entry in jobTable)
        {
            var audioType = entry.Key;
            var audioTrackInUse = GetAudioTrack(audioType);
            if (audioTrackInUse.source != audioTrackNeeded.source) continue;
            conflictAudio = audioType;
            break;
        }

        if (conflictAudio != AudioType.None)
        {
            RemoveJob(conflictAudio);
        }
    }

    private IEnumerator RunAudioJob(AudioJob job)
    {
        var track = GetAudioTrack(job.Type);
        track.source.clip = GetAudioClipFromAudioTrack(job.Type, track);

        var initial = 0f;
        var target = 1f;
        switch (job.Action)
        {
            // Delay time should be smaller than cool down time
            case AudioAction.Start when job.Delay > 0f && job.Cooldown > 0f:
                if (Time.time >= track.readyTime)
                {
                    track.source.PlayDelayed(job.Delay);
                    track.readyTime = Time.time + job.Cooldown;
                }
                break;
            
            case AudioAction.Start when job.Cooldown > 0f:
                if (Time.time >= track.readyTime)
                {
                    track.source.Play();
                    track.readyTime = Time.time + job.Cooldown;
                }
                break;
            
            case AudioAction.Start when job.Delay > 0f:
                track.source.PlayDelayed(job.Delay);
                break;

            case AudioAction.Start:
                track.source.Play();
                break;

            case AudioAction.Pause:
                track.source.Pause();
                break;

            case AudioAction.Stop when !job.Fade:
                track.source.Stop();
                break;

            case AudioAction.Stop:
                initial = 1f;
                target = 0f;
                break;

            case AudioAction.Restart:
                track.source.Stop();
                track.source.Play();
                break;
        }

        // fade volume
        if (job.Fade)
        {
            const float duration = 1.0f;
            var timer = 0.0f;

            while (timer <= duration)
            {
                track.source.volume = Mathf.Lerp(initial, target, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            // if _timer was 0.9999 and Time.deltaTime was 0.01 we would not have reached the target
            // make sure the volume is set to the value we want
            track.source.volume = target;

            if (job.Action == AudioAction.Stop)
            {
                track.source.Stop();
            }
        }

        jobTable.Remove(job.Type);
    }


    private AudioTrack GetAudioTrack(AudioType type)
    {
        return audioTable.ContainsKey(type) ? audioTable[type] : null;
    }

    private static AudioClip GetAudioClipFromAudioTrack(AudioType type, AudioTrack track)
    {
        return track.type == type ? track.clip : null;
    }

    #endregion
}

#region Audio

public enum AudioAction
{
    Start,
    Pause,
    Stop,
    Restart
}

[Serializable]
public class AudioTrack
{
    public AudioType type;
    public AudioSource source;
    public AudioClip clip;
    [HideInInspector] public float readyTime;
}

public class AudioJob
{
    public readonly AudioAction Action;
    public readonly AudioType Type;
    public readonly float Cooldown;
    public readonly float Delay;
    public readonly bool Fade;

    public AudioJob(AudioAction action, AudioType type, float cooldown, float delay, bool fade) 
    {
        Action = action;
        Type = type;
        Cooldown = cooldown;
        Delay = delay;
        Fade = fade;
    }
}

#endregion
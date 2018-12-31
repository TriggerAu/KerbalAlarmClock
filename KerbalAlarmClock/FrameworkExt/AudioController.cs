using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    internal class AudioController:MonoBehaviourExtended
    {
        //Parent Objects
        internal KerbalAlarmClock mbKAC;
        //private Settings settings;  //Commented because usage removed

        AudioSource audiosourceAlarm;

        internal void Init()
        {
            //settings = KerbalAlarmClock.settings;  //Commented cause usage removed

            //if (Resources.clipAlarms.ContainsKey(settings.AlarmsAlertSound))
            //    mbARP.clipAlarmsAlert = Resources.clipAlarms[settings.AlarmsAlertSound];
            //if (Resources.clipAlarms.ContainsKey(settings.AlarmsWarningSound))
            //    mbARP.clipAlarmsWarning = Resources.clipAlarms[settings.AlarmsWarningSound];

            audiosourceAlarm = mbKAC.gameObject.AddComponent<AudioSource>();
            audiosourceAlarm.spatialBlend = 0;
            audiosourceAlarm.playOnAwake = false;
            audiosourceAlarm.loop = false;
            audiosourceAlarm.Stop();
        }

        internal void Play(AudioClip clipToPlay) { Play(clipToPlay, 1); }
        internal void Play(AudioClip clipToPlay, Int32 Repeats)
        {
            audiosourceAlarm.clip = clipToPlay;
            audiosourceAlarm.loop = false;
            audiosourceAlarm.volume = Volume;

            RepeatCounter = 0;
            RepeatLimit = Repeats;
            Playing = true;

            audiosourceAlarm.Play();

            if (onPlayStarted!=null)
                onPlayStarted(this, clipToPlay);
        }

        internal Single Volume
        {
            get
            {
                if (KerbalAlarmClock.settings.AlarmsVolumeFromUI)
                    return GameSettings.UI_VOLUME;
                else
                    return KerbalAlarmClock.settings.AlarmsVolume;
            }
        }
        internal Int32 VolumePct { get { return (Int32)(Volume * 100); } }

        internal void Stop()
        {
            audiosourceAlarm.Stop();
            Playing = false;
            if (onPlayFinished != null)
                onPlayFinished(this, audiosourceAlarm.clip);
        }


        //internal Boolean isClipPlaying() { return audiosourceAlarm.isPlaying; }
        internal Boolean isClipPlaying(AudioClip clip)
        {
            return (Playing && (clip == audiosourceAlarm.clip));
        }

        internal Boolean isPlaying
        {
            get
            {
                return Playing;
            }
        }

        private Boolean Playing;
        private Int32 RepeatCounter;
        private Int32 RepeatLimit;

        //check status of playing and do whats next;
        internal override void Update()
        {
            

            //if the audioclip is done
            if (!audiosourceAlarm.isPlaying && Playing)
            {

                //increase the repeat counter
                RepeatCounter++;
                if (RepeatCounter < RepeatLimit ||RepeatLimit>5)
                {
                    //play it again
                    audiosourceAlarm.Play();
                }
                else
                {
                    //halt playing
                    Playing = false;
                    if (onPlayFinished != null)
                        onPlayFinished(this, audiosourceAlarm.clip);
                }
            }
        }

        internal delegate void AudioEventArgs(AudioController sender,AudioClip clip);
        internal event AudioEventArgs onPlayFinished;
        internal event AudioEventArgs onPlayStarted;

        
    }
}

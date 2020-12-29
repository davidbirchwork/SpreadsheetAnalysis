using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Utilities.Loggers.Timing {
    public sealed class EventTimer {

        private readonly ConcurrentDictionary<string, Tuple<Stopwatch,string>> _runningTimers = new ConcurrentDictionary<string, Tuple<Stopwatch, string>>();

        private readonly ConcurrentDictionary<string, ConcurrentBag<TimedEvent>> _timedEvents = new ConcurrentDictionary<string, ConcurrentBag<TimedEvent>>();

        #region making this class a singleton according to http://www.yoda.arachsys.com/csharp/singleton.html

        private static readonly EventTimer TheTimer = new EventTimer();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
// ReSharper disable EmptyConstructor
        static EventTimer() {            
// ReSharper restore EmptyConstructor
        }

        private static EventTimer GetTimer {
            get {
                return TheTimer;
            }
        }

        #endregion

        #region Time Methods

        public static void StartTiming(string uniqueEventName,string eventClass) {
            
            Stopwatch stopwatch = new Stopwatch();

            Tuple<Stopwatch, string> swe = new Tuple<Stopwatch, string>(stopwatch,eventClass);

            if (!GetTimer._runningTimers.TryAdd(uniqueEventName, swe)) {
                Logger.ERROR("Duplicate timer " + uniqueEventName);
            } else {
                stopwatch.Start();
            }
        }

        public static void FinishTiming(string uniqueEventName) {            
            Tuple<Stopwatch, string> swe;
            if (GetTimer._runningTimers.TryRemove(uniqueEventName, out swe)) {
                swe.Item1.Stop();
                var te = new TimedEvent(swe.Item2, uniqueEventName, swe.Item1.Elapsed.TotalMilliseconds, DateTime.Now);
                AddTimedEvent(te);
            } else {
                Logger.ERROR("Time was not found " + uniqueEventName);
            }
        }

        public static void CountEventHit(string eventName) {
            AddTimedEvent(new TimedEvent(eventName, "", 0, DateTime.Now));
        }

        private static void AddTimedEvent(TimedEvent te) {
            GetTimer._timedEvents.AddOrUpdate(te.EventClass,
                                             new ConcurrentBag<TimedEvent> {te},
                                             (key, value) => {
                                                 value.Add(te);
                                                 return value;
                                             }
                );
        }

        #endregion

        #region Logging

        public static void PrintAverages() {

            foreach (var timedEventClass in GetTimer._timedEvents.ToArray()) {
                string eventClassName = timedEventClass.Key;
                double totalTime = 0;
                int occurances = 0;
                foreach (var timedEvent in timedEventClass.Value.ToArray()) {
                    totalTime += timedEvent.TotalMilliseconds;
                    occurances++;
                }
                double avems = totalTime/occurances;
                Logger.INFO("EventTimer: event "+eventClassName+" occurred "+occurances+" times, Average milliseconds: "+avems);
            }
        }

        #endregion

        #region Saving

        public static string TimedEventsToString() {
            StringBuilder sb= new StringBuilder();
            foreach (var timedEventClass in GetTimer._timedEvents.ToArray()) {                
                foreach (var timedEvent in timedEventClass.Value.ToArray()) {
                    sb.AppendLine(timedEvent.ToString());
                }
            }
            return sb.ToString();
        }

        public static void ClearTimedEvents() {
            GetTimer._timedEvents.Clear();
        }

        public static void AddEventsToLog() {
            string timerFile = UserConfig.UserConfig.Configure<LoggingConfig>().TimerFile;

            File.AppendAllText(timerFile,TimedEventsToString());
            ClearTimedEvents();
        }

        public static void LoadLogFile(bool clearCurrentEvents = true) {
            if (clearCurrentEvents) {
                ClearTimedEvents();
            }

            string timerFile = UserConfig.UserConfig.Configure<LoggingConfig>().TimerFile;
            foreach (var readLine in File.ReadLines(timerFile)) {                
                AddTimedEvent(new TimedEvent(readLine));
            }
        }

        #endregion
    }

    public struct TimedEvent {
        public readonly string EventClass; 
        public readonly string Name;
        public readonly double TotalMilliseconds;
        public readonly DateTime Occurence;

        public TimedEvent(string savedString) {
            string[] strings = savedString.Split(new[] { "\t" }, StringSplitOptions.None);
            if (strings.Length != 4) {
                Logger.DEBUG("EventTimer failed to read timerfile line " + savedString);
            }

            this.EventClass = strings[0];
            this.Name = strings[1];
            this.TotalMilliseconds = Double.Parse(strings[2]);
            this.Occurence = DateTime.Parse(strings[3]);
        }

        public TimedEvent(string eventClass, string name, double totalMilliseconds, DateTime occurence) {
            this.Name = name;
            this.TotalMilliseconds = totalMilliseconds;
            this.Occurence = occurence;
            this.EventClass = eventClass;
        }

        public override string ToString() {
            return EventClass + "\t" + Name + "\t" + TotalMilliseconds + "\t" + Occurence;
        }
    }
}

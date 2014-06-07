using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.IO;
namespace Cosmos
{
    public static class AtomicTime
    {
        private static DateTime _currentAtomicTime;
        private static bool _canConnectToServer = true;
        private static Stopwatch _timeSinceLastValue = new Stopwatch();
        private static readonly object Locker = new object();
        private static Countdown _countdown;
        
        public class Countdown
        {
            readonly object _locker = new object();
            int _value;
            public Countdown()
            { }
            public Countdown(int initialCount)
            {
                _value = initialCount;
            }
            public void Signal()
            {
                AddCount(-1);
            }
            public void PulseAll()
            {
                lock (_locker)
                {
                    _value = 0;
                    Monitor.PulseAll(_locker);
                }
            }
            public void AddCount(int amount)
            {
                lock (_locker)
                {
                    _value += amount;
                    if (_value <= 0)
                        Monitor.PulseAll(_locker);
                }
            }
            public void Wait()
            {
                lock (_locker)
                    while (_value > 0)
                        Monitor.Wait(_locker);
            }
        }

        public static DateTime Now
        {
            get
            {
                if (_canConnectToServer == false)
                    return DateTime.MinValue;
                if (_currentAtomicTime != DateTime.MinValue)
                {
                    _currentAtomicTime += _timeSinceLastValue.Elapsed;
                    _timeSinceLastValue.Reset();
                    _timeSinceLastValue.Start();
                }
                else
                {
                    lock (Locker)
                    {
                        if (_currentAtomicTime != DateTime.MinValue)
                        {
                            _currentAtomicTime += _timeSinceLastValue.Elapsed;
                            _timeSinceLastValue.Reset();
                            _timeSinceLastValue.Start();
                        }
                        else
                        {
                            var servers = new[]
                            {
                                "nist1-ny.ustiming.org",
                                "nist1-nj.ustiming.org",
                                "nist1-pa.ustiming.org",
                                "nist1.aol-va.symmetricom.com",
                                "nist1.columbiacountyga.gov",
                                "nist1-atl.ustiming.org",
                                "nist.expertsmi.com",
                                "nisttime.carsoncity.k12.mi.us",
                                "nist1-lnk.binary.net",
                                "www.nist.gov",
                                "utcnist.colorado.edu",
                                "utcnist2.colorado.edu",
                                "ntp-nist.ldsbc.edu",
                                "nist1-lv.ustiming.org",
                                "nist-time-server.eoni.com",
                                "nist1.aol-ca.symmetricom.com",
                                "nist1.symmetricom.com",
                                "nist1-la.ustiming.org",
                                "nist1-sj.ustiming.org"
                            };
                            var rnd = new Random();
                            _countdown = new Countdown(5);
                            foreach (string server in servers.OrderBy(s => rnd.NextDouble()).Take(5))
                            {
                                string server1 = server;
                                var t = new Thread(o => GetDateTimeFromServer(server1));
                                t.SetApartmentState(ApartmentState.STA);
                                t.Start();
                            }
                            _countdown.Wait();
                            if (_currentAtomicTime == DateTime.MinValue)
                                _canConnectToServer = false; }
                    }
                }
                return _currentAtomicTime;
            }
        }

        private static void GetDateTimeFromServer(string server)
        {
            if (_currentAtomicTime == DateTime.MinValue)
            {
                try {
                    string serverResponse;
                    using (var reader = new StreamReader(new System.Net.Sockets.TcpClient(server, 13).GetStream()))
                        serverResponse = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(serverResponse) || _currentAtomicTime != DateTime.MinValue)
                    {
                        string[] tokens = serverResponse.Replace("n", "").Split(' ');
                        if (tokens.Length >= 6) {
                            string health = tokens[5];
                            if (health == "0") {
                                string[] dateParts = tokens[1].Split('-');
                                string[] timeParts = tokens[2].Split(':');
                                var utcDateTime = new DateTime( Convert.ToInt32(dateParts[0]) + 2000, Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]), Convert.ToInt32(timeParts[0]), Convert.ToInt32(timeParts[1]), Convert.ToInt32(timeParts[2]));
                                if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == "," && tokens[6].Contains(".")) tokens[6] = tokens[6].Replace(".", ",");
                                else if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == "." && tokens[6].Contains(",")) tokens[6] = tokens[6].Replace(",", ".");
                                double millis;
                                double.TryParse(tokens[6], out millis); utcDateTime = utcDateTime.AddMilliseconds(-millis);
                                if (_currentAtomicTime == DateTime.MinValue)
                                {
                                    _currentAtomicTime = utcDateTime.ToLocalTime();
                                    _timeSinceLastValue = new Stopwatch();
                                    _timeSinceLastValue.Start();
                                    _countdown.PulseAll();
                                }
                            }
                        }
                    }
                }
                catch { }   
            }
            _countdown.Signal();
        }
    }
}
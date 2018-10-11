using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Networking.Connectivity;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace IntelligentKioskSample
{
    class ApplicationInsightsHelper
    {

        private static TelemetryClient context = null;
        private static string InstrumentationKey = SettingsHelper.Instance.ApplicationInsightsKey;
        private static bool enable = false;
        public static void Initialize()
        {
            if (InstrumentationKey.Length == 0)
            {
                return;
            }

            TelemetryClient tc = new TelemetryClient();
            tc.InstrumentationKey = InstrumentationKey;
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();

            // Set session data:
            tc.Context.User.Id = deviceInfo.FriendlyName;
            tc.Context.Session.Id = Guid.NewGuid().ToString();
            tc.Context.Device.OperatingSystem = deviceInfo.FriendlyName;

            context = tc;
            enable = true;
        }

        public static void TrackPage(string name)
        {
            if (context == null) { Initialize(); }
            if (!enable) { return; }
            PageViewTelemetry page = new PageViewTelemetry(name);
            page.Context.Operation.Name = name;
            context.TrackPageView(page);
        }


        public static void TrackException(Exception e)
        {
            if (context == null) { Initialize(); }
            if (!enable) { return; }
            ExceptionTelemetry exception = new ExceptionTelemetry(e);
            exception.Context.Operation.Name = "Exception";
            context.TrackException(exception);
        }

        public static void TrackEvent(string ename, Dictionary<String, String> dictionary)
        {
            if (context == null) { Initialize(); }
            if (!enable) { return; }

            EventTelemetry e = new EventTelemetry();
            e.Name = ename;
            foreach (KeyValuePair<string, string> item in dictionary)
            {
                e.Properties.Add(item);
            }
            context.TrackEvent(e);
        }
        public static void TrackRequest(string name, Uri uri, string code, bool success, long start, long end)
        {
            if (context == null) { Initialize(); }
            if (!enable) { return; }
            RequestTelemetry request = new RequestTelemetry();
            request.Name = name;
            request.ResponseCode = code;
            request.Success = success;
            request.Url = uri;
            request.Timestamp = DateTime.Now;
            request.Start(start);
            request.Context.Operation.Name = name;
            request.Duration = new TimeSpan(end - start);
            context.TrackRequest(request);
        }

        public static void TrackRequest(string name, Uri uri, string code, bool success, TimeSpan time)
        {
            if (context == null) { Initialize(); }
            if (!enable) { return; }
            RequestTelemetry request = new RequestTelemetry();
            request.Name = name;
            request.ResponseCode = code;
            request.Success = success;
            request.Url = uri;
            request.Timestamp = DateTime.Now;
            request.Duration = time;
            request.Context.Operation.Name = name;
            context.TrackRequest(request);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using Rainmeter;

// Overview: This is a blank canvas on which to build your plugin.

// Note: GetString, ExecuteBang and an unnamed function for use as a section variable
// have been commented out. If you need GetString, ExecuteBang, and/or section variables 
// and you have read what they are used for from the SDK docs, uncomment the function(s)
// and/or add a function name to use for the section variable function(s). 
// Otherwise leave them commented out (or get rid of them)!

namespace HttpRequestPlugin
{
    public class Measure
    {
        static public implicit operator Measure(IntPtr data)
        {
            return (Measure)GCHandle.FromIntPtr(data).Target;
        }
        public IntPtr buffer = IntPtr.Zero;

        private Rainmeter.API api;
        private string url;
        private string method;
        private Dictionary<string, string> httpParams = new Dictionary<string, string>();
        private Dictionary<string, string> headers = new Dictionary<string, string>();

        public Measure(API api)
        {
            this.api = api;
        }

        internal void SetUrl(string v)
        {
            this.url = v;
        }

        internal void AddParam(string param)
        {
            int split = param.IndexOf("=");
            string name = param.Substring(0, split);
            string value = param.Substring(split + 1);
            httpParams.Add(name, value);
        }

        internal void AddHeader(string header)
        {
            int split = header.IndexOf(":");
            string name = header.Substring(0, split);
            string value = header.Substring(split + 1);
            headers.Add(name, value);
        }

        internal void SetResponse(string responseString)
        {
            if (this.buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.buffer);
                this.buffer = IntPtr.Zero;
            }

            this.buffer = Marshal.StringToHGlobalUni(responseString);
        }

        internal void SetMethod(string method)
        {
            this.method = method;
        }

        internal void Update()
        {
            try
            {
                WebClient webClient = new WebClient();
                foreach (var header in headers)
                {
                    webClient.Headers.Add(header.Key, header.Value);
                }

                api.Log(API.LogType.Debug, "Invoking URL: " + this.url);

                if (this.GetHttpMethod() == HttpMethod.Get)
                {
                    foreach (var param in httpParams)
                    {
                        webClient.QueryString.Add(param.Key, param.Value);
                    }

                    SetResponse(webClient.DownloadString(this.url));

                }
                else if (this.GetHttpMethod() == HttpMethod.Post)
                {
                    var reqparm = new System.Collections.Specialized.NameValueCollection();
                    foreach (var param in httpParams)
                    {
                        reqparm.Add(param.Key, param.Value);
                    }
                    byte[] responsebytes = webClient.UploadValues(this.url, reqparm);
                    SetResponse(Encoding.UTF8.GetString(responsebytes));
                }

            }
            catch (Exception e)
            {
                api.Log(API.LogType.Error, "Error: " + e.Message);
                SetResponse("");
            }
        }

        private HttpMethod GetHttpMethod()
        {
            if ("get" == this.method.ToLower())
            {
                return HttpMethod.Get;
            }
            else if ("post" == this.method.ToLower())
            {
                return HttpMethod.Post;
            }
            else
            {
                throw new ArgumentException("Unsupported HttpMethod: " + this.method);
            }
        }

        internal void Reload()
        {
            try
            {
                this.SetMethod(api.ReadString("Method", "GET"));
                this.SetUrl(api.ReadString("URL", ""));
                this.httpParams.Clear();
                this.headers.Clear();

                for (int i = 1; i < 100; i++)
                {
                    api.Log(API.LogType.Debug, "Reading Param" + i);
                    string param = api.ReadString("Param" + i, "");
                    api.Log(API.LogType.Debug, "Valure Read:" + param);
                    if (String.IsNullOrEmpty(param))
                    {
                        break;
                    }

                    this.AddParam(param);
                }

                for (int i = 1; i < 100; i++)
                {
                    api.Log(API.LogType.Debug, "Reading Header" + i);
                    string param = api.ReadString("Header" + i, "");
                    api.Log(API.LogType.Debug, "Valure Read:" + param);
                    if (String.IsNullOrEmpty(param))
                    {
                        break;
                    }

                    this.AddHeader(param);
                }
            }
            catch (Exception e)
            {
                api.Log(API.LogType.Error, e.Message);
            }
        }
    }

    public class Plugin
    {

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            Rainmeter.API api = (Rainmeter.API)rm;
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure(api)));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            Measure measure = (Measure)data;
            if (measure.buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(measure.buffer);
            }
            GCHandle.FromIntPtr(data).Free();
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = (Measure)data;
            Rainmeter.API api = (Rainmeter.API)rm;
            measure.Reload();
            measure.Update();
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure)data;
            return 0.0;
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = (Measure)data;
            return measure.buffer;
        }

        //[DllExport]
        //public static void ExecuteBang(IntPtr data, [MarshalAs(UnmanagedType.LPWStr)]String args)
        //{
        //    Measure measure = (Measure)data;
        //}

        //[DllExport]
        //public static IntPtr (IntPtr data, int argc,
        //    [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1)] string[] argv)
        //{
        //    Measure measure = (Measure)data;
        //    if (measure.buffer != IntPtr.Zero)
        //    {
        //        Marshal.FreeHGlobal(measure.buffer);
        //        measure.buffer = IntPtr.Zero;
        //    }
        //
        //    measure.buffer = Marshal.StringToHGlobalUni("");
        //
        //    return measure.buffer;
        //}
    }
}


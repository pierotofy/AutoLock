using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AutoLock
{
    
    class Listener : IDisposable
    {
        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        int port;
        string token;
        CancellationTokenSource cancelToken = new CancellationTokenSource();

        public Listener(int port, string token)
        {
            this.port = port;
            this.token = token;
        }

        public async Task Listen()
        {
            try
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add("http://*:" + port.ToString() + "/lock/");
                listener.Start();

                var requests = new HashSet<Task>();
                requests.Add(listener.GetContextAsync());

                while (!cancelToken.Token.IsCancellationRequested)
                {
                    Task t = await Task.WhenAny(requests);
                    requests.Remove(t);

                    if (t is Task<HttpListenerContext>)
                    {
                        var context = (t as Task<HttpListenerContext>).Result;
                        requests.Add(ProcessRequestAsync(context));
                        requests.Add(listener.GetContextAsync());
                    }
                }
            }catch(Exception e)
            {
                await Task.Run(() =>
                {
                    MessageBox.Show(e.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Exit();
                });
            }
            
        }

        public async Task ProcessRequestAsync(HttpListenerContext context)
        {
            using (StreamWriter s = new StreamWriter(context.Response.OutputStream))
            {
                context.Response.ContentType = "text/plain";

                if (this.token == "" || 
                    (context.Request.QueryString.Count > 0 && context.Request.QueryString.Get("token") == this.token))
                {
                    context.Response.StatusCode = 200;

                    // Lock
                    LockWorkStation();

                    s.Write("OK");
                }
                else
                {
                    context.Response.StatusCode = 404;
                    s.Write("Unauthorized (need to pass ?token=password query string)");
                }
            }

            context.Response.Close();
        }


        public void Dispose()
        {
            if (cancelToken != null) cancelToken.Cancel(false);
        }
    }
}
